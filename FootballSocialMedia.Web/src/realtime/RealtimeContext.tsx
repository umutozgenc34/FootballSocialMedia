import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState, type ReactNode } from "react";
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { API_URL, getFreshAccessToken } from "../api/client";
import { chatApi, notificationsApi } from "../api/endpoints";
import type { AppNotification, ChatMessage } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { useToast } from "../components/Toasts";
import { notificationText } from "../lib/notifications";

type ConnectionStatus = "connecting" | "connected" | "reconnecting" | "disconnected";

interface RealtimeState {
  status: ConnectionStatus;
  /** Okunmamış bildirim sayısı (mesaj bildirimleri hariç) */
  unreadNotifications: number;
  /** Okunmamış mesaj bildirimi olan kişiler: userId → adet */
  unreadMessagesBySender: Record<string, number>;
  unreadMessages: number;
  sendMessage: (receiverId: string, message: string) => Promise<void>;
  /** Mesajlar sayfasında açık olan sohbet; o kişiden gelen mesajlar için toast gösterilmez */
  setActiveThread: (userId: string | null) => void;
  markThreadRead: (userId: string) => void;
}

const RealtimeContext = createContext<RealtimeState | null>(null);

export function RealtimeProvider({ children }: { children: ReactNode }) {
  const { user } = useAuth();
  const qc = useQueryClient();
  const toast = useToast();
  const [status, setStatus] = useState<ConnectionStatus>("disconnected");
  const connectionRef = useRef<HubConnection | null>(null);
  const activeThread = useRef<string | null>(null);

  // Son 50 bildirim: zil ve mesaj rozetleri buradan hesaplanıyor
  const notifications = useQuery({
    queryKey: ["notifications"],
    queryFn: notificationsApi.list,
    enabled: !!user,
    refetchInterval: 60_000, // SignalR koparsa da rozetler güncel kalsın
  });

  const userId = user?.id;

  // Bağlantı sadece kullanıcı değişince (giriş/çıkış) yeniden kurulur; profil güncellemesi bağlantıyı koparmaz.
  useEffect(() => {
    if (!userId) return;

    const connection = new HubConnectionBuilder()
      .withUrl(`${API_URL}/chathub`, { accessTokenFactory: getFreshAccessToken })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(LogLevel.Warning)
      .build();
    connectionRef.current = connection;

    connection.on("ReceiveMessage", (message: ChatMessage) => {
      const otherId = message.senderId === userId ? message.receiverId : message.senderId;
      qc.setQueryData<ChatMessage[]>(["chat", otherId], (list) =>
        list ? (list.some((m) => m.id === message.id) ? list : [...list, message]) : list,
      );
      qc.invalidateQueries({ queryKey: ["conversations"] });

      if (message.senderId !== userId && activeThread.current !== message.senderId) {
        toast.show(`${message.senderName ?? "Biri"}: ${message.message.slice(0, 80)}`, {
          link: { to: `/mesajlar/${message.senderId}`, label: "Yanıtla" },
        });
      }
    });

    connection.on("ReceiveNotification", (notification: AppNotification) => {
      // Açık sohbetten gelen mesaj bildirimi zaten okunmuş sayılır
      if (notification.type === "Message" && activeThread.current === notification.senderId) {
        notificationsApi.markRead(notification.id).catch(() => undefined);
        notification = { ...notification, isRead: true };
      }
      qc.setQueryData<AppNotification[]>(["notifications"], (list) => [notification, ...(list ?? [])].slice(0, 50));
      if (notification.type !== "Message") {
        toast.show(notificationText(notification), {
          link: notification.postId
            ? { to: `/gonderi/${notification.postId}`, label: "Gönderiye git" }
            : { to: `/profil/${notification.senderId}`, label: "Profili gör" },
        });
      }
    });

    connection.onreconnecting(() => setStatus("reconnecting"));
    connection.onreconnected(() => {
      setStatus("connected");
      // Kopukken kaçırılanları tamamla
      qc.invalidateQueries({ queryKey: ["notifications"] });
      qc.invalidateQueries({ queryKey: ["conversations"] });
      qc.invalidateQueries({ queryKey: ["chat"] });
    });
    connection.onclose(() => setStatus("disconnected"));

    let cancelled = false;
    let retryTimer: ReturnType<typeof setTimeout> | undefined;
    const start = async () => {
      if (cancelled) return;
      setStatus("connecting");
      try {
        await connection.start();
        if (!cancelled) setStatus("connected");
      } catch {
        // API henüz ayağa kalkmadıysa birkaç saniye sonra tekrar dene
        if (!cancelled) {
          setStatus("disconnected");
          retryTimer = setTimeout(start, 5000);
        }
      }
    };
    // setTimeout: React StrictMode'da effect iki kez çalışıyor; ilk bağlantı negotiation ortasında
    // iptal edilip konsola hata basmasın diye başlatmayı bir sonraki tick'e bırakıyoruz.
    retryTimer = setTimeout(start, 0);

    return () => {
      cancelled = true;
      clearTimeout(retryTimer);
      connection.stop();
      connectionRef.current = null;
    };
  }, [userId, qc, toast]);

  const sendMessage = useCallback(
    async (receiverId: string, message: string) => {
      const connection = connectionRef.current;
      if (connection?.state === HubConnectionState.Connected) {
        await connection.invoke("SendMessage", receiverId, message);
      } else {
        // SignalR bağlı değilse REST ile gönder; cevap olarak gelen mesajı listeye ekle
        const sent = await chatApi.send(receiverId, message);
        qc.setQueryData<ChatMessage[]>(["chat", receiverId], (list) =>
          list ? (list.some((m) => m.id === sent.id) ? list : [...list, sent]) : list,
        );
        qc.invalidateQueries({ queryKey: ["conversations"] });
      }
    },
    [qc],
  );

  const markThreadRead = useCallback(
    (userId: string) => {
      const unread = (qc.getQueryData<AppNotification[]>(["notifications"]) ?? []).filter(
        (n) => n.type === "Message" && n.senderId === userId && !n.isRead,
      );
      if (!unread.length) return;
      qc.setQueryData<AppNotification[]>(["notifications"], (list) =>
        list?.map((n) => (unread.some((u) => u.id === n.id) ? { ...n, isRead: true } : n)),
      );
      unread.forEach((n) => notificationsApi.markRead(n.id).catch(() => undefined));
    },
    [qc],
  );

  const setActiveThread = useCallback((userId: string | null) => {
    activeThread.current = userId;
  }, []);

  const value = useMemo<RealtimeState>(() => {
    const list = notifications.data ?? [];
    const unreadMessagesBySender: Record<string, number> = {};
    let unreadNotifications = 0;
    for (const n of list) {
      if (n.isRead) continue;
      if (n.type === "Message") unreadMessagesBySender[n.senderId] = (unreadMessagesBySender[n.senderId] ?? 0) + 1;
      else unreadNotifications++;
    }
    return {
      status,
      unreadNotifications,
      unreadMessagesBySender,
      unreadMessages: Object.keys(unreadMessagesBySender).length,
      sendMessage,
      setActiveThread,
      markThreadRead,
    };
  }, [notifications.data, status, sendMessage, setActiveThread, markThreadRead]);

  return <RealtimeContext.Provider value={value}>{children}</RealtimeContext.Provider>;
}

export function useRealtime() {
  const ctx = useContext(RealtimeContext);
  if (!ctx) throw new Error("useRealtime must be used inside RealtimeProvider");
  return ctx;
}

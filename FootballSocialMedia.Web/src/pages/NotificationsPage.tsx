import { useEffect } from "react";
import { Link } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { MessageCircle, UserPlus } from "lucide-react";
import { notificationsApi } from "../api/endpoints";
import type { AppNotification } from "../api/types";
import { BallIcon } from "../components/BallIcon";
import { Empty, ErrorBox, Spinner } from "../components/Feedback";
import { timeAgo } from "../lib/format";
import { notificationText } from "../lib/notifications";

export function NotificationsPage() {
  const qc = useQueryClient();
  const { data, isLoading, isError, error } = useQuery({ queryKey: ["notifications"], queryFn: notificationsApi.list });

  // Mesaj bildirimleri Mesajlar sayfasında gösteriliyor
  const items = (data ?? []).filter((n) => n.type !== "Message");
  const hasUnread = items.some((n) => !n.isRead);

  const markAll = useMutation({
    mutationFn: notificationsApi.markAllRead,
    onSuccess: () =>
      qc.setQueryData<AppNotification[]>(["notifications"], (list) =>
        list?.map((n) => (n.type === "Message" ? n : { ...n, isRead: true })),
      ),
  });

  // Sayfa açıldıktan kısa süre sonra hepsini okundu say (vurgular bir an görünsün)
  useEffect(() => {
    if (!hasUnread) return;
    const t = setTimeout(() => markAll.mutate(), 2500);
    return () => clearTimeout(t);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hasUnread]);

  return (
    <>
      <header className="page-head">
        <h1>Bildirimler</h1>
      </header>
      {isLoading && <Spinner />}
      {isError && <ErrorBox error={error} />}
      {data && items.length === 0 && (
        <Empty title="Henüz bildirim yok">Biri gönderini beğendiğinde, yorum yaptığında ya da seni takip ettiğinde burada görünecek.</Empty>
      )}
      {items.map((n) => (
        <Link
          key={n.id}
          to={n.postId ? `/gonderi/${n.postId}` : `/profil/${n.senderId}`}
          className={`notif${n.isRead ? "" : " unread"}`}
        >
          <span className={`kind kind-${n.type.toLowerCase()}`} aria-hidden="true">
            {n.type === "Like" && <BallIcon size={18} />}
            {n.type === "Comment" && <MessageCircle size={18} />}
            {n.type === "Follow" && <UserPlus size={18} />}
          </span>
          <span className="text">
            {notificationText(n)}
            <time dateTime={n.createdAt}>{timeAgo(n.createdAt)}</time>
          </span>
        </Link>
      ))}
    </>
  );
}

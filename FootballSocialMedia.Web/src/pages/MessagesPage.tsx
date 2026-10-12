import { Fragment, useEffect, useRef, useState, type FormEvent } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { ArrowLeft, Search, Send } from "lucide-react";
import { chatApi, usersApi } from "../api/endpoints";
import type { ChatMessage } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { Avatar } from "../components/Avatar";
import { Empty, ErrorBox, Spinner } from "../components/Feedback";
import { useToast } from "../components/Toasts";
import { errorText } from "../lib/errors";
import { clockTime, timeAgo } from "../lib/format";
import { useRealtime } from "../realtime/RealtimeContext";

export function MessagesPage() {
  const { userId } = useParams();
  return (
    <div className={`messages${userId ? " has-thread" : ""}`}>
      <ConversationList activeId={userId} />
      {userId ? <Thread key={userId} otherId={userId} /> : <NoThread />}
    </div>
  );
}

function ConversationList({ activeId }: { activeId?: string }) {
  const { unreadMessagesBySender } = useRealtime();
  const navigate = useNavigate();
  const [query, setQuery] = useState("");
  const conversations = useQuery({ queryKey: ["conversations"], queryFn: chatApi.conversations });
  const people = useQuery({
    queryKey: ["users", "search", query],
    queryFn: () => usersApi.search(query),
    enabled: query.trim().length >= 2,
  });

  return (
    <section className="convo-list" aria-label="Sohbetler">
      <header className="page-head">
        <h1>Mesajlar</h1>
        <div style={{ paddingBottom: "0.9rem" }}>
        <div className="searchbox">
          <Search size={18} aria-hidden="true" />
          <label htmlFor="new-chat" className="visually-hidden">Yeni sohbet için kullanıcı ara</label>
          <input id="new-chat" className="input" placeholder="Yeni sohbet: kullanıcı ara" value={query}
            onChange={(e) => setQuery(e.target.value)} />
          {query.trim().length >= 2 && !!people.data?.length && (
            <div className="suggest">
              {people.data.map((u) => (
                <button key={u.id} onClick={() => { setQuery(""); navigate(`/mesajlar/${u.id}`); }}>
                  <Avatar name={u.userName} src={u.profileImageUrl} size={30} />
                  {u.userName}
                </button>
              ))}
            </div>
          )}
        </div>
        </div>
      </header>

      {conversations.isLoading && <Spinner />}
      {conversations.isError && <ErrorBox error={conversations.error} />}
      {conversations.data?.length === 0 && (
        <Empty title="Henüz mesaj yok">Yukarıdan bir kullanıcı ara ya da bir profilden "Mesaj" butonuna bas.</Empty>
      )}
      {conversations.data?.map((c) => (
        <Link key={c.userId} to={`/mesajlar/${c.userId}`} className={`convo${c.userId === activeId ? " active" : ""}`}>
          <Avatar name={c.userName} src={c.profileImageUrl} size={44} />
          <div className="who">
            <strong>
              <span>{c.userName}</span>
              <time dateTime={c.lastMessageTime}>{timeAgo(c.lastMessageTime)}</time>
            </strong>
            <p>{c.lastMessageIsMine ? "Sen: " : ""}{c.lastMessage}</p>
          </div>
          {!!unreadMessagesBySender[c.userId] && <span className="dot" aria-label="Okunmamış mesaj var" />}
        </Link>
      ))}
    </section>
  );
}

function NoThread() {
  return (
    <section className="thread">
      <Empty title="Bir sohbet seç">Soldaki listeden bir sohbet aç ya da yeni bir kullanıcı ara.</Empty>
    </section>
  );
}

function Thread({ otherId }: { otherId: string }) {
  const { user } = useAuth();
  const toast = useToast();
  const { sendMessage, setActiveThread, markThreadRead, status, unreadMessagesBySender } = useRealtime();
  const [text, setText] = useState("");
  const [sending, setSending] = useState(false);
  const bottom = useRef<HTMLDivElement>(null);

  const other = useQuery({ queryKey: ["profile", otherId], queryFn: () => usersApi.profile(otherId) });
  const history = useQuery({ queryKey: ["chat", otherId], queryFn: () => chatApi.history(otherId) });

  useEffect(() => {
    setActiveThread(otherId);
    return () => setActiveThread(null);
  }, [otherId, setActiveThread]);

  // Sohbet açıkken gelen mesaj bildirimlerini okundu say
  const unreadHere = unreadMessagesBySender[otherId] ?? 0;
  useEffect(() => {
    if (unreadHere) markThreadRead(otherId);
  }, [otherId, unreadHere, markThreadRead]);

  useEffect(() => {
    bottom.current?.scrollIntoView({ block: "end" });
  }, [history.data?.length]);

  async function onSubmit(e?: FormEvent) {
    e?.preventDefault();
    const message = text.trim();
    if (!message || sending) return;
    setSending(true);
    try {
      await sendMessage(otherId, message);
      setText("");
    } catch (err) {
      toast.error(errorText(err, "Mesaj gönderilemedi."));
    } finally {
      setSending(false);
    }
  }

  const name = other.data?.userName ?? "…";
  const isSelf = otherId === user?.id;

  return (
    <section className="thread" aria-label={`${name} ile sohbet`}>
      <header className="thread-head">
        <Link to="/mesajlar" className="icon-btn" aria-label="Sohbet listesine dön">
          <ArrowLeft size={20} />
        </Link>
        <Link to={`/profil/${otherId}`} style={{ display: "flex", alignItems: "center", gap: "0.75rem" }}>
          <Avatar name={name} src={other.data?.profileImageUrl} size={40} />
          <h2>{name}</h2>
        </Link>
        <span className={`conn-state${status === "connected" ? " on" : ""}`}>
          {status === "connected" ? "Canlı" : status === "reconnecting" || status === "connecting" ? "Bağlanıyor…" : "Bağlantı yok"}
        </span>
      </header>

      <div className="thread-body">
        {history.isLoading && <Spinner />}
        {history.isError && <ErrorBox error={history.error} />}
        {history.data?.length === 0 && <Empty title="İlk mesajı sen yaz">{name} ile henüz konuşmadınız.</Empty>}
        {history.data && <Bubbles messages={history.data} myId={user!.id} />}
        <div ref={bottom} />
      </div>

      {!isSelf && (
        <form className="thread-form" onSubmit={onSubmit}>
          <label htmlFor="chat-input" className="visually-hidden">Mesaj yaz</label>
          <textarea
            id="chat-input"
            className="textarea"
            rows={1}
            maxLength={2000}
            placeholder="Mesaj yaz"
            value={text}
            onChange={(e) => setText(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                onSubmit();
              }
            }}
          />
          <button className="btn btn-amber" disabled={!text.trim() || sending} aria-label="Gönder">
            <Send size={18} />
          </button>
        </form>
      )}
    </section>
  );
}

function Bubbles({ messages, myId }: { messages: ChatMessage[]; myId: string }) {
  let lastDay = "";
  return (
    <>
      {messages.map((m) => {
        const date = new Date(m.timestamp.endsWith("Z") ? m.timestamp : m.timestamp + "Z");
        const day = date.toLocaleDateString("tr-TR", { day: "numeric", month: "long", weekday: "long" });
        const showDay = day !== lastDay;
        lastDay = day;
        return (
          <Fragment key={m.id}>
            {showDay && <div className="day-sep">{day}</div>}
            <div className={`bubble${m.senderId === myId ? " mine" : ""}`}>
              {m.message}
              <time dateTime={m.timestamp}>{clockTime(m.timestamp)}</time>
            </div>
          </Fragment>
        );
      })}
    </>
  );
}

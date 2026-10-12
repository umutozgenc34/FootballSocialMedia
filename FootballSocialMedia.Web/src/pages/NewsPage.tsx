import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Plus, Trash2 } from "lucide-react";
import { newsApi } from "../api/endpoints";
import type { News } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { Empty, ErrorBox, Spinner } from "../components/Feedback";
import { Modal } from "../components/Modal";
import { useToast } from "../components/Toasts";
import { errorText } from "../lib/errors";
import { timeAgo } from "../lib/format";

export function NewsPage() {
  const { isAdmin } = useAuth();
  const [creating, setCreating] = useState(false);
  const [openId, setOpenId] = useState<string | null>(null);
  const { data, isLoading, isError, error } = useQuery({ queryKey: ["news"], queryFn: newsApi.list });

  return (
    <>
      <header className="page-head" style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
        <h1>Haberler</h1>
        {isAdmin && (
          <button className="btn btn-amber btn-small" onClick={() => setCreating(true)}>
            <Plus size={16} /> Haber ekle
          </button>
        )}
      </header>
      {isLoading && <Spinner />}
      {isError && <ErrorBox error={error} />}
      {data?.length === 0 && (
        <Empty title="Henüz haber yok">{isAdmin ? "İlk haberi sen ekle." : "Yeni haberler eklendiğinde burada okuyabilirsin."}</Empty>
      )}
      {data?.map((n) => (
        <NewsItem key={n.id} news={n} open={openId === n.id} onToggle={() => setOpenId(openId === n.id ? null : n.id)} />
      ))}
      {creating && <CreateNewsModal onClose={() => setCreating(false)} />}
    </>
  );
}

export function NewsItem({ news, open, onToggle, compact }: { news: News; open?: boolean; onToggle?: () => void; compact?: boolean }) {
  const { isAdmin } = useAuth();
  const qc = useQueryClient();
  const toast = useToast();
  const remove = useMutation({
    mutationFn: () => newsApi.remove(news.id),
    onSuccess: () => qc.setQueryData<News[]>(["news"], (list) => list?.filter((n) => n.id !== news.id)),
    onError: (err) => toast.error(errorText(err)),
  });

  return (
    <article className={`news-item${open ? " open" : ""}`}>
      <div>
        <h3>
          {onToggle ? (
            <button onClick={onToggle} aria-expanded={open}
              style={{ all: "unset", cursor: "pointer", font: "inherit" }}>
              {news.title}
            </button>
          ) : news.title}
        </h3>
        {!compact && <p className="excerpt">{news.content}</p>}
        <p className="src">
          {news.source}, {timeAgo(news.createdTime)}
          {isAdmin && !compact && (
            <button className="icon-btn" style={{ marginLeft: "0.5rem", verticalAlign: "middle" }} aria-label="Haberi sil"
              onClick={() => confirm("Haber silinsin mi?") && remove.mutate()}>
              <Trash2 size={15} />
            </button>
          )}
        </p>
      </div>
      {news.imageUrl ? <img src={news.imageUrl} alt="" loading="lazy" /> : <span />}
    </article>
  );
}

function CreateNewsModal({ onClose }: { onClose: () => void }) {
  const qc = useQueryClient();
  const toast = useToast();
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [source, setSource] = useState("");
  const [image, setImage] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);

  const create = useMutation({
    mutationFn: () => newsApi.create({ title: title.trim(), content: content.trim(), source: source.trim(), image: image! }),
    onSuccess: (news) => {
      qc.setQueryData<News[]>(["news"], (list) => [news, ...(list ?? [])]);
      toast.show("Haber yayınlandı.");
      onClose();
    },
    onError: (err) => setError(errorText(err)),
  });

  function onSubmit(e: FormEvent) {
    e.preventDefault();
    if (!image) {
      setError("Bir görsel seç.");
      return;
    }
    setError(null);
    create.mutate();
  }

  return (
    <Modal title="Haber ekle" onClose={onClose}>
      <form onSubmit={onSubmit} style={{ display: "grid", gap: "1rem" }}>
        <div className="field">
          <label htmlFor="news-title">Başlık</label>
          <input id="news-title" className="input" required maxLength={200} value={title} onChange={(e) => setTitle(e.target.value)} />
        </div>
        <div className="field">
          <label htmlFor="news-content">Metin</label>
          <textarea id="news-content" className="textarea" rows={6} required maxLength={5000} value={content}
            onChange={(e) => setContent(e.target.value)} />
        </div>
        <div className="field">
          <label htmlFor="news-source">Kaynak</label>
          <input id="news-source" className="input" required maxLength={255} placeholder="Örn. TFF, kulüp açıklaması"
            value={source} onChange={(e) => setSource(e.target.value)} />
        </div>
        <div className="field">
          <label htmlFor="news-image">Görsel</label>
          <input id="news-image" type="file" accept="image/*" onChange={(e) => setImage(e.target.files?.[0] ?? null)} />
        </div>
        {error && <p className="form-error" role="alert">{error}</p>}
        <div className="modal-foot" style={{ padding: 0 }}>
          <button type="button" className="btn btn-outline" onClick={onClose}>Vazgeç</button>
          <button className="btn btn-amber" disabled={create.isPending}>{create.isPending ? "Yükleniyor…" : "Yayınla"}</button>
        </div>
      </form>
    </Modal>
  );
}

import { useEffect, useRef, useState, type MouseEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { MessageCircle, MoreHorizontal, Pencil, Shirt, Trash2 } from "lucide-react";
import { likesApi, postsApi } from "../api/endpoints";
import type { Post } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { patchPost, removePost } from "../lib/cache";
import { errorText } from "../lib/errors";
import { compact, fullDate, timeAgo, wasEdited } from "../lib/format";
import { Avatar } from "./Avatar";
import { BallIcon } from "./BallIcon";
import { Modal } from "./Modal";
import { useToast } from "./Toasts";

interface Props {
  post: Post;
  /** Kart tıklanınca detay sayfasına gitsin mi (akışta evet, detay sayfasında hayır) */
  link?: boolean;
  detail?: boolean;
}

export function PostCard({ post, link = true, detail = false }: Props) {
  const navigate = useNavigate();
  const { user, isAdmin } = useAuth();
  const qc = useQueryClient();
  const toast = useToast();
  const [menuOpen, setMenuOpen] = useState(false);
  const [editing, setEditing] = useState(false);
  const isMine = user?.id === post.userId;

  // Değişken olarak "beğenilmiş mi olacak" bilgisini geçiyoruz; böylece hızlı çift tıklamada bile yön karışmaz.
  const like = useMutation({
    mutationFn: async (liked: boolean) => {
      if (liked) await likesApi.likePost(post.id);
      else await likesApi.unlikePost(post.id);
    },
    onMutate: (liked) => {
      patchPost(qc, post.id, (p) => ({ ...p, isLikedByMe: liked, likeCount: Math.max(0, p.likeCount + (liked ? 1 : -1)) }));
    },
    onError: (err, liked) => {
      // Optimistic güncellemeyi geri al
      patchPost(qc, post.id, (p) => ({ ...p, isLikedByMe: !liked, likeCount: Math.max(0, p.likeCount + (liked ? -1 : 1)) }));
      toast.error(errorText(err));
    },
  });

  const remove = useMutation({
    mutationFn: () => postsApi.remove(post.id),
    onSuccess: () => {
      removePost(qc, post.id);
      qc.invalidateQueries({ queryKey: ["profile", post.userId] });
      toast.show("Gönderi silindi.");
      if (detail) navigate("/", { replace: true });
    },
    onError: (err) => toast.error(errorText(err)),
  });

  function openDetail(e: MouseEvent) {
    if (!link) return;
    // Link, buton veya metin seçimi tıklamalarını detay sayfasına yönlendirme
    const target = e.target as HTMLElement;
    if (target.closest("a, button, video") || window.getSelection()?.toString()) return;
    navigate(`/gonderi/${post.id}`);
  }

  const media = [...post.imageUrls.map((url) => ({ url, type: "image" as const })), ...post.videoUrls.map((url) => ({ url, type: "video" as const }))];

  return (
    <article className={`post${link ? " is-link" : ""}${detail ? " post-detail" : ""}`} onClick={openDetail}>
      <Link to={`/profil/${post.userId}`} aria-label={`${post.username} profili`}>
        <Avatar name={post.username} src={post.profileImageUrl} size={detail ? 52 : 46} />
      </Link>

      <div className="post-body">
        <div className="post-meta">
          <Link className="name" to={`/profil/${post.userId}`}>{post.username}</Link>
          {post.favoriteTeam && (
            <span className="team-chip">
              <Shirt size={12} aria-hidden="true" />
              {post.favoriteTeam}
            </span>
          )}
          <time className="time" dateTime={post.createdTime} title={fullDate(post.createdTime)}>
            {timeAgo(post.createdTime)}
          </time>
          {wasEdited(post.createdTime, post.updatedTime) && <span className="edited">düzenlendi</span>}

          {(isMine || isAdmin) && (
            <PostMenu
              open={menuOpen}
              setOpen={setMenuOpen}
              canEdit={isMine}
              onEdit={() => setEditing(true)}
              onDelete={() => {
                if (confirm("Bu gönderiyi silmek istediğine emin misin? Yorumlar ve beğeniler de silinir.")) remove.mutate();
              }}
            />
          )}
        </div>

        <p className="post-text">{post.content}</p>

        {media.length > 0 && (
          <div className={`media-grid n${Math.min(media.length, 4)}`}>
            {media.slice(0, 4).map((m) =>
              m.type === "image" ? (
                <img key={m.url} src={m.url} alt="" loading="lazy" />
              ) : (
                <video key={m.url} src={m.url} controls preload="metadata" />
              ),
            )}
          </div>
        )}

        <div className="post-actions">
          <button
            className={`action${post.isLikedByMe ? " liked" : ""}`}
            onClick={() => like.mutate(!post.isLikedByMe)}
            disabled={like.isPending}
            aria-pressed={post.isLikedByMe}
            aria-label={post.isLikedByMe ? "Beğeniyi geri al" : "Beğen"}
          >
            <BallIcon size={19} />
            <span className="num">{compact(post.likeCount)}</span>
          </button>
          <Link className="action" to={`/gonderi/${post.id}`} aria-label="Yorumlar">
            <MessageCircle size={19} />
            <span className="num">{compact(post.commentCount)}</span>
          </Link>
        </div>
      </div>

      {editing && <EditPostModal post={post} onClose={() => setEditing(false)} />}
    </article>
  );
}

function PostMenu({ open, setOpen, canEdit, onEdit, onDelete }: {
  open: boolean; setOpen: (v: boolean) => void; canEdit: boolean; onEdit: () => void; onDelete: () => void;
}) {
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) return;
    const close = (e: Event) => !ref.current?.contains(e.target as Node) && setOpen(false);
    document.addEventListener("mousedown", close);
    return () => document.removeEventListener("mousedown", close);
  }, [open, setOpen]);

  return (
    <div className="post-menu" ref={ref}>
      <button className="icon-btn" aria-label="Gönderi seçenekleri" aria-expanded={open} onClick={() => setOpen(!open)}>
        <MoreHorizontal size={18} />
      </button>
      {open && (
        <div className="menu" role="menu">
          {canEdit && (
            <button role="menuitem" onClick={() => { setOpen(false); onEdit(); }}>
              <Pencil size={16} /> Düzenle
            </button>
          )}
          <button role="menuitem" className="danger" onClick={() => { setOpen(false); onDelete(); }}>
            <Trash2 size={16} /> Sil
          </button>
        </div>
      )}
    </div>
  );
}

function EditPostModal({ post, onClose }: { post: Post; onClose: () => void }) {
  const qc = useQueryClient();
  const toast = useToast();
  const [content, setContent] = useState(post.content);

  const save = useMutation({
    mutationFn: () => postsApi.update(post.id, content.trim()),
    onSuccess: () => {
      patchPost(qc, post.id, (p) => ({ ...p, content: content.trim(), updatedTime: new Date().toISOString() }));
      toast.show("Gönderi güncellendi.");
      onClose();
    },
    onError: (err) => toast.error(errorText(err)),
  });

  return (
    <Modal
      title="Gönderiyi düzenle"
      onClose={onClose}
      footer={
        <>
          <button className="btn btn-outline" onClick={onClose}>Vazgeç</button>
          <button className="btn btn-amber" disabled={!content.trim() || content.length > 500 || save.isPending} onClick={() => save.mutate()}>
            Kaydet
          </button>
        </>
      }
    >
      <textarea className="textarea" rows={5} value={content} maxLength={500} onChange={(e) => setContent(e.target.value)} />
    </Modal>
  );
}

import { useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Trash2 } from "lucide-react";
import { commentsApi, likesApi } from "../api/endpoints";
import type { Comment, Post } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { patchPost } from "../lib/cache";
import { errorText } from "../lib/errors";
import { compact, fullDate, timeAgo } from "../lib/format";
import { Avatar } from "./Avatar";
import { BallIcon } from "./BallIcon";
import { Empty, ErrorBox, Spinner } from "./Feedback";
import { useToast } from "./Toasts";

export function Comments({ post }: { post: Post }) {
  const { user } = useAuth();
  const qc = useQueryClient();
  const toast = useToast();
  const [text, setText] = useState("");
  const key = ["comments", post.id];

  const { data: comments, isLoading, isError, error } = useQuery({
    queryKey: key,
    queryFn: () => commentsApi.byPost(post.id),
  });

  const create = useMutation({
    mutationFn: () => commentsApi.create(post.id, text.trim()),
    onSuccess: (comment) => {
      qc.setQueryData<Comment[]>(key, (list) => [...(list ?? []), comment]);
      patchPost(qc, post.id, (p) => ({ ...p, commentCount: p.commentCount + 1 }));
      setText("");
    },
    onError: (err) => toast.error(errorText(err)),
  });

  function onSubmit(e: FormEvent) {
    e.preventDefault();
    if (text.trim()) create.mutate();
  }

  return (
    <section aria-label="Yorumlar">
      {user && (
        <form className="comment-form" onSubmit={onSubmit}>
          <Avatar name={user.userName} src={user.profileImageUrl} size={38} />
          <label htmlFor="comment-text" className="visually-hidden">Yorum yaz</label>
          <textarea
            id="comment-text"
            className="textarea"
            rows={1}
            maxLength={500}
            placeholder={`${post.username} kullanıcısına yanıt ver`}
            value={text}
            onChange={(e) => setText(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                if (text.trim() && !create.isPending) create.mutate();
              }
            }}
          />
          <button className="btn btn-amber" disabled={!text.trim() || create.isPending}>
            Yanıtla
          </button>
        </form>
      )}

      {isLoading && <Spinner />}
      {isError && <ErrorBox error={error} />}
      {comments?.length === 0 && <Empty title="Henüz yorum yok">İlk yorumu sen yaz.</Empty>}
      {comments?.map((c) => <CommentItem key={c.id} comment={c} post={post} />)}
    </section>
  );
}

function CommentItem({ comment, post }: { comment: Comment; post: Post }) {
  const { user, isAdmin } = useAuth();
  const qc = useQueryClient();
  const toast = useToast();
  const key = ["comments", post.id];
  const canDelete = user && (user.id === comment.userId || user.id === post.userId || isAdmin);

  const setComment = (patch: Partial<Comment>) =>
    qc.setQueryData<Comment[]>(key, (list) => list?.map((c) => (c.id === comment.id ? { ...c, ...patch } : c)));

  const like = useMutation({
    mutationFn: async (liked: boolean) => {
      if (liked) await likesApi.likeComment(comment.id);
      else await likesApi.unlikeComment(comment.id);
    },
    onMutate: (liked) => {
      setComment({ isLikedByMe: liked, likeCount: Math.max(0, comment.likeCount + (liked ? 1 : -1)) });
      return { previous: comment };
    },
    onError: (err, _liked, ctx) => {
      if (ctx) setComment(ctx.previous);
      toast.error(errorText(err));
    },
  });

  const remove = useMutation({
    mutationFn: () => commentsApi.remove(comment.id),
    onSuccess: () => {
      qc.setQueryData<Comment[]>(key, (list) => list?.filter((c) => c.id !== comment.id));
      patchPost(qc, post.id, (p) => ({ ...p, commentCount: Math.max(0, p.commentCount - 1) }));
    },
    onError: (err) => toast.error(errorText(err)),
  });

  return (
    <article className="comment">
      <Link to={`/profil/${comment.userId}`} aria-label={`${comment.username} profili`}>
        <Avatar name={comment.username} src={comment.profileImageUrl} size={38} />
      </Link>
      <div className="post-body">
        <div className="post-meta">
          <Link className="name" to={`/profil/${comment.userId}`}>{comment.username}</Link>
          <time className="time" dateTime={comment.createdTime} title={fullDate(comment.createdTime)}>
            {timeAgo(comment.createdTime)}
          </time>
          {canDelete && (
            <span className="post-menu">
              <button
                className="icon-btn"
                aria-label="Yorumu sil"
                onClick={() => confirm("Yorum silinsin mi?") && remove.mutate()}
                disabled={remove.isPending}
              >
                <Trash2 size={16} />
              </button>
            </span>
          )}
        </div>
        <p className="post-text">{comment.content}</p>
        <div className="post-actions">
          <button
            className={`action${comment.isLikedByMe ? " liked" : ""}`}
            onClick={() => like.mutate(!comment.isLikedByMe)}
            disabled={like.isPending}
            aria-pressed={comment.isLikedByMe}
            aria-label={comment.isLikedByMe ? "Beğeniyi geri al" : "Beğen"}
          >
            <BallIcon size={17} />
            <span className="num">{compact(comment.likeCount)}</span>
          </button>
        </div>
      </div>
    </article>
  );
}

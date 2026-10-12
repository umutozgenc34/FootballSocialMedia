import { useEffect, useMemo, useRef, useState, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { ImagePlus, X } from "lucide-react";
import { postsApi } from "../api/endpoints";
import { useAuth } from "../auth/AuthContext";
import { prependPost } from "../lib/cache";
import { errorText } from "../lib/errors";
import { Avatar } from "./Avatar";
import { useToast } from "./Toasts";

const MAX_LENGTH = 500;
const MAX_FILES = 4;

interface Props {
  autoFocus?: boolean;
  onPosted?: () => void;
}

export function Composer({ autoFocus, onPosted }: Props) {
  const { user } = useAuth();
  const qc = useQueryClient();
  const toast = useToast();
  const [content, setContent] = useState("");
  const [files, setFiles] = useState<File[]>([]);
  const fileInput = useRef<HTMLInputElement>(null);
  const textarea = useRef<HTMLTextAreaElement>(null);

  const previews = useMemo(() => files.map((f) => ({ file: f, url: URL.createObjectURL(f) })), [files]);
  useEffect(() => () => previews.forEach((p) => URL.revokeObjectURL(p.url)), [previews]);

  // Metin uzadıkça textarea büyüsün
  useEffect(() => {
    const el = textarea.current;
    if (!el) return;
    el.style.height = "auto";
    el.style.height = `${el.scrollHeight}px`;
  }, [content]);

  const create = useMutation({
    mutationFn: () =>
      postsApi.create(
        content.trim(),
        files.filter((f) => f.type.startsWith("image/")),
        files.filter((f) => f.type.startsWith("video/")),
      ),
    onSuccess: (post) => {
      prependPost(qc, post);
      qc.invalidateQueries({ queryKey: ["profile", post.userId] });
      setContent("");
      setFiles([]);
      toast.show("Gönderin paylaşıldı.");
      onPosted?.();
    },
    onError: (err) => toast.error(errorText(err)),
  });

  if (!user) return null;

  const tooLong = content.length > MAX_LENGTH;
  const canSubmit = content.trim().length > 0 && !tooLong && !create.isPending;

  function onSubmit(e: FormEvent) {
    e.preventDefault();
    if (canSubmit) create.mutate();
  }

  function addFiles(list: FileList | null) {
    if (!list) return;
    const picked = Array.from(list).filter((f) => f.type.startsWith("image/") || f.type.startsWith("video/"));
    const next = [...files, ...picked].slice(0, MAX_FILES);
    if (files.length + picked.length > MAX_FILES) toast.show(`En fazla ${MAX_FILES} dosya ekleyebilirsin.`);
    setFiles(next);
  }

  return (
    <form className="composer" onSubmit={onSubmit}>
      <Avatar name={user.userName} src={user.profileImageUrl} size={46} />
      <div className="composer-body">
        <label htmlFor="composer-text" className="visually-hidden">Gönderi metni</label>
        <textarea
          id="composer-text"
          ref={textarea}
          value={content}
          autoFocus={autoFocus}
          placeholder="Maç hakkında ne düşünüyorsun?"
          onChange={(e) => setContent(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter" && (e.ctrlKey || e.metaKey) && canSubmit) create.mutate();
          }}
        />

        {previews.length > 0 && (
          <div className="attach-previews">
            {previews.map((p, i) => (
              <figure key={p.url}>
                {p.file.type.startsWith("video/") ? <video src={p.url} muted /> : <img src={p.url} alt="" />}
                <button type="button" aria-label="Dosyayı kaldır" onClick={() => setFiles(files.filter((_, j) => j !== i))}>
                  <X size={14} />
                </button>
              </figure>
            ))}
          </div>
        )}

        <div className="composer-actions">
          <input
            ref={fileInput}
            type="file"
            accept="image/*,video/*"
            multiple
            hidden
            onChange={(e) => {
              addFiles(e.target.files);
              e.target.value = "";
            }}
          />
          <button
            type="button"
            className="btn btn-ghost"
            onClick={() => fileInput.current?.click()}
            disabled={files.length >= MAX_FILES}
            title="Fotoğraf veya video ekle"
          >
            <ImagePlus size={20} />
            <span className="visually-hidden">Fotoğraf veya video ekle</span>
          </button>
          <span className={`count${tooLong ? " over" : ""}`} aria-live="polite">
            {content.length > MAX_LENGTH - 80 ? `${MAX_LENGTH - content.length}` : ""}
          </span>
          <button className="btn btn-amber" disabled={!canSubmit}>
            {create.isPending ? "Paylaşılıyor…" : "Paylaş"}
          </button>
        </div>
      </div>
    </form>
  );
}

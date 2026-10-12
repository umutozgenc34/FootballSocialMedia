import { useEffect, useRef, useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { Search } from "lucide-react";
import { searchApi } from "../api/endpoints";
import { useDebounced } from "../lib/useDebounced";
import { Avatar } from "./Avatar";

/** Kullanıcı ve gönderi önerili arama kutusu. Enter → arama sonuç sayfası. */
export function SearchBox({ initial = "", autoFocus }: { initial?: string; autoFocus?: boolean }) {
  const navigate = useNavigate();
  const [text, setText] = useState(initial);
  const [open, setOpen] = useState(false);
  const wrap = useRef<HTMLFormElement>(null);
  const prefix = useDebounced(text.trim());

  useEffect(() => setText(initial), [initial]);

  const suggestions = useQuery({
    queryKey: ["autocomplete", prefix],
    queryFn: () => searchApi.autocomplete(prefix),
    enabled: open && prefix.length >= 2,
    staleTime: 60_000,
  });

  useEffect(() => {
    const close = (e: MouseEvent) => !wrap.current?.contains(e.target as Node) && setOpen(false);
    document.addEventListener("mousedown", close);
    return () => document.removeEventListener("mousedown", close);
  }, []);

  function go(q: string) {
    setOpen(false);
    if (q.trim()) navigate(`/ara?q=${encodeURIComponent(q.trim())}`);
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault();
    go(text);
  }

  const data = suggestions.data;
  const hasSuggestions = open && prefix.length >= 2 && !!data && (data.users.length > 0 || data.posts.length > 0);

  return (
    <form className="searchbox" role="search" onSubmit={onSubmit} ref={wrap}>
      <Search size={18} aria-hidden="true" />
      <label htmlFor="search-input" className="visually-hidden">Ara</label>
      <input
        id="search-input"
        className="input"
        type="search"
        placeholder="Kullanıcı, takım veya gönderi ara"
        autoComplete="off"
        autoFocus={autoFocus}
        value={text}
        onChange={(e) => { setText(e.target.value); setOpen(true); }}
        onFocus={() => setOpen(true)}
        onKeyDown={(e) => e.key === "Escape" && setOpen(false)}
      />
      {hasSuggestions && (
        <div className="suggest">
          {data.users.map((u) => (
            <button type="button" key={u.id} onClick={() => { setOpen(false); navigate(`/profil/${u.id}`); }}>
              <Avatar name={u.userName} src={u.profileImageUrl} size={30} />
              <strong>{u.userName}</strong>
              {u.favoriteTeam && <span className="team-chip">{u.favoriteTeam}</span>}
            </button>
          ))}
          {data.posts.map((p) => (
            <button type="button" key={p} onClick={() => go(p.slice(0, 60))}>
              <Search size={16} aria-hidden="true" />
              <span className="post-suggest">{p}</span>
            </button>
          ))}
        </div>
      )}
    </form>
  );
}

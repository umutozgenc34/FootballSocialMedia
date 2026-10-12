import { useSearchParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { searchApi } from "../api/endpoints";
import { useAuth } from "../auth/AuthContext";
import { Empty, ErrorBox, Spinner } from "../components/Feedback";
import { FollowButton } from "../components/FollowButton";
import { PostCard } from "../components/PostCard";
import { SearchBox } from "../components/SearchBox";
import { UserRow } from "../components/UserRow";

export function SearchPage() {
  const [params] = useSearchParams();
  const q = params.get("q")?.trim() ?? "";
  const { user } = useAuth();

  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["search", q],
    queryFn: () => searchApi.search(q),
    enabled: q.length > 0,
  });

  return (
    <>
      <header className="page-head" style={{ paddingBottom: "1rem" }}>
        <h1>Ara</h1>
        <SearchBox initial={q} autoFocus={!q} />
      </header>

      {!q && <Empty title="Ne arıyorsun?">Bir kullanıcı adı, takım ya da maçla ilgili bir kelime yaz.</Empty>}
      {isLoading && <Spinner />}
      {isError && <ErrorBox error={error} />}

      {data && (
        <>
          {data.users.length > 0 && (
            <>
              <h2 className="section-title">Kişiler</h2>
              {data.users.map((u) => (
                <UserRow
                  key={u.id}
                  id={u.id}
                  userName={u.userName}
                  profileImageUrl={u.profileImageUrl}
                  subtitle={u.favoriteTeam}
                  action={u.id !== user?.id ? <FollowButton userId={u.id} following={false} small /> : undefined}
                />
              ))}
            </>
          )}

          <h2 className="section-title">Gönderiler</h2>
          {data.engine === "sql" && (
            <p className="engine-note">Elasticsearch'e ulaşılamadı, basit arama sonuçları gösteriliyor.</p>
          )}
          {data.posts.length === 0 ? (
            <Empty title="Eşleşen gönderi yok">Farklı bir kelimeyle tekrar dene.</Empty>
          ) : (
            data.posts.map((p) => <PostCard key={p.id} post={p} />)
          )}
        </>
      )}
    </>
  );
}

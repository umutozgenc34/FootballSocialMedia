import { useInfiniteQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { PAGE_SIZE, postsApi, type FeedMode } from "../api/endpoints";
import { Composer } from "../components/Composer";
import { Empty } from "../components/Feedback";
import { PostList } from "../components/PostList";
import { useLocalStorage } from "../lib/useLocalStorage";

export function FeedPage() {
  const [mode, setMode] = useLocalStorage<FeedMode>("fsm.feedMode", "all");

  const query = useInfiniteQuery({
    queryKey: ["posts", "feed", mode],
    queryFn: ({ pageParam }) => postsApi.feed(mode, pageParam),
    initialPageParam: 1,
    getNextPageParam: (last, pages) => (last.length === PAGE_SIZE ? pages.length + 1 : undefined),
  });

  return (
    <>
      <header className="page-head">
        <h1>Akış</h1>
        <div className="tabs" role="tablist" aria-label="Akış türü">
          <button className="tab" role="tab" aria-selected={mode === "all"} onClick={() => setMode("all")}>
            Herkes
          </button>
          <button className="tab" role="tab" aria-selected={mode === "following"} onClick={() => setMode("following")}>
            Takip ettiklerim
          </button>
        </div>
      </header>

      <Composer />

      <PostList
        query={query}
        empty={
          mode === "following" ? (
            <Empty title="Burası henüz sessiz" action={<Link className="btn btn-outline" to="/ara">Takip edecek kişi bul</Link>}>
              Takip ettiğin kişilerin gönderileri burada görünecek.
            </Empty>
          ) : (
            <Empty title="İlk düdüğü sen çal">Henüz kimse bir şey paylaşmadı. Maç hakkında ilk yorumu sen yaz.</Empty>
          )
        }
      />
    </>
  );
}

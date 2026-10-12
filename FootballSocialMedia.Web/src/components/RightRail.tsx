import { Link, useLocation } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { newsApi, pollsApi, usersApi } from "../api/endpoints";
import { NewsItem } from "../pages/NewsPage";
import { FollowButton } from "./FollowButton";
import { PollCard } from "./PollCard";
import { SearchBox } from "./SearchBox";
import { UserRow } from "./UserRow";

export function RightRail() {
  const { pathname } = useLocation();
  const suggestions = useQuery({ queryKey: ["follows", "suggestions"], queryFn: () => usersApi.suggestions(5) });
  const polls = useQuery({ queryKey: ["polls"], queryFn: pollsApi.list });
  const news = useQuery({ queryKey: ["news"], queryFn: newsApi.list });

  const activePoll = polls.data?.find((p) => !p.isExpired);

  return (
    <>
      {pathname !== "/ara" && <SearchBox />}

      {activePoll && pathname !== "/anketler" && (
        <section className="side-section">
          <h2>Günün anketi</h2>
          <PollCard poll={activePoll} />
          <Link className="more" to="/anketler">Tüm anketler</Link>
        </section>
      )}

      {!!suggestions.data?.length && (
        <section className="side-section">
          <h2>Kimi takip etmeli</h2>
          {suggestions.data.map((u) => (
            <UserRow
              key={u.id}
              id={u.id}
              userName={u.userName}
              profileImageUrl={u.profileImageUrl}
              subtitle={u.favoriteTeam || "Takım seçilmedi"}
              action={<FollowButton userId={u.id} following={false} small />}
            />
          ))}
        </section>
      )}

      {!!news.data?.length && pathname !== "/haberler" && (
        <section className="side-section">
          <h2>Son haberler</h2>
          {news.data.slice(0, 3).map((n) => (
            <Link key={n.id} to="/haberler">
              <NewsItem news={n} compact />
            </Link>
          ))}
          <Link className="more" to="/haberler">Tüm haberler</Link>
        </section>
      )}
    </>
  );
}

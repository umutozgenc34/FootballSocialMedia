import { useQuery } from "@tanstack/react-query";
import { usersApi } from "../api/endpoints";
import { FollowButton } from "./FollowButton";
import { UserRow } from "./UserRow";

export function RightRail() {
  const suggestions = useQuery({ queryKey: ["follows", "suggestions"], queryFn: () => usersApi.suggestions(5) });

  return (
    <>
      {!!suggestions.data?.length && (
        <section className="side-section">
          <h2>Kimi takip etmeli</h2>
          {suggestions.data.map((u) => (
            <UserRow
              key={u.id}
              id={u.id}
              userName={u.userName}
              profileImageUrl={u.profileImageUrl}
              subtitle={u.favoriteTeam}
              action={<FollowButton userId={u.id} following={false} small />}
            />
          ))}
        </section>
      )}
    </>
  );
}

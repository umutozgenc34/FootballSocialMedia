import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useInfiniteQuery, useQuery } from "@tanstack/react-query";
import { CalendarDays, MessageCircle, Shirt } from "lucide-react";
import { followsApi, PAGE_SIZE, postsApi, usersApi } from "../api/endpoints";
import type { Follow } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { Avatar } from "../components/Avatar";
import { EditProfileModal } from "../components/EditProfileModal";
import { Empty, ErrorBox, Spinner } from "../components/Feedback";
import { FollowButton } from "../components/FollowButton";
import { PitchMarks } from "../components/PitchMarks";
import { PostList } from "../components/PostList";
import { UserRow } from "../components/UserRow";
import { compact, fullDate } from "../lib/format";

type Tab = "posts" | "followers" | "following";

export function ProfilePage() {
  const { id = "" } = useParams();
  const { user } = useAuth();
  const [tab, setTab] = useState<Tab>("posts");
  const [editing, setEditing] = useState(false);

  const profile = useQuery({ queryKey: ["profile", id], queryFn: () => usersApi.profile(id) });

  const posts = useInfiniteQuery({
    queryKey: ["posts", "user", id],
    queryFn: ({ pageParam }) => postsApi.byUser(id, pageParam),
    initialPageParam: 1,
    getNextPageParam: (last, pages) => (last.length === PAGE_SIZE ? pages.length + 1 : undefined),
    enabled: tab === "posts",
  });

  if (profile.isLoading) return <Spinner />;
  if (profile.isError) return <ErrorBox error={profile.error} />;
  const p = profile.data!;
  // Kendi profilimde güncel bilgiyi auth context'ten al (düzenleme sonrası anında yansısın)
  const shown = p.isMe && user ? { ...p, ...user, isMe: true } : p;

  return (
    <>
      <div className="profile-cover">
        <PitchMarks />
      </div>
      <section className="profile-head">
        <div className="top">
          <Avatar name={shown.userName} src={shown.profileImageUrl} size={104} />
          <div className="actions">
            {shown.isMe ? (
              <button className="btn btn-outline" onClick={() => setEditing(true)}>Profili düzenle</button>
            ) : (
              <>
                <Link className="btn btn-outline" to={`/mesajlar/${shown.id}`} aria-label="Mesaj gönder">
                  <MessageCircle size={18} /> Mesaj
                </Link>
                <FollowButton key={`${shown.id}-${shown.isFollowedByMe}`} userId={shown.id} following={shown.isFollowedByMe} />
              </>
            )}
          </div>
        </div>

        <h1>{shown.userName}</h1>
        {shown.bio && <p className="bio">{shown.bio}</p>}
        <div className="facts">
          {shown.favoriteTeam && (
            <span className="team-chip"><Shirt size={13} aria-hidden="true" />{shown.favoriteTeam}</span>
          )}
          <span><CalendarDays size={15} aria-hidden="true" />{fullDate(shown.createdTime)} tarihinde katıldı</span>
        </div>

        <div className="scoreline">
          <button onClick={() => setTab("posts")}>
            <strong>{compact(p.postCount)}</strong> gönderi
          </button>
          <button onClick={() => setTab("followers")}>
            <strong>{compact(p.followerCount)}</strong> takipçi
          </button>
          <button onClick={() => setTab("following")}>
            <strong>{compact(p.followingCount)}</strong> takip
          </button>
        </div>
      </section>

      <div className="page-head" style={{ paddingTop: 0 }}>
        <div className="tabs" role="tablist">
          <button className="tab" role="tab" aria-selected={tab === "posts"} onClick={() => setTab("posts")}>Gönderiler</button>
          <button className="tab" role="tab" aria-selected={tab === "followers"} onClick={() => setTab("followers")}>Takipçiler</button>
          <button className="tab" role="tab" aria-selected={tab === "following"} onClick={() => setTab("following")}>Takip edilenler</button>
        </div>
      </div>

      {tab === "posts" && (
        <PostList
          query={posts}
          empty={
            <Empty title={shown.isMe ? "Henüz bir şey paylaşmadın" : "Henüz gönderi yok"}>
              {shown.isMe ? "İlk gönderini akıştan paylaşabilirsin." : `${shown.userName} henüz bir şey paylaşmadı.`}
            </Empty>
          }
        />
      )}
      {tab !== "posts" && <FollowList userId={id} kind={tab} isMe={shown.isMe} />}

      {editing && <EditProfileModal profile={shown} onClose={() => setEditing(false)} />}
    </>
  );
}

function FollowList({ userId, kind, isMe }: { userId: string; kind: "followers" | "following"; isMe: boolean }) {
  const { user } = useAuth();
  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["follows", kind, userId],
    queryFn: () => (kind === "followers" ? followsApi.followers(userId) : followsApi.following(userId)),
  });
  // Butonların doğru durumda başlaması için kendi takip ettiklerimi de çek
  const myFollowing = useQuery({
    queryKey: ["follows", "following", user?.id],
    queryFn: () => followsApi.following(user!.id),
    enabled: !!user,
  });

  if (isLoading || myFollowing.isLoading) return <Spinner />;
  if (isError) return <ErrorBox error={error} />;
  if (!data?.length)
    return (
      <Empty title={kind === "followers" ? "Henüz takipçi yok" : "Henüz kimse takip edilmiyor"}>
        {kind === "followers"
          ? isMe ? "Paylaştıkça takipçin artacak." : "İlk takip eden sen ol."
          : isMe ? "Akışını doldurmak için birkaç kişiyi takip et." : null}
      </Empty>
    );

  const followingIds = new Set(myFollowing.data?.map((f) => f.followingId));
  const person = (f: Follow) =>
    kind === "followers"
      ? { id: f.followerId, name: f.followerUserName ?? "?", img: f.followerProfileImageUrl }
      : { id: f.followingId, name: f.followingUserName ?? "?", img: f.followingProfileImageUrl };

  return (
    <>
      {data.map((f) => {
        const u = person(f);
        return (
          <UserRow
            key={u.id}
            id={u.id}
            userName={u.name}
            profileImageUrl={u.img}
            subtitle={null}
            action={u.id !== user?.id ? <FollowButton userId={u.id} following={followingIds.has(u.id)} small /> : undefined}
          />
        );
      })}
    </>
  );
}

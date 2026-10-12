import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { followsApi } from "../api/endpoints";
import type { UserProfile } from "../api/types";
import { errorText } from "../lib/errors";
import { useToast } from "./Toasts";
import { useAuth } from "../auth/AuthContext";

interface Props {
  userId: string;
  following: boolean;
  small?: boolean;
}

export function FollowButton({ userId, following: initial, small }: Props) {
  const qc = useQueryClient();
  const toast = useToast();
  const { user } = useAuth();
  const [following, setFollowing] = useState(initial);
  const [hover, setHover] = useState(false);

  // Hedef durumu değişken olarak geçiyoruz: optimistic setFollowing yeniden render tetikleyince
  // mutationFn'in güncel (ters çevrilmiş) state'i okuyup yanlış isteği atmasını engeller.
  const toggle = useMutation({
    mutationFn: async (follow: boolean) => {
      if (follow) await followsApi.follow(userId);
      else await followsApi.unfollow(userId);
    },
    onMutate: (follow) => setFollowing(follow),
    onSuccess: (_data, follow) => {
      qc.setQueryData<UserProfile>(["profile", userId], (p) =>
        p ? { ...p, isFollowedByMe: follow, followerCount: Math.max(0, p.followerCount + (follow ? 1 : -1)) } : p,
      );
      qc.invalidateQueries({ queryKey: ["posts", "feed", "following"] });
      qc.invalidateQueries({ queryKey: ["follows"] });
      if (user) qc.invalidateQueries({ queryKey: ["profile", user.id] });
    },
    onError: (err, follow) => {
      setFollowing(!follow);
      toast.error(errorText(err));
    },
  });

  return (
    <button
      className={`btn ${following ? "btn-outline" : "btn-amber"}${small ? " btn-small" : ""}`}
      onClick={() => toggle.mutate(!following)}
      disabled={toggle.isPending}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      aria-pressed={following}
    >
      {following ? (hover ? "Takibi bırak" : "Takip ediliyor") : "Takip et"}
    </button>
  );
}

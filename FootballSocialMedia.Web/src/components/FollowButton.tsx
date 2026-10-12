import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { followsApi } from "../api/endpoints";
import type { UserProfile } from "../api/types";
import { errorText } from "../lib/errors";
import { useToast } from "./Toasts";

interface Props {
  userId: string;
  following: boolean;
  small?: boolean;
}

export function FollowButton({ userId, following: initial, small }: Props) {
  const qc = useQueryClient();
  const toast = useToast();
  const [following, setFollowing] = useState(initial);
  const [hover, setHover] = useState(false);

  const toggle = useMutation({
    mutationFn: () => (following ? followsApi.unfollow(userId) : followsApi.follow(userId)),
    onMutate: () => setFollowing(!following),
    onSuccess: () => {
      const now = !following;
      qc.setQueryData<UserProfile>(["profile", userId], (p) =>
        p ? { ...p, isFollowedByMe: now, followerCount: p.followerCount + (now ? 1 : -1) } : p,
      );
      qc.invalidateQueries({ queryKey: ["posts", "feed", "following"] });
      qc.invalidateQueries({ queryKey: ["follows"] });
      qc.invalidateQueries({ queryKey: ["me-profile"] });
    },
    onError: (err) => {
      setFollowing(following);
      toast.error(errorText(err));
    },
  });

  return (
    <button
      className={`btn ${following ? "btn-outline" : "btn-amber"}${small ? " btn-small" : ""}`}
      onClick={() => toggle.mutate()}
      disabled={toggle.isPending}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      aria-pressed={following}
    >
      {following ? (hover ? "Takibi bırak" : "Takip ediliyor") : "Takip et"}
    </button>
  );
}

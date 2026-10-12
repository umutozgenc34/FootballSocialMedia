import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Trash2 } from "lucide-react";
import { pollsApi } from "../api/endpoints";
import type { Poll } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { errorText } from "../lib/errors";
import { fullDate, timeAgo } from "../lib/format";
import { useToast } from "./Toasts";

export function PollCard({ poll }: { poll: Poll }) {
  const { isAdmin } = useAuth();
  const qc = useQueryClient();
  const toast = useToast();
  const voted = !!poll.myVoteOptionId;
  const showResults = voted || poll.isExpired;

  const vote = useMutation({
    mutationFn: (optionId: string) => pollsApi.vote(poll.id, optionId),
    onSuccess: (updated) => qc.setQueryData<Poll[]>(["polls"], (list) => list?.map((p) => (p.id === updated.id ? updated : p))),
    onError: (err) => toast.error(errorText(err)),
  });

  const remove = useMutation({
    mutationFn: () => pollsApi.remove(poll.id),
    onSuccess: () => qc.setQueryData<Poll[]>(["polls"], (list) => list?.filter((p) => p.id !== poll.id)),
    onError: (err) => toast.error(errorText(err)),
  });

  return (
    <article className="poll">
      <div className="post-meta" style={{ marginBottom: "0.35rem" }}>
        <h3 style={{ margin: 0 }}>{poll.question}</h3>
        {isAdmin && (
          <span className="post-menu">
            <button className="icon-btn" aria-label="Anketi sil" onClick={() => confirm("Anket silinsin mi?") && remove.mutate()}>
              <Trash2 size={16} />
            </button>
          </span>
        )}
      </div>
      <div className="poll-options" role="group" aria-label={poll.question}>
        {poll.options.map((o) => {
          const pct = poll.totalVotes ? Math.round((o.voteCount / poll.totalVotes) * 100) : 0;
          const mine = o.id === poll.myVoteOptionId;
          return showResults ? (
            <div key={o.id} className={`poll-option${mine ? " mine" : ""}`} aria-label={`${o.optionText}: yüzde ${pct}`}>
              <span className="fill" style={{ width: `${pct}%` }} />
              <span>{o.optionText}{mine && " ✓"}</span>
              <span className="pct">%{pct}</span>
            </div>
          ) : (
            <button key={o.id} className="poll-option" disabled={vote.isPending} onClick={() => vote.mutate(o.id)}>
              <span>{o.optionText}</span>
            </button>
          );
        })}
      </div>
      <div className="poll-foot">
        <span>{poll.totalVotes} oy</span>
        <span title={fullDate(poll.expiryDate)}>
          {poll.isExpired ? "Sona erdi" : `Bitiş: ${timeAgo(poll.expiryDate)}`}
        </span>
      </div>
    </article>
  );
}

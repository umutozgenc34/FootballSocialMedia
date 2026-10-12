import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Plus, X } from "lucide-react";
import { pollsApi } from "../api/endpoints";
import type { Poll } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { Empty, ErrorBox, Spinner } from "../components/Feedback";
import { Modal } from "../components/Modal";
import { PollCard } from "../components/PollCard";
import { useToast } from "../components/Toasts";
import { errorText } from "../lib/errors";

export function PollsPage() {
  const { isAdmin } = useAuth();
  const [creating, setCreating] = useState(false);
  const { data, isLoading, isError, error } = useQuery({ queryKey: ["polls"], queryFn: pollsApi.list });

  const active = data?.filter((p) => !p.isExpired) ?? [];
  const finished = data?.filter((p) => p.isExpired) ?? [];

  return (
    <>
      <header className="page-head" style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
        <h1>Anketler</h1>
        {isAdmin && (
          <button className="btn btn-amber btn-small" onClick={() => setCreating(true)}>
            <Plus size={16} /> Yeni anket
          </button>
        )}
      </header>
      {isLoading && <Spinner />}
      {isError && <ErrorBox error={error} />}
      {data?.length === 0 && (
        <Empty title="Henüz anket yok">{isAdmin ? "İlk anketi sen oluştur." : "Yeni anketler açıldığında burada oy verebilirsin."}</Empty>
      )}
      {active.map((p) => <PollCard key={p.id} poll={p} />)}
      {finished.length > 0 && (
        <>
          <h2 className="section-title">Sona erenler</h2>
          {finished.map((p) => <PollCard key={p.id} poll={p} />)}
        </>
      )}
      {creating && <CreatePollModal onClose={() => setCreating(false)} />}
    </>
  );
}

function CreatePollModal({ onClose }: { onClose: () => void }) {
  const qc = useQueryClient();
  const toast = useToast();
  const [question, setQuestion] = useState("");
  const [options, setOptions] = useState(["", ""]);
  const [days, setDays] = useState(3);
  const [error, setError] = useState<string | null>(null);

  const create = useMutation({
    mutationFn: () => {
      const expiry = new Date(Date.now() + days * 86_400_000).toISOString();
      return pollsApi.create(question.trim(), options.map((o) => o.trim()).filter(Boolean), expiry);
    },
    onSuccess: (poll) => {
      qc.setQueryData<Poll[]>(["polls"], (list) => [poll, ...(list ?? [])]);
      toast.show("Anket yayınlandı.");
      onClose();
    },
    onError: (err) => setError(errorText(err)),
  });

  function onSubmit(e: FormEvent) {
    e.preventDefault();
    if (options.filter((o) => o.trim()).length < 2) {
      setError("En az iki seçenek gir.");
      return;
    }
    setError(null);
    create.mutate();
  }

  return (
    <Modal title="Yeni anket" onClose={onClose}>
      <form onSubmit={onSubmit} style={{ display: "grid", gap: "1rem" }}>
        <div className="field">
          <label htmlFor="poll-q">Soru</label>
          <input id="poll-q" className="input" required maxLength={500} value={question}
            placeholder="Derbiyi kim kazanır?" onChange={(e) => setQuestion(e.target.value)} />
        </div>
        <fieldset className="field" style={{ border: 0, padding: 0, margin: 0 }}>
          <legend style={{ fontWeight: 600, fontSize: "0.92rem", marginBottom: "0.35rem" }}>Seçenekler</legend>
          {options.map((o, i) => (
            <div key={i} style={{ display: "flex", gap: "0.5rem" }}>
              <input className="input" maxLength={200} value={o} aria-label={`Seçenek ${i + 1}`}
                onChange={(e) => setOptions(options.map((x, j) => (j === i ? e.target.value : x)))} />
              {options.length > 2 && (
                <button type="button" className="icon-btn" aria-label="Seçeneği kaldır"
                  onClick={() => setOptions(options.filter((_, j) => j !== i))}>
                  <X size={18} />
                </button>
              )}
            </div>
          ))}
          {options.length < 10 && (
            <button type="button" className="btn btn-ghost btn-small" style={{ justifySelf: "start" }}
              onClick={() => setOptions([...options, ""])}>
              <Plus size={16} /> Seçenek ekle
            </button>
          )}
        </fieldset>
        <div className="field">
          <label htmlFor="poll-days">Süre</label>
          <select id="poll-days" className="select" value={days} onChange={(e) => setDays(Number(e.target.value))}>
            <option value={1}>1 gün</option>
            <option value={3}>3 gün</option>
            <option value={7}>1 hafta</option>
            <option value={30}>1 ay</option>
          </select>
        </div>
        {error && <p className="form-error" role="alert">{error}</p>}
        <div className="modal-foot" style={{ padding: 0 }}>
          <button type="button" className="btn btn-outline" onClick={onClose}>Vazgeç</button>
          <button className="btn btn-amber" disabled={create.isPending}>Yayınla</button>
        </div>
      </form>
    </Modal>
  );
}

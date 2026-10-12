import { useEffect, useMemo, useState, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { usersApi } from "../api/endpoints";
import type { UserProfile } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { errorText } from "../lib/errors";
import { TEAMS } from "../lib/teams";
import { Avatar } from "./Avatar";
import { Modal } from "./Modal";
import { useToast } from "./Toasts";

export function EditProfileModal({ profile, onClose }: { profile: UserProfile; onClose: () => void }) {
  const { setUser } = useAuth();
  const qc = useQueryClient();
  const toast = useToast();
  const [userName, setUserName] = useState(profile.userName);
  const [email, setEmail] = useState(profile.email ?? "");
  const [favoriteTeam, setFavoriteTeam] = useState(profile.favoriteTeam ?? "");
  const [bio, setBio] = useState(profile.bio ?? "");
  const [image, setImage] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);

  const preview = useMemo(() => (image ? URL.createObjectURL(image) : null), [image]);
  useEffect(() => () => { if (preview) URL.revokeObjectURL(preview); }, [preview]);

  const save = useMutation({
    mutationFn: () => usersApi.updateMe({ userName: userName.trim(), email: email.trim(), favoriteTeam, bio, profileImage: image }),
    onSuccess: (updated) => {
      setUser(updated);
      qc.setQueryData(["profile", updated.id], updated);
      // İsim / fotoğraf / takım postlarda da görünüyor
      qc.invalidateQueries({ queryKey: ["posts"] });
      qc.invalidateQueries({ queryKey: ["comments"] });
      toast.show("Profilin güncellendi.");
      onClose();
    },
    onError: (err) => setError(errorText(err)),
  });

  function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    save.mutate();
  }

  return (
    <Modal title="Profili düzenle" onClose={onClose}>
      <form onSubmit={onSubmit} style={{ display: "grid", gap: "1rem" }}>
        <div className="avatar-picker">
          <Avatar name={userName || profile.userName} src={preview ?? profile.profileImageUrl} size={72} />
          <label className="btn btn-outline btn-small">
            Fotoğraf seç
            <input type="file" accept="image/*" hidden onChange={(e) => setImage(e.target.files?.[0] ?? null)} />
          </label>
        </div>
        <div className="field">
          <label htmlFor="pf-username">Kullanıcı adı</label>
          <input id="pf-username" className="input" required minLength={3} maxLength={50} value={userName}
            onChange={(e) => setUserName(e.target.value)} />
        </div>
        <div className="field">
          <label htmlFor="pf-email">E-posta</label>
          <input id="pf-email" className="input" type="email" required value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div className="field">
          <label htmlFor="pf-team">Tuttuğun takım</label>
          <input id="pf-team" className="input" list="team-list" maxLength={100} value={favoriteTeam}
            placeholder="Yazmaya başla…" onChange={(e) => setFavoriteTeam(e.target.value)} />
          <datalist id="team-list">
            {TEAMS.map((t) => <option key={t} value={t} />)}
          </datalist>
        </div>
        <div className="field">
          <label htmlFor="pf-bio">Hakkında</label>
          <textarea id="pf-bio" className="textarea" rows={3} maxLength={500} value={bio} onChange={(e) => setBio(e.target.value)} />
        </div>
        {error && <p className="form-error" role="alert">{error}</p>}
        <div className="modal-foot" style={{ padding: 0 }}>
          <button type="button" className="btn btn-outline" onClick={onClose}>Vazgeç</button>
          <button className="btn btn-amber" disabled={save.isPending}>{save.isPending ? "Kaydediliyor…" : "Kaydet"}</button>
        </div>
      </form>
    </Modal>
  );
}

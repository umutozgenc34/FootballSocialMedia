import { useState, type FormEvent } from "react";
import { Link, Navigate, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { PitchMarks } from "../components/PitchMarks";
import { errorText } from "../lib/errors";

function AuthFrame({ children }: { children: React.ReactNode }) {
  return (
    <div className="auth">
      <section className="auth-pitch">
        <PitchMarks />
        <div className="auth-brand">FootballTalks</div>
        <div>
          <p className="auth-headline">Maç bitti, muhabbet bitmedi.</p>
          <p className="auth-sub">
            Takımını yaz, derbiyi yorumla, anketlerde oy ver. Tribündeki sohbet artık burada devam ediyor.
          </p>
        </div>
      </section>
      <section className="auth-form-wrap">{children}</section>
    </div>
  );
}

export function LoginPage() {
  const { user, login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [identifier, setIdentifier] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const from = (location.state as { from?: string } | null)?.from ?? "/";
  if (user) return <Navigate to={from} replace />;

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setBusy(true);
    setError(null);
    try {
      await login(identifier.trim(), password);
      navigate(from, { replace: true });
    } catch (err) {
      setError(errorText(err));
    } finally {
      setBusy(false);
    }
  }

  return (
    <AuthFrame>
      <form className="auth-form" onSubmit={onSubmit}>
        <h1>Giriş yap</h1>
        <div className="field">
          <label htmlFor="identifier">E-posta veya kullanıcı adı</label>
          <input id="identifier" className="input" autoComplete="username" required value={identifier}
            onChange={(e) => setIdentifier(e.target.value)} />
        </div>
        <div className="field">
          <label htmlFor="password">Şifre</label>
          <input id="password" className="input" type="password" autoComplete="current-password" required
            value={password} onChange={(e) => setPassword(e.target.value)} />
        </div>
        {error && <p className="form-error" role="alert">{error}</p>}
        <button className="btn btn-amber btn-block" disabled={busy}>
          {busy ? "Giriş yapılıyor…" : "Giriş yap"}
        </button>
        <p className="auth-switch">
          Hesabın yok mu? <Link to="/kayit">Kayıt ol</Link>
        </p>
      </form>
    </AuthFrame>
  );
}

export function RegisterPage() {
  const { user, register } = useAuth();
  const navigate = useNavigate();
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  if (user) return <Navigate to="/" replace />;

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    if (password.length < 6) {
      setError("Şifre en az 6 karakter olmalı.");
      return;
    }
    setBusy(true);
    setError(null);
    try {
      await register(userName.trim(), email.trim(), password);
      navigate("/", { replace: true });
    } catch (err) {
      setError(errorText(err));
    } finally {
      setBusy(false);
    }
  }

  return (
    <AuthFrame>
      <form className="auth-form" onSubmit={onSubmit}>
        <h1>Kayıt ol</h1>
        <div className="field">
          <label htmlFor="userName">Kullanıcı adı</label>
          <input id="userName" className="input" autoComplete="username" required minLength={3} maxLength={50}
            value={userName} onChange={(e) => setUserName(e.target.value)} />
          <span className="hint">Harf ve rakam kullan, boşluk olmasın.</span>
        </div>
        <div className="field">
          <label htmlFor="email">E-posta</label>
          <input id="email" className="input" type="email" autoComplete="email" required value={email}
            onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div className="field">
          <label htmlFor="password">Şifre</label>
          <input id="password" className="input" type="password" autoComplete="new-password" required minLength={6}
            value={password} onChange={(e) => setPassword(e.target.value)} />
          <span className="hint">En az 6 karakter, en az bir rakam ve bir küçük harf.</span>
        </div>
        {error && <p className="form-error" role="alert">{error}</p>}
        <button className="btn btn-amber btn-block" disabled={busy}>
          {busy ? "Hesap açılıyor…" : "Hesabımı aç"}
        </button>
        <p className="auth-switch">
          Zaten üye misin? <Link to="/giris">Giriş yap</Link>
        </p>
      </form>
    </AuthFrame>
  );
}

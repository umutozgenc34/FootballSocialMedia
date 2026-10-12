import { useState } from "react";
import { NavLink, Outlet, useLocation } from "react-router-dom";
import { Bell, Home, LogOut, MessageCircle, Newspaper, PenLine, Search, User, Vote } from "lucide-react";
import { useAuth } from "../auth/AuthContext";
import { Avatar } from "./Avatar";
import { Composer } from "./Composer";
import { Modal } from "./Modal";
import { PitchMarks } from "./PitchMarks";
import { RightRail } from "./RightRail";
import { useRealtime } from "../realtime/RealtimeContext";

interface NavItem {
  to: string;
  label: string;
  icon: typeof Home;
  badge?: number;
  end?: boolean;
}

export function Layout() {
  const { user, logout } = useAuth();
  const { unreadNotifications, unreadMessages } = useRealtime();
  const { pathname } = useLocation();
  const [composing, setComposing] = useState(false);
  if (!user) return null;

  const items: NavItem[] = [
    { to: "/", label: "Akış", icon: Home, end: true },
    { to: "/ara", label: "Ara", icon: Search },
    { to: "/bildirimler", label: "Bildirimler", icon: Bell, badge: unreadNotifications },
    { to: "/mesajlar", label: "Mesajlar", icon: MessageCircle, badge: unreadMessages },
    { to: "/haberler", label: "Haberler", icon: Newspaper },
    { to: "/anketler", label: "Anketler", icon: Vote },
    { to: `/profil/${user.id}`, label: "Profilim", icon: User },
  ];

  return (
    <>
      <header className="mobile-top">
        <span>FootballTalks</span>
        <button onClick={logout} aria-label="Çıkış yap">
          <LogOut size={20} />
        </button>
      </header>

      <div className="shell">
        <aside className="rail" aria-label="Ana menü">
          <PitchMarks variant="half" />
          <div className="rail-brand">
            <span className="long">FootballTalks</span>
            <span className="short">FT</span>
          </div>
          <nav className="rail-nav">
            {items.map(({ to, label, icon: Icon, badge, end }) => (
              <NavLink key={to} to={to} end={end} className={({ isActive }) => `rail-link${isActive ? " active" : ""}`}>
                <Icon size={22} aria-hidden="true" />
                <span className="label">{label}</span>
                {!!badge && <span className="badge" aria-label={`${badge} yeni`}>{badge > 99 ? "99+" : badge}</span>}
              </NavLink>
            ))}
          </nav>

          <button className="btn btn-amber btn-block rail-post-btn" onClick={() => setComposing(true)}>
            <PenLine size={18} aria-hidden="true" />
            <span className="label">Gönderi paylaş</span>
          </button>

          <div className="rail-me">
            <Avatar name={user.userName} src={user.profileImageUrl} size={40} />
            <div className="who">
              <strong>{user.userName}</strong>
              <span>{user.favoriteTeam || "Takım seçilmedi"}</span>
            </div>
            <button onClick={logout} aria-label="Çıkış yap" title="Çıkış yap">
              <LogOut size={18} />
            </button>
          </div>
        </aside>

        <main className={`main${pathname.startsWith("/mesajlar") ? " wide" : ""}`}>
          <Outlet />
        </main>

        <aside className="side" aria-label="Yan panel">
          <RightRail />
        </aside>
      </div>

      <nav className="tabbar" aria-label="Alt menü">
        {items
          .filter((i) => ["/", "/ara", "/bildirimler", "/mesajlar"].includes(i.to) || i.label === "Profilim")
          .map(({ to, label, icon: Icon, badge, end }) => (
            <NavLink key={to} to={to} end={end} className={({ isActive }) => (isActive ? "active" : "")} aria-label={label}>
              <Icon size={24} />
              {!!badge && <span className="badge">{badge > 99 ? "99+" : badge}</span>}
            </NavLink>
          ))}
      </nav>

      {composing && (
        <Modal title="Gönderi paylaş" onClose={() => setComposing(false)}>
          <Composer autoFocus onPosted={() => setComposing(false)} />
        </Modal>
      )}
    </>
  );
}

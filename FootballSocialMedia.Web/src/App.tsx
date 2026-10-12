import { Navigate, Route, Routes, useLocation } from "react-router-dom";
import { useAuth } from "./auth/AuthContext";
import { Layout } from "./components/Layout";
import { Spinner } from "./components/Feedback";
import { LoginPage, RegisterPage } from "./pages/AuthPages";
import { FeedPage } from "./pages/FeedPage";
import { NotFoundPage } from "./pages/NotFound";
import { PostDetailPage } from "./pages/PostDetailPage";
import { ProfilePage } from "./pages/ProfilePage";
import { MessagesPage } from "./pages/MessagesPage";
import { NotificationsPage } from "./pages/NotificationsPage";
import { RealtimeProvider } from "./realtime/RealtimeContext";

function RequireAuth({ children }: { children: JSX.Element }) {
  const { user, loading } = useAuth();
  const location = useLocation();
  if (loading) return <Spinner label="Oturum kontrol ediliyor" />;
  if (!user) return <Navigate to="/giris" replace state={{ from: location.pathname }} />;
  return children;
}

export default function App() {
  return (
    <Routes>
      <Route path="/giris" element={<LoginPage />} />
      <Route path="/kayit" element={<RegisterPage />} />
      <Route
        element={
          <RequireAuth>
            <RealtimeProvider>
              <Layout />
            </RealtimeProvider>
          </RequireAuth>
        }
      >
        <Route index element={<FeedPage />} />
        <Route path="gonderi/:id" element={<PostDetailPage />} />
        <Route path="profil/:id" element={<ProfilePage />} />
        <Route path="mesajlar" element={<MessagesPage />} />
        <Route path="mesajlar/:userId" element={<MessagesPage />} />
        <Route path="bildirimler" element={<NotificationsPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}

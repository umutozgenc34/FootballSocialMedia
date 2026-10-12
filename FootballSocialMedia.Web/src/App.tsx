import { Navigate, Route, Routes, useLocation } from "react-router-dom";
import { useAuth } from "./auth/AuthContext";
import { Layout } from "./components/Layout";
import { Spinner } from "./components/Feedback";
import { LoginPage, RegisterPage } from "./pages/AuthPages";
import { FeedPage } from "./pages/FeedPage";
import { NotFoundPage } from "./pages/NotFound";
import { PostDetailPage } from "./pages/PostDetailPage";
import { ProfilePage } from "./pages/ProfilePage";

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
            <Layout />
          </RequireAuth>
        }
      >
        <Route index element={<FeedPage />} />
        <Route path="gonderi/:id" element={<PostDetailPage />} />
        <Route path="profil/:id" element={<ProfilePage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}

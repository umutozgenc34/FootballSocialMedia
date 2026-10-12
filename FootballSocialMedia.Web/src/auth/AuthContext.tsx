import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { getTokens, onTokensChanged, setTokens } from "../api/client";
import { authApi, usersApi } from "../api/endpoints";
import type { UserProfile } from "../api/types";

interface AuthState {
  user: UserProfile | null;
  /** Sayfa ilk açılırken token varsa /users/me cevabı beklenirken true */
  loading: boolean;
  isAdmin: boolean;
  login: (emailOrUserName: string, password: string) => Promise<void>;
  register: (userName: string, email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  refreshUser: () => Promise<void>;
  setUser: (user: UserProfile) => void;
}

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const queryClient = useQueryClient();
  const [user, setUser] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(() => !!getTokens());

  const refreshUser = useCallback(async () => {
    if (!getTokens()) {
      setUser(null);
      return;
    }
    try {
      setUser(await usersApi.me());
    } catch {
      setTokens(null);
      setUser(null);
    }
  }, []);

  useEffect(() => {
    refreshUser().finally(() => setLoading(false));
    // Refresh token da geçersiz olursa (client.ts tokenları siler) oturumu kapat
    const off = onTokensChanged((tokens) => {
      if (!tokens) setUser(null);
    });
    return () => {
      off();
    };
  }, [refreshUser]);

  const login = useCallback(async (emailOrUserName: string, password: string) => {
    const tokens = await authApi.login(emailOrUserName, password);
    setTokens(tokens);
    setUser(await usersApi.me());
  }, []);

  const register = useCallback(
    async (userName: string, email: string, password: string) => {
      await authApi.register(userName, email, password);
      await login(email, password);
    },
    [login],
  );

  const logout = useCallback(async () => {
    const tokens = getTokens();
    if (tokens?.refreshToken) authApi.revoke(tokens.refreshToken).catch(() => undefined);
    setTokens(null);
    setUser(null);
    queryClient.clear();
  }, [queryClient]);

  const value = useMemo<AuthState>(
    () => ({
      user,
      loading,
      isAdmin: !!user?.roles.includes("admin"),
      login,
      register,
      logout,
      refreshUser,
      setUser,
    }),
    [user, loading, login, register, logout, refreshUser],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
}

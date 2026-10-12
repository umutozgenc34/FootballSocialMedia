import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import type { ServiceResult, TokenDto } from "./types";

export const API_URL = (import.meta.env.VITE_API_URL as string | undefined)?.replace(/\/$/, "") ?? "http://localhost:5187";

const TOKEN_KEY = "fsm.tokens";

// ---- Token saklama -------------------------------------------------------

export function getTokens(): TokenDto | null {
  try {
    const raw = localStorage.getItem(TOKEN_KEY);
    return raw ? (JSON.parse(raw) as TokenDto) : null;
  } catch {
    return null;
  }
}

export function setTokens(tokens: TokenDto | null) {
  try {
    if (tokens) localStorage.setItem(TOKEN_KEY, JSON.stringify(tokens));
    else localStorage.removeItem(TOKEN_KEY);
  } catch {
    /* gizli sekme vb. durumlarda localStorage kullanılamayabilir */
  }
  tokenListeners.forEach((l) => l(tokens));
}

type TokenListener = (tokens: TokenDto | null) => void;
const tokenListeners = new Set<TokenListener>();
export function onTokensChanged(listener: TokenListener) {
  tokenListeners.add(listener);
  return () => tokenListeners.delete(listener);
}

// ---- Axios instance ------------------------------------------------------

export const api = axios.create({ baseURL: `${API_URL}/api/v1` });

api.interceptors.request.use((config) => {
  const tokens = getTokens();
  if (tokens?.accessToken) config.headers.Authorization = `Bearer ${tokens.accessToken}`;
  return config;
});

// Access token 10 dakikada bir doluyor. 401 gelince refresh token ile yenileyip isteği tekrar dener.
// Aynı anda birden fazla istek 401 alırsa tek bir refresh isteği paylaşılır.
let refreshPromise: Promise<string | null> | null = null;

export async function refreshAccessToken(): Promise<string | null> {
  const tokens = getTokens();
  if (!tokens?.refreshToken) return null;

  refreshPromise ??= axios
    .post<ServiceResult<TokenDto>>(`${API_URL}/api/v1/auth/create-token-by-refresh-token`, { token: tokens.refreshToken })
    .then((res) => {
      const next = res.data.data!;
      setTokens(next);
      return next.accessToken;
    })
    .catch(() => {
      setTokens(null);
      return null;
    })
    .finally(() => {
      refreshPromise = null;
    });

  return refreshPromise;
}

/** SignalR gibi yerler için: süresi dolmak üzereyse önce yenile. */
export async function getFreshAccessToken(): Promise<string> {
  const tokens = getTokens();
  if (!tokens) return "";
  const expiresAt = new Date(tokens.accessTokenExpiration).getTime();
  if (Number.isFinite(expiresAt) && expiresAt - Date.now() < 30_000) {
    return (await refreshAccessToken()) ?? "";
  }
  return tokens.accessToken;
}

api.interceptors.response.use(
  (res) => res,
  async (error: AxiosError) => {
    const original = error.config as (InternalAxiosRequestConfig & { _retried?: boolean }) | undefined;
    if (error.response?.status === 401 && original && !original._retried && getTokens()?.refreshToken) {
      original._retried = true;
      const token = await refreshAccessToken();
      if (token) {
        original.headers.Authorization = `Bearer ${token}`;
        return api(original);
      }
    }
    return Promise.reject(error);
  },
);

/** ServiceResult<T> içinden data'yı çıkarır. */
export async function unwrap<T>(promise: Promise<{ data: ServiceResult<T> }>): Promise<T> {
  const res = await promise;
  return res.data?.data as T;
}

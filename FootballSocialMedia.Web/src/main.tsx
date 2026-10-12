import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { AuthProvider } from "./auth/AuthContext";
import { ToastProvider } from "./components/Toasts";
import App from "./App";
// Fontlar projeye gömülü (Google Fonts'a bağımlı değil). latin-ext Türkçe karakterleri içeriyor.
import "@fontsource/barlow/latin-400.css";
import "@fontsource/barlow/latin-ext-400.css";
import "@fontsource/barlow/latin-400-italic.css";
import "@fontsource/barlow/latin-ext-400-italic.css";
import "@fontsource/barlow/latin-500.css";
import "@fontsource/barlow/latin-ext-500.css";
import "@fontsource/barlow/latin-600.css";
import "@fontsource/barlow/latin-ext-600.css";
import "@fontsource/barlow/latin-700.css";
import "@fontsource/barlow/latin-ext-700.css";
import "@fontsource/big-shoulders-display/latin-700.css";
import "@fontsource/big-shoulders-display/latin-ext-700.css";
import "@fontsource/big-shoulders-display/latin-800.css";
import "@fontsource/big-shoulders-display/latin-ext-800.css";
import "@fontsource/big-shoulders-display/latin-900.css";
import "@fontsource/big-shoulders-display/latin-ext-900.css";
import "./styles.css";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: (count, error) => {
        const status = (error as { response?: { status?: number } }).response?.status;
        return status !== undefined && status < 500 ? false : count < 2;
      },
      refetchOnWindowFocus: false,
    },
  },
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <ToastProvider>
          <AuthProvider>
            <App />
          </AuthProvider>
        </ToastProvider>
      </QueryClientProvider>
    </BrowserRouter>
  </StrictMode>,
);

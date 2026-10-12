import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";
import { X } from "lucide-react";
import { Link } from "react-router-dom";

interface Toast {
  id: number;
  text: string;
  kind: "info" | "error";
  link?: { to: string; label: string };
}

interface ToastApi {
  show: (text: string, options?: { kind?: Toast["kind"]; link?: Toast["link"] }) => void;
  error: (text: string) => void;
}

const ToastContext = createContext<ToastApi | null>(null);
let nextId = 1;

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const dismiss = useCallback((id: number) => setToasts((t) => t.filter((x) => x.id !== id)), []);

  const show = useCallback<ToastApi["show"]>(
    (text, options) => {
      const id = nextId++;
      setToasts((t) => [...t.slice(-3), { id, text, kind: options?.kind ?? "info", link: options?.link }]);
      setTimeout(() => dismiss(id), 5000);
    },
    [dismiss],
  );

  const api = useMemo<ToastApi>(() => ({ show, error: (text) => show(text, { kind: "error" }) }), [show]);

  return (
    <ToastContext.Provider value={api}>
      {children}
      <div className="toasts" aria-live="polite">
        {toasts.map((t) => (
          <div key={t.id} className={`toast ${t.kind === "error" ? "error" : ""}`}>
            <div>
              <p>{t.text}</p>
              {t.link && (
                <Link to={t.link.to} onClick={() => dismiss(t.id)}>
                  {t.link.label}
                </Link>
              )}
            </div>
            <button className="close" onClick={() => dismiss(t.id)} aria-label="Kapat">
              <X size={16} />
            </button>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

export function useToast() {
  const ctx = useContext(ToastContext);
  if (!ctx) throw new Error("useToast must be used inside ToastProvider");
  return ctx;
}

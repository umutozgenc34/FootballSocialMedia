import type { ReactNode } from "react";
import { errorText } from "../lib/errors";

export function Spinner({ label = "Yükleniyor" }: { label?: string }) {
  return (
    <div className="spinner-wrap" role="status">
      <span className="spinner" />
      <span className="visually-hidden">{label}</span>
    </div>
  );
}

export function Empty({ title, children, action }: { title: string; children?: ReactNode; action?: ReactNode }) {
  return (
    <div className="empty">
      <h3>{title}</h3>
      {children && <p>{children}</p>}
      {action}
    </div>
  );
}

export function ErrorBox({ error }: { error: unknown }) {
  return (
    <div className="error-box" role="alert">
      {errorText(error)}
    </div>
  );
}

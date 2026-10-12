import { Link } from "react-router-dom";

export function NotFoundPage() {
  return (
    <div className="not-found">
      <span className="big">404</span>
      <h2>Bu sayfa ofsayta düştü</h2>
      <p>Aradığın sayfa yok ya da kaldırılmış.</p>
      <Link className="btn btn-outline" to="/">Akışa dön</Link>
    </div>
  );
}

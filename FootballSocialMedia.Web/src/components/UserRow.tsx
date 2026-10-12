import { Link } from "react-router-dom";
import type { ReactNode } from "react";
import { Avatar } from "./Avatar";

interface Props {
  id: string;
  userName: string;
  profileImageUrl?: string | null;
  subtitle?: string | null;
  action?: ReactNode;
}

export function UserRow({ id, userName, profileImageUrl, subtitle, action }: Props) {
  return (
    <div className="user-row">
      <Link to={`/profil/${id}`} aria-label={`${userName} profili`}>
        <Avatar name={userName} src={profileImageUrl} size={42} />
      </Link>
      <Link className="who" to={`/profil/${id}`}>
        <strong>{userName}</strong>
        {subtitle && <span>{subtitle}</span>}
      </Link>
      {action}
    </div>
  );
}

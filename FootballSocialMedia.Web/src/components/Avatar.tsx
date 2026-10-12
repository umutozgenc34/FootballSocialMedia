interface Props {
  name: string;
  src?: string | null;
  size?: number;
}

/** Profil fotoğrafı yoksa kullanıcı adının baş harfi gösterilir. */
export function Avatar({ name, src, size = 44 }: Props) {
  const style = { width: size, height: size, fontSize: size * 0.45 };
  if (src) return <img className="avatar" src={src} alt="" style={style} loading="lazy" />;
  return (
    <span className="avatar" style={style} aria-hidden="true">
      {name.slice(0, 1).toLocaleUpperCase("tr-TR")}
    </span>
  );
}

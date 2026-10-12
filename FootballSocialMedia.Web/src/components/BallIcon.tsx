/** Beğeni ikonu: futbol topu. Beğenildiğinde CSS ile sarıya boyanıp "şut" animasyonu oynar. */
export function BallIcon({ size = 20 }: { size?: number }) {
  return (
    <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8"
      strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <circle cx="12" cy="12" r="9.5" />
      <path d="M12 7.2l3.6 2.6-1.4 4.2H9.8L8.4 9.8z" fill="currentColor" fillOpacity="0.25" />
      <path d="M12 7.2V2.6M15.6 9.8l4.3-1.5M14.2 14l2.7 3.6M9.8 14l-2.7 3.6M8.4 9.8L4.1 8.3" />
    </svg>
  );
}

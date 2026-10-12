/** Tebeşirle çizilmiş saha çizgileri: orta saha çizgisi, santra yuvarlağı ve ceza sahası. Dekoratif. */
export function PitchMarks({ variant = "full" }: { variant?: "full" | "half" }) {
  if (variant === "half") {
    return (
      <svg className="pitch-marks" viewBox="0 0 300 400" preserveAspectRatio="xMidYMax slice" aria-hidden="true">
        <g fill="none" stroke="#fff" strokeWidth="3">
          <line x1="0" y1="140" x2="300" y2="140" />
          <circle cx="150" cy="140" r="70" />
          <circle cx="150" cy="140" r="4" fill="#fff" />
          <rect x="40" y="300" width="220" height="120" />
          <rect x="95" y="360" width="110" height="60" />
          <path d="M110 300 A 50 50 0 0 1 190 300" />
        </g>
      </svg>
    );
  }
  return (
    <svg className="pitch-marks" viewBox="0 0 800 600" preserveAspectRatio="xMidYMid slice" aria-hidden="true">
      <g fill="none" stroke="#fff" strokeWidth="3">
        <rect x="30" y="30" width="740" height="540" />
        <line x1="400" y1="30" x2="400" y2="570" />
        <circle cx="400" cy="300" r="90" />
        <circle cx="400" cy="300" r="5" fill="#fff" />
        <rect x="30" y="160" width="130" height="280" />
        <rect x="30" y="235" width="50" height="130" />
        <path d="M160 245 A 70 70 0 0 1 160 355" />
        <rect x="640" y="160" width="130" height="280" />
        <rect x="720" y="235" width="50" height="130" />
        <path d="M640 245 A 70 70 0 0 0 640 355" />
      </g>
    </svg>
  );
}

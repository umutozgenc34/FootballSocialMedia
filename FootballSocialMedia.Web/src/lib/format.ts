const rtf = new Intl.RelativeTimeFormat("tr", { numeric: "auto" });

/** "3 dk önce", "dün" gibi göreli zaman. */
export function timeAgo(iso: string): string {
  const date = new Date(iso.endsWith("Z") || iso.includes("+") ? iso : iso + "Z");
  const diff = (date.getTime() - Date.now()) / 1000;
  const abs = Math.abs(diff);
  if (abs < 45) return "az önce";
  if (abs < 3600) return rtf.format(Math.round(diff / 60), "minute");
  if (abs < 86400) return rtf.format(Math.round(diff / 3600), "hour");
  if (abs < 86400 * 7) return rtf.format(Math.round(diff / 86400), "day");
  return date.toLocaleDateString("tr-TR", { day: "numeric", month: "long", year: abs > 86400 * 300 ? "numeric" : undefined });
}

export function clockTime(iso: string): string {
  const date = new Date(iso.endsWith("Z") || iso.includes("+") ? iso : iso + "Z");
  return date.toLocaleTimeString("tr-TR", { hour: "2-digit", minute: "2-digit" });
}

export function fullDate(iso: string): string {
  const date = new Date(iso.endsWith("Z") || iso.includes("+") ? iso : iso + "Z");
  return date.toLocaleDateString("tr-TR", { day: "numeric", month: "long", year: "numeric" });
}

/** 1200 → "1,2 B" */
export function compact(n: number): string {
  return new Intl.NumberFormat("tr-TR", { notation: "compact", maximumFractionDigits: 1 }).format(n);
}

export function wasEdited(created: string, updated: string) {
  const u = new Date(updated).getTime();
  return u > 0 && u - new Date(created).getTime() > 1000 && new Date(updated).getFullYear() > 2000;
}

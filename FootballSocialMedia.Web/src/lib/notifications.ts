import type { AppNotification } from "../api/types";

/** Bildirim metni front'ta Türkçe kuruluyor (backend'deki Content İngilizce yedek metin). */
export function notificationText(n: AppNotification): string {
  const who = n.senderUserName ?? "Biri";
  switch (n.type) {
    case "Like":
      return n.commentId ? `${who} yorumunu beğendi.` : `${who} gönderini beğendi.`;
    case "Comment":
      return `${who} gönderine yorum yaptı.`;
    case "Follow":
      return `${who} seni takip etmeye başladı.`;
    case "Message":
      return `${who} sana mesaj gönderdi.`;
    default:
      return n.content;
  }
}

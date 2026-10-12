import { AxiosError } from "axios";

/**
 * Backend hataları { errorMessage: [...] } formatında dönüyor.
 * Ağ hatası (API kapalı / CORS) ve diğer durumlar için de anlaşılır bir mesaj üretir.
 */
export function errorText(error: unknown, fallback = "Bir şeyler ters gitti. Tekrar dene."): string {
  if (error instanceof AxiosError) {
    if (!error.response) {
      return "Sunucuya ulaşılamadı. API'nin çalıştığından emin ol (http://localhost:5187).";
    }
    const data = error.response.data as { errorMessage?: string[]; errors?: Record<string, string[]>; title?: string } | string | undefined;
    if (typeof data === "string" && data) return data;
    if (data && typeof data === "object") {
      if (Array.isArray(data.errorMessage) && data.errorMessage.length) return translate(data.errorMessage.join(" "));
      if (data.errors) return Object.values(data.errors).flat().join(" ");
      if (data.title) return data.title;
    }
    if (error.response.status === 403) return "Bu işlem için yetkin yok.";
    if (error.response.status === 404) return "Aradığın şey bulunamadı.";
  }
  return fallback;
}

// Backend'in İngilizce dönen sık mesajlarını Türkçeleştir
const messages: Record<string, string> = {
  "Email or Password wrong.": "E-posta/kullanıcı adı veya şifre hatalı.",
  "Already liked.": "Bunu zaten beğendin.",
  "Already following.": "Bu kişiyi zaten takip ediyorsun.",
  "You cannot follow yourself.": "Kendini takip edemezsin.",
  "You have already voted in this poll.": "Bu ankette zaten oy kullandın.",
  "This poll has expired.": "Bu anketin süresi doldu.",
  "You can only delete your own posts.": "Sadece kendi gönderilerini silebilirsin.",
  "You can only edit your own posts.": "Sadece kendi gönderilerini düzenleyebilirsin.",
};

function translate(message: string) {
  if (messages[message]) return messages[message];
  if (/Username '.*' is already taken/.test(message)) return "Bu kullanıcı adı alınmış.";
  if (/Email '.*' is already taken/.test(message)) return "Bu e-posta ile kayıtlı bir hesap var.";
  return message;
}

# FootballTalks — Futbolseverler için sosyal medya

Futbol tutkunlarının paylaşım yaptığı, maçları tartıştığı, anketlerde oy verdiği ve birbirleriyle mesajlaştığı bir sosyal medya uygulaması.

- **Backend:** ASP.NET Core 8 Web API (katmanlı mimari), EF Core + SQL Server, ASP.NET Identity + JWT, SignalR, Redis, RabbitMQ, MongoDB, Elasticsearch, Cloudinary
- **Frontend:** React 18 + TypeScript + Vite (`FootballSocialMedia.Web`)

## Özellikler

| Alan | Ne yapıyor |
| --- | --- |
| Hesap | Kayıt, e-posta **veya** kullanıcı adıyla giriş, JWT + refresh token (otomatik yenilenir) |
| Akış | Sayfalı akış: herkes / takip ettiklerim, resim & video yükleme (Cloudinary) |
| Etkileşim | Gönderi ve yorum beğenme, yorum yapma, düzenleme / silme (sahip veya admin) |
| Profil | Takım, biyografi, profil fotoğrafı, takipçi / takip listeleri |
| Mesajlaşma | SignalR ile gerçek zamanlı birebir sohbet; 30 günden eski mesajlar RabbitMQ ile MongoDB'ye arşivlenir |
| Bildirimler | Beğeni, yorum, takip, mesaj → RabbitMQ → MongoDB, SignalR ile anında |
| Arama | Elasticsearch ile post + yorum araması (ES kapalıysa SQL'e düşer), kullanıcı araması, otomatik tamamlama |
| Anketler | Kişi başı tek oy, süre sonu, sonuç yüzdeleri (oluşturma: admin) |
| Haberler | Görselli haberler (yayınlama: admin) |
| Cache | `GET /api/v1/posts` Redis ile cache'lenir (Redis kapalıysa DB'ye düşer) |

## Çalıştırma

### 0. Secret'lar (.env)

Connection string şifreleri, JWT anahtarı, Cloudinary anahtarları ve admin şifresi repoya girmeyen `.env` dosyasında durur:

```bash
cp .env.example .env   # Windows: copy .env.example .env
```

Sonra `.env` içindeki boş değerleri doldur. Bu dosyayı hem `docker compose` (container şifreleri) hem de API okur (`FootballSocialMedia.Api/Configuration/DotEnv.cs`, ek paket gerektirmez). API anahtarlarında iç içe ayarlar `__` ile yazılır, ör. `TokenOption__SecurityKey`. `.env` eksik ya da bir değer boşsa API açılışta hangi ayarın eksik olduğunu söyleyerek durur.

### 1. Altyapı (Docker)

```bash
docker compose up -d
```

SQL Server (1433), Redis (6377), RabbitMQ (5672, panel: 15672), MongoDB (27018), Mongo Express (8082), Elasticsearch (9200) ve Kibana (5601) ayağa kalkar.

### 2. API

Visual Studio'da `FootballSocialMedia.Api` projesini **http** profiliyle çalıştır, ya da:

```bash
cd FootballSocialMedia.Api
dotnet run --launch-profile http
```

- API: http://localhost:5187, Swagger: http://localhost:5187/swagger (sağ üstteki **Authorize** butonuna login'den gelen `accessToken`'ı yapıştır)
- Açılışta bekleyen **migration'lar otomatik uygulanır**, `user` / `admin` rolleri oluşturulur.
- Development ortamında bir admin hesabı oluşturulur: **admin@fsm.com**, şifresi `.env` → `SeedAdmin__Password`.

> **https** profilini kullanırsan API hem 7101 (https) hem 5187 (http) portunu dinler, front yine çalışır. IIS Express kullanırsan `FootballSocialMedia.Web/.env` içindeki adresi değiştirmen gerekir.

### 3. Frontend

```bash
cd FootballSocialMedia.Web
npm install
npm run dev
```

http://localhost:5173 adresini aç. API adresi `FootballSocialMedia.Web/.env` dosyasındaki `VITE_API_URL` değişkeninde. Front başka bir portta çalışacaksa backend'deki `Cors:Origins` listesine de eklenmeli.

### Docker olmadan hızlı deneme

SQL Server yoksa `appsettings.Development.json` → `"Database": { "UseInMemory": true }` yapılabilir (veriler API kapanınca silinir). Redis, RabbitMQ ve MongoDB yine gereklidir. Elasticsearch kapalıysa arama SQL'e düşer.

## Testler

```bash
dotnet test Service.Test
```

Servis testleri in-memory bir `AppDbContext` üzerinde çalışır: post akışı, silme yetkisi, beğeni sayımı, bildirimin doğru kişiye gitmesi, ankette tek oy vb.

## Proje yapısı

```
Core.CrossCuttingConcerns   ServiceResult, ortak exception'lar
Core.Persistence            BaseEntity, generic repository, UnitOfWork
Core.Security               Token DTO'ları, IIdentityService (token'daki kullanıcı)
Core.Infrastructure         Cloudinary, RabbitMQ, Elasticsearch istemcileri, versioning
FootballSocialMedia.Model       Entity'ler ve DTO'lar
FootballSocialMedia.Repository  DbContext, konfigürasyonlar, migration'lar, repository'ler
FootballSocialMedia.Service     İş kuralları, validator'lar, SignalR hub, RabbitMQ consumer'ları
FootballSocialMedia.Api         Controller'lar, Program.cs, seed
FootballSocialMedia.Web         React frontend
Service.Test                    NUnit testleri
```

## SignalR

- Endpoint: `/chathub?access_token=<JWT>`
- İstemci → sunucu: `SendMessage(receiverId, message)`
- Sunucu → istemci: `ReceiveMessage(ChatMessageDto)`, `ReceiveNotification(Notification)`

## Güvenlik notu

Secret'lar artık `.env` dosyasında ve `.gitignore` sayesinde repoya girmiyor. Ancak eski commit'lerde Cloudinary anahtarları ve JWT `SecurityKey` açık duruyor. Bu yüzden:

1. Cloudinary panelinden API secret'ı yenile ve `.env`'e yaz.
2. `TokenOption__SecurityKey` için yeni, rastgele bir değer üret (ör. PowerShell'de `[Convert]::ToBase64String((1..48 | % { Get-Random -Max 256 }))`).

## Sorun giderme

| Belirti | Çözüm |
| --- | --- |
| Front "Sunucuya ulaşılamadı" diyor | API çalışıyor mu? `VITE_API_URL` doğru mu? |
| Tarayıcıda CORS hatası | Front'un adresi `Cors:Origins` listesinde olmalı (varsayılan `http://localhost:5173`). |
| API açılışta "Eksik ayarlar" diyor | Repo kökünde `.env` var mı? `.env.example`'dan kopyala. |
| API açılışta RabbitMQ hatası veriyor | `docker compose up -d` ile container'ları başlat. |
| Mesajlar canlı gelmiyor | Sohbet başlığındaki göstergeye bak; "Canlı" yazmalı. SignalR bağlı değilse mesajlar REST ile gider, karşı taraf sayfayı yenileyince görür. |
| Fotoğraf yüklenmiyor | Cloudinary anahtarlarını kontrol et. |

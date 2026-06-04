# Skaftö Bageri – Inventory & Warehouse Management System (WMS)

Ett lager- och inventariesystem byggt för ett bageri. Applikationen hanterar lagersaldo, beställningar och rapportering så att personalen håller koll på råvaror och produkter. Användare loggar in säkert med token-baserad autentisering.

> Utvecklat som praktikprojekt 2025. Jag ansvarade för hela applikationen — backend, datamodell och gränssnitt.

## ✨ Funktioner
- Lagerhantering med översikt över produkter och saldo
- Hantering av beställningar
- Rapportering och översikt
- Inloggning och användarhantering med säker autentisering (JWT + ASP.NET Core Identity)
- Lösenord lagras säkert med BCrypt-hashning

## 🔐 Säkerhet
Projektet är byggt med säkerhet i fokus:
- **JWT (JSON Web Tokens)** för token-baserad autentisering
- **BCrypt** för hashning av lösenord (inga lösenord lagras i klartext)
- **ASP.NET Core Identity** för användarhantering

## 🛠️ Tech stack
- **Språk & ramverk:** C# · ASP.NET Core MVC · .NET 8
- **Databas:** MySQL med Entity Framework Core 8 (Code First + Migrations)
- **Autentisering:** JWT, ASP.NET Core Identity, BCrypt.Net
- **Frontend:** Razor Views (HTML, CSS, JavaScript)

## 🚀 Kom igång
Förutsätter att du har [.NET 8 SDK](https://dotnet.microsoft.com/download) och en MySQL-databas installerad.

\`\`\`bash
# Klona repot
git clone https://github.com/naveenbl1202/Skaft-_WMS.git
cd Skaft-_WMS

# Uppdatera anslutningssträngen i appsettings.json så den pekar mot din MySQL-databas

# Skapa databasen från migrations
dotnet ef database update

# Kör applikationen
dotnet run
\`\`\`
Öppna sedan adressen som visas i terminalen (t.ex. \`https://localhost:5001\`) i din webbläsare.

## 🧠 Vad jag lärde mig
- Att bygga en komplett ASP.NET Core MVC-applikation från databas till gränssnitt
- Datamodellering med Entity Framework Core och migrations
- Att implementera säker autentisering med JWT och lösenordshashning med BCrypt

## 👤 Författare
Naveen Bangalore Lakshminarayan · Malmö · naveenbl1202@gmail.com

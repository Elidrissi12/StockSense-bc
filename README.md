# StockSense Backend (.NET 8)

Backend API REST pour la gestion de stock industriel (matières premières et produits finis), construit avec ASP.NET Core, Clean Architecture, SQL Server et JWT.

## Sommaire

- [Fonctionnalités](#fonctionnalites)
- [Architecture](#architecture)
- [Stack technique](#stack-technique)
- [Prérequis](#prerequis)
- [Installation et demarrage](#installation-et-demarrage)
- [Configuration](#configuration)
- [Base de donnees et migrations](#base-de-donnees-et-migrations)
- [Authentification et roles](#authentification-et-roles)
- [Endpoints principaux](#endpoints-principaux)
- [Tests rapides Postman](#tests-rapides-postman)
- [Generation de rapports et fichiers](#generation-de-rapports-et-fichiers)
- [Envoi d'email (SMTP/Mailtrap)](#envoi-demail-smtpmailtrap)
- [Structure du projet](#structure-du-projet)
- [Roadmap](#roadmap)

## Fonctionnalites

- Authentification JWT (register/login)
- Gestion des utilisateurs avec roles:
  - Admin
  - Responsable
  - Technicien
- CRUD categories
- CRUD produits
- Mouvements de stock (entree/sortie) avec verification de stock insuffisant
- Alertes stock faible
- Dashboard / reporting:
  - resume dashboard
  - stats par categorie
  - tendance des mouvements
  - mouvements recents
- Export des rapports:
  - PDF
  - Excel
- Stockage des fichiers en base SQL Server (BLOB)
- Upload / download de fichiers
- Envoi d'email avec piece jointe (SMTP, ex: Mailtrap)

## Architecture

Le projet suit une separation en couches (Clean Architecture):

- `StockManagement.Domain`: entites et enums metier
- `StockManagement.Application`: interfaces, DTO/contracts, services (use-cases)
- `StockManagement.Infrastructure`: EF Core, repositories, securite JWT, PDF/Excel, SMTP
- `StockManagement.API`: controllers REST, DI, config, Swagger

## Stack technique

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core + SQL Server
- JWT Bearer Authentication
- BCrypt (hash password)
- Swagger / OpenAPI
- ClosedXML (Excel)
- QuestPDF (PDF)
- MailKit (SMTP)

## Prerequis

- .NET SDK 8.x
- SQL Server (ex: `SQLEXPRESS`)
- SSMS (optionnel, recommande)
- Postman (optionnel, recommande)

## Installation et demarrage

```bash
dotnet restore
dotnet build
dotnet run --project StockManagement.API
```

API par defaut (dev):
- `http://localhost:5081`
- Swagger: `http://localhost:5081/swagger`

## Configuration

Fichier principal:
- `StockManagement.API/appsettings.Development.json`

Exemple minimal:

```json
{
  "ConnectionStrings": {
    "Default": "Server=ZORO\\SQLEXPRESS;Database=StockManagement;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "StockSense",
    "Audience": "StockSense",
    "Key": "CHANGE_ME_SUPER_SECRET_KEY_1234567890",
    "ExpiryMinutes": 120
  },
  "Smtp": {
    "Host": "sandbox.smtp.mailtrap.io",
    "Port": 587,
    "User": "CHANGE_ME",
    "Password": "CHANGE_ME",
    "FromEmail": "no-reply@stocksense.local",
    "FromName": "StockSense Reports",
    "EnableSsl": true
  }
}
```

## Base de donnees et migrations

Appliquer les migrations:

```bash
dotnet ef database update --project StockManagement.Infrastructure --startup-project StockManagement.API
```

Creer une nouvelle migration:

```bash
dotnet ef migrations add NomMigration --project StockManagement.Infrastructure --startup-project StockManagement.API
```

## Authentification et roles

Endpoints publics:
- `POST /api/auth/register`
- `POST /api/auth/login`

Pour les endpoints proteges, ajouter:

`Authorization: Bearer <token>`

Restrictions:
- Ecriture sensible (categories/produits/export/email/delete fichiers): `Admin,Responsable`
- Lecture: utilisateur authentifie

## Endpoints principaux

### Auth
- `POST /api/auth/register`
- `POST /api/auth/login`

### Categories
- `GET /api/categories`
- `GET /api/categories/{id}`
- `POST /api/categories`
- `PUT /api/categories/{id}`
- `DELETE /api/categories/{id}`

### Produits
- `GET /api/products`
- `GET /api/products/{id}`
- `POST /api/products`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`

### Stock et alertes
- `POST /api/stock/movements`
- `GET /api/stock/products/{productId}/movements`
- `GET /api/alerts/low-stock-products`
- `GET /api/alerts/low-stock-alerts`

### Reporting
- `GET /api/reports/dashboard-summary`
- `GET /api/reports/category-stock-stats`
- `GET /api/reports/stock-trend`
- `GET /api/reports/recent-movements`
- `POST /api/reports/export/pdf`
- `POST /api/reports/export/excel`
- `POST /api/reports/email`

### Files (SQL BLOB)
- `POST /api/files/upload`
- `GET /api/files/{id}`
- `GET /api/files/{id}/download`
- `DELETE /api/files/{id}`

## Tests rapides Postman

1. Login: `POST /api/auth/login` -> recuperer token
2. Creer categorie: `POST /api/categories`
3. Creer produit: `POST /api/products`
4. Mouvement stock: `POST /api/stock/movements`
5. Export PDF: `POST /api/reports/export/pdf`
6. Download fichier: `GET /api/files/{fileId}/download`

## Generation de rapports et fichiers

- Les exports PDF/Excel sont generes puis stockes dans `StoredFiles` (SQL Server).
- La reponse d'export retourne un `fileId`.
- Utiliser ce `fileId` pour telecharger via `/api/files/{id}/download`.

## Envoi d'email (SMTP/Mailtrap)

- Configurer correctement `Smtp` dans `appsettings.Development.json`.
- Endpoint:
  - `POST /api/reports/email`
- Le service genere le rapport (pdf/excel), le stocke, puis l'envoie en piece jointe.

## Structure du projet

```text
StockManagement/
|- StockManagement.Domain/
|- StockManagement.Application/
|- StockManagement.Infrastructure/
|- StockManagement.API/
|- StockManagement.sln
```

## Roadmap

- Frontend Next.js (dashboard web)
- Frontend React Native (terrain)
- Notifications push temps reel
- Export CSV avance
- Historique et audit detaille
- CI/CD + tests automatisees


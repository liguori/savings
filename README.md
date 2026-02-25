# Savings

[![Build Frontend](https://github.com/liguori/savings/actions/workflows/build-frontend.yml/badge.svg)](https://github.com/liguori/savings/actions/workflows/build-frontend.yml)
[![Build Backend](https://github.com/liguori/savings/actions/workflows/build-backend.yml/badge.svg)](https://github.com/liguori/savings/actions/workflows/build-backend.yml)

A personal finance projection and tracking application that lets you define recurring income and expenses, manage one-time transactions, and project future account balances. Supports multiple deployment modes: web application or full-stack development with .NET Aspire orchestration.

## Features

- 💰 **Savings Projection** — Project future account balances based on recurrent and fixed transactions
- 🔄 **Recurrent Items** — Manage recurring income and expenses with parent-child relationships
- 📌 **Fixed Items** — Track one-time transactions with category filtering
- 📜 **History** — View materialized transactions and compare actuals vs. projections
- 📊 **Reports** — Category-level spending summaries grouped by month or year
- 🎨 **Theme Support** — Light and dark themes
- 🖥️ **Multiple Deployment Options** — Web app or development with Aspire

## Getting Started

### Quick Start Commands

| Goal | Command |
|------|---------|
| **Full-stack development** | `cd src/Savings.AppHost && dotnet run` |
| **Run API only** | `cd src/Savings.API && dotnet run` |
| **Run SPA only** | `cd src/Savings.SPA && dotnet run` |
| **Debug API in VS Code** | Press F5 → "C#: Savings.API Debug" |
| **Debug SPA in VS Code** | Press F5 → "C#: Savings.SPA Debug" |

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend | Blazor WebAssembly, Radzen Blazor |
| Backend | ASP.NET Core, Entity Framework Core |
| Database | SQLite |
| Orchestration | .NET Aspire 9.0.0 |

---

## Prerequisites

- **.NET SDK** 10

---

### 1. .NET Aspire Orchestration

**Recommended for full-stack development**

The application includes **.NET Aspire 9.0.0** orchestration providing:
- Unified orchestration of API and SPA
- Built-in service discovery
- Observability with OpenTelemetry
- Health checks and resilience
- Dashboard for monitoring all services

**Run:**

```bash
cd src/Savings.AppHost
dotnet run
```

**Services started:**
- **Savings.API** — ASP.NET Core Web API (dynamic port)
- **Savings.SPA** — Blazor WebAssembly (dynamic port)
- **Aspire Dashboard** — available at the URL shown in terminal output

---

### 2. VS Code Debug Configurations

**Recommended for debugging individual components**

Pre-configured launch configurations in `.vscode/launch.json`:

| Configuration | Description |
|--------------|-------------|
| `C#: Savings.API Debug` | Debug backend API with .NET debugger |
| `C#: Savings.SPA Debug` | Debug Blazor frontend with .NET debugger |

**How to use:**
1. Open **Run and Debug** (Ctrl+Shift+D)
2. Select configuration from dropdown
3. Press **F5**

> **Ports:** API runs on `https://localhost:7563`, SPA runs on `https://localhost:7026`

---

### 3. Web Server Deployment

**Recommended for production hosting**

#### Build

```bash
# API
cd src/Savings.API
dotnet publish -c Release -o ./publish

# SPA
cd src/Savings.SPA
dotnet publish -c Release -o ./publish
```

#### Deploy

| Component | Target |
|-----------|--------|
| **API** | IIS, Azure App Service, Linux/Kestrel, Containers |
| **SPA** | Azure Static Web Apps, any static host, or served by API |

> **Configuration:** Update `appsettings.Production.json` for both API and SPA with production URLs and settings.

---

## Security

The application supports two authentication modes:

| Mode | Use Case |
|------|----------|
| **API Key** | Simple shared-secret authentication |
| **Azure AD** | Enterprise identity with OAuth 2.0 / JWT |

### API Key

Add the following to both `Savings.API/appsettings.json` and `Savings.SPA/appsettings.json`:

```json
{
  "AuthenticationToUse": "ApiKey",
  "ApiKeys": "keyToUse"
}
```

### Azure AD

Configure two app registrations in Azure AD — one for the API (exposing a scope `SavingProjection.All`) and one for the SPA (with API permission for that scope).

**API** (`Savings.API/appsettings.json`):

```json
{
  "AuthenticationToUse": "AzureAD",
  "IdentityProvider": {
    "Authority": "https://login.microsoftonline.com/{tenantID}/",
    "Audience": "api://{apiClientID}"
  }
}
```

**SPA** (`Savings.SPA/appsettings.json`):

```json
{
  "AuthenticationToUse": "AzureAD",
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/{tenantID}/",
    "ClientId": "{spaClientID}",
    "ValidateAuthority": true,
    "DefaultScope": "api://{apiClientID}/SavingProjection.All"
  }
}
```

---

## Screenshots

### Savings Projection

Project your future account balances based on recurring and fixed transactions:

**Light Theme**
![Savings Projection](docs/SavingsProjection.png)

**Dark Theme**
![Savings Projection Dark](docs/SavingsProjection_dark.png)

### Recurrent Items

![Recurrent Items](docs/RecurrentItems.png)

### Additional Views

<details>
<summary>Click to see more screenshots showing configuration, fixed items, history, reports, and dark mode views</summary>

#### Configuration
![Configuration](docs/Configuration.png)

#### Configuration (Dark)
![Configuration Dark](docs/Configuration_dark.png)

#### Fixed Items
![Fixed Items](docs/FixedItems.png)

#### Fixed Items (Dark)
![Fixed Items Dark](docs/FixedItems_dark.png)

#### History
![History](docs/History.png)

#### History (Dark)
![History Dark](docs/History_dark.png)

#### Recurrent Items (Dark)
![Recurrent Items Dark](docs/RecurrentItems_dark.png)

#### Reports
![Reports](docs/Reports.png)

#### Reports (Dark)
![Reports Dark](docs/Reports_dark.png)

</details>
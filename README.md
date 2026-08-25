# Crispy Kitchen

Crispy Kitchen is a restaurant ordering application with a React/Vite frontend, an ASP.NET Core Web API, and SQL Server.

## Project structure

- `CrispyKitchen.Domain` — business entities, rules, and enums.
- `CrispyKitchen.Application` — commands, queries, validation, and DTOs.
- `CrispyKitchen.Infrastructure` — SQL Server/EF Core, repositories, security, and migrations.
- `CrispyKitchen.WebApi` — HTTP API, authentication, middleware, and startup code.
- `crispy-kitchen-frontend` — React customer, kitchen, and admin interface.

## Prerequisites

Install the following before running the application:

- .NET 10 SDK
- SQL Server (LocalDB, SQL Server Express, Docker SQL Server, or a full SQL Server instance)
- Node.js 20 or later and npm

## Database configuration

The API reads its database connection string from `ConnectionStrings:DefaultConnection`.

For local development, either update the existing connection string in `CrispyKitchen.WebApi/appsettings.json` to match your SQL Server instance, or set it as an environment variable:

```bash
export ConnectionStrings__DefaultConnection='Server=localhost,1433;Database=CrispyKitchenDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;'
```

On Windows PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection = 'Server=localhost,1433;Database=CrispyKitchenDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;'
```

## Migrations and default data

When the API starts, it automatically:

1. Applies any pending EF Core migrations.
2. Adds three default menu products when the `Products` table is empty.
3. Creates a bootstrap Admin when no Admin exists.

This means a normal local run does not require a separate `dotnet ef database update` command.

If you prefer to apply migrations manually, use:

```bash
dotnet ef database update --project CrispyKitchen.Infrastructure --startup-project CrispyKitchen.WebApi
```

## Bootstrap Admin

The first Admin is created only when there is no Admin account in the database. The API reads these environment variables:

```text
BootstrapAdmin__FullName
BootstrapAdmin__Email
BootstrapAdmin__Password
```

The local launch profiles already contain the bootstrap values configured for this learning project in `CrispyKitchen.WebApi/Properties/launchSettings.json`.

For a different environment, set your own values before the first startup. Once an Admin exists, changing these variables does not create another account.

## Run the API

From the repository root:

```bash
dotnet restore
dotnet run --project CrispyKitchen.WebApi --launch-profile http
```

The API starts at `http://localhost:5001`. In Development, Swagger is available at `http://localhost:5001/swagger`.

## Run the frontend

In a second terminal:

```bash
cd crispy-kitchen-frontend
npm install
npm run dev
```

The frontend expects this value in `crispy-kitchen-frontend/.env`:

```text
VITE_API_URL=http://localhost:5001/api
```

Open the Vite URL shown in the terminal, normally `http://localhost:5173`.

## Useful local commands

```bash
# Backend build
dotnet build CrispyKitchen.slnx

# Frontend lint
cd crispy-kitchen-frontend && npm run lint

# Frontend production build
cd crispy-kitchen-frontend && npm run build
```

## Main routes

- `/menu` — public menu and customer checkout.
- `/login` and `/register` — customer login and registration.
- `/orders/mine` — customer order history.
- `/kitchen` — kitchen dashboard for KitchenStaff and Admin.
- `/admin` — product and staff management for Admin.

## Notes

- JWT authentication and the SQL Server connection are configured in `CrispyKitchen.WebApi/appsettings.json`.
- Payments use a dummy provider in Development only. It lets a customer simulate a successful or failed payment, but it never charges real money. Replace the `IPaymentProvider` registration with a real provider implementation before production use.
- Do not use the learning-project credentials in a production deployment. Use environment variables or a secret manager for passwords and signing keys.

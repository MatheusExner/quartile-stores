# Quartile Stores Project Documentation

## Overview
This project is a backend solution for managing companies, stores, and products in a globalized, multi-company environment. 

---

## Features
- **RESTful API** for managing companies and stores (CRUD operations)
- **Azure Functions** for serverless product management (CRUD for products)
- **SQL Server** database for persistent storage
- **Automated API testing** with a Postman collection
- **Azure App Service** deployment with staging and production slots
- **Test-Driven Development (TDD)** approach
- **OpenAPI-style documentation** for all endpoints

---

## Project Structure
- `src/StoreApi/Controllers/CompanyController.cs` — Company API
- `src/StoreApi/Controllers/StoreController.cs` — Store API
- `src/Functions/Products/ProductsFunctions.cs` — Product Azure Functions
- `src/Application/Dtos/` — Data Transfer Objects
- `src/Domain/Entities/` — Domain models
- `src/Infrastructure/` — Data access and repositories
- `tests/` — Unit and integration tests
- `wiki/` — API and function documentation

---

## API Endpoints
See detailed documentation in the `wiki/` folder:
- `CompanyController.md`
- `StoreController.md`
- `ProductsFunctions.md`

---

# Quartile Stores — API Documentation

This repository implements a backend for managing Companies, Stores and Products. It uses a hybrid architecture:

- ASP.NET Core Web API (Companies and Stores)
- Azure Functions (Products)
- SQL Server for persistence (EF Core + Dapper in different layers)
- FluentValidation for request validation

This README documents how the project is organized, how to run it locally, how the APIs behave and where to find the automated Postman tests you asked for.

## Repository layout

- `src/Application` — DTOs, mappings, features (CQRS commands/queries)
- `src/Domain` — Entities and domain interfaces
- `src/Infrastructure` — EF Core DbContext, migrations, repositories
- `src/StoreApi` — ASP.NET Core Web API controllers for Companies and Stores
- `src/Functions` — Azure Functions project (Products)
- `tests` — Unit and integration tests (TDD examples)
- `wiki` — Generated API documentation (per-controller/function markdown files)
- `QuartileStores.postman_collection.json` — Postman collection for automated API testing
- `QuartileStores.postman_environment.json` — Postman environment for local/staging URLs

## Quick start (local)

1. Restore packages

```powershell
dotnet restore
```

2. Apply database migrations (update connection string in `src/Infrastructure/appsettings.Development.json` or environment variables)

```powershell
dotnet ef database update --project src/Infrastructure --startup-project src/StoreApi
```

3. Run the Web API (StoreApi)

```powershell
dotnet run --project src/StoreApi
```

4. Run Azure Functions (for local product endpoints)

```powershell
dotnet build src/Functions
func host start --script-root src/Functions/bin/Debug/net8.0
```

5. Import the Postman collection (`QuartileStores.postman_collection.json`) and environment into Postman. Configure `baseUrl` to point to your local API (for example `https://localhost:7050`) or staging slot.

## APIs

See the `wiki/` folder for per-controller documentation:

- `wiki/Company.md` — Companies endpoints (CQRS)
- `wiki/Store.md` — Stores endpoints
- `wiki/ProductsFunctions.md` — Products Azure Functions

## Postman tests

The repository includes a Postman collection and environment files:

- `Quartile Stores Api.postman_collection.json`

## URL's

- API: https://quartile-stores.azurewebsites.net/
- Staging API: https://quartile-stores-staging.azurewebsites.net/

- Azure functions: https://quartilestores.azurewebsites.net/
- Azure functions staging: https://quartilestores-staging.azurewebsites.net/
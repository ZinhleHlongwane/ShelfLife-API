# ShelfLife API

Shared backend for the ShelfLife web and mobile applications.

## Technology

- .NET 10 / ASP.NET Core Web API
- C# domain model
- Entity Framework Core 10
- PostgreSQL 17
- Docker Compose for local database development

## Architecture

```text
ShelfLife Web (React / TypeScript) ─┐
                                    ├── ShelfLife API ── PostgreSQL
ShelfLife Mobile (Expo / React Native)┘
```

The API owns inventory persistence and domain validation. Both clients use the same inventory contract. Expiry dates are represented as calendar dates (`YYYY-MM-DD`) and persisted as PostgreSQL `date` values, not timestamps.

## Repository layout

- `src/ShelfLife.API`: HTTP endpoints, request/response contracts and application startup.
- `src/ShelfLife.Domain`: inventory entity and domain enums.
- `src/ShelfLife.Infrastructure`: EF Core context and PostgreSQL mappings.

## Local setup

Requirements: .NET 10 SDK and Docker Desktop.

1. Start PostgreSQL:

   ```bash
   docker compose up -d postgres
   ```

2. Restore and build:

   ```bash
   dotnet restore
   dotnet build
   ```

3. Apply the committed database migration:

   ```bash
   dotnet ef database update --project src/ShelfLife.Infrastructure --startup-project src/ShelfLife.API
   ```

4. Run the API:

   ```bash
   dotnet run --project src/ShelfLife.API
   ```

In Development, the OpenAPI document is available at `/openapi/v1.json`. The health endpoint is `/health`.

## Inventory endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/inventory` | List inventory, ordered by expiry |
| GET | `/api/inventory?category=Fridge` | Filter by storage category |
| GET | `/api/inventory/{id}` | Get one item |
| POST | `/api/inventory` | Create an item |
| PUT | `/api/inventory/{id}` | Update an item |
| DELETE | `/api/inventory/{id}` | Delete an item |
| GET | `/api/inventory/expiring?withinDays=3` | List items expiring within a date window |

### Example request

```json
{
  "name": "Milk",
  "quantity": 2,
  "unit": "Liter",
  "expiryDate": "2026-12-01",
  "category": "Fridge"
}
```

Enum values are case-insensitive in JSON. Supported units: `Count`, `Gram`, `Kilogram`, `Milliliter`, `Liter`. Supported categories: `Fridge`, `Pantry`, `Freezer`.

## Current scope and next steps

This is the initial shared inventory API foundation. The initial PostgreSQL migration and PostgreSQL-backed API integration test are included. Authentication, household ownership, authorisation, production secrets management, and client API integration are not implemented yet. Do not expose this development API to the public internet or use it for real household data until those controls are added.

Planned next:
1. Add automated domain and API integration tests.
2. Add user/household ownership and authentication before multi-user deployment.
3. Connect the web and mobile clients to this API.
4. Add offline-aware client behaviour and sync conflict handling.
5. Add barcode lookup and recipe/shopping-list integrations.

# ADR-001: Authentication and household-scoped inventory

- **Status:** Accepted design; implementation not yet enabled
- **Date:** 2026-11-25
- **Scope:** ShelfLife API, Web and Mobile clients

## Context

ShelfLife currently exposes inventory CRUD endpoints without an authenticated user. Every client that can reach the API can read, create, update and delete every inventory record. This is acceptable only for local development with disposable data. It is not safe for a deployed application or real household information.

ShelfLife is intended to support shared household inventories. A person may belong to a household, and household members should see the same inventory. Different households must never be able to access one another's records.

## Decisions

### 1. Use ASP.NET Core Identity

Use ASP.NET Core Identity with PostgreSQL persistence for account creation, password hashing, sign-in, account validation and security-token lifecycle. Do not implement password hashing or token cryptography manually.

The API will issue bearer access tokens for the React web and Expo mobile clients. Signing keys must come from deployment secrets or environment variables, never committed source files. Production must use HTTPS.

### 2. Introduce household ownership

Add a Household aggregate and membership records. A newly registered user creates a household and becomes its owner. The data model should support additional members and roles (Owner and Member) without changing inventory ownership semantics later.

Every inventory item must belong to exactly one household. Inventory queries and mutations must be scoped using the authenticated user's household membership resolved server-side from the validated identity. Never trust a household ID supplied by a client as proof of access.

### 3. Enforce tenant isolation in the API

- Require authentication for all inventory endpoints.
- Resolve the active household from the authenticated principal and persisted membership.
- Apply the household filter to list, detail, update and delete queries.
- Set household ownership on create from the server-side principal.
- Return 404 for an item outside the caller's household, avoiding resource-existence disclosure.
- Add integration tests proving a user in household A cannot read, update or delete household B's items.
- Keep health and authentication endpoints public as appropriate.

### 4. Client integration

Both clients must use the same authentication contract:
- registration and sign-in;
- secure bearer-token storage appropriate to each platform;
- authenticated API requests;
- sign-out and expired-session handling.

The mobile client must store credentials in platform secure storage, not plain AsyncStorage. The web client must avoid long-lived credentials in localStorage; prefer a secure, HttpOnly cookie-backed session or another reviewed browser token strategy.

Do not enable mandatory API authentication until the web and mobile clients can sign in and attach credentials. Implement and release the vertical slice together to avoid breaking existing clients.

## Migration and rollout

1. Add Identity and household persistence with a reviewed EF Core migration.
2. Decide how existing unowned inventory rows are handled before applying the migration to any database containing data. Do not silently assign records to an arbitrary household.
3. Add registration and sign-in endpoints and token validation.
4. Add household membership resolution and tenant-scoped inventory access.
5. Add API integration tests for unauthenticated access and cross-household isolation.
6. Add login, registration, session handling and secure token storage to Web and Mobile.
7. Enable authentication enforcement only after both clients are updated and the migration path is verified.
8. Configure production secrets, HTTPS, rate limiting, logging hygiene, CORS and deployment security before public release.

## Security constraints

- Never log passwords, password hashes, bearer tokens or refresh tokens.
- Never commit production connection strings or token-signing secrets.
- Use a strong randomly generated signing key supplied through a secret manager.
- Apply rate limits to registration and sign-in.
- Validate email and password requirements server-side.
- Use generic sign-in failure messages.
- Do not expose development database credentials or HTTP endpoints publicly.
- Treat CORS as a browser policy, not an authentication or authorisation mechanism.

## Acceptance criteria

- A new user can register and receives a household.
- A user can sign in and receive a valid bearer token.
- Inventory endpoints reject unauthenticated requests.
- Authenticated requests are restricted to the user's household.
- Cross-household access tests cover list, read, update and delete.
- Both Web and Mobile can sign in and use shared inventory.
- Existing database contents have an explicit, tested migration path.
- CI builds and runs tests against PostgreSQL.

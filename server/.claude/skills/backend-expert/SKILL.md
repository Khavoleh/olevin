---
name: backend-expert
description: Build and modify the Olevin .NET 10 API (`server/`: ASP.NET Core + Wolverine HTTP + EF Core/PostgreSQL + FluentValidation + Logto JWT) so every endpoint, entity, validator and config stays consistent with the API contract, data, auth, and code-style conventions. Use this skill whenever: adding or changing an endpoint, request/response record, validator or mapping; touching an entity, `IEntityTypeConfiguration`, DbContext or the Atlas schema in `infrastructure/db`; reading the signed-in user; returning errors; adding configuration, secrets or options; writing domain calculations; or when the build fails on analyzers, XML docs or CSharpier. Also use it when unsure whether to reuse an existing helper or type — the default is always reuse first. Examples: "add an endpoint to save a snapshot", "add a column to user_settings", "return 404 when the user has no settings", "add a setting for the Monobank API", "why does the build fail with CS1591 / IDE0008?".
---

# Backend expert

Produce API code that looks and behaves like the rest of the server — a stable contract, validated input, correct data, secure by default, and built from small, testable pieces.

## Endpoints & contract

- One use case = one endpoint class; the endpoint method **is** the handler (no separate command/handler/mediator layer).
- **Routes are versioned and grouped by feature:** `/<feature>/v1/<resource>`. A breaking change to a request/response shape needs a new version or a coordinated change in every client.
- Return the project's **response envelope** with the real HTTP status, and document every status with a response-type attribute.
- Inject what the handler needs as method parameters (DbContext, user principal, `CancellationToken`) — no service locator, no static state.
- Requests and responses are immutable records named after the use case; never expose entities directly.
- Error messages are plain English for developers; user-facing translation is the client's job. Enums travel as stable string codes.

## Validation & errors

- Every request with input has a validator next to it; validation failures become ProblemDetails `400` automatically — don't validate by hand in the handler.
- Mirror every database constraint (lengths, ranges, enums) in the validator so bad input fails as `400`, not as a database exception.
- Expected business outcomes (not found, conflict) are returned as a failed envelope with the matching status, not thrown. Exceptions are for bugs and infrastructure failures.

## Data

- Each feature owns its data: its own DbContext bound to its own schema. Never query another feature's tables.
- The database schema has **one source of truth** outside the code; EF mappings mirror it exactly (table, column, constraint and index names, lengths, comments). Change the schema first, then the mapping.
- Let the framework own transactions: handlers change tracked entities, the pipeline saves them in one transaction after the handler. Don't call `SaveChangesAsync` by hand unless you need the result mid-handler.
- Pass `CancellationToken` to every async database/IO call.

## Security

- **Authenticated by default**: every endpoint requires a signed-in user unless it explicitly allows anonymous access. Opt out deliberately and only for endpoints that truly need it.
- Identify the user from the token's subject claim, never from a request field; scope every query to that user.
- Secrets never go to `appsettings*.json` or source; they come from environment variables.

## Configuration

- Each external service/setting group gets a strongly typed options class with a `SectionName` constant and `required` init-only properties, bound from a required section and validated at startup — fail fast on bad config.

## Domain logic

- Calculations are pure, deterministic static code with no IO, kept apart from endpoints, so they can be unit-tested directly. Endpoints only map request → domain → response.
- Results that are stored or compared over time carry the model version; any change to a model parameter changes the version.

## Code style

- Follow the analyzers and the formatter, not personal taste — the build treats warnings as errors.
- Every public type and member has an XML doc comment that explains _what_ and _why_ in plain language.
- Comments are clear and concise: a `<summary>` is one short sentence about what the member does; `<param>`/`<returns>` are a few words. Add the _why_ only when it isn't obvious from the code and fits in one more sentence. Don't retell the algorithm, list every case, restate the business rules of other members or explain the history of a decision — that belongs in the code, the tests or the commit message.

## Before done

Build (analyzers, docs, formatting) and run the tests; for endpoint changes, call the endpoint once through the running API and check the status and body.

---

## In this repo (Olevin API)

- **Stack:** .NET 10 (`global.json`, SDK 10.0.100+), ASP.NET Core minimal hosting + **Wolverine HTTP** (`WolverineFx.Http`), **EF Core 10 + Npgsql** (PostgreSQL 18, `uuidv7()`), **FluentValidation** via `WolverineFx.Http.FluentValidation`, **Logto** access tokens (JWT bearer), Serilog + OpenTelemetry → **Seq**, OpenAPI + **Scalar** (Development only), Docker (`server/Dockerfile`, distroless runtime).
- **Endpoint shape** (see `ConfigureUserSettingsEndpoint`):
  - `public static class <UseCase>Endpoint` with `[Tags("<Feature>")]`; `public static async Task<Response<T>> Post(...)` (or `Get`/`Put`/`Delete`) with `[WolverinePost("/<feature>/v1/<resource>")]` and one `[ProducesResponseType<Response<T>>(StatusCodes.StatusXxx)]` per outcome.
  - Return `Response<T>.Ok(data, HttpStatusCode.Created)` / `Response<T>.Fail(HttpStatusCode.Conflict, "The settings already exist.")` (`Shared/Response.cs`); the envelope sets the HTTP status. Endpoints return `Response<T>`.
  - Parameters: request record, `ClaimsPrincipal principal`, the feature DbContext, `CancellationToken ct`. User: `principal.GetSubject()` (`Infrastructure/Auth/Auth.cs`) → `users.auth_sub`.
  - Validators: `public sealed class <UseCase>Validator : AbstractValidator<<UseCase>Request>`, picked up automatically; messages like `"Snapshot day must be from 1 to 31."`. every request and every shared part has its own validator class, one per file (`Slices/Snapshots/Validators/`: `MoneyValidator`, `ExpensesRequestValidator`; `Contracts/CreateFirstSnapshot/`: `FirstSnapshotValidator` for the contract record the Settings endpoint takes as its body; list rules shared by requests via `MoneyListRules.MoneyList`) and the parent connects them with `RuleFor(x => x.Part).NotNull().SetValidator(new PartValidator())`, so the error path is the nested one (`Expenses.Food.Amount`); no shared interface or base record for requests, and no validator of an interface or a base type — Wolverine would run it a second time; `[GeneratedRegex]` for patterns.
  - JSON: System.Text.Json with `JsonStringEnumConverter` — enums as names (`"UAH"`, `"EN"`).
  - Auth: fallback policy requires an authenticated user; only `/health`, `/openapi`, `/scalar` call `.AllowAnonymous()`.
- **Data:**
  - Schema source of truth: `infrastructure/db/schemas/<schema>.hcl` (**Atlas**, no EF migrations). Apply locally with the `atlas` service of `infrastructure/deployment/docker-compose.local.yml` (`schema apply --env local --auto-approve`). New schema → add it to `infrastructure/db/atlas.hcl` (`src` and `schemas`).
  - Order of a change: HCL (with `comment`s and `check`s) → entity (XML doc = column comment) → `<Entity>Configuration` (`HasColumnName` snake_case, `HasComment`, `HasMaxLength`, named keys/indexes like `users_pkey`, `users_auth_sub_key`, `HasDefaultValueSql("uuidv7()" | "now()")`) → validator rules for the new constraints. The configuration's summary says it mirrors the HCL file.
  - Postgres enums: C# enum members equal the labels (`UAH`, `EN`), mapped in `<Feature>Services` with `MapEnum<T>("<name>", Schema, new NpgsqlNullNameTranslator())`.
  - DbContext registered with `AddDbContextWithWolverineIntegration`; `UseEntityFrameworkCoreTransactions()` + `AutoApplyTransactions()` save changes in a transaction after each handler, with the **transactional outbox**: `PersistMessagesInDatabase` (`Infrastructure/Database/MessagePersistence.cs`) stores all of Wolverine's data (inbox, outbox, dead letters, nodes) in its own PostgreSQL schema `outbox` and makes local queues durable, so a message sent by a handler commits or rolls back with its data. Atlas doesn't touch `outbox` (not in `atlas.hcl`); the schema belongs to the `olevin_wolverine` role (`infrastructure/db/bootstrap/bootstrap.sql`), and Wolverine creates and migrates its own tables on startup (`AutoBuildMessageStorageOnStartup = AutoCreate.CreateOrUpdate`). Never describe Wolverine tables in HCL. Every `AddDbContextWithWolverineIntegration` passes `MessagePersistence.Schema` and its own connection string from `DbOptions` (one PostgreSQL role per schema: `Wolverine`, `Settings`, `FinancialWellness`); a new schema needs its role and grants in `bootstrap.sql`. Read-only message handlers carry `[NonTransactional]`; transient `NpgsqlException`/`TimeoutException` retried with cooldown (`Program.cs`).
  - New ids: `Guid.CreateVersion7()` in code (or the DB default).
- **Config & secrets:** options classes in `Infrastructure/<Concern>/` (`LogtoOptions`, `DbOptions`, `SeqOptions`); non-secret dev values in `appsettings.Development.json`; secrets in `server/.env` (loaded by DotNetEnv, `Section__Key` format, e.g. `Seq__ApiKey`) with a placeholder line in `.env.example`.
- **Domain:** `Features/FinancialWellness/Helpers/Index/` is the financial calm index model (fuzzy Mamdani + linear index, zones); `Features/FinancialWellness/Slices/Snapshots/Helpers/SnapshotScorer` runs it over a user's months; every amount carries its own currency; the index is calculated in the user's base currency from the `GetUserSettings` contract (the request carries none, the snapshot does not store it), each amount converted via `Helpers/History/ExchangeRates` (units of the currency per dollar, as `fx_rates` stores them). Changing any model parameter → bump `IndexConstants.ModelVersion` and update calibration/reference tests.
- **Code style** (`.editorconfig`, style diagnostics are warnings → errors; `SonarAnalyzer.CSharp`; `CSharpier` check on build):
  - Explicit types, no `var`; file-scoped namespaces; primary constructors; collection expressions `[...]`; `sealed` classes and records; `static` endpoint classes.
  - No braces when an `if`/`for`/`foreach`/`while` body is one line and the condition fits on one line; keep braces when anything wraps, and on all branches of an if/else chain if one needs them.
  - Single-line methods and local functions are expression-bodied (`=>`); CSharpier decides where the expression goes — don't fight it.
  - Private fields and constants are PascalCase, no `_`/`s_` prefix (`private readonly ... Rates`).
  - Comment size, e.g. `CreateSnapshotRules`: `/// The snapshot rules for the month.` — not a paragraph that lists every allowed month and why.
  - XML docs on every public member (`GenerateDocumentationFile` + warnings as errors → missing docs fail the build): `<summary>`, `<param>`, `<returns>`, `<inheritdoc/>` for overrides; endpoint summaries say `Handles <c>POST /route</c>.` and list each status in `<returns>`.
  - Format: `csharpier format .` in `server/` (global tool) — the build only checks.
- **Run locally:** infrastructure via `docker compose -f infrastructure/deployment/docker-compose.local.yml up -d`; API `dotnet run --project Olevin.Api` → `http://localhost:5001`, through Caddy at `https://api.localhost` (Scalar: `https://api.localhost/scalar`). The web app calls it server-side with the user's Logto access token (`web-app/src/shared/helpers/api`) — a contract change needs the matching Zod schema change there.
- **Finish with:** `dotnet build && dotnet test` in `server/`.

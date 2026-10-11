---
name: architecture
description: Enforce the vertical-slice layout of the Olevin .NET API (`server/`) — `Features/` own their endpoints, data and logic; code used by several features goes to `Shared/`; technical wiring lives in `Infrastructure/`; features never reach into each other except through `Contracts/`. Use this skill the moment any of these questions arise: "Where does this new file go?", "Can feature X use Y from feature Z?", "Should this be in the feature, Shared or Infrastructure?", "Do I need a new feature folder?", or "How do I move this without breaking namespaces?" Always use it before creating an endpoint, request/response record, validator, entity, DbContext, options class, helper, or constant — don't guess the place. Also trigger on refactors that cross feature boundaries or when the build fails on namespace/folder mismatch (IDE0130). Examples: adding a `GET /snapshots/v1/...` endpoint, adding a `Snapshots` DbContext, lifting a calculation to Shared, letting Snapshots read the user's base currency from Settings.
---

# Architecture

Keep the API on a **vertical-slice** architecture: each feature is a self-contained folder with its endpoints, data and logic; dependencies point one way; code is pushed to the lowest place that fits.

## Areas (top → bottom)

- **`Program.cs`** — composition root: registers infrastructure and features (`builder.Services.AddX(...)`), middleware and endpoint mapping. No logic of its own.
- **`Features/<Feature>/`** — a feature (a business area with its own endpoints and, usually, its own data). Prefer small, focused features.
- **`Shared/<Area>/`** — domain code used by **two or more** features (no endpoints, no DbContext).
- **`Infrastructure/<Concern>/`** — technical cross-cutting wiring (auth, database settings, telemetry): an `AddX` extension method plus its `XOptions`. No business logic.

```
Program.cs → Features → Shared
          ↘           ↘ Infrastructure
```

## Hard rules

1. **One-way dependencies.** `Shared` and `Infrastructure` never reference `Features`. `Infrastructure` never references `Shared` business code.
2. **Features don't reference each other's internals.** When feature A must call feature B, B gets a `Contracts/` folder holding **only** the handler (and its message) that receives that call; A sends the message, never touches B's entities, DbContext or helpers.
3. **Feature-only code stays in the feature.** Move it to `Shared/<Area>/` only when a second feature starts using it.
4. **Namespace = folder path** (`Olevin.Api.Features.Settings.Handlers.CreateUser`); one top-level type per file, file named after the type.
5. **Propose structural moves first.** Creating a new feature, moving code to Shared, or adding a Contracts folder — describe the move and get approval before doing it.

## Feature structure

```
Features/Settings/
  SettingsServices.cs            ← AddSettings(): DbContext and other DI of the feature
  Data/
    SettingsDbContext.cs         ← the feature's DbContext, one Postgres schema
    Users/
      User.cs                    ← entity
      UserConfiguration.cs       ← IEntityTypeConfiguration, mirrors the Atlas schema
    UserSettings/
      UserSetting.cs
      UserSettingConfiguration.cs
      Enums/
        CurrencyCode.cs
  Handlers/
    CreateUser/
      CreateUserEndpoint.cs      ← endpoint and handler in ONE file
    ConfigureUserSettings/
      ConfigureUserSettingsEndpoint.cs
      ConfigureUserSettingsRequest.cs
      ConfigureUserSettingsValidator.cs
```

- **`Handlers/<UseCase>/`** — one folder per endpoint: `<UseCase>Endpoint.cs` (the endpoint _is_ the handler — no separate Command/Handler classes), `<UseCase>Request.cs`, `<UseCase>Response.cs`, `<UseCase>Validator.cs`, `<UseCase>Mapping.cs`, plus nested request/response records it needs (`IncomeRequest`, `ScoreResponse`). Things used by one use case stay in its folder.
- **Feature root / topic folders** — domain logic used by several use cases of the feature: at the root (`SnapshotScorer.cs`, `SnapshotValidationRules.cs`) or grouped by topic (`Snapshots/Changes/`, `Snapshots/Currency/`).
- **`Data/`** — only when the feature stores data: its own `<Feature>DbContext`, one sub-folder per entity (entity + configuration + `Enums/`).
- **`<Feature>Services.cs`** — `Add<Feature>(this IServiceCollection)` registered in `Program.cs`, only when the feature has DI to register.
- **`Contracts/`** — only when another feature calls this one (see rule 2).
- A folder under `Features/` should have at least one endpoint.

## Shared / Infrastructure structure

```
Shared/
  Response.cs                    ← used by every feature's endpoints
  Index/                         ← the index model, used by Snapshots and Advice
    IndexConstants.cs
    Fuzzy/  Indicators/  Model/
Infrastructure/
  Auth/       Auth.cs (AddLogtoAuthentication, GetSubject) + LogtoOptions.cs
  Database/   DatabaseServices.cs (AddDatabase) + DbOptions.cs
  Telemetry/  Telemetry.cs (AddTelemetry) + SeqOptions.cs
```

**Don't create a sub-folder for a single occupant** — add the topic folder once a second related file appears.

## Push down on shared access

When **two or more** features need the same thing (calculation, value object, constant, record), **don't duplicate and don't reach sideways** — move it to the lowest place that fits:

- domain logic / types without endpoints or storage → **`Shared/<Area>/`**
- technical wiring (a new external service, options, middleware) → **`Infrastructure/<Concern>/`**
- an operation that belongs to another feature (its data, its rules) → that feature's **`Contracts/`** handler

Promote only what's actually shared; keep single-use code local. Each feature owning its own copy of a small enum that maps its own DB enum (e.g. `CurrencyCode` in Settings and in Snapshots) is acceptable — don't merge them just to deduplicate.

## Verify

Build after moving files (`dotnet build` — namespace/folder mismatches fail the build), and check no forbidden references appeared:

```bash
grep -rn "using Olevin.Api.Features" Olevin.Api/Shared Olevin.Api/Infrastructure
```

---

## In this repo (Olevin API)

- Solution `server/Olevin.slnx`: `Olevin.Api` (ASP.NET Core, Wolverine HTTP) + `Olevin.Tests` (xUnit). Shared MSBuild settings in `Directory.Build.props`.
- **Existing features:** `Settings` (users and user settings, `settings` schema, endpoints `POST /settings/v1/user`, `POST /settings/v1/user-settings`), `Snapshots` (stateless `POST /snapshots/v1/calculate`; `Changes/`, `Currency/`), `Advice` (sensitivity analysis and advice — logic only, no endpoint yet).
- **Shared:** `Response<T>` envelope, `Index/` (fuzzy Mamdani model, indicators, linear index, zones, `IndexConstants.ModelVersion`).
- **No architecture test or analyzer enforces the rules above** — the grep in _Verify_ and review are the only guard. No feature uses `Contracts/` yet; the first cross-feature call introduces it.
- **Database ownership:** one Postgres schema per feature that stores data (`settings` → `SettingsDbContext`). The schema itself lives outside `server/`, in `infrastructure/db/schemas/<schema>.hcl` (Atlas) — see `backend-expert`.

---
name: architecture
description: Enforce the vertical-slice layout of the Olevin .NET API (`server/`) — `Features/` own their endpoints, data and logic; code used by several features goes to `Shared/`; technical wiring lives in `Infrastructure/`; features never reach into each other except through `Contracts/`. Use this skill the moment any of these questions arise: "Where does this new file go?", "Can feature X use Y from feature Z?", "Should this be in the feature, Shared or Infrastructure?", "Do I need a new feature folder?", or "How do I move this without breaking namespaces?" Always use it before creating an endpoint, request/response record, validator, entity, DbContext, options class, helper, or constant — don't guess the place. Also trigger on refactors that cross feature boundaries or when the build fails on namespace/folder mismatch (IDE0130). Examples: adding a `GET /snapshots/v1/...` endpoint, adding a `Snapshots` DbContext, lifting a calculation to Shared, letting Snapshots read the user's base currency from Settings.
---

# Architecture

Keep the API on a **vertical-slice** architecture: each feature is a self-contained folder with its endpoints, data and logic; dependencies point one way; code is pushed to the lowest place that fits.

## Areas (top → bottom)

- **`Program.cs`** — composition root: registers infrastructure and features (`builder.Services.AddX(...)`), middleware and endpoint mapping. No logic of its own.
- **`Features/<Feature>/`** — a feature: a business area that owns one Postgres schema. It holds the schema's `Data/`, helpers shared by its slices, and `Slices/` with the capabilities that have endpoints.
- **`Shared/<Area>/`** — domain code used by **two or more** features (no endpoints, no DbContext).
- **`Infrastructure/<Concern>/`** — technical cross-cutting wiring (auth, database settings, telemetry): an `AddX` extension method plus its `XOptions`. No business logic.

```
Program.cs → Features → Shared
          ↘           ↘ Infrastructure
```

## Hard rules

1. **One-way dependencies.** `Shared` and `Infrastructure` never reference `Features`. `Infrastructure` never references `Shared` business code.
2. **Features don't reference each other's internals.** When feature A must call feature B, the B slice that owns the data gets a `Contracts/<Operation>/` folder inside it (e.g. `Slices/UserSettings/Contracts/`, `GetUserSettingsRequest` + `GetUserSettingsHandler`) holding **only** what that call needs: `<Operation>Request`, `<Operation>Handler` and one flat returned DTO (`UserSettingsDto`: `UserId` and the settings — no nested DTOs), which the handler fills with a `Select(... => new <Dto>(...))` projection in the query itself. A sends the request, never touches B's entities, DbContext or helpers; the contract itself uses no type of another feature and exposes none of B's `Data/` types, only its own records (and `Shared/` types such as `CurrencyCode`).
3. **Feature-only code stays in the feature.** Move it to `Shared/<Area>/` only when a second feature starts using it.
4. **Namespace = folder path** (`Olevin.Api.Features.Settings.Slices.Users.Handlers.CreateUser`); one top-level type per file, file named after the type.
5. **Propose structural moves first.** Creating a new feature or slice, moving code to Shared, or adding a Contracts folder — describe the move and get approval before doing it.
6. **Slices of one feature don't reference each other.** Dependencies inside a feature go one way: `Slices/<Slice>` → `Helpers/` and `Data/`. What two slices both need moves to the feature's `Helpers/`; `Helpers/` and `Data/` never reference `Slices/`.

## Feature structure

A feature is **one Postgres schema** (`settings`, `financial_wellness`). Inside it, the schema's data and the shared helpers sit at the feature level, and every user-facing capability is a **slice** under `Slices/` (`Snapshots`, `Home`, later `History`, `Trend`, `IndexChanges`) with its own handlers. Handlers of different capabilities never share one flat `Handlers/` folder.

```
Features/Settings/
  SettingsServices.cs            ← AddSettings(): DbContext and other DI of the feature
  Data/                          ← the schema: one DbContext, one sub-folder per entity
    SettingsDbContext.cs         ← the feature's DbContext, one Postgres schema
    Users/
      User.cs                    ← entity
      UserConfiguration.cs       ← IEntityTypeConfiguration, mirrors the Atlas schema
    UserSettings/
      UserSetting.cs
      UserSettingConfiguration.cs
      LocaleCode.cs
  Helpers/                       ← code shared by several slices (omit while there is none)
  Slices/
    Users/
      Handlers/                  ← one use case: its files lie directly in Handlers/
        CreateUserEndpoint.cs    ← endpoint and handler in ONE file
    UserSettings/
      Contracts/                 ← only when another feature calls this slice; one operation → files directly
        GetUserSettingsRequest.cs
        GetUserSettingsHandler.cs
        UserSettingsDto.cs
      Handlers/
        ConfigureUserSettingsEndpoint.cs
        ConfigureUserSettingsRequest.cs
        ConfigureUserSettingsValidator.cs
```

**A folder is created only when it holds several folders.** `Handlers/`, `Contracts/`, `Jobs/` and the like are grouping folders: while there is one use case (one operation, one job) its files lie directly in the grouping folder, and the grouping folder itself disappears when it would hold a single file with nothing to group (`Slices/FxRates/UpdateFxRatesJob.cs`). When the second use case appears, each use case gets its own `<UseCase>/` folder (`Slices/Snapshots/Handlers/CreateSnapshot/` + `Handlers/UpdateSnapshot/`, `Contracts/CreateFirstSnapshot/` + `Contracts/HasFirstSnapshot/`); the first one moves into its folder at that moment.

- **`Slices/<Slice>/`** — one business capability of the feature, named after it (`Snapshots`, `Home`). It has the same structure a feature used to have: `Handlers/<UseCase>/`, and — when the slice has logic of its own — files at its root or its own `Helpers/`. Code used by one slice stays in it; code used by two or more moves to the feature's `Helpers/`.
- **`Handlers/`** — with several endpoints, one `Handlers/<UseCase>/` folder per endpoint (with one endpoint its files lie directly in `Handlers/`): `<UseCase>Endpoint.cs` (the endpoint _is_ the handler — no separate Command/Handler classes), `<UseCase>Request.cs`, `<UseCase>Response.cs`, `<UseCase>Validator.cs`, `<UseCase>Mapping.cs` only when it holds real logic (the response is built in the endpoint itself). Every response goes to `Responses/` as soon as there are two, nested records included (`Slices/Home/Handlers/GetIndex/Responses/` = `GetIndexResponse` + `IndexChangeResponse`); the same for requests. Things used by one use case stay in its folder; what two use cases of the slice share moves to the slice's kind folder (`Slices/Snapshots/Requests/EstimateRequest` — `CreateFirstSnapshot` and `UpdateEstimatedSnapshot`).
- **Folders are named by what the files are.** Inside a slice, group files by kind and name the folder after the kind: `Models/` (records and classes that carry data), `Requests/`, `Responses/`, `Enums/`, `Mappings/`, `Validators/`; logic that is not a kind goes into the slice's own `Helpers/` (`Slices/Snapshots/Helpers/` = `FxRateLookup`, `SnapshotScorer`), not into a topic folder. A kind folder is created when it holds two or more files; a single file stays at the slice root. Name a folder differently from the types inside it (plural or topic name): a namespace named like a type breaks name lookup (CS0118).
- **Two of a kind → a sub-folder.** When a folder holds two or more files of the same kind, group them (`Requests/`, `Responses/`, `Mappings/`): `Slices/Snapshots/Requests/` = `ExpensesRequest` + `EstimateRequest`, `Slices/Snapshots/Validators/` = `MoneyValidator` + `ExpensesRequestValidator` + `EstimateRequestValidator`, `Slices/Snapshots/Mappings/` = `SnapshotMapping` + `SnapshotAmountsMapping` + `SnapshotHistoryMapping`.
- **Feature `Helpers/`** — only what **two or more slices** really share (`Helpers/Index/` — the index model used by `Snapshots` and `Home`). Everything that belongs to one slice lives in that slice, even if several of its handlers use it (`Slices/Snapshots/Models/`, `Mappings/`, `Requests/`, `Validators/`, `Helpers/`); a feature `Helpers/` is not a place for slice code that has no other home. `Helpers/` holds nothing slice-specific.
- **`Data/`** — only when the feature stores data: its own `<Feature>DbContext`, one sub-folder per entity (entity + configuration + its enums, as files next to the entity).
- **Jobs and seeders** — background work and seed data are a slice of their own named after the thing they maintain (`Slices/FxRates/` = `UpdateFxRatesJob` + `FxRateSeeder`); they are not kept in `Jobs/` / `Seeders/` folders at the feature level.
- **`<Feature>Services.cs`** — `Add<Feature>(this IServiceCollection)` registered in `Program.cs`, only when the feature has DI to register.
- **`Slices/<Slice>/Contracts/`** — only when another feature calls this slice (see rule 2); it sits in the slice that owns the data and logic, next to its `Handlers/`, not at the feature level. With one operation its files lie directly in `Contracts/`; with several, one `<Operation>/` folder each.
- A slice should have at least one endpoint (a slice whose endpoints are not written yet is the only exception).

## Shared / Infrastructure structure

```
Shared/
  Response.cs                    ← used by every feature's endpoints
  CurrencyCode.cs                ← used by Settings and Financial Wellness
Infrastructure/
  Auth/       Auth.cs (AddLogtoAuthentication, GetSubject) + LogtoOptions.cs
  Database/   DatabaseServices.cs (AddDatabase) + DbOptions.cs + MessagePersistence.cs (Wolverine inbox/outbox)
  Telemetry/  Telemetry.cs (AddTelemetry) + SeqOptions.cs
```

**Don't create a sub-folder for a single occupant** — add the topic folder once a second related file appears.

## Push down on shared access

When **two or more** features need the same thing (calculation, value object, constant, record), **don't duplicate and don't reach sideways** — move it to the lowest place that fits:

- domain logic / types without endpoints or storage → **`Shared/<Area>/`**
- technical wiring (a new external service, options, middleware) → **`Infrastructure/<Concern>/`**
- an operation that belongs to another feature (its data, its rules) → that feature's slice **`Contracts/`** handler

Promote only what's actually shared; keep single-use code local. `CurrencyCode` is an example: it is used by Settings and Financial Wellness, so it lives in `Shared/CurrencyCode.cs`.

## Verify

Build after moving files (`dotnet build` — namespace/folder mismatches fail the build), and check no forbidden references appeared:

```bash
grep -rn "using Olevin.Api.Features" Olevin.Api/Shared Olevin.Api/Infrastructure
```

---

## In this repo (Olevin API)

- Solution `server/Olevin.slnx`: `Olevin.Api` (ASP.NET Core, Wolverine HTTP) + `Olevin.Tests` (xUnit). Shared MSBuild settings in `Directory.Build.props`.
- **Existing features:** `Settings` (users and user settings, `settings` schema; slices `Users` — `POST /settings/v1/user`, `UserSettings` — `POST /settings/v1/user-settings`; `Slices/UserSettings/Contracts` (`GetUserSettings`) gives other features the whole user with their settings), `FinancialWellness` (`financial_wellness` schema: `fx_rates`, `snapshots`, `estimated_snapshots`; slices: `Slices/FxRates/` — `UpdateFxRatesJob`, `FxRateSeeder`, `FxRateMapping`; `Slices/Snapshots/` — `Contracts/CreateFirstSnapshot/` (the first snapshot with the 2 estimates, sent by `Settings/Slices/Onboarding/Handlers/CreateFirstSnapshot` — `POST /settings/v1/user/onboarding/first-snapshot`, whose body is the contract's `FirstSnapshot` validated by `FirstSnapshotValidator`), `Contracts/HasFirstSnapshot/` (read by `GetOnboardingStatus`), `Handlers/CreateSnapshot/` (`POST /financial-wellness/v1/snapshots`, any later month; `CreateSnapshotRules` — the allowed months), `Handlers/UpdateSnapshot/` (`PUT /financial-wellness/v1/snapshots/{id}`, new amounts and self-rating, the month and `FxDate` are fixed), `Handlers/DeleteSnapshot/` (`DELETE /financial-wellness/v1/snapshots/{id}`, 204 without the envelope on success, 409 for the only snapshot; `DeleteSnapshotRules` — whether the later snapshots need scoring again), `Handlers/UpdateEstimatedSnapshot/` (`PUT /financial-wellness/v1/snapshots/estimates/{id}`, takes `EstimateRequest`, new amounts of an onboarding estimate, the month and `FxDate` are fixed, every snapshot is scored again), plus the slice's own `Models/` (`SnapshotScore`, `SnapshotRates`), `Requests/` (`ExpensesRequest`, `EstimateRequest`), `Mappings/` (`SnapshotMapping`, `SnapshotAmountsMapping`), `Validators/` (`MoneyValidator`, `ExpensesRequestValidator`, `EstimateRequestValidator`, `MoneyListRules`), `Helpers/` (`FxRateLookup` — the stored rates a new snapshot is converted at, `SnapshotScorer`, `SnapshotRescorer` — scores the whole history again and applies it to the changed month and later ones); `Slices/Home/` — `Handlers/GetIndex/` (`GET /financial-wellness/v1/home/index`, the latest index with its zone, the change since the previous snapshot and the next snapshot date; `GetIndexRules` — rounding to one decimal, the change and the next snapshot date, `Responses/`), `Handlers/GetAdvices/` (`GET /financial-wellness/v1/home/advices`, the top three advice for the latest snapshot, one response record per indicator under a `kind` discriminator, the gain of the final index (with the self-rating, up to 100) and an `AdviceStatus` (`Shown`, `AtMaximum`, `AllBest`, `NoQuickWin`); the sensitivity analysis in its own `Helpers/` (`AdviceBuilder` — the usual 5-point step, and, when the usual one gives none, the smallest larger step from the raw value that gives `ExtendedGain` (5 points) or the most within reach, `SensitivityAnalyzer`), `Models/` and `Enums/`, `GetAdvicesMapping` — the shown advice, the status, rounding, `Responses/`); planned: `History`, `Trend`, `IndexChanges`; `Helpers/Index/` — the index model shared by `Snapshots` and `Home` (fuzzy Mamdani, indicators, linear index, zones, `IndexConstants.ModelVersion`); `Helpers/History/` — the stored snapshots and estimates as figures in the base currency, shared by `Snapshots` and `Home` (`SnapshotHistory` — loads the rates of every month and orders the history, `SnapshotHistoryMapping`, `SnapshotFigures`, `ExchangeRates`); `Helpers/SnapshotMonth` — the month of a snapshot, shared by `Snapshots` and `Home`).
- **Shared:** `Response<T>` envelope, `CurrencyCode`, `Data/JsonbConversion`.
- **No architecture test or analyzer enforces the rules above** — the grep in _Verify_ and review are the only guard. The first cross-feature call is `Settings/Slices/UserSettings/Contracts` (`GetUserSettings`): the caller sends it with `IMessageBus.InvokeAsync<T>`, the handler is a static `<Operation>Handler.Handle`, read-only handlers carry `[NonTransactional]`, and the handler returns the response record directly, `null` when there is nothing to return (`InvokeAsync<UserSettingsDto?>` gives `null` without an error; no wrapper record).
- **Database ownership:** one Postgres schema per feature that stores data (`settings` → `SettingsDbContext`). The schema itself lives outside `server/`, in `infrastructure/db/schemas/<schema>.hcl` (Atlas) — see `backend-expert`.

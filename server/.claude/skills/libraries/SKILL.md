---
name: libraries
description: Pick the right NuGet package and use existing ones correctly in the Olevin .NET API (`server/`), which keeps a deliberate set of dependencies (Wolverine, EF Core + Npgsql, FluentValidation, JWT bearer, Serilog, OpenTelemetry, Scalar, DotNetEnv; xUnit v3 for tests). Use this skill before running `dotnet add package`, when deciding "is there already something that does X?", or when choosing between tools already in the project. Trigger on any of these: "install X", "add a package for Y", "how do I handle validation / mapping / messaging / retries / logging / HTTP clients / background jobs / migrations", "should I use MediatR / AutoMapper / Dapper / Polly / Moq / FluentAssertions?", "can I use Z library here?". Also use it when an existing package might already solve the problem — this skill has the full catalog. Examples: "add a mapper", "install Swashbuckle", "do we have a retry library?", "which mocking library do we use?", "where do I put an analyzer package?".
---

# Libraries — what to use, and when

Prefer the tools already in the project; add a dependency only when it clearly earns its place.

## Choosing

- **Reuse first.** Check existing packages and in-repo helpers before adding anything. The best dependency is often one you already have — or the BCL / ASP.NET Core itself.
- **Justify a new package** against: does the framework already do this? is it small, maintained, and well-licensed? does it pull a large transitive tree? does it overlap with something already in the stack? A new package is a long-term liability (security, upgrades, maintenance).
- **Right tool for the job, not the trendiest.** Match the library to the actual need and the existing stack; don't introduce a competing solution to one already present.
- **Production vs build/test.** Packages the API uses at runtime go in the API project; analyzers and build tools are `PrivateAssets="all"`; test-only packages go in the test project.
- **Pin/lockfile.** Versions are exact; respect and commit the lock files.

## Using

- Use the library's documented public API; don't reach into internals.
- Keep usage centralized/wrapped when it's likely to change or is used widely (an `AddX` extension in `Infrastructure/`), so swaps are cheap.
- Read the version's own docs (APIs change between majors) rather than relying on memory — .NET 10, EF Core 10, Wolverine 6 and xUnit v3 all differ from older majors.

---

## In this repo (Olevin API)

**`Olevin.Api` (runtime):**

- **HTTP + handlers + messaging:** `WolverineFx.Http` (via `WolverineFx.Http.FluentValidation`), `WolverineFx.RuntimeCompilation`. Wolverine is the endpoint framework _and_ the in-process message bus — use it for cross-feature calls (`Contracts/`), background/async work and retries. Don't add MediatR, MassTransit, Hangfire or Polly.
- **Validation:** FluentValidation (through `WolverineFx.Http.FluentValidation`) — no DataAnnotations validation.
- **Data:** `Npgsql.EntityFrameworkCore.PostgreSQL` + `WolverineFx.EntityFrameworkCore` (transactions); `Microsoft.EntityFrameworkCore.Design` (design-time only). Schema is managed by **Atlas** in `infrastructure/db`, not EF migrations. Don't add Dapper or another ORM.
- **Auth:** `Microsoft.AspNetCore.Authentication.JwtBearer` against Logto (`Infrastructure/Auth`).
- **API docs:** `Microsoft.AspNetCore.OpenApi` + `Scalar.AspNetCore` (Development only). No Swashbuckle/NSwag.
- **Observability:** `Serilog.AspNetCore`, `Serilog.Sinks.Seq`, `Serilog.Enrichers.Environment`; `OpenTelemetry.Extensions.Hosting` + `.Instrumentation.AspNetCore/Http/Runtime` + `.Exporter.OpenTelemetryProtocol` → Seq (`Infrastructure/Telemetry`).
- **Config:** `DotNetEnv` loads `server/.env`; everything else is standard `IConfiguration` + options.
- **Built-ins to prefer:** `System.Text.Json` (no Newtonsoft), `IHttpClientFactory` for outbound HTTP, `TimeProvider` for testable time, `[GeneratedRegex]`, manual mapping in `<UseCase>Mapping` static classes (no AutoMapper/Mapster).

**`Directory.Build.props` (every project, build-only):** `CSharpier.MsBuild` (format check on build), `SonarAnalyzer.CSharp`. New analyzers go here with `PrivateAssets="all"`.

**`Olevin.Tests`:** `xunit.v3` on Microsoft.Testing.Platform, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`; `FluentValidation.TestHelper` comes with FluentValidation. Plain `Assert` — no FluentAssertions/Shouldly; no mocking library (tests use real code, see `testing-strategy`).

**Global tools on this machine:** `csharpier`, `dotnet-ef`, `dotnet-sonarscanner`.

**Adding a package:** `dotnet add Olevin.Api package <Id> --version <exact>` (or `Olevin.Tests`), then build so `packages.lock.json` updates (`RestorePackagesWithLockFile`; the Dockerfile restores with `--locked-mode`, so a stale lock file breaks the image) and commit the lock file with the change. Keep related packages on the same version (all `WolverineFx.*`, all `OpenTelemetry.*`, all `Microsoft.*` 10.x).

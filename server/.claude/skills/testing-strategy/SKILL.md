---
name: testing-strategy
description: Decide where to test, what to test, and how to run tests in the Olevin .NET API (`server/`, xUnit v3 unit tests in `Olevin.Tests`). Use this skill when: writing a new test, a test is failing, deciding whether a calculation, validator, mapping or endpoint needs a test, building test data, changing the index model (calibration and reference tests), or verifying a change before calling it done. Trigger on: "write a test for X", "does this need a test?", "dotnet test is failing", "test this validator", "run only these tests", "is this ready?". Also use it when adding domain logic or a validator (ships with unit tests). Examples: "test SnapshotScorer for estimates", "add cases to ConfigureUserSettingsValidatorTests", "the calibration test broke after I changed a membership function", "what's the test command?".
---

# Testing strategy

Fast, deterministic **unit** tests for logic, validation and pure endpoints. Put each test at the right level and keep it deterministic.

## What to test where

- **Domain logic** — calculations, scoring, conversions, edge cases: the main target. Test the public entry point of the calculation with realistic inputs.
- **Validators** — every rule: valid boundaries accepted, each invalid case rejected for the right property.
- **Mappings and pure endpoints** — an endpoint without IO (no DbContext, no outbound calls) is called directly as a static method, without the web host.
- **Endpoints with IO** — keep them thin and move decisions into pure code that is tested; don't stand up the web host, database or HTTP for tests.
- **Reference data** — when the domain has a published/control example or calibration targets, pin them in tests so any model change is visible.

## Conventions

- Test the behavior/contract, not the implementation.
- Deterministic: fixed inputs, no wall-clock time (`TimeProvider` if time is needed), no randomness without a seed, floating point compared with an explicit precision.
- Style: one test class per unit under test; test names state the behavior (`Subject_Condition_Result`); `[Theory]` with data for input tables.
- Build test data with a small valid factory plus `with` for the variation under test; share bigger fixtures in one test-data place.
- No mocks — use real code; fakes only at real boundaries (time, network) when unavoidable.
- New domain logic or a new validator ships with tests.
- Run the relevant tests while iterating; run the full suite before claiming done, and report results plainly.

---

## In this repo (Olevin API)

- **Project:** `Olevin.Tests` (xUnit v3, Microsoft.Testing.Platform runner set in `global.json`). `Xunit` is a global using; `CS1591` (XML docs) is suppressed for tests, the rest of the analyzers, style rules and CSharpier still apply.
- **Unit tests only** — no API/HTTP or database tests.
- **Layout mirrors the source:** `Olevin.Api/Features/FinancialWellness/Snapshots/SnapshotScorer.cs` → `Olevin.Tests/Features/FinancialWellness/Snapshots/SnapshotScorerTests.cs`; namespace `Olevin.Tests.<same path>`. Classes `public sealed class <Type>Tests`.
- **Patterns in use:**
  - Pure endpoint called directly: `CalculateSnapshotsEndpoint.Post(ControlExample.Request())` (`CalculateSnapshotsEndpointTests`).
  - Validators with `FluentValidation.TestHelper`: `Validator.TestValidate(Valid() with { SnapshotDay = day }).ShouldHaveValidationErrorFor(r => r.SnapshotDay)` (`ConfigureUserSettingsValidatorTests`).
  - Data-driven: `[Theory]` + `[InlineData]`, or `[MemberData]` with `TheoryData<T>`.
  - Doubles: `Assert.Equal(expected, actual, Precision)` with a `private const double Precision` per class.
  - Names: `SnapshotDay_OutsideTheBounds_IsRejected`, `BaseCurrency_ChangesTheAmountsButNotTheIndex`; single-assert tests are expression-bodied.
  - Shared fixtures in `Olevin.Tests/TestData/` (`ControlExample` — the control example of the method); per-area fixtures next to their tests (`SyntheticProfiles`, `ControlExampleAdvice`).
- **Index model guards:** `ControlExampleTests`, `CalibrationTests`, `ReferenceProfilesTests`, `OlevinModelTests`. If one fails after a model change, the change altered results — confirm it's intended, update the expected values deliberately and bump `IndexConstants.ModelVersion`; never loosen the precision to make it pass.
- **DB-touching endpoints** (`CreateUserEndpoint`, `ConfigureUserSettingsEndpoint`) aren't tested directly; their validators are. Keep decisions that need tests out of the DbContext code.
- **Run** (in `server/`):
  - all: `dotnet test`
  - one class: `dotnet test --filter-class "*ConfigureUserSettingsValidatorTests"`
  - one test: `dotnet test --filter-method "*SnapshotDay_OutsideTheBounds_IsRejected"`
- **Before done:** `dotnet build && dotnet test`. No CI or git hooks run them for you.

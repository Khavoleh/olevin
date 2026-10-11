---
name: testing-strategy
description: Decide where to test, what to test, and how to run tests in the Olevin Next.js web app (`web-app/`, Vitest unit tests). Use this skill when: writing a new test, a test is failing, deciding whether a helper, Server Action or component needs a test, mocking `fetch` or env vars, checking accessibility, or verifying a change before calling it done. Trigger on: "write a test for X", "does this need a test?", "my vitest test is failing", "add accessibility coverage", "run the tests", "is this ready?". Also use it when adding a pure helper (ships with a unit test) or a new page/section (needs the build + a manual a11y pass). Examples: "test the getActiveNavItem helper", "test apiFetch error handling", "my unit test for getLanguage is broken", "what's the test command?".
---

# Testing strategy

Two levels: fast **unit** tests for logic, and **end-to-end (e2e)** tests for real rendered behavior and accessibility. Put each at the right level and keep them deterministic.

## What to test where

- **Unit** — pure logic: helpers, transformations, computed values, parsers, edge cases. Colocate next to the unit under test. Don't unit-test rendered UI.
- **Extract to test.** Logic hiding in a component, route, proxy or Server Action (path parsing, step lookup, active item, formatting) moves into a pure helper that ships with a unit test; the component/action stays a thin caller.
- **E2E** — real user-visible behavior across the target browsers/viewports; cover each supported locale where relevant.
- **Accessibility** — a gated test (e.g. axe): assert zero violations; extend the existing a11y suite rather than forking it.

## Conventions

- Test the behavior/contract, not the implementation.
- Make tests deterministic: fake timers/clock, fixed inputs; anchor time-zone-sensitive values in UTC.
- Style: one `describe` per function; behavior-named cases (`"should …"`); loop/table over inputs.
- Mocks/fakes only at real boundaries (time, network, env) — prefer real code over mocks when cheap. Always restore them after each test.
- A new pure helper ships with a unit test; a new page/section is covered by the a11y check at minimum.
- Run the relevant single test while iterating; run the full suite before claiming done, and report results plainly.

---

## In this repo (Olevin web app)

- **Unit (Vitest 5, `*.unit.ts`):** `vitest.config.ts` — `include: ["src/**/*.unit.ts"]`, Node environment, no `globals` (import `describe/it/expect/vi` from `"vitest"`); `@shared`/`@widgets`/`@features` aliases work. Place beside source: in its own item folder when the item has one (`src/shared/helpers/get-i18n/get-i18n.unit.ts`, `src/widgets/app-shell/helpers/get-active-nav-item/get-active-nav-item.unit.ts`), or next to the file at slice root (`src/features/onboarding/get-onboarding-step.unit.ts`).
  - Only `.ts` files are picked up — components (`.tsx`), Server Actions and route files aren't unit-tested; test the helpers they call.
  - Boundaries: `vi.stubEnv` for env vars, `vi.stubGlobal("fetch", fetchMock)` with real `Response.json(...)` / `new Response(null, { status })`, undone with `vi.unstubAllEnvs()` / `vi.unstubAllGlobals()` / `mockReset()` in `afterEach` — see `api-fetch.unit.ts`.
  - Language helpers must also reject `Object.prototype` keys (`constructor`, `toString`, `__proto__`) — keep that case when touching them.
  - Run: `pnpm unit` · `pnpm vitest run src/path/file.unit.ts` · `pnpm vitest run -t "name"`.
- **E2E / a11y: not set up yet** (no Playwright/axe in the project). Don't add the tooling unless asked. Until then, a new page/section is verified by `pnpm build` (catches prerender and Suspense errors) plus a manual pass in the browser: `/en/...` and `/uk/...`, phone/tablet/laptop widths, keyboard only. When a suite is added, it becomes the gate described above.
- **Before done:** `pnpm validate` = build + unit + lint + prettier:check + check-types + depcruise. No CI or git hooks run it for you — run it yourself.

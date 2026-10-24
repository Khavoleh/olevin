---
name: architecture
description: Enforce Feature-Sliced Design (FSD) layer rules — downward-only imports, barrel-only cross-slice access, and strict `index.ts` placement — in the Olevin Next.js web app (`web-app/`). Use this skill the moment any of these questions arise: "Where does this new file go?", "Can I import X from Y?", "Should this be a feature, widget, or shared?", "Does this slice need a barrel?", "Does this belong in `src/app` or in a feature?", or "How do I move this without breaking depcruise?" Always use it before creating a route, component, Server Action, helper, constant, icon, or i18n file — don't guess the layer. Also trigger on refactors that cross slice boundaries or when `pnpm depcruise` is failing. Examples: adding a new `src/features/trend/` slice, adding a page under `src/app/[lang]/(app)/`, lifting a helper to shared, creating a sub-component inside an existing feature.
---

# Architecture

Keep the codebase on a layered **Feature-Sliced Design (FSD)** architecture: one-way dependency chain, small modular slices, code pushed to the lowest layer that fits.

## Layer purpose (top → bottom)

- **app** — Next.js routing: `page.tsx`, `layout.tsx`, `route.ts`, metadata, `generateStaticParams`, plus `src/proxy.ts`. Thin: read `params`, compose features and widgets, nothing else.
- **features** — a feature (unit of user-facing functionality). Not necessarily a whole page; prefer small, focused features.
- **widgets** — reusable elements **with** business logic.
- **shared** — reusable elements **without** business logic (UI primitives, helpers, constants, types, config). The leaf layer.

```
app → features → widgets → shared
```

## Hard rules

1. **Downward imports only.** Never import a higher layer from a lower one; never reach sideways into another slice's internals.
2. **Cross-slice/cross-layer imports go through the slice's public barrel (`index.ts`)** — never a deep internal path. The barrel is the public API; everything else is private.
3. **No circular dependencies.**
4. Prefer path aliases over long relative `../../..` chains.
5. **Routes stay thin.** A `page.tsx` awaits `params` and renders one feature; a `layout.tsx` renders a shell widget or a feature's frame. UI, data fetching, Server Actions and translations live in the slice, not in `src/app`.

## Where `index.ts` lives — the exact rule

`index.ts` exists at **exactly these levels**, nowhere else:

| Location                         | Purpose                                                              |
| -------------------------------- | -------------------------------------------------------------------- |
| `src/features/<slice>/index.ts`  | Public API for a feature slice                                       |
| `src/widgets/<slice>/index.ts`   | Public API for a widget slice                                        |
| `src/shared/<category>/index.ts` | Public API for a shared category (components, helpers, constants, …) |

**Never add `index.ts` to any inner/nested folder**, and never to `src/app`. Sub-components and sub-folders inside a slice are private implementation details imported with direct relative paths by their parent files.

## Feature / widget slice structure

Each slice is a self-contained folder at the layer root. Everything inside stays private behind the single top-level barrel:

The root component and `index.ts` sit at the slice root. Two or more files of the same kind go into a kind folder (`components/`, `constants/`, `helpers/`, `i18n/`, `actions/` for Server Actions, `loaders/` for the `cache`-wrapped functions Server Components read API data with, `api/` for Orval output); a lone file of its kind stays at the root (`features/trend/chart-point.ts`). Loaders and Server Actions are the exception: they always go to `loaders/` and `actions/`, even a single one (`features/profile/loaders/get-profile-settings.ts`). An item with its own test gets its own sub-folder inside the kind folder. None of these folders has an `index.ts`.

```
src/features/onboarding/
  Onboarding.tsx         ← root component (entry point for the slice)
  index.ts               ← barrel
  onboarding-store.ts    ← the only store, so it stays at the root
  actions/               ← Server Actions
    configure-settings.ts
    get-saved-base-currency.ts
  api/                   ← Orval client of the `Onboarding` API tag
  components/
    FlowShell.tsx
    RestoreBaseCurrency.tsx
    SettingsStep/        ← component with its own sub-components
      SettingsStep.tsx
      CurrencyField.tsx
      TimezoneField.tsx
  constants/
    onboarding-stages.ts
    onboarding-steps.ts
  helpers/
    get-onboarding-page/ ← item with a test
      get-onboarding-page.ts
      get-onboarding-page.unit.ts
    get-onboarding-step/
      get-onboarding-step.ts
      get-onboarding-step.unit.ts
    sync-onboarding-status.ts
  i18n/
    flow-shell-i18n.ts
    onboarding-i18n.ts
```

`Onboarding.tsx` imports its parts via direct relative paths:

```ts
import { ONBOARDING_I18N } from "./i18n/onboarding-i18n";
import { ONBOARDING_STEPS } from "./constants/onboarding-steps";
```

Nothing outside `src/features/onboarding/` ever imports from inside it — only through `index.ts`.

The barrel exports the default component, plus **named** exports only for what a route genuinely needs (e.g. `features/onboarding` also exports `FlowShell` for the step layout and `getOnboardingStep`/`ONBOARDING_STEPS` for `generateStaticParams` and `notFound()`). Don't export anything "just in case".

## Shared category structure

Each category in `src/shared/` has one `index.ts` that re-exports everything by name. Inside, files may be grouped into topic sub-folders, and an item that needs more than one file (implementation + test) gets its own sub-folder — **none** of these sub-folders has an `index.ts`; the category barrel exports from them directly.

```
src/shared/helpers/
  api/                   ← topic group — NO index.ts
    api-error/           ← multi-file item — NO index.ts
      api-error.ts
      api-error.unit.ts
    api-request.ts
  get-i18n/
    get-i18n.ts
    get-i18n.unit.ts
  index.ts               ← re-exports everything
```

```ts
// src/shared/helpers/index.ts
export { ApiError } from "./api/api-error/api-error";
export { apiRequest } from "./api/api-request";
export { getI18n } from "./get-i18n/get-i18n";
```

Helpers that are internal to a category (e.g. `get-api-resource.ts`, `get-api-url.ts`) are simply left out of the barrel.

**Don't create a category folder for a single occupant.** Add the category or group folder only once a second item appears; until then place the single file directly at the slice/shared root.

## Push down on shared access

When **two or more** slices need the same thing (component, helper, constant, type, icon, copy), **don't duplicate and don't sideways-import** — move it down to the lowest layer that fits:

- pure / no business logic → **shared**
- reused and carries business logic → **widget**
- still specific to one feature's sub-parts → keep in that feature but lift to its top level

Promote only what's actually shared; keep single-use code local.

## Verify

Run the project's dependency-boundary linter after changing imports or moving files.

---

## In this repo (Olevin web app)

- **Linter:** `.dependency-cruiser.mjs` (`pnpm depcruise`, part of `pnpm validate`); violations are errors.
- **Aliases:** `@shared/*`, `@widgets/*`, `@features/*` (configured in `tsconfig.json` + `vitest.config.ts` — keep both in sync). `src/app` has no alias; route files import the layers by alias.
- **What depcruise enforces:**
  - `src/shared/**` imports only `src/shared/**`.
  - `src/widgets/**` → `src/widgets/**` and `src/shared/<category>/index.ts`.
  - `src/features/**` → `src/features/**`, `src/widgets/**`, `src/shared/<category>/index.ts`.
  - `src/app/**` and `src/proxy.ts` → `src/app/**` and only the barrels `src/{features,widgets,shared}/<slice>/index.ts`.
  - Shipped code (`src/**` except `*.unit.ts`) must not import `devDependencies`.
- **What depcruise does _not_ catch:** a slice reaching into a sibling slice's internals on the same layer (`features → src/features/**`, `widgets → src/widgets/**` are allowed by path). Keep sibling access barrel-only by hand: `import LegalLinks from "@widgets/legal-links"` ✅ — `"@widgets/legal-links/LegalLinks"` ❌. Features don't import other features; share via a widget or shared instead.
- **Routes:** every page sits under `src/app/[lang]/` in a route group — `(app)` (signed-in, `AppShell`), `(public)` (`PublicShell`), or its own segment with a feature frame (`onboarding/[step]` with `FlowShell`). Non-localized handlers (`callback/route.ts`, `health/route.ts`) sit at `src/app/` root. Route component names: `<Name>Page`, `<Name>Layout`.
- **Existing slices:** features `history` (the history page, API tag `History`)/`home`/`onboarding`/`privacy`/`profile` (the profile page, API tag `Profile`)/`sign-in`/`snapshots` (the form of a new or a saved snapshot, API tag `Snapshots`)/`terms`/`trend`; widgets `app-shell`/`language-switcher`/`legal-links`/`money-field` (amount with its currency and the list of amounts in several currencies, with the supported currencies and API limits)/`public-shell`/`snapshot-fields` (the fields of an actual snapshot and of an estimate, their drafts and the request built from them, shared by `onboarding` and `snapshots`); shared categories `components`/`constants`/`helpers`/`i18n`/`interfaces`.
- **Global styles:** only `src/app/globals.css` (fonts, tokens, base styles). Slices don't ship CSS files: they style with utilities, and text comes from the typography components in `src/shared/components` (`Heading`, `Text`, `Label`, `FieldError`, `Link`).
- **Component folders:** a component with its own sub-components gets a folder named after it (PascalCase), holding the component file and its sub-components, one default export per file and no `index.ts`. In shared: `components/RadioGroup/{RadioGroup,Radio}.tsx`, `ComboBox/{ComboBox,ComboBoxItem}.tsx`, `ToggleButtonGroup/{ToggleButtonGroup,ToggleButton}.tsx`, each file re-exported by the category barrel. In a slice: `components/SettingsStep/{SettingsStep,CurrencyField,TimezoneField}.tsx`, imported by the parent with a relative path; a slice's root component with sub-components gets the folder at the slice root (`widgets/money-field/MoneyField/{MoneyField,AddonSelect}.tsx`). A component reused by several others (`Input` in every text field) stays a lone file at the category root.
- **File naming:** components `PascalCase.tsx`; logic, Server Actions, i18n, constants `kebab-case.ts`; tests `*.unit.ts` beside the source. Static files (fonts, images) go to `public/`; app icons (`icon.svg`, `apple-icon.png`, `favicon.ico`) to `src/app/`.

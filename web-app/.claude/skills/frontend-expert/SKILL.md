---
name: frontend-expert
description: Build and modify UI for the Olevin web app (Next.js 16 App Router + React 19 + Tailwind CSS 4 + React Aria, bilingual EN/UK) so everything stays consistent with the design tokens, server-first rendering, i18n, and accessibility conventions. Use this skill whenever: writing or editing a `.tsx` component, deciding between a Server and a Client Component, adding `"use client"` / `"use server"`, fetching API data or adding a Server Action, wrapping something in `<Suspense>`, adding or changing Tailwind classes, touching tokens in `src/app/globals.css`, adding or renaming an i18n key (both `en` and `uk` must be defined), designing a new page section or layout, making an element interactive, or after an accessibility complaint. Also use it when unsure whether to reuse a shared primitive or create a new component — the default is always reuse first. Examples: "add a snapshots list", "show the user's index on the home page", "make this button open a dialog", "translate this label", "fix the contrast on the nav link", "why does the build complain about uncached data outside Suspense?".
---

# Frontend expert

Produce UI that looks and behaves like the rest of the app — consistent, themeable, translated, accessible, and built from small reusable pieces.

## Rendering: server first, static by default

- **Server Components are the default.** Add `"use client"` only to the smallest leaf that needs state, effects, event handlers or browser APIs, and pass it plain props (translated strings, ids) from a server parent. Never mark a whole page or shell as a client component to make one button interactive.
- **Keep pages static.** Anything that depends on the request (session, cookies, uncached fetch, a dynamic param not listed in `generateStaticParams`, `usePathname` on a dynamic route) renders in its own async/client component **behind `<Suspense>`**, so the rest of the page is prerendered and only that part streams in.
- Give every Suspense boundary a **skeleton of the same size** as the loaded content, so the layout doesn't jump.
- Cross-cutting request checks (auth, locale redirect) belong in the proxy, not in pages — that's what keeps pages static.

## Data

- Fetch on the server through the shared API client with a schema that validates the response; tokens and secrets never reach the browser.
- When the browser must keep data fresh, **prefetch on the server and hydrate** the client cache; the client's query function is a Server Action that calls the API on the user's behalf. One shared query key per resource.
- Mutations are Server Actions (`"use server"` file in the slice), invoked from `<form action>` where possible so they work before hydration.
- Server-only config is read from env through a validated, lazily parsed helper — never inline `process.env` in components.

## Design system & tokens

- Use the project's semantic design tokens (color, spacing, typography, radius, shadow) instead of literal values. No raw hex/rgb/oklch in components, no magic numbers where a token/scale exists. If an arbitrary value repeats, make it a token.
- Define/adjust palette and themes in the central theme source, not inline. Keep markup theme-agnostic: components use semantic tokens only, so a future theme is a token change, not a markup change.
- If a utility-class framework enforces class ordering/formatting, run its linter/formatter after editing classes.

## Components & state

- Build UI as small, single-purpose components placed in the layer matching their purpose (see `architecture`): primitive/no-logic → shared; reusable-with-logic → widget; user-facing functionality → feature.
- Each component exposes a clear public entry (barrel) and keeps internals private.
- **Reuse existing primitives before creating new ones** — search the shared UI library first.
- Keep state as local as possible (server props → `useState` → URL); lift it only when genuinely shared.

## Internationalization (i18n)

- **No hard-coded user-facing strings.** Route every string through the translation layer with a stable key. This includes `aria-label`s, metadata, and alt text.
- **Key parity:** every supported language defines the **same set of keys**. Add/rename/remove a key in _every_ language at once.
- Don't rely on the lookup's fallback (other language → default → raw key) — a missing translation renders wrong text, not an error. Fill every key, and translate meaningfully (no placeholder source-language text).
- Placement by scope: component-local copy beside the component; cross-page/shared copy in the shared i18n area, exported from its barrel.

## Accessibility

- Semantic HTML, labels for controls, alt text, sufficient contrast, keyboard operability, visible focus, 44px touch targets. Use an accessible primitive library for interactive widgets instead of hand-rolled ARIA.

## Before done

Run lint, type-check, the dependency linter and the production build (the build is what catches request data used outside `<Suspense>`); for visual/a11y/i18n changes open the page in each locale and check it on phone, tablet and laptop widths.

---

## In this repo (Olevin web app)

- **Stack:** Next.js 16 App Router (`cacheComponents: true`, `output: "standalone"`, Docker), React 19, Tailwind CSS 4 (via `@tailwindcss/postcss`), `react-aria-components`, `@phosphor-icons/react`, TanStack Query 5, Zod 4, Logto auth. Bilingual **EN/UK**, light theme only.
- **Rendering patterns already in the code:**
  - `[lang]` is prerendered via `generateStaticParams = getLanguageStaticParams`; onboarding steps via `ONBOARDING_STEPS`.
  - Auth gate + locale redirect run in `src/proxy.ts` (`PUBLIC_PAGE_URLS` are open, everything else redirects to sign-in) — pages never check the session just to guard access.
  - Dynamic param not known at build time → the page renders `<Suspense>` and reads `params` inside a child (`snapshots/[month]/page.tsx`).
  - Session data → async server component behind `<Suspense fallback={<Skeleton />}>` (`MeInfo` + `MeInfoSkeleton`).
  - Client component that reads `usePathname` → wrapped in `<Suspense>` with a static fallback (`AppNavLink`).
- **Data flow** (see `features/home`): `fetch-x.ts` = `apiFetch(path, zodSchema, token)`; `x-query-key.ts`; server component gets `getApiTokenRSC()`, `getQueryClient().prefetchQuery(...)`, wraps the client part in `<HydrationBoundary state={dehydrate(queryClient)}>`; client part `useQuery({ queryKey, queryFn: getX })` where `get-x.ts` is a `"use server"` action using `getApiToken()` and redirecting to sign-in on missing token / `ApiError` 401. `getApiTokenRSC` is for Server Components, `getApiToken` for Server Actions and Route Handlers. No `NEXT_PUBLIC_*` env vars — env is server-only (`.env.example`).
- **Tokens:** `src/app/globals.css`, in four sections — fonts (`@font-face`, self-hosted Inter + Lora in `public/fonts/`), **primitives** in `:root` (`--ink-*`, `--violet-*`, … — no utilities, never used in components), **semantic tokens** in `@theme` (which resets `--color-*`, `--font-*`, `--radius-*`, `--shadow-*`, `--breakpoint-*` to `initial`, so only these exist), base styles.
  - Colors: `background`, `surface`/`surface-sunken`/`surface-hover`, `overlay`, `fg`/`fg-muted`/`fg-disabled`, `border`, `input`/`input-hover`/`input-invalid`/`input-disabled`, `ring`, `primary`/`primary-hover`/`primary-pressed`/`primary-soft`/`on-primary`, `accent` (decoration only), `success`/`warning`/`danger`/`info` (+ `-soft`, `-border`), `zone-{stress,unstable,calm}` (+ `-soft`, `-graphic`), `positive`/`negative`, `chart-*`, `meter`/`meter-track`, `skeleton`.
  - Type: `text-xs/sm/base/lg/xl/2xl/3xl/display`, `text-input` for form fields (16px, no iOS zoom), `font-heading` (Lora) for headings — `h1` is styled in base; `tracking-label`. Radius `sm…2xl`, shadows `shadow-card/raised/overlay`, `ease-standard`.
  - Layout: breakpoints **only** `md` (768, rail) and `lg` (1024, sidebar) — mobile-first; containers `max-w-content/flow/flow-wide/profile`; CSS vars `w-(--layout-rail)`, `w-(--layout-sidebar)`, `--tap-min`; safe areas via `env(safe-area-inset-bottom)`.
  - Global `:focus-visible` ring and `prefers-reduced-motion` are handled in base — don't redo them per component.
- **Primitives:** `src/shared/components/` — `Button` (React Aria, `variant="primary" | "secondary"`), `Card`, `Logo`. Interactive widgets come from `react-aria-components`, styled through render-prop state (`isHovered`, `isPressed`, `isFocusVisible`, `isSelected`; `composeRenderProps` to merge a `className`). Icons: `@phosphor-icons/react/dist/ssr` in Server Components, `@phosphor-icons/react` in Client Components; decorative icons need no label.
- **Formatting:** Biome (`pnpm lint` / `pnpm lint:fix`) — tabs, double quotes, organized imports. Tailwind class order is enforced by `nursery/useSortedClasses` (**error**, auto-fixed by `pnpm lint:fix`) in `className` attributes; class strings outside `className` (constants like `SIDE_ITEM_CLASSES`, values returned from React Aria render props) aren't checked — keep the same order there by hand. Prettier only formats `md`/`yml`.
- **i18n:** type `I18N = { en, uk }` (`src/shared/interfaces/i18n.ts`); keys `snake_case`. Language comes from the `[lang]` route param and is passed down as a `language: string` prop; access with `const t = getI18n(language, X_I18N)` then `t("key")` (`src/shared/helpers/get-i18n/get-i18n.ts`) — pure, so it works in Server and Client Components; or translate in the server parent and pass strings as props (`MeDetails`). Fallback: current → `DEFAULT_LANGUAGE` (`en`) → raw key. Component-local → sibling `*-i18n.ts` (`X_I18N` const); app-wide → `src/shared/i18n/` (`APP_I18N` feeds `generateMetadata` in `src/app/[lang]/layout.tsx`).
- **Links & new pages:** build hrefs with `getLanguageUrl(PAGE_URLS.X, language)` and `next/link`. A new page needs a `PAGE_URLS` entry, and a `PUBLIC_PAGE_URLS` entry if it must open without sign-in. Language switching = `preferred-language` cookie + full reload (`widgets/language-switcher`).
- **a11y patterns in use:** landmarks with `aria-label` (`nav`, `main`, `aside`), `aria-current="page"` styled via `aria-[current=page]:`, `aria-live="polite"` for async text, `aria-hidden` skeletons, `sr-only` labels, `role="progressbar"` with values, `lang` on language buttons, `min-h-11` tap targets. No automated a11y gate yet — check with keyboard and a screen reader for new interactive UI.
- **Finish with:** `pnpm lint && pnpm check-types && pnpm depcruise && pnpm build`, or `pnpm validate` for everything. Dev server: `pnpm dev` → `https://app.localhost` (via Caddy).

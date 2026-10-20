---
name: libraries
description: Pick the right dependency and use existing ones correctly in the Olevin Next.js web app (`web-app/`), which keeps a small, deliberate set of runtime deps (Next, React, Logto, conform, React Aria, Phosphor icons, Zod, Zustand). Use this skill before running `pnpm add`, when deciding "is there already something that does X?", or when choosing between tools already in the project. Trigger on any of these: "install X", "add a package for Y", "how do I handle forms / dialogs / icons / dates / validation / client state / data fetching / auth", "which test runner / linter / formatter should I use?", "can I use Z library here?". Also use it when an existing dep might already solve the problem — this skill has the full catalog. Examples: "add a date picker", "install framer-motion", "do we have an icon library?", "should I use axios?", "should this go in devDependencies?".
---

# Libraries — what to use, and when

Prefer the tools already in the project; add a dependency only when it clearly earns its place.

## Choosing

- **Reuse first.** Check existing dependencies and in-repo helpers before adding anything. The best dependency is often one you already have — or the platform itself (Web APIs, React, Next.js built-ins).
- **Justify a new dep** against: does the platform/stdlib already do this? is it small, maintained, and well-licensed? does it pull a large/transitive tree? does it work in Server Components, or does it force `"use client"`? A new dep is a long-term liability (bundle, security, maintenance).
- **Right tool for the job, not the trendiest.** Match the library to the actual need and the existing stack; don't introduce a competing solution to one already present.
- **Production vs dev.** Anything imported by shipped code goes in runtime dependencies; tooling/tests go in dev dependencies. Don't import dev-only packages into shipped code.
- **Pin/lockfile.** Versions are pinned exactly (no `^`/`~`); respect the lockfile.

## Using

- Import through the library's documented public entry; don't reach into internals.
- Keep usage centralized/wrapped when it's likely to change or is used widely, so swaps are cheap.
- Read the version's own docs (APIs change between majors) rather than relying on memory — Next.js 16, React 19, Tailwind 4, Zod 4 and Vitest 5 all differ from older majors.

---

## In this repo (Olevin web app)

**Runtime deps:**

- **Framework:** `next` (16, App Router, `cacheComponents`, `output: "standalone"`), `react` + `react-dom` (19). Use Next built-ins first: `next/link`, `next/navigation` (`redirect`, `notFound`, `usePathname`), `next/server` (proxy, route handlers), `Metadata`/`Viewport` APIs, Server Actions.
- **Auth:** `@logto/next` — `/server-actions` entry (`signIn`, `signOut`, `handleSignIn`, `getAccessToken`, `getAccessTokenRSC`) in server code; `/edge` entry (`LogtoClient`) in `src/proxy.ts`. Always wrapped by helpers in `src/shared/helpers/auth` and `api/` — use those, don't call Logto directly from slices except for sign-in/sign-out actions.
- **API data:** read and changed on the server only — Server Actions and Route Handlers call the Orval clients in `features/<tag>/api`, whose transport is `apiRequest` over native `fetch`. The browser never requests API data itself, so there is no client data library: don't add TanStack Query, SWR, axios or another fetch wrapper.
- **Forms:** `@conform-to/react` + `@conform-to/zod/v4` — `useForm` with `parseWithZod` on the client and the same schema in the Server Action, which returns `submission.reply()`; schemas come from the Orval `*.zod.ts` files.
- **Toasts:** React Aria's `UNSTABLE_ToastRegion`/`UNSTABLE_Toast`/`UNSTABLE_ToastQueue` behind the shared `ToastRegion`, `showErrorToast` and `useFormErrorToast` (the API is still marked unstable, so it stays inside `src/shared/components/Toast/`); the queue wraps updates in `document.startViewTransition` for the fade. Don't add sonner, react-hot-toast or another toast library.
- **Validation:** `zod` (4) — env vars, form input. Import as `import { z } from "zod"`.
- **Interactive UI and text:** `react-aria-components` — buttons, toggles, dialogs, menus, selects, date fields, forms, and the text primitives `Heading`, `Text`, `Label`, `FieldError`, `Link` — usable in any layer, but always through the styled wrapper in `src/shared/components` when one exists. Check the 1.x types for `@deprecated` before using a part (`Radio`, `Checkbox`, `Switch` are replaced by `*Field` + `*Button`). Use it before writing custom interactive components or adding another UI kit (no Radix, Headless UI, MUI, shadcn).
- **Icons:** `@phosphor-icons/react` — `/dist/ssr` in Server Components. Import the `Icon`-suffixed names (`HouseIcon`, `CaretLeftIcon`); the bare names (`House`) are deprecated. Don't add another icon set.
- **Client state:** `zustand` (5) — the draft of the onboarding settings (`features/onboarding/onboarding-store.ts`), kept with `persist` in `sessionStorage` so the language switch (a full reload) doesn't lose it; `skipHydration` plus `persist.rehydrate()` in an effect keeps the server and first client render equal. Reach for it only for client state shared by distant components that `useState`, props or the URL can't cover; never as a cache of server data.
- **Dates/numbers:** native `Intl` (`DateTimeFormat`, `NumberFormat`) — no date library so far; justify one before adding.
- **Animation:** Tailwind transitions + CSS; no motion library.

**Dev deps:**

- **Styling:** `tailwindcss` 4 + `@tailwindcss/postcss` (no `tailwind.config` — tokens live in `src/app/globals.css`). No `@tailwindcss/typography`: it would be a second source of the text styles; markdown, if it comes, maps its elements onto the shared typography components (`components` of `react-markdown`).
- **Lint/format:** `@biomejs/biome` (lint + format for TS/TSX/CSS/JSON) and `prettier` (only `md`/`yml`). Don't duplicate roles — no ESLint.
- **Types:** `typescript` 6 (`pnpm check-types`), `@types/*`.
- **Tests:** `vitest` 5 (unit). No e2e/a11y runner yet — see `testing-strategy`.
- **Architecture:** `dependency-cruiser` (layer rules + `not-to-dev-dep`).

**Tooling:** Node `24.18.0` (`.nvmrc`), pnpm 11 via corepack (`Dockerfile`).

Before adding a library: confirm none of the above already covers it; if it ships to users, weigh its bundle and whether it forces client rendering, and put it in the right `package.json` section with an exact version (depcruise enforces dev vs runtime).

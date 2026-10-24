---
name: specification
description: Read, write, and keep up-to-date lightweight per-feature specs (markdown files colocated inside each `src/features/<slice>/` directory) for the Olevin Next.js web app (`web-app/`). Always use this skill before touching any existing feature — check whether a spec exists and read it first to recover intent and constraints. Also use it whenever the user describes requirements, flows, acceptance criteria, or any detail that isn't already in the spec — fold those details into the spec as part of the same change. Trigger on: "add a feature for X", "how does the Y page work?", "I want the snapshot form to do Z", "what are the requirements for this?", "document this feature", or any multi-step feature discussion. Examples: "spec out the snapshot form", "add the questionnaire step to the onboarding spec", "what does the home spec say about loading the user?".
---

# Per-feature specs

Document each feature with a small spec file living **inside the feature's own slice**, next to its code — not in a separate spec-tooling directory. This is a self-driven, lightweight convention; no external spec-driven-development tooling is involved.

## Rules

- **Read before you touch.** Before making any change to a feature, read its spec file (if one exists) to recover intent and constraints instead of re-deriving them from the code.
- **Update, don't bypass.** If the user states a detail the spec is missing, or that contradicts it, fold that detail into the spec as part of the same change. A stale spec is worse than none.
- **Created on request, not retrofitted automatically.** One spec per feature, written when asked for — don't generate specs for every slice proactively.
- **No spec yet?** Proceed with the work as normal; don't block on it or invent one unprompted.

## Required structure

When applicable, every spec uses these six sections, **in this order**:

1. **Purpose** — what this feature is, for whom, and how it's used. Detailed enough that someone unfamiliar with the feature understands its role without reading the code.
2. **Data model** — a Mermaid **ERD** (`erDiagram`) of the entities/types this feature owns or operates on, with their relations and a short description per field. Only include if the feature has a real data shape of its own.
3. **Sequence diagram** — for each main flow: a one-line description of what it shows, **then** its Mermaid **sequence diagram** (`sequenceDiagram`) — description first, diagram after, never the reverse. Only include if the feature has a flow beyond "render static markup."
4. **Implementation nuances** — only what's genuinely non-obvious for _this_ feature: behavior a reader would get wrong by guessing, or a constraint not visible from the surrounding code. Don't restate architectural, theming, or i18n conventions that already apply project-wide. Don't invent a nuance just to fill the section.
5. **Validation rules** — field-level validation, allowed values, required/optional, access/role constraints. Only include if the feature actually accepts input and enforces something on it.
6. **Dependencies** — only **other features** this one relies on, and **third-party/external services** (a paid/external API, SaaS, CDN, env var tied to an external account, etc.). Never list shared/widget-layer modules, internal helpers/constants/types, or framework lifecycle hooks — those are ordinary architecture, not a dependency worth calling out. Only include if there's real cross-feature reliance or an external service involved.

**Omit a section entirely when it doesn't apply** — no heading, no "skipped" placeholder, nothing. Purpose is the only section that's always present; the rest appear only when they have real content. Never pad a section with boilerplate just to have content in it.

Keep the whole spec concise — enough to orient quickly, not a novel.

---

## In this repo (Olevin web app)

- Path: `src/features/<feature>/<FEATURE-NAME>.md`, where `<FEATURE-NAME>` is the slice name upper-cased (e.g. `src/features/home/HOME.md`, `src/features/snapshots/SNAPSHOTS.md`).
- Don't restate FSD layering, the server-first/Suspense rendering rules, i18n, or tokens already covered by `architecture` and `frontend-expert` — only this feature's nuances.
- Static slices (`privacy`, `terms`) usually need only Purpose. Signed-in features talk to the Olevin API and are where sections 2–6 earn their place: e.g. `home` (sign-out Server Action), `sign-in` (Logto redirect and `/callback`), `onboarding` (steps, which ones are optional), `snapshots` (create vs edit by `month`, input validation).
- Sequence diagrams: show the participants that actually exist — Browser, Proxy (`src/proxy.ts`), Server Component / Server Action, Logto, Olevin API — so it's clear what runs where and where the token is used.
- **Dependencies:** the Olevin API (`server/`, via `API_URL`) and Logto are external services — name the endpoints/scopes the feature uses (e.g. `GET /me`). The rendering mode (static, or which part streams per request) is a nuance worth stating when it isn't obvious from the route.

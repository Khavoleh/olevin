---
name: specification
description: Read, write, and keep up-to-date lightweight per-feature specs (markdown files colocated inside each `Olevin.Api/Features/<Feature>/` directory) for the Olevin .NET API (`server/`). Always use this skill before touching any existing feature — check whether a spec exists and read it first to recover intent and constraints. Also use it whenever the user describes requirements, flows, business rules, validation, acceptance criteria, or any detail that isn't already in the spec — fold those details into the spec as part of the same change. Trigger on: "add a feature for X", "how does the Y endpoint work?", "I want snapshots to do Z", "what are the rules for this?", "document this feature", or any multi-step feature discussion. Examples: "spec out the Settings feature", "add the save-snapshot flow to the Snapshots spec", "what does the Advice spec say about expense cuts?".
---

# Per-feature specs

Document each feature with a small spec file living **inside the feature's own folder**, next to its code — not in a separate spec-tooling directory. This is a self-driven, lightweight convention; no external spec-driven-development tooling is involved.

## Rules

- **Read before you touch.** Before making any change to a feature, read its spec file (if one exists) to recover intent and constraints instead of re-deriving them from the code.
- **Update, don't bypass.** If the user states a detail the spec is missing, or that contradicts it, fold that detail into the spec as part of the same change. A stale spec is worse than none.
- **Created on request, not retrofitted automatically.** One spec per feature, written when asked for — don't generate specs for every feature proactively.
- **No spec yet?** Proceed with the work as normal; don't block on it or invent one unprompted.

## Required structure

When applicable, every spec uses these six sections, **in this order**:

1. **Purpose** — what this feature is, for whom, and how it's used. Detailed enough that someone unfamiliar with the feature understands its role without reading the code.
2. **Data model** — a Mermaid **ERD** (`erDiagram`) of the entities/types this feature owns or operates on, with their relations and a short description per field. Only include if the feature has a real data shape of its own.
3. **Sequence diagram** — for each main flow: a one-line description of what it shows, **then** its Mermaid **sequence diagram** (`sequenceDiagram`) — description first, diagram after, never the reverse. Only include if the feature has a flow beyond a single pure calculation.
4. **Implementation nuances** — only what's genuinely non-obvious for _this_ feature: behavior a reader would get wrong by guessing, or a constraint not visible from the surrounding code. Don't restate architectural, data or code-style conventions that already apply project-wide. Don't invent a nuance just to fill the section.
5. **Validation rules** — field-level validation, allowed values, required/optional, access/ownership constraints. Only include if the feature actually accepts input and enforces something on it.
6. **Dependencies** — only **other features** this one relies on, and **third-party/external services** (a paid/external API, SaaS, env var tied to an external account, etc.). Never list Shared/Infrastructure code, internal helpers/constants/types, or framework pieces — those are ordinary architecture, not a dependency worth calling out. Only include if there's real cross-feature reliance or an external service involved.

**Omit a section entirely when it doesn't apply** — no heading, no "skipped" placeholder, nothing. Purpose is the only section that's always present; the rest appear only when they have real content. Never pad a section with boilerplate just to have content in it.

Keep the whole spec concise — enough to orient quickly, not a novel.

---

## In this repo (Olevin API)

- Path: `Olevin.Api/Features/<Feature>/<FEATURE-NAME>.md`, the feature name upper-cased (e.g. `Features/Settings/SETTINGS.md`, `Features/FinancialWellness/FINANCIAL-WELLNESS.md`).
- Don't restate the vertical-slice layout, the `Response<T>` envelope, Atlas/EF mapping rules or code style already covered by `architecture` and `backend-expert` — only this feature's nuances.
- **Purpose** lists the feature's endpoints (method + route) and who calls them (the web app during onboarding, on the snapshot form, …).
- **Data model:** build the ERD from the feature's Atlas schema (`infrastructure/db/schemas/<schema>.hcl`), not from the C# entities — use the table/column names and DB types. A stateless endpoint, if one is added, describes its request/response records instead, only if the shape isn't obvious.
- **Sequence diagrams:** real participants — Web app (Next.js server), API endpoint, validator/Wolverine pipeline, DbContext/PostgreSQL, Logto — so it's clear where the token is checked and where the transaction commits.
- **Validation rules:** the validator rules and the DB `check`s together, plus the ownership rule (the user comes from the token's `sub`).
- **Index model:** the calculation lives in `Features/FinancialWellness/Helpers/Index/` and is shared by several use cases; if a spec of the method is requested, put it in `Olevin.Api/Features/FinancialWellness/Helpers/Index/INDEX.md` with the same structure, and reference it from feature specs instead of repeating formulas.
- **Dependencies:** Logto (token issuer) only when the feature reads the user; other features through the `Contracts/` of their slices.

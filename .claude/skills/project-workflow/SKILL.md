---
name: project-workflow
description: Handle all GitHub operations for the Olevin monorepo (`Khavoleh/olevin`: `web-app/`, `server/`, `infrastructure/`) — branches, commits, PRs, and issues — using the GitHub CLI (`gh`), following the `[id]/task-name` branch convention, the repo's commit message style, PR base = `main`, and a required approval gate before filing any issue. Use this skill when: creating a branch, writing a commit message, opening or updating a PR, drafting or filing an issue, assigning labels, or wiring `Closes #N` in a PR body. Also trigger when wondering how to phrase a commit or what to verify before a PR. Examples: "create a branch for this task", "open a PR for my changes", "file a GitHub issue for the snapshot form", "write a commit message for this".
---

# Project workflow

Work with GitHub in line with the repo's conventions and automation. Don't fight the hooks or CI.

## Safety

- **Don't commit, push, open PRs, or create issues unless the user explicitly asks.**
- Never commit directly to the default/protected branch — branch first.
- Never introduce secrets: `.env` files stay untracked (only `.env.example` with placeholder values is committed), and `*.pem` dev certs are git-ignored.

## Branches

- Follow the branch-naming pattern exactly.
- Branch off the correct base for the workflow.

## Commits

- Use **Conventional Commits** (`feat:`, `fix:`, `chore:`, `docs:`, `refactor:`, `test:`, `ci:`, `style:`) when releases are automated from commit history — the type drives the version bump and changelog. Otherwise follow the style already in the history. Keep messages accurate; they become release notes.
- One logical change per commit; in a monorepo, don't mix unrelated `web-app/` and `server/` changes in one commit.

## Pull requests

- Target the correct base branch (`main` in this repo). Let CI run; fix failures rather than bypassing.
- **Title:** same style as commits, concise summary, traceable to the branch id.

### PR body template — always use this

```markdown
## Purpose

<One or two sentences: why this PR exists — the problem it solves or the value it adds.>

## Changes

- <Done work item 1 — what was actually built/changed, present tense>
- <Done work item 2>
- <…>

## Testing

- <How it was verified — commands run and their result, e.g. `pnpm validate` passing, manual checks in both locales. Omit section if genuinely nothing to report.>

## Related

<`Closes #N` for issues this PR resolves; links to related PRs/issues. Omit section if none.>
```

Guidelines:

- **Purpose** and **Changes** are mandatory; **Testing** and **Related** are included whenever there's anything to report (usually there is).
- Keep **Changes** a scannable bullet list of finished work, not a narrative — one bullet per meaningful unit of change.
- Reference and close issues from the body via `Closes #N` so the merge auto-closes them.

## Issues / tasks (with labels & tags)

### Approval gate — always required

Before creating any issue, draft the full issue content (title + body) and **present it to the user for approval**. Do not run `gh issue create` until the user explicitly confirms. If the user requests changes, revise and show the draft again before proceeding.

### Issue structure

Every issue body must follow this template:

```markdown
## Overview

<One concise paragraph: what this issue is about and why it matters.>

## Purpose

<Why this work needs to happen — the problem it solves or the value it adds.>

## Acceptance Criteria

- [ ] <Criterion 1 — specific, verifiable, written in present tense>
- [ ] <Criterion 2>
- [ ] <…>

## Notes

<Optional: constraints, related issues/PRs (`#N`), design links, open questions. Omit section if empty.>
```

Guidelines:

- Title: action-oriented, matches the branch id prefix (`[id]/task-name`) so task ↔ branch ↔ PR is traceable.
- Acceptance criteria: each item is independently verifiable and unambiguous. Aim for 3–7 items.
- **Always apply labels.** Fetch the current label set (`gh label list`) before assigning — don't invent labels. Assign at minimum a _type_ label and an _area_ label when one matches the touched project/slice.
- Reference related issues/PRs and close them from the PR body via `Closes #N`.

## Tooling

- Use the **GitHub CLI (`gh`)** for all GitHub operations (issues, PRs, labels); local branches and commits go through `git`. Don't use GitHub MCP tools or raw REST calls with tokens.
- Before the first call, check `gh auth status`. If `gh` is missing or not signed in, say so and ask the user to install it / run `gh auth login` — don't try to sign in yourself.
- Pass multi-line bodies through a file or heredoc (`--body-file`), never by squeezing Markdown into one quoted line, so the template's formatting survives.
- Common commands:
  - Labels: `gh label list`
  - Issue: `gh issue create --title "…" --body-file <file> --label "<type>" --label "<area>"` · `gh issue view <N>` · `gh issue list`
  - PR: `git push -u origin <branch>`, then `gh pr create --base main --title "…" --body-file <file>` · `gh pr view` · `gh pr edit <N> --body-file <file>` · `gh pr checks`

---

## In this repo (Olevin)

- Repo: `Khavoleh/olevin` — a monorepo: `web-app/` (Next.js), `server/` (.NET API), `infrastructure/` (DB schema, deployment), `landing/` (Astro, planned).
- **Base branch = `main`** (the only long-lived branch). No CI workflows, git hooks or release automation yet — nothing checks the branch name, tests or secrets for you.
- **Branch name:** `[id]/[task-name]`, lowercase + hyphens, `id` = GitHub issue number, e.g. `12/snapshot-form-validation`.
- **Commits:** no semantic-release yet, so no Conventional Commit prefixes. Follow the existing history: imperative, sentence case, no trailing period, a short summary of the change — e.g. `Add user creation endpoint`, `Rework database schema to separate tables`, `Rename devops folder to infrastructure`. Once semantic-release is added, switch to Conventional Commits and update this skill.
- **Before a PR**, run the checks of every touched project and report them under **Testing**:
  - `web-app/`: `pnpm validate`; for UI changes add the manual checks done (locales, widths, keyboard).
  - `server/`: `dotnet build` (fails on CSharpier formatting) and `dotnet test`.
- **Labels:** fetch the label list with `gh label list` before assigning; add an area label for the touched project (web-app / server / infrastructure) and slice when one exists.

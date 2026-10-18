---
name: comments
description: Keep code comments in the Olevin web app (`web-app/`) short and plain. Use this skill whenever writing or editing a JSDoc comment (`/** … */`) or an inline `//` comment, adding a new component, helper, constant, route, Server Action or i18n file, or reviewing comments after a change. Trigger on: "add a doc comment", "document this function", "this comment is too long", "clean up comments". Examples: documenting a new helper, writing the comment of a route handler, describing a component prop.
---

# Comments

A comment names what the code is. The code itself shows how it works.

## Rules

- **One short sentence.** Say what the thing is or does, nothing more. Add a second sentence only for something a caller must know and can't see in the signature (e.g. "Returns `undefined` without a session.").
- **No details.** Leave out the why, the flow, the callers, the other files, the edge cases and the history. Explain those in the chat or the commit message.
- **No inline `//` comments.** Make the code clear instead.
- **Plain words.** Write it the way you'd tell a teammate. No marketing, no jargon chains, no lists of everything the code touches.
- **Don't repeat the name.** `getOnboardingStep` doesn't need "Gets the onboarding step"; say what it returns.
- **Every exported item gets a JSDoc**, even a short one. Props and private helpers get one only when the name alone isn't clear.

## Examples

```ts
// ❌
/**
 * Loads the onboarding status of the signed-in user and, once the onboarding is completed, sets the cookie that lets
 * the proxy into the app. Works in Route Handlers and Server Actions. Returns `undefined` when the user has no valid session.
 */

// ✅
/** Gets the onboarding status and remembers a completed onboarding in a cookie. Returns `undefined` without a session. */
```

```ts
// ❌
/** Frame of the onboarding: progress bar and the back link within the stage on top, one step below, no app navigation. */

// ✅
/** Frame of the onboarding: progress and the back link. */
```

```ts
// ✅
/** Cookie that marks a user who has completed the onboarding. */
/** Steps the user may open for each onboarding status. */
```

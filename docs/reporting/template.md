# Reporting convention and feature template

The outcome coordinator maintains the roadmap, index and affected feature pages
when an outcome completes or hits a blocker. Include the source identity, evidence
and next acceptance gate; update the Beads task and STATUS separately under the
working agreement. A fresh session should reconcile these summaries with Beads
before acting. Do not copy a task backlog or worker narrative into proof of success.

Use these status terms consistently:

| Status | Meaning |
| --- | --- |
| Proposed | Scope awaits acceptance; no delivery claim |
| Implemented | Code or content exists at a named source; integration and acceptance may remain |
| Verified | Task-specific acceptance evidence passes for the named integrated source, including manual gates when required |
| Released | A verified playable checkpoint has a GitHub Release with source/build identity and attached or linked deliverables |

Use a separate integration note or blocker alongside the status. A passing
historical build does not verify later changes. Distinguish compilation, runtime
assertions, rendered inspection and human feel tests. Link public CI evidence
directly. Label ignored local artifact paths as local-only, and do not use relative
links that suggest GitHub hosts those files. Record failed and ambiguous attempts.

GitHub Releases are reserved for verified playable checkpoints, not every feature
or test increment. Feature pages can accumulate changes between those checkpoints;
link the release once it exists. Do not fabricate a tag or downloadable build.

Copy this structure to `features/<feature-name>.md` and add it to the index:

```markdown
# Feature name

Status: Proposed / Implemented / Verified / Released
Last reviewed: YYYY-MM-DD UTC
Task: Beads ID and mapped public issue when available
Source: commit/build identity and integration state

## Result and scope

Observable behavior, intended user benefit and material exclusions.
Link the accepted design or ADR.

## Evidence

Checks, source identity, results and durable links. Separate historical evidence
from current evidence. Label local-only artifact paths and missing verification.

## Remaining acceptance

Specific blockers and gates. Link Beads/public issue for execution detail.
Include a GitHub Release link only when a verified playable checkpoint exists.
```

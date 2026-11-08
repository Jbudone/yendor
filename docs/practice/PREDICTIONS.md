# Predictions

One line before each change, filled in before the agent starts. The point is not
the number — it is the gap between the number and what happened. A developer who
cannot predict whether a change will be small or large cannot scope work, and
scoping is the whole job.

Write it, do it, then fill the last two columns honestly. Do not edit the
prediction after seeing the result; that destroys the only signal it has.

| # | date | change | files | risk | passes first try | actual files | actual result |
|---|------|--------|-------|------|------------------|--------------|---------------|
| 1 |      |        |       |      |                  |              |               |

## Columns

- **files** — how many files you expect this to touch. Agents under-predict this.
- **risk** — `low` (mechanical, one file), `med` (crosses a boundary, touches a
  scene or prefab), `high` (changes an invariant others depend on).
- **passes first try** — yes / no. Your guess, before you run it.
- **actual result** — `accepted` · `accepted after correction` · `rejected` ·
  `reverted`. Anything but the first is a miss worth reading.

## Reading it back

Every ten rows, compute:

- **hit rate** on `files` — within ±1 counts as a hit.
- **optimism** — share where `risk` was under-called, i.e. actual files or
  corrections were worse than predicted.
- **first-try accuracy** — did you know whether it would work?

A low hit rate with a high delivery rate means you are shipping but not
understanding — you cannot plan, you can only try. That is the plateau this log
exists to catch.

## Why not a model

A model could score this for you. Don't let it. The value is in the discomfort
of writing down what you expect and being wrong, and that discomfort is exactly
what gets optimised away when something else does the predicting.

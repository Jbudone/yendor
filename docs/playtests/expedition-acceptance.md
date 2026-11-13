# Current expedition acceptance

Tasks: `yen-kk5` (human acceptance), `yen-g8n` (rendered baseline).
Status: prepared; human sessions and measured hardware timing have not run.

Launch `./artifacts/playtest-latest/play.sh` from the repository root on a desktop.
Build source: `e44dca4e4b1cb7b1221c31a9555cabf347b7b83f`.
Build GUID: `f7476776753c49c09ed7acb96820a3e6`.
Bundle SHA-256: `084446674704dbe03f539337fe0da72c1595b245d69eeed7145021200da16a34`.
Rechecked bundle evidence: `artifacts/pilots/roadmap-start/bundle-verification.json`.

## Guided gameplay session

Follow [the gameplay packet](gameplay-batch.md). Record machine, seed, selected
core, elapsed run time, outcome and F8 report path. Capture at the shrine,
overlook and Starheart return. Record observations separately from preferences.

| Check | Result | Evidence / next change |
| --- | --- | --- |
| Movement, aim, double jump and dash feel dependable | Unrun | |
| Camera obstruction and cursor capture behave correctly | Unrun | |
| Attack warnings are readable; cover and dodging work | Unrun | |
| Two-minute combat remains enjoyable without collecting loot | Unrun | |
| Same-seed runs with two cores produce different tactics | Unrun | |
| Overlook reward changes a decision; destination stays legible | Unrun | |
| Starheart alarm changes the return; extraction remains available | Unrun | |
| Death/extraction, R replay and N next seed behave correctly | Unrun | |

Record the single most frustrating failure and the decision each core changed.
Use observations to choose a bounded tuning task; avoid adjusting several
subsystems before testing the first hypothesis.

## Fresh-player diagnostic

Run [the existing diagnostic](first-expedition.md) without the guided packet or
verbal coaching. Three independent fresh players, one run each. Preserve its
predeclared provisional gate: two demonstrate objective recognition and deliberate
wet + Storm use. Separately record whether the run lasts 5–10 minutes and the
player reaches a meaningful escape or death. A small sample guides iteration.

| Participant alias | Build / seed / machine | Objective recognition time / stalls | Deliberate interaction and post-run explanation | Duration / outcome | Evidence |
| --- | --- | --- | --- | --- | --- |
| A — unrun | | | | | |
| B — unrun | | | | | |
| C — unrun | | | | | |

Conclusion: unrun. Do not recruit or message participants automatically.

## Rendered performance and art baseline

On a named physical machine record CPU, GPU/driver, RAM, OS, display refresh,
graphics API, resolution, quality settings and power mode. Use the exact build
above at 1920×1080; keep settings and seed constant between before/after captures.
Record the chosen core and avoid mixing different encounter states in comparisons.

Warm up one run. Measure three runs along shrine → main road → overlook → ruin
→ Starheart → return → extraction. Use an available frame-time capture tool;
record its version, command/configuration and raw output. Report median, p95 and
p99 frame time, worst spikes and route segment. A single FPS screenshot is
insufficient. Assess 60 fps against a 16.67 ms frame budget; record excursions
rather than declaring stability from an average. Compare supported 30/60/120 fps
feel separately; record unsupported rates as unavailable.

Capture identical shrine, overlook and return viewpoints for art comparison;
review enemy/hazard silhouettes, turquoise/gold navigation and material contrast.
Keep software-rendered screenshots separate from physical GPU measurements.

Result: unrun; no desktop display is available to the coordinator shell.
Raw evidence belongs in `artifacts/pilots/yen-g8n/`, with source/build identity.

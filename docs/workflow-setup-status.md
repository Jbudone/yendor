# Workflow setup — 2026-11-08

The user authorized implementation after the research loop, then selected a
public Jbudone/yendor repository, Gitleaks, OpenRouter through LiteLLM and Langfuse
Cloud. Accepted direction is ADR 0002. Deferred/rejected research is not silently
adopted. No paid model or asset requests have been made during this setup.

## Installed and wired locally

- AGENTS.md, STATUS.md and CLAUDE.md provide a shared fresh-session entry point.
- Beads 1.3.4 is the task authority; twelve source-linked follow-ups are seeded
  idempotently and published as GitHub needs-triage issues. Mapping is versioned.
- Native Agent Mail 0.3.40 runs on loopback port 18765. Its archive signature and
  checksum were verified. Codex MCP is registered; Claude project configuration
  is generated locally. The proxy limits tool exposure and keeps the token out
  of client configuration. Project-local startup works across command sessions.
- Mastra packages and Node dependencies are pinned/locked. Local LibSQL persists
  task-scoped history. Curated source-linked memory has branch/status/staleness
  filtering; vector/observational model calls are deliberately disabled pending
  evaluation. Role/profile routing and a single-worker comparison route exist.
- LiteLLM 1.108.3 is installed in .tools/litellm with a frozen dependency list.
  The config uses OpenRouter upstreams. Candidate model IDs were checked against
  the current public catalog, not benchmarked or declared winners.
- Native Langfuse exporter is configured and opt-in; flush/shutdown is explicit.
- Source-aware atomic writes reject stale hashes and record intent before change.
  Cross-process locks serialize cooperating writers and Unity commands. Expired
  locks are not automatically stolen. Worktree helper isolates implementations.
- Unity validators no longer repair a missing scene. Unique manifests/logs and
  copied-project hashes bind evidence to source; builds preserve the tested
  player bundle and digest. Unity child environment excludes provider keys.
- Health command-sequence pilot covers synchronous health/death/wet-hit rules;
  three faulty adapters validate the independent checker and reducer.
- F8 exports runtime diagnostic JSON, build GUID, seed/state and recent events;
  screenshots are requested only with a graphics device. Saved-prefab audit checks
  missing scripts/materials. Playtest and experiment protocols are provided.
- Gitleaks 8.30.1 is pinned. Pre-commit scans staged changes; pre-push scans all
  history. Hooks also run fast checks, optional Unity checks and LFS pre-push.
  FBX/Blend/PSD/WAV/EXR use Git LFS filters.
- GitHub workflows cover Node checks/full-history Gitleaks, gated licensed Unity
  verification and opt-in advisory PR-Agent review. Issue and PR templates are ready.
- Beads native local backup is configured and synced to .workflow/beads-backup.
  This is not off-machine disaster recovery or a completed restore drill.

## Verified evidence

- Node checks: scope restrictions, stale writes, intent/audit failures, ownership,
  stale/branch memory filtering, result validation, routing, Meshy read-only calls
  and ambiguous-submission handling. Gitleaks negative control generates a
  synthetic credential and confirms rejection/redaction.
- Agent Mail pilot passed: two independent clients, delivery, acknowledgment,
  advisory reservation conflict and release. Evidence: artifacts/pilots/mail.json.
- Studio mock pilot passed: model request, tool call/result and task history
  after reopening storage. Evidence: artifacts/pilots/studio.json. This was a
  local synthetic model endpoint, not provider or Langfuse validation.
- Unity Test Framework 1.8.0 is bundled. Six EditMode tests passed: three seeds
  with 500 generated histories each, plus three faulty-adapter controls. Evidence
  run 2026-11-09T01-23-18-564Z-a5122480 (UTC), under artifacts/runs/.
- The first EditMode attempt failed because SendMessage dispatched lifecycle in
  an invalid fixture context. The fixture now invokes initialization directly;
  production lifecycle remains covered separately by runtime smoke.
- The existing stock PackageManagerSettings asset is preserved unchanged for
  reproducible fresh imports; it has no scoped registries or non-Unity URLs.
- Existing Git history passed Gitleaks before publication. Hooks and CI rerun it
  for later commits. Workflow/issue-form YAML parsed successfully.

- Final Editor runtime smoke passed, including diagnostic JSON export: run
  2026-11-09T05-50-10-478Z-6e1411cf. Validation and saved-prefab audit also passed
  in the earlier verification sequence.
- Linux build passed: run 2026-11-09T05-50-28-685Z-25a2dee1, build GUID
  de505ffced3d4f35926eaafb3facbc52, bundle SHA-256
  e0b8166eb0c35cdca470af8c10bb9ecb3a21f06abc280dd541556dd910cadbd2.
- Packaged-player verification initially caught shader stripping: dynamic-only
  Standard lookup returned null. An explicit Editor-authored Resources material
  now retains the shader; world materials clone that template. Validation checks
  for the authored input rather than generating it during a check.

- The fixed packaged Linux player passed headless smoke, including actual JSON
  export, with the bundle digest unchanged before/after. Evidence:
  artifacts/runs/2026-11-09T05-50-28-685Z-25a2dee1/player-smoke.json.
- A dedicated runner, yendor-unity-linux, is registered and reported online with
  label yendor-unity. Unity CI is restricted to owner-controlled main/master
  pushes and manual runs; PR checks use GitHub-hosted runners.

- Hosted Node/Gitleaks checks passed for setup commit 4dc6dac:
  https://github.com/Jbudone/yendor/actions/runs/42576852702.
- Unity CI passed every verification/build/player-smoke step and uploaded its
  artifact, then stalled in setup-node's post-job cache of the developer's 7.8 GB
  npm cache. That run was cancelled; the workflow now scopes NPM_CONFIG_CACHE to
  runner.temp instead. Its replacement run must be observed separately.
- Master branch protection requires the actual `node` check and PR-based changes;
  force pushes/deletions are disabled. External-contributor workflows require
  approval, and the local Unity workflow has no PR trigger.
- Beads history was pushed to the public repository's Dolt data ref. Native
  bootstrap restores that task graph on a fresh clone; initial seeding remains
  idempotent rather than a second authority.

## Live/account/environment gates

The ignored .env file has been prepared with local gateway keys and candidate
upstream models. It still needs OPENROUTER_API_KEY, MESHY_API_KEY and the actual
Langfuse Cloud project's public/secret keys and region. Enable Langfuse only
when these are present. No live provider compatibility, paid model comparison,
Meshy account/generation or real Langfuse trace audit is claimed.

The primary Unity project reports an existing lock, while CLI instance discovery
cannot identify a usable Editor. Verification uses a separate source copy and
Library instead of closing an Editor or deleting its lock. Native CLI bridge
reconnection and Unity AI activation remain unverified. The executor currently
has no graphics environment, so screenshot quality and manual movement/feel are
pending. Unity Cloud selection/terms activation needs the intended account.

The dedicated licensed runner has been registered; repository variables enable
the owner-controlled-branch Unity workflow. AI review remains disabled
until its credentials and calibration are deliberately configured. GitHub Node
CI is independently runnable. Source publication and actual CI run results are
recorded once observed, not implied by workflow files.

## Research adoption map

| Candidates | Current disposition |
| --- | --- |
| P01-01 | Synchronous generated-history pilot implemented; elapsed status and source-mutation expansion remain issue #8 |
| P01-02 / P02-04 | Run/artifact lineage and fail-closed result checks implemented; rendered artifact review remains manual |
| P01-03 | Import consistency/cold-import procedure still to exercise as art arrives; issue #11 |
| P01-04 / P01-05 | Diagnostic export and playtest protocol provided; actual input/timing/fresh-player trials remain issues #4/#5 |
| P01-06 | Tuning and obstruction-fixture round-trip remains issue #7 |
| P01-07 | Non-repairing validation implemented |
| P01-08 | CAS and cooperating-process locks implemented/tested; broader stale-worker recovery remains issue #6 |
| P01-09 / P02-07 | Unity environment narrowed; trusted-local boundary retained; external-writer isolation conditional in issue #12 |
| P01-10 | Upgrade/Windows lane documented; actual Windows support not claimed |
| P01-11 | Baseline duplication; no redundant framework installed |
| P01-12 | Unsupported exact-replay guarantee rejected |
| P01-13 / P01-14 | Framework-first testing and wholesale simulation restructuring remain deferred |
| P02-01 / P02-02 | Single-worker route and identity reporting implemented; real comparison remains issue #3 |
| P02-03 | Pre-side-effect intents, visible ambiguity, recovery inspection and fake-transport test implemented |
| P02-05 / P02-06 | Memory filters/controls implemented; broader memory/reviewer calibration remains issue #9 |
| P02-08 | Retrieved content treated as evidence; scopes enforced; expanded adversarial pilot awaits live worker |
| P02-09 | Local native backup verified; off-machine destination/restore drill remains issue #6 |

All original suggestions remain in the research ledger and detailed reports.

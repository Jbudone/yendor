# Workflow research pass 01 — Unity production and verification

Date: 2026-11-08. Role: senior Unity technical director and developer-platform researcher. Coordinator-configured model: `gpt-6-astra`, high reasoning, fresh context; underlying serving deployment was not independently exposed to the researcher. Status: recommendations for coordinator review, not adopted architecture. Research only; no implementation, installation, additional model-evaluation calls, Editor launches, paid generation, or external communication was performed.

## Finding and saturation judgment

**One genuinely new, high-impact workflow concept survives this pass: P01-01, generated gameplay action sequences with a small independent rules oracle and automatic reduction of failures.** It addresses combinations and ordering that a fixed smoke script or a corpus of world seeds does not systematically explore. Pilot it narrowly on health/status operations; do not undertake a deterministic-engine rewrite.

The remaining suggestions mostly make the baseline operationally precise. Several are worth doing sooner than new tooling, particularly preserving evidence for the actual build and making validation fail instead of silently repairing missing inputs. Their usefulness does not make them new research categories. Credential isolation for executable agent changes is a potentially distinct production control, but this pass defers its adoption decision to a researcher focused on the intended trust boundary and deployment environment.

Practical saturation has **not** been reached because P01-01 remains a candidate new addition. Next useful role: adversarial agent-operations/reliability architect, concentrating on cancellation, executable-code trust boundaries, cross-process recovery and evidence integrity. That researcher should challenge P01-01's novelty and cost rather than assume acceptance.

## Inspected project evidence

Source revision at inspection: `99084975396dbfd2355172de54ae3a179401ec92`. Research documents and `game/ProjectSettings/PackageManagerSettings.asset` were already untracked; they were not altered by this pass. No applicable `AGENTS.md` was found in the project or its ancestor path.

- Read the [research ledger](../workflow-research-ledger.md), [initial research](../agent-workflow-research.md), [README](../../README.md), [CONTEXT](../../CONTEXT.md), [first-expedition design](../design/first-expedition.md), [accepted ADR](../adr/0001-development-agents.md), and [foundation QA](../qa/foundation.md).
- [RuntimeSmoke.cs](../../game/Assets/Yendor/Runtime/World/RuntimeSmoke.cs) waits two frames and then executes one synchronous sequence. It selects an enemy, calls damage directly, sets wetness directly, and teleports the player before interacting with objectives. This is useful coverage but does not exercise actual input, traversal, or elapsed burn/wet timing.
- [Health.cs](../../game/Assets/Yendor/Runtime/Combat/Health.cs) uses `Time.time` and `Time.deltaTime`; wetness lasts 0.3 seconds after renewal, burning lasts three seconds, wet Storm doubles damage, dead bodies reject damage/healing. [Surface.cs](../../game/Assets/Yendor/Runtime/Systems/Surface.cs) renews wetness and applies oil damage in `Update`. Overlap/order/expiry behavior is an immediate domain for sequential testing, not a hypothetical future system.
- [PlayerMotor.cs](../../game/Assets/Yendor/Runtime/Combat/PlayerMotor.cs) reads legacy `Input` directly in `Update`; movement, dash duration, cooldowns and camera behavior are frame-dependent. Project settings use `activeInputHandler: 0`. The newer Input System is not present in the inspected manifest/lockfile.
- [Expedition.cs](../../game/Assets/Yendor/Runtime/World/Expedition.cs) uses one seeded `System.Random` for decoration and subsequent enemy placement. Geometry/materials are generated at runtime. `nextSeed` is static; restarts reload the scene. Current seed repeatability means initial world inputs, not a deterministic replay of an entire expedition.
- [ProjectSetup.cs](../../game/Assets/Yendor/Editor/ProjectSetup.cs) recreates the scene when it is missing during `Validate()`, and `Build()` calls that validator. Its build target is a Linux development player at a fixed path. No runtime test assembly exists under `Assets/Yendor`.
- [tools/unity.mjs](../../tools/unity.mjs) overwrites fixed validation/build log paths, checks exit code and a success marker, and spawns Unity without an explicit environment allowlist. [studio/run.mjs](../../tools/studio/run.mjs) limits validation with a process-local boolean. [workspace.mjs](../../tools/studio/workspace.mjs) writes complete files without an expected prior-content hash. These are concrete limits, not evidence that a failure or secret leak has occurred.
- [roles.mjs](../../tools/studio/roles.mjs) gives QA Markdown-only write scope, while the world role owns Editor C#. A test-authoring pilot needs deliberate scope ownership and a sequential handoff.
- [packages-lock.json](../../game/Packages/packages-lock.json) resolves **Test Framework 1.8.0**, `source: builtin`, with `com.unity.ext.nunit` 2.1.0. The local Editor's `Editor/Data/Resources/PackageManager/BuiltInPackages/com.unity.test-framework/package.json` confirms 1.8.0. Absence from the direct manifest does **not** mean Test Framework is unavailable. Pipeline's dependency request for 1.1.33 is not the resolved version.

The missing rendered/manual pass, absent npm lockfile, Editor child-process licensing issues, and unactivated Unity AI are already recorded baseline gaps. They are not counted as discoveries here.

## Candidate register

Effort estimates are researcher judgments for bounded pilots, not measured delivery promises. “High impact” describes potential value; only the separately marked **new addition** contributes to the stopping rule.

| ID | Candidate | Disposition and novelty | Potential impact | Pilot effort |
| --- | --- | --- | --- | --- |
| P01-01 | Generate and shrink gameplay action sequences against independent rules | **New addition; recommend narrow pilot** | High for systemic interactions and agent-authored regressions | 1–2 focused days |
| P01-02 | Bind results/captures to exact source and built artifact; promote that artifact | Baseline refinement of CI, diagnostics and source revision evidence | High | Half to one day |
| P01-03 | Verify asset imports independently of a warm Library cache | Baseline refinement of asset checks and reproducible integration | Medium now; high when import tooling grows | Half day initially |
| P01-04 | Verify real input paths across timing/render conditions | Baseline refinement of runtime/visual/performance tests | High for combat feel | One day for first bounded scenario; input migration extra |
| P01-05 | Turn milestone playtest questions into explicit observation protocols | Baseline refinement of playable review and fresh-player criteria | High | Hours plus participant sessions |
| P01-06 | Capture tuned parameters and problematic arrangements back into reviewed source fixtures | Baseline refinement of experiment presets, diagnostics and test arenas | Medium–high during feel iteration | One day for one preset/fixture path |
| P01-07 | Separate validation from creation/repair | Baseline refinement; small concrete integrity fix | High relative to cost | Hours |
| P01-08 | Enforce stale-write rejection and cross-process Editor fencing | Baseline refinement of ownership, leasing and resumability | High when concurrency begins | One to several days depending on scope |
| P01-09 | Separate credential-bearing orchestration from executable Unity validation | Potentially new control; **defer trust-boundary decision**, do not count this pass | High if less-trusted workers/code enter | Environment-dependent; unestimated |
| P01-10 | Use a small, explicit engine/package/platform upgrade lane | Baseline refinement of CI and capability/version checks | Medium | Hours for checklist; target-machine setup extra |
| P01-11 | More scene/GUID indexes, semantic diffs, asset provenance and staging | Duplicate categories | Useful when assets grow | No additional pilot proposed here |
| P01-12 | Treat a seed, input recording or fixed physics step as exact cross-machine replay | **Reject unsupported guarantee** | Misleading assurance | No adoption |
| P01-13 | Install FsCheck/Stryker or replace the orchestrator as the default response | **Defer tools; reject framework-first adoption** | Unproven versus a small pilot | Compatibility investigation required |
| P01-14 | Split the whole game into pure simulation assemblies before further feature work | **Defer broad refactor**; allow minimal seams required by P01-01 | Unproven at current size | Potentially substantial |

### P01-01 — Generated action sequences, independent rules, reduced failures

**Rationale and novelty.** The baseline proposes seed regression and runtime checks. This adds a different test construction method: generate legal action histories, check rules after each action, and automatically remove irrelevant actions from a failure. The output is an understandable minimal reproduction, not simply another failed seed. World seeds alone cannot explore Ember → wet → expiry → Storm → death → additional hit permutations. One developer-written assertion script also provides little evidence that future tests detect the intended mistake.

FsCheck documents state-machine operations with a separate model and shrinking; its stateful API is explicitly experimental and may break on minor releases. Its guidance also requires fresh/reset mutable state for reliable shrinking. These establish the method, not proven Unity compatibility or a reason to install FsCheck. [FsCheck stateful testing](https://fscheck.github.io/FsCheck/StatefulTesting.html), [mutable-state guidance](https://fscheck.github.io/FsCheck/TipsAndTricks.html).

**Pilot.** Before adding more statuses, design/QA writes a short rule table derived from accepted design: legal nonnegative damage/healing, health bounds, no resurrection through Heal, at most one death callback, dry Ember ignition, and wet Storm multiplier. Explicitly resolve whether extinguishing must happen immediately or on the next update; current code and prose should not silently become each other's oracle. The implementer then adds a small seeded command generator and sequence reducer to Unity tests. Use fixed command values and explicit commands such as SetWet, ApplyEmber, ApplyStorm, ApplyKinetic, Heal and Kill. Serialize both generator seed and full command list so future generator changes do not destroy reproduction.

Start with **500 synchronous, bounded sequences** that do not advance game time, constructing a fresh Health component for every replay. Validate health/death/wet-hit properties without refactoring the simulation. Run three intentionally broken variants in disposable copies: wet Storm multiplier becomes 1, dead-target protection is removed, and healing is allowed after death. Each must fail for the intended reason. Preserve the reduced cases as normal regressions. This is a small negative-control exercise, not a proposal for whole-project mutation infrastructure.

Only after that succeeds, add a controlled clock/tick seam for status expiry and burn application; keep MonoBehaviour integration tests that advance actual Unity frames to verify the adapter. Do not simulate elapsed `Time.time` by calling `Physics.Simulate`. Unity states that this does not call `FixedUpdate`; it certainly does not execute Yendor's `Update` loop for us. [Unity 6000.6 Physics.Simulate](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/physics/simulate).

**Pass criteria.** The clean implementation passes all 500 sequences on three fresh runs; each broken variant fails and yields a replayable reduced sequence of at most five commands; no reduced trace depends on prior scene/static state. A second reviewer can explain the failed domain rule using the trace without reading production implementation. Report tested command combinations and rejected sequences; test count alone is not coverage. Time-based coverage remains explicitly incomplete until the second phase passes.

**Dependencies and failure modes.** Needs a small test assembly, deliberate QA/test write ownership, a reviewable rule table, and a resettable fixture. Unity's installed 1.8.0 framework is sufficient to host a project-local generator; verify assembly references through that version's bundled examples. Unity 6000.6 documents Edit/Play tests and coroutine/frame support. [Unity testing guide](https://docs.unity.com/en-us/engine/6000.6/manual/programming-environment/test-framework-introduction). Do not copy a separate stock NUnit package into the project. Risks include duplicating implementation bugs in the oracle, generating mostly invalid histories, shrinking into illegal histories, and hiding a real timing defect through a fake clock. Constrain inputs, check command preconditions during reduction, and preserve actual-frame adapter tests. Cross-platform exact physics is outside this pilot.

### P01-02 — Evidence lineage and artifact promotion

**Rationale/novelty.** Fixed logs and a fixed player path are convenient but can be overwritten after a review. Strengthens the baseline's source-revision and artifact references; not a new CI category.

**Pilot/pass.** Produce a run directory containing source revision plus dirty-source digest, Unity version, package-lock hash, build options, configuration/seed, per-check outcome, and hashes of the complete player bundle and captures. Keep the tested bundle for manual review instead of rebuilding under the same label. Unity exposes a player build GUID, but it is empty in the Editor and is not a content hash. [Application.buildGUID](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/application/buildguid). Deliberately pair an older capture with a newer bundle: the review manifest must identify the mismatch. Mutating one built data file must invalidate the bundle digest. A valid prior run remains accessible.

**Dependencies/failure modes.** Requires artifact retention and a small manifest helper. A commit SHA misses dirty files; hashing only the executable misses Unity data; GUIDs alone do not prove contents. Avoid a general provenance platform or bit-identical-build promise for this project.

### P01-03 — Import consistency and clean-checkout checks

**Rationale/novelty.** Future Meshy imports, postprocessors and URP work may accidentally depend on local cached imports. This specifies an existing reproducibility/asset-check goal.

Unity 6000.6 supplies `-consistencyCheck` and `-consistencyCheckSourceMode local`; inconsistencies are reported in logs. No Accelerator is required for local checking. This is not proof that the final player is bit-identical. [Version-matched import checker](https://docs.unity.com/en-us/engine/6000.6/manual/assets-and-media/import-assets/import-analyze/importer-consistency).

**Pilot/pass.** Use a disposable project copy with its own Library. Run import consistency against a warmed import, then separately build from a fresh project copy to expose missing source dependencies. Compare imported asset identities and expected scene/asset checks. Pass when there are no importer inconsistency messages and the fresh copy validates/builds without hand-copied Library contents. When a custom importer exists, demonstrate the checker catches one deliberately unstable importer in a disposable fixture.

**Dependencies/failure modes.** Needs licensed Editor access and time/storage for a cold import. Parse warnings rather than assuming exit zero means consistency. Do not delete the only historical Library before a local comparison, share a mutable Library between workers, or introduce Accelerator before import cost warrants it.

### P01-04 — Input, timing and rendered verification matrix

**Rationale/novelty.** Current smoke teleports around the traversal. Actual input coverage is already a baseline requirement; this makes the first case and its limits explicit.

Factorio's lead developer reports value from tests that drive low-level input through the whole GUI processing path. That is first-person evidence for a strategy, not Unity automation compatibility. [Kovarex, 2021](https://www.factorio.com/blog/post/fff-366). Unity InputEventTrace can record/load/replay Input System events, but it does not target the legacy Input calls used here. [Input System 1.13 API](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.13/api/UnityEngine.InputSystem.LowLevel.InputEventTrace.html).

**Pilot/pass.** First provide one controllable input seam or use actual device events on a graphics-enabled player. Exercise movement, double jump, dash, aim/fire, cursor release/recapture and interaction without teleporting. Repeat at measured approximately 30, 60 and 120 fps where the machine can sustain them, with recorded frame times and tolerances for distance/cooldown behavior. Require no lost action or unbounded difference under the chosen tolerances; retain video and observations for the camera/feel judgment. A frame cap is not proof of the achieved frame rate.

**Dependencies/failure modes.** Needs graphics and a named machine; numeric tolerances must follow design intent. Full Input System migration is a separate product decision. Do not confuse replayed input events with deterministic world simulation, pixel equality with readability, or this route's success with all-seed reachability.

### P01-05 — Small hypothesis-driven playtests

**Rationale/novelty.** The milestone already asks whether fresh players understand objectives and exploit interactions. A protocol is a useful refinement, not a new discovery. Mike Ambinder's Valve presentation describes treating designs as hypotheses, using observation alongside questions, and recognizing observer/question bias. This is a 2009 first-person practice report, not a present-day Unity product claim. [Valve presentation](https://cdn.akamai.steamstatic.com/apps/valve/2009/GDC2009_ValvesApproachToPlaytesting.pdf), [official publication listing](https://www.valvesoftware.com/en/publications).

**Pilot/pass.** Once a playable build exists, run three individually observed fresh-player sessions. State one hypothesis beforehand: the player identifies the relic objective and intentionally exploits wet + Storm without verbal coaching. Record objective recognition time, relevant observed action, stalls and a short post-run explanation. Decide what to change from the observations. The workflow passes if every session yields evidence linked to the exact build and a clear supported/unsupported/inconclusive conclusion. A provisional gameplay gate can be two of three players demonstrating both behaviors; this small sample is diagnostic, not population-level proof.

**Dependencies/failure modes.** Requires consenting participants and a human observer; recruitment is a later authorized task. Avoid coaching, moving the success criterion after results, or letting an agent's screenshot review substitute for a fresh person's understanding. No new analytics service is needed.

### P01-06 — Preserve tuning and authored reproductions

**Rationale/novelty.** Movement constants are largely in C# and generated scene edits are ephemeral. Existing experiment presets/diagnostics/test arenas should include an explicit route back to reviewed source. Factorio's developers describe generating fixture code from an in-game entity arrangement to avoid manually guessing spatial setups. [Kovarex, 2016](https://www.factorio.com/blog/post/fff-154).

**Pilot/pass.** For one dash/camera preset, export an explicit proposed configuration with base revision, changed values and observation. Apply it through a reviewed config/source change. For one camera obstruction case, export a small labeled arrangement and spawn pose to a JSON fixture consumed by the existing world builder. Restart the Editor/player and reproduce both from source alone. Pass when another worker can recreate the arrangement and parameter values without the originating scene/session.

**Dependencies/failure modes.** Requires a bounded schema and stable entity labels. Existing JSON scopes are enough for an initial data path; a full custom editor is optional. Do not serialize an entire arbitrary live scene or promote transient simulation state into authoring defaults. ScriptableObject assets can hold authoring data, but a deployed player cannot save an asset by using Editor serialization; exported player tuning needs an explicit file path and subsequent import. [Unity ScriptableObject documentation, 6000.1](https://docs.unity3d.com/6000.1/Documentation/Manual/class-ScriptableObject.html). This older version documents the distinction; verify any chosen 6000.6 editing API before implementation.

### P01-07 — Validators must expose missing inputs

**Rationale/novelty.** `ProjectSetup.Validate()` currently creates a missing scene. This can turn a deleted required artifact into a passing validation and alter settings during a check. A stronger specification of baseline independent verification; project source is the primary evidence.

**Pilot/pass.** Retain explicit `CreateScene()` as an authoring command, make validation fail on a missing required scene, and run checks in a disposable copy. Compare tracked source before/after validation; generated outputs belong in designated output directories. Temporarily remove the scene only in the disposable test fixture: validation must fail and leave it absent. Normal validation must leave tracked source unchanged.

**Dependencies/failure modes.** Minimal code change plus regression fixture. Legitimate Unity version/package migrations may write settings; report those separately rather than silently accepting an altered subject. Do not forbid documented build outputs or intentional generation stages.

### P01-08 — Enforce ownership at mutation boundaries

**Rationale/novelty.** Whole-file writes can overwrite work read earlier; the validation boolean is not shared between separate processes. These sharpen the baseline's worktree, ownership, Editor lease and recovery proposals.

**Pilot/pass.** Add expected-content identity to writes and require re-read on mismatch. When workers gain separate processes, use a cross-process Editor lease with a generation/fencing token checked at execution, plus explicit command state and result identity. Exercise two writes from the same old version, two validators and a worker resuming after its lease has been replaced. Pass if the stale write/lease is rejected, one Editor operation runs, and the result belongs to the current owner.

**Dependencies/failure modes.** A compare-then-write without serialized mutation is still racy; a lease timeout alone cannot stop a paused old process from acting. Durable command IDs and idempotency are useful specifications of already proposed resumability, not a separate orchestrator recommendation. Immediate writes need not wait for broad concurrency infrastructure.

### P01-09 — Decide the executable-code trust boundary

**Rationale/novelty.** A fixed Unity command still loads project-supplied Editor code. World workers can author C# in the Editor directory, and Unity supports launch-time execution through `InitializeOnLoad`. [Unity 6000.6 API](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityeditor/initializeonloadattribute). Node currently inherits its environment into Unity. Therefore path-scoped text tools do not imply execution isolation; the accepted ADR already explicitly calls the system trusted local tooling.

**Disposition.** A distinct control if adopting less-trusted external workers, but not enough evidence here to recommend a particular sandbox for the existing trusted-local mode. Preserve for the next researcher. The generic separation of untrusted build code and privileged credentials is also documented by GitHub, but its Actions-specific events are not Yendor's present local topology. [GitHub guidance](https://docs.github.com/en/actions/reference/security/securely-using-pull_request_target).

**Conditional pilot/pass.** Define which workers and imported code are trusted; start with an environment allowlist for Unity that excludes provider/Meshy credentials. If OS isolation is required, validate an ephemeral restricted runner with only the necessary project, outputs and licensing access. A harmless canary Editor script must fail to read a synthetic credential/out-of-scope fixture while normal compilation succeeds. Never use real secrets as a probe. Dependencies: an explicit trust decision and workable Unity licensing/IPC. Failure mode: environment filtering alone does not prevent reads from same-user files; a worktree is not a security boundary. No feasibility or security pass was established here.

### P01-10 — Controlled upgrades and eventual Windows checks

**Rationale/novelty.** The exact Editor is pinned and Windows is eventual, so another matrix is unnecessary now. Keep a small upgrade lane within baseline CI/version checks. Unity 6000.6's Test Framework is a core package fixed to the Editor version, so old advice to independently upgrade it can be wrong. [Core-package documentation](https://docs.unity.com/en-us/engine/6000.6/manual/packages-list/packages-all/pack-core/com-unity-test-framework).

**Pilot/pass.** At the next deliberate engine, render-pipeline or major package change, use a disposable branch/copy and compare compilation, bundled test version, import behavior, player smoke and one representative rendered/feel capture. Pass only with explainable differences and a rollback to the previous source/environment. Before claiming Windows support, build and run on a named Windows machine; source compilation or a Linux player is insufficient.

**Dependencies/failure modes.** Target build modules, OS access and an actual change justify the matrix. Avoid a weekly upgrade treadmill, an immediate IL2CPP migration without a product need, or claiming restored source alone reverses every cache/settings migration.

### P01-11 — Duplicate asset/semantic infrastructure

Scene/GUID indexes, semantic scene/prefab diffs, stable asset provenance, reviewed import staging and geometry/material budgets already appear in the baseline and accepted art workflow. Runtime geometry summaries would refine that coverage, not create a new category. Dependency-based selective validation similarly refines CI once check costs warrant it. No additional product evaluation or standalone pilot is justified at this stage. Reuse the baseline pilots and require them to find a planted missing-reference/asset-budget fault before expansion. Main failure mode: investing in indexes that cannot stay current with runtime-generated geometry.

### P01-12 — Reject exact replay claims from insufficient controls

Neither the one shared world RNG nor recorded device events proves cross-machine gameplay determinism. `Physics.Simulate`'s fixed-step advice is narrower than an end-to-end guarantee, and current controller/status logic executes outside that manual physics call. Sources: P01-04's InputEventTrace documentation and P01-01's version-matched physics reference. Reject the claim, not useful replay diagnostics. If exact replay becomes a requirement, a separate pilot must establish state hashes over controlled simulation ticks across named builds/machines and explain every divergence. That cost is not justified for this slice's present seed-replay promise.

### P01-13 — Defer new frameworks until the small method works

FsCheck is a method reference, not an approved package. Its experimental state-machine API and Unity's custom NUnit integration require compatibility work; no release pin, Unity adapter compatibility, or standalone Stryker execution path was verified. A project-wide mutation-score gate, another orchestrator, or another scene automation brand adds maintenance before value is known. The narrow three-broken-variant control in P01-01 is sufficient to evaluate the idea. Only compare libraries after measuring repeated maintenance pain; a future pass criterion would be identical failing cases/reduction quality with demonstrably lower maintenance and no test-runner conflicts.

### P01-14 — Defer wholesale simulation/assembly restructuring

A pure rules core and explicit time/input adapters can improve testability, but the game is six small runtime files. Splitting every subsystem or adopting an ECS/deterministic physics stack would exceed the evidence. Allow only the assembly boundary needed for tests and, later, the smallest clock seam needed by measured timing cases. Pass criteria for any extraction: unchanged gameplay smoke/feel behavior plus genuinely simpler tests; abandon it if it merely mirrors the implementation or delays the combat-feel milestone. This is an architectural tradeoff, not a new workflow tool.

## Source status and limitations

All cited pages were fetched on 2026-11-08. Unity 6000.6 pages were preferred over search results for 6000.7 or legacy 2022/6000.0 documentation. Official docs establish supported concepts; no proposed pilot was executed. FsCheck's own documentation explicitly marks stateful testing experimental. InputEventTrace documentation is package 1.13, while this project uses legacy Input; it is not a drop-in claim. Test Framework 1.8.0 availability was verified both in the local Editor and project lockfile. The ScriptableObject citation is older 6000.1 and supports only the authoring-versus-player persistence distinction.

The Factorio reports (2016 and 2021) and Valve presentation (2009) are historical first-person production evidence. They support learning from practice, not causal estimates of time saved in Yendor, Unity package compatibility, or a claim that large-studio processes are necessary here. Recommendations, effort estimates, quantitative pilot sizes and pass thresholds are this researcher's proposals. No headless run was represented as visual approval; no model-family superiority, cost savings, full deterministic replay, or secure sandbox was established.

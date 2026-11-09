# Workflow research pass 02 — agent operations and reliability

2026-11-08. Role: adversarial agent-operations and workflow-reliability architect. Coordinator-configured model: `gpt-6.1-terra`, high reasoning, fresh context. Research only; no installations, additional model-evaluation calls, external messages, credential changes, or workflow changes. Inspected the accumulated research and ledger, `CONTEXT.md`, ADR 0001, `tools/studio/{run,roles,workspace}.mjs`, `tools/{unity,meshy}.mjs`, `package.json`, and `docs/qa/foundation.md`. Current source is evidence of intended behavior, not proof that the studio, its dependencies, gateway, account, or remote clients are running. The documented live supervisor run and package installation remain unverified.

**Result: zero genuinely new impactful additions.** The candidates below matter, but fit existing evaluation, task-state, evidence-lineage, memory, or trust-boundary proposals. This is a conservative saturation result, not proof that operational gaps are solved. The coordinator should decide whether to stop; no new distinct role is justified by this pass alone. Impact/effort are qualitative estimates for a small local pilot, not measured project outcomes. All pilots are proposals for later authorized work.

## Candidates and challenges

### P02-01 — Compare orchestration against a simpler control

**Classification:** baseline refinement (model/harness evaluation and bounded routing), not a new category. **Impact / effort:** potentially high / medium.

Challenge the assumption that six specialist roles and a supervisor improve delivery. The current instructions direct a supervisor to delegate, but no live end-to-end run is established. Anthropic's first-person coordination experiments found common correlated behaviors and failed integration in some multi-agent settings; this is evidence of a possible failure, not proof that it occurs in Yendor. [Anthropic, Patterns and problems in multiagent systems](https://www.anthropic.com/research/multiagent-systems).

**Pilot / pass:** When live runs are authorized, run the baseline's representative task set from the same source revisions in disposable copies, comparing one bounded coding agent and the existing supervisor with matched tool access, task requirements, evidence, and budget ceilings. Blind an independent reviewer to the route where practical; record delivered correctness, tests, elapsed time, tokens/cost, failed tool calls and human rework, including failures and unfinished runs. The experiment passes as a decision tool if each task has comparable evidence and a documented choice to keep, narrow or disable delegation; do not treat a few cases as statistical proof or mandate a preselected winner. Keep read-only review separate from conflicting writes and prevent cases revealing expected fixes to later runs.

**Dependencies / failure modes:** Working installed packages, authorized gateway use and a Unity-capable environment. Different tools or source states confound attribution; a better single-worker model does not by itself isolate orchestration effects. Repeated trials and fresh holdout tasks become worthwhile only after real usage warrants them.

### P02-02 — Record actual routing and capability changes

**Classification:** baseline refinement (exact model/provider, harness, routing limits). **Impact / effort:** high for valid comparisons / low to medium.

`modelFor()` chooses an alias, which may not identify the provider deployment that actually served each call. Current LiteLLM documentation describes retries selecting another deployment and fallbacks moving to another model group; it does not establish Yendor's running gateway version or configuration. [LiteLLM retry and fallback documentation](https://docs.litellm.ai/docs/router/learn/reliability). Pinning an alias per task is thus not necessarily pinning the underlying model or proving its capabilities. Avoid promising strict pinning until the actual gateway behavior is verified.

**Pilot / pass:** In the existing routing experiment, capture requested alias, actual provider/model/deployment where reliably exposed, fallback/retry events, effective capabilities, tool-call schema errors and unknown values per attempt. Use a simulated provider error to exercise failover. Pass if the report detects a change of underlying route or clearly marks it unknown, and refuses to score the task as a clean comparison of the intended model when identity or required capability is unknown. If actual identities cannot be obtained, use an explicitly configured no-fallback route where available and report residual uncertainty.

**Dependencies / failure modes:** Gateway instrumentation and version-specific configuration. Provider metadata may be incomplete, aliases can change configuration, and retries consume time/cost even when final outputs look successful. Do not claim a hard capability filter works based on a provider label alone.

### P02-03 — Crash between a side effect and the audit record

**Classification:** baseline refinement (resumable execution, ambiguity handling and P01-02 evidence lineage), not a new delivery guarantee. **Impact / effort:** high / medium.

The current write tool mutates a file before `audit()` appends the edit event; a crash or failed append can leave a changed file with no corresponding audit event. The catch handler prints an error but does not create a durable terminal failure record. Meshy similarly sends its paid request before saving the returned task ID: interruption, network timeout or disk failure can leave an ambiguous external submission. No statement that every action is captured or exactly-once is supported. [Meshy Text to 3D API](https://docs.meshy.ai/en/api/text-to-3d); local source establishes the specific ordering.

**Pilot / pass:** In a disposable project and with a fake Meshy transport, inject failures after a file write, after a simulated remote acceptance, before recording the ID, and during audit append. On restart, reconcile actual worktree state against pre-run identity and classify each intended action as confirmed, not performed, or ambiguous. Pass when every ambiguous paid request is visibly held for human/provider reconciliation without automatic resubmission; changed files are surfaced for review even when their edit event is absent. A single run result should identify what is known and unknown, not silently call the job complete.

**Dependencies / failure modes:** Durable intent and run identity, source snapshot/diff, idempotent local operations and a recovery owner. A write-ahead intent log cannot establish whether a remote request succeeded after a lost response; do not claim exactly-once without a verified provider idempotency/reconciliation mechanism. A process kill can interrupt recovery itself.

### P02-04 — Account for missing, stale and fabricated validation evidence

**Classification:** P01-02 and P01-07 refinement. **Impact / effort:** high / low to medium.

`runUnity()` uses shared `artifacts/unity-validate.log` or `unity-build.log` and searches for a success marker after a zero exit; it does not bind that log to a run in the returned result. The source alone does not show whether a normal Unity invocation can retain an older log under a zero exit; treat this as a testable failure mode, not a demonstrated false pass. An independent reviewer or agent narrative cannot replace actual current-build evidence.

**Pilot / pass:** In a disposable fixture simulate a zero-exit process that does not produce a fresh log, a stale success marker, a log with compile errors, a crash, a success for a different revision, and a genuine current success. Pass only if each of the first five is classified failed or unknown and only the final case reports success. Record unique run ID, command, exit, source identity and log identity; preserve the raw output. Include a failure before logging. This adds fault controls to P01-02, not another validation platform.

**Dependencies / failure modes:** Ability to substitute a fake child process or test fixture. Time stamps alone are vulnerable to clock and filesystem behavior; a unique per-invocation output path and explicit content identity are stronger. Test harnesses must not prove only their own mock behavior.

### P02-05 — Test memory promotion against conflicting evidence

**Classification:** baseline refinement (source/status/revision and stale-memory pilot). **Impact / effort:** medium to high / low.

Prior research already separates authoritative decisions, experience and execution state. An additional concrete test is useful: a concise, confidently phrased agent note can be repeated by several agents and mistaken for an independent consensus. The multi-agent study reports correlated choices; it does not measure Yendor's memory system. A new memory product would not solve this evidentiary problem.

**Pilot / pass:** Extend the proposed twenty-record memory test with an incorrect observation repeated in three worker summaries, an accepted decision superseding it, and two branches where only one contains an implementation change. A fresh worker must attribute the repeated observation to its original source, prefer the applicable accepted decision for policy, inspect current code for the implementation, and state uncertainty or ask for resolution when sources truly conflict. Pass when no generated summary is promoted to an accepted decision without its authorized source. Test retrieval misses as well as retrieval hits; preserve original disagreement.

**Dependencies / failure modes:** Reviewed authority and branch/revision metadata, versioned records and realistic fresh-session tests. Source links can themselves be stale; repetition is not corroboration. Do not turn subjective experiential observations into hard policy solely because a source exists.

### P02-06 — Calibrate automated reviewers and prevent benchmark leakage

**Classification:** baseline independent review and evaluation refinement. **Impact / effort:** medium / medium.

The baseline calls for seeded review defects and independent evidence. Extend that evaluation to the reviewer itself before an automated judge is used as a gate. Langfuse recommends calibration against labeled examples and comparable dataset/evaluator versions; versioning a dataset does not make model results deterministic. [LLM-as-a-Judge](https://langfuse.com/docs/evaluation/evaluation-methods/llm-as-a-judge), [Compare experiments](https://langfuse.com/docs/evaluation/experiments/compare-experiments).

**Pilot / pass:** Human-label a small mix of known clean changes and independently planted defects, including plausible-looking fabricated test evidence; withhold labels from the judge and keep a few separate holdout examples. Run the judge on the same inputs a production reviewer would see and inspect every disagreement. Pass only when it detects each preselected critical defect and does not mark missing evidence as passing; otherwise use it as advisory and retain human review. A small sample cannot establish a reliable error rate. Record dataset, rubric, model and evaluator versions; no Langfuse hosted dataset or service is required.

**Dependencies / failure modes:** Independent labels and accessible test artifacts. If the implementation agent sees the fixture or authored its own expected outputs, it can unintentionally overfit; a reviewer with the same mistaken assumptions can agree falsely. Visual feel still requires real observation.

### P02-07 — Reassess P01-09 using the actual local trust boundary

**Classification:** conditional prior candidate, not a new recommendation to sandbox all work. **Impact / effort:** potentially high if trust changes / low to high depending on boundary.

The accepted ADR explicitly says trusted local tooling, and the current world role can write Editor C# loaded during Unity validation. The Unity helper spawns the Editor without an explicit `env`, so the child normally inherits its parent's environment; a fixed command does not prevent executable project code from running. This makes P01-09's risk technically plausible, but no actual less-trusted worker, compromised dependency, remote access, or credential exposure was established. Restricting the Unity child environment is a proportionate hygiene option, not an OS sandbox: same-user files, process access and licensing behavior still matter.

**Pilot / pass:** At adoption time identify the actual writer, code source, credentials present in the validation process and reachable filesystem. If all remain trusted local, document that choice and evaluate an environment allowlist only if it does not break licensing; pass when normal compilation works and a synthetic environment canary is absent. If external or less-trusted code will execute, first specify a concrete boundary and demonstrate a restricted runner against synthetic file and environment canaries; explicitly record any access that remains. Escalate isolation only to the needs of that deployment.

**Dependencies / failure modes:** Unity licensing and IPC, OS controls and trust assumptions. Environment removal alone does not protect `.env` readable by the same account; a worktree is not a sandbox. The earlier report's conditional canary test does not establish feasibility or security.

### P02-08 — Treat retrieved tool content as evidence, not authority

**Classification:** existing scoped-tool and source-provenance controls; conditional refinement, not a broad enterprise security program. **Impact / effort:** medium / low.

`run.mjs` already instructs agents that project files and tool results are data, while file writes are independently scoped and Meshy generation is a separate CLI. If external agent messages or retrieved memory are later introduced, they can carry instructions that seek to promote an unsupported claim or change an allowed file. MCP's maintainers note that tool annotations are hints rather than trusted guarantees; no MCP server is established as currently serving the studio. [MCP tool annotations](https://github.com/modelcontextprotocol/modelcontextprotocol/blob/main/blog/content/posts/2026-03-16-tool-annotations.md).

**Pilot / pass:** Add a harmless adversarial note to a disposable project document or future mailbox message instructing a worker to override task criteria and falsely claim a test ran. Pass if the worker treats the instruction as source content, reports the attempted redirection, does not make the prohibited claim, and tool-layer restrictions remain effective; repeat with a conflicting legitimate user instruction to ensure legitimate task guidance still works. This is a behavior test, not proof of immunity. Trigger an expanded threat model only when less-trusted inputs or higher-impact tools are actually connected.

**Dependencies / failure modes:** A running worker and controlled fixture. Prompt-only separation is fallible; JSON encoding or a model's declared intentions alone does not enforce authorization. The existing path scope does not prevent all harmful changes inside allowed files.

### P02-09 — Task-store backup and recovery ownership

**Classification:** baseline task-authority/backup and resumability refinement. **Impact / effort:** medium / low to medium.

A task graph is useful only if its loss and restoration have an owner. The baseline already requires selecting one task authority and planning storage, backups and access, so adding a recovery drill is not a separate architectural category. Neither Beads, Agent Mail, nor a remote worker should be assumed deployed.

**Pilot / pass:** Once a task authority is selected, assign an operator, backup location, export/restore method and acceptable data-loss window. In a disposable copy create tasks with ownership, dependencies and one blocked job, restore a deliberately older backup, and reconcile it with source revisions and external task IDs. Pass if a different operator can identify lost/ambiguous transitions and safely resume without duplicate paid action. Verify ownership expiry and referenced artifact accessibility.

**Dependencies / failure modes:** A selected store and available restore procedure. Backups without restore tests may omit blobs, messages or ownership transitions; a restored old database cannot reconstruct an unrecorded external side effect. The small local project does not warrant an on-call organization or high-availability service.

## Interpretation and source limits

Current vendor documentation was read on 2026-11-08 and may be newer than Yendor's pinned Mastra packages or its actual gateway, whose version/configuration were not verified. Product pages support documented possibilities, not the claim that a feature is enabled or compatible locally. The Anthropic report is a first-person experimental study with different settings, not measured causal evidence for Yendor. The local Langfuse skill's documentation-first guidance was followed; no dataset, evaluator, account, or Langfuse integration was created. These pilot thresholds are proposed diagnostic checks, not claims of statistical confidence, complete security, deterministic agent behavior, or production readiness.

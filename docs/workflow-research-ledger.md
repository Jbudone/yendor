# Workflow research ledger

Started: 2026-11-08. Status: research complete; practical saturation after two passes.

The user requested fresh high-quality agents with different roles, repeating
research while impactful new suggestions emerge. Research is authorized;
installation and adoption are not part of this task.

## Baseline

Read [the initial research](agent-workflow-research.md) and existing project docs.
Previously considered: session instructions and handoffs; task graph/Beads;
Mastra memory, Mem0, Graphiti; Agent Mail; worktrees and ownership; capability
routing and model/harness evaluation; resumable workflows; CI and independent
review; Unity CLI/Pipeline, Unity AI, Meshy; semantic C# navigation; scene/GUID
indexes; asset checks; test scenes, seed regression, diagnostics, performance
and capture; Langfuse; ACP/A2A and alternative orchestrators.

## Impact and stopping rule

A new suggestion is impactful when it identifies a materially different,
actionable workflow/tool/control that addresses a concrete Yendor need and
could substantially improve delivery, continuity, validation, coordination or
asset production. Another product in an existing category, minor configuration,
or restatement is recorded but does not independently justify another pass.

Each fresh researcher reads the accumulated ledger and pass reports, checks
current primary sources, distinguishes evidence from inference, and records
all candidates it considered, including duplicates, deferred and rejected ones.
The coordinator evaluates novelty and impact. Continue with a fresh model and
role when at least one new impactful suggestion survives review. Stop when
a fresh pass finds none. This is a practical saturation test, not proof that
no useful tool exists elsewhere.

## Pass records

### Pass 01 — Unity technical director

Configured model: `gpt-6-astra`, high reasoning, fresh context. Report:
[workflow-pass-01.md](research/workflow-pass-01.md).

Coordinator decision: **continue**. P01-01 is a genuinely new testing method,
distinct from fixed smoke checks and a world-seed corpus. A narrow pilot is
appropriate; no broad simulation rewrite or library installation is accepted.
Root independently confirmed the inspected revision and the relevant health and
validation code observations. The remaining suggestions remain recorded even
when they do not justify another research pass.

Next role: adversarial agent operations and reliability architect.

### Pass 02 — Agent operations and reliability architect

Configured model: `gpt-6.1-terra`, high reasoning, fresh context. Report:
[workflow-pass-02.md](research/workflow-pass-02.md).

Coordinator decision: **stop**. The fresh pass found zero genuinely new
impactful additions. Its nine candidates strengthen previously recorded
evaluation, routing, recovery, evidence, memory and trust-boundary proposals.
Source inspection supports those refinements, but none meets the novelty rule
for another pass. For example, comparing single-worker execution against a
supervisor is a useful control within the existing model/harness evaluation;
restore drills operationalize the already proposed backup/recovery requirements.

This completes the user-requested loop at practical saturation. It does not claim
exhaustive knowledge of all tools or prove the pilots will improve Yendor.
Two fresh agents used different configured models and roles sequentially; no
parallel research team or recursive delegation was used.

## Consolidated suggestions

| ID | Suggestion | Coordinator disposition |
| --- | --- | --- |
| P01-01 | Generated gameplay command histories, independent oracle, reduced failures and deliberate broken controls | New impactful candidate; narrow pilot recommended |
| P01-02 | Exact artifact/evidence lineage and promotion | Valuable baseline refinement |
| P01-03 | Warm-cache import consistency and separate clean-checkout validation | Valuable baseline refinement |
| P01-04 | Real-input timing/render verification matrix | Valuable baseline refinement |
| P01-05 | Hypothesis-driven fresh-player observation protocol | Valuable baseline refinement |
| P01-06 | Export tuning and authored failure fixtures back into reviewed source | Valuable baseline refinement |
| P01-07 | Non-repairing validators | Valuable concrete baseline refinement |
| P01-08 | Stale-write rejection and cross-process Editor fencing | Valuable baseline refinement |
| P01-09 | Separate executable validation from credential-bearing orchestration | Conditional candidate for reliability review |
| P01-10 | Deliberate upgrade and eventual Windows verification lane | Baseline refinement; trigger when relevant |
| P01-11 | More asset/semantic infrastructure | Duplicate; retain baseline work |
| P01-12 | Exact replay guarantees from seeds/input/fixed physics alone | Unsupported guarantee rejected |
| P01-13 | FsCheck/Stryker/framework-first adoption | Deferred until small method proves useful |
| P01-14 | Wholesale pure simulation/assembly restructuring | Deferred; only minimal test seams justified |
| P02-01 | Compare supervisor orchestration against a single-worker control | Evaluation refinement; measure before assuming delegation helps |
| P02-02 | Record actual deployment/fallback identity and capability changes | Routing/evidence refinement |
| P02-03 | Reconcile crashes between a side effect and its audit record | Recovery refinement; paid ambiguities must not silently resubmit |
| P02-04 | Reject stale, missing, fabricated or wrong-revision validation evidence | Evidence/validator refinement with fault controls |
| P02-05 | Test memory with repeated false notes and conflicting branches | Memory-correctness refinement |
| P02-06 | Calibrate reviewers with clean cases, planted defects and holdouts | Evaluation refinement; advisory until demonstrated useful |
| P02-07 | Reassess executable-code trust boundary proportionately | Conditional refinement of P01-09; no blanket sandbox requirement |
| P02-08 | Test retrieved messages/content as evidence rather than authority | Existing scoped-tool/provenance refinement |
| P02-09 | Assign task-store restore ownership and rehearse recovery | Backup/recovery refinement |

Compatibility correction from pass 01: Unity Test Framework **1.8.0** is already
bundled/resolved; lack of a direct manifest entry does not mean it is absent.

All **23** candidates remain in the register and detailed reports, including
duplicates, rejected guarantees and deferred ideas. They are recommendations,
not an instruction to adopt every item.

## Suggested incorporation sequence

1. During the existing workflow foundation work, include non-repairing validation
   (P01-07), per-run evidence lineage (P01-02/P02-04), source-aware writes (P01-08)
   and explicit ambiguous/failed task outcomes (P02-03). Avoid rebuilding these
   as separate overlapping systems.
2. During the model/harness pilot, add a single-worker control (P02-01), actual
   routing identity (P02-02), reviewer negative controls/holdouts (P02-06), and
   the existing memory pilot's conflict tests (P02-05).
3. Before extending status interactions, trial the newly proposed generated
   command-sequence tests with independent rules and reduced failures (P01-01).
   Use the installed Unity test framework and a small project-local generator
   first; libraries and architectural changes remain deferred.
4. During combat/visual iteration, adopt real-input timing checks (P01-04),
   build-linked playtest hypotheses (P01-05) and tuning/fixture round-trips
   (P01-06). Import consistency becomes more valuable as generated art arrives
   (P01-03).
5. As tools and workers are connected, exercise stale ownership, recovery,
   restore and content-authority boundaries (P01-08/P02-03/P02-08/P02-09).
   Decide P01-09/P02-07 against the actual environment, not a hypothetical one.
   Trigger upgrade/Windows checks when those changes become relevant (P01-10).

Each detailed report supplies rationale, source links, dependencies, limitations
and proposed pilot/pass criteria. The initial research retains the previously
proposed tools; this ledger captures additions and refinements from the loop.

## Adoption

No installations, subscriptions, external messages, account changes or gameplay
changes are authorized by this research record. Accepted recommendations will
be selected for a later implementation plan.

## Implementation authorization

The user subsequently authorized hooking up the selected workflow, a public
Jbudone/yendor repository with Gitleaks, OpenRouter through LiteLLM and Langfuse
Cloud. The research-only restriction above describes the earlier research turn;
see docs/workflow-setup-status.md and ADR 0002 for adoption and remaining gates.

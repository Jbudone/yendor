# Workflow verification

See ../workflow-setup-status.md for observed results and remaining gates.
Machine-readable evidence stays under ignored artifacts/runs and artifacts/pilots.
The source identity in each manifest includes dirty/untracked source files;
a Git revision alone does not identify a work-in-progress build.

The generated-history suite tests 1,500 synchronous histories across three seeds
and three deliberately faulty adapter controls. Time-based burning/wet expiry,
actual input and manual feel are not established by these tests. The controls
emulate known faults and do not claim source-level mutation coverage.

The Node suite exercises scope/CAS, lock contention, memory staleness, invalid
verification outputs, private environment filtering, paid-job ambiguity and
Gitleaks rejection/redaction. The studio pilot uses an entirely local fake model.

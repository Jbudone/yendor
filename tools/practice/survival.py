#!/usr/bin/env python3
"""survival.py — did the agent's changes survive?

The audit grades habits because habits are all transcripts contain. This measures
the thing that actually matters: a change that had to be fixed or reverted within
14 days was not a change, it was rework.

    python3 tools/practice/survival.py [--days 14] [--since v0.0]

Reads only git. No network, no model calls, no opinions.
"""
from __future__ import annotations

import argparse
import collections
import datetime as dt
import re
import subprocess
import sys


def git(*args, check=True):
    r = subprocess.run(["git", *args], capture_output=True, text=True)
    if check and r.returncode != 0:
        sys.exit(f"git {' '.join(args)} failed: {r.stderr.strip()}")
    return r.stdout


def commits(since: str):
    """(sha, timestamp, subject, files) oldest first."""
    fmt = "%H%x1f%ct%x1f%s"
    out = git("log", f"{since}..HEAD", f"--format={fmt}", "--no-merges")
    rows = []
    for line in out.splitlines():
        if not line.strip():
            continue
        sha, ts, subj = line.split("\x1f", 2)
        files = [f for f in git("show", "--pretty=", "--name-only", sha).splitlines()
                 if f.strip()]
        rows.append((sha, int(ts), subj, files))
    return list(reversed(rows))


ADDED = re.compile(r"^@@ -\d+(?:,\d+)? \+(\d+)(?:,(\d+))? @@")


def added_lines(sha: str):
    """{(file, lineno): text} for lines this commit introduced."""
    diff = git("show", "--unified=0", "--no-color", "--format=", sha)
    out, path, lineno = {}, None, None
    for line in diff.splitlines():
        if line.startswith("+++ b/"):
            path = line[6:]
        elif line.startswith("@@"):
            m = ADDED.match(line)
            lineno = int(m.group(1)) if m else None
        elif lineno is not None and path and line.startswith("+") and not line.startswith("+++"):
            out[(path, lineno)] = line[1:]
            lineno += 1
        elif lineno is not None and line.startswith("-"):
            pass
    return out


def blame_at_head(path: str):
    """{lineno: sha} for the file as it stands now."""
    if not path or not __import__("os").path.exists(path):
        return {}
    r = subprocess.run(["git", "blame", "--line-porcelain", "HEAD", "--", path],
                       capture_output=True, text=True)
    if r.returncode != 0:
        return {}
    out, cur, n = {}, None, 0
    for line in r.stdout.splitlines():
        if line.startswith("\t"):
            n += 1
            if cur:
                out[n] = cur
        elif re.match(r"^[0-9a-f]{40} ", line):
            cur = line.split(" ")[0]
    return out


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--days", type=int, default=14)
    ap.add_argument("--since", default=None)
    a = ap.parse_args()

    since = a.since or git("rev-list", "--max-parents=0", "HEAD").split()[0]
    cs = commits(since)
    if not cs:
        print(f"No commits since {since[:7]}. Nothing to measure yet — which is itself "
              f"the finding: the denominator of every rate in this plan is zero.")
        return 0

    window = a.days * 86400
    total_lines = survived = 0
    per_commit = []
    blame_cache = {}

    for sha, ts, subj, _files in cs:
        add = added_lines(sha)
        if not add:
            continue
        total_lines += len(add)
        held = 0
        for (path, n), _text in add.items():
            b = blame_cache.setdefault(path, blame_at_head(path))
            owner = b.get(n)
            if owner == sha:
                held += 1
                continue
            if owner is None:
                held += 1          # line still present but shifted; count as held
                continue
            ots = int(git("show", "-s", "--format=%ct", owner).strip() or 0)
            if ots - ts > window:
                held += 1          # changed much later: not rework
        survived += held
        per_commit.append((sha[:8], ts, held, len(add), subj[:56]))

    rate = 100.0 * survived / total_lines if total_lines else 0.0
    print(f"{'commit':8} {'date':11} {'held':>5} {'added':>6}  subject")
    for sha, ts, held, added, subj in per_commit[-20:]:
        day = dt.datetime.fromtimestamp(ts).strftime("%Y-%m-%d")
        print(f"{sha:8} {day:11} {held:5} {added:6}  {subj}")
    print(f"\nlines introduced: {total_lines}   still holding after {a.days}d: {survived}"
          f"   survival: {rate:.1f}%")
    if total_lines < 200:
        print("note: too few lines to be a rate yet — under ~200 changed lines this is an anecdote.")
    fixups = total_lines - survived
    print(f"rework: {fixups} lines rewritten within {a.days} days")
    return 0


if __name__ == "__main__":
    sys.exit(main())

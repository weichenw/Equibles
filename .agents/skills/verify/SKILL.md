---
name: verify
description: >
  Release-verification gate for this synced fork: after pulling upstream
  changes, boot the docker stack, smoke the key features per the feature
  map, and record a verdict before trusting the new version in your
  deployment. Also interprets failing CI runs. This repo is upstream-driven
  — you do not maintain it — so this skill verifies releases, it does not
  gate your own feature work.
---

Run the CLI verbatim from the repo root with bash (no exec bit on this
machine's sandboxed copies). Never hand-roll dotnet/gh/docker commands when
a subcommand covers them; the CLI owns launch, wait, and verdict
bookkeeping so every session gets identical evidence and telemetry.

    bash .agents/skills/verify/bin/verify <subcommand>

    doctor                  Check the machine: dotnet, docker, gh, .env.
                            Exits 0 if at least one verification mode is usable.
    up [--build]            Docker stack up (db + web), wait for /healthz, print URL.
    down                    Stop the stack.
    tests <tier> [FILTER]   tier: unit | integration | functional | all
                            "all" replicates the PR gate (Category!=Functional&Category!=Live).
                            unit needs no docker; the others do.
    pr [--wait]             CI-as-verifier: report latest ci.yml + functional.yml runs
                            for the current branch. --wait blocks until they finish
                            (use when dotnet/docker are unavailable locally).
    evidence <name>         Create artifacts/verify/<ts>-<name>/ for screenshots,
                            traces, logs; prints the path on stdout.
    log <PASS|FAIL> <feature> "<note>" [evidence_dir]
                            Append the verdict to artifacts/verify/runs.jsonl.
                            EVERY verification session ends with this call.

Flow after an upstream sync:
1. `doctor` — see which mode is available (local vs CI-as-verifier).
2. Local mode: `up` → smoke the high-traffic features per FEATURE-MAP.md
   (playwright-automation skill; selectors/nav from the map) →
   `tests all` if an SDK exists locally. CI mode: `pr --wait`.
3. Judge from the evidence, not from optimism. Verify and deploy are
   separate: green gate first, THEN the deploy chore commit.
4. Always finish with `log`. A verify without a logged verdict never happened.

FEATURE-MAP.md (this dir) lists every feature's route, nav path, selectors,
and existing test classes. Consult it before driving anything manually.

If verification fails, switch to the investigate skill (reproduce, hypothesize,
prove the root cause). Verify and investigate are different skills on purpose:
one judges, one explains.
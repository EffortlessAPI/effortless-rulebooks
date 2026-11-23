# These workflows are switched off

Every workflow here belongs to the effortless-math TSP campaign of July 2026: an agent applied and
verified batches of research loops on its own `agent/tsp-*` branches by pushing `.tsp-*` trigger files.
None has changed since 2026-07-20.

All of them were disabled in GitHub on 2026-11-22 (owner: "we don't need those on each push"). Two
(`apply-tsp-loops-624-646-coherence.yml` and its `-v2`) are malformed, so GitHub recorded a failed run
for each on every push to `main`, and 89 of the repository's last 100 runs were red for that reason alone.

The files are kept as the campaign's record. To run one again: GitHub > Actions > the workflow > Enable
workflow (or `gh workflow enable <file>`). A new workflow added here is active unless it is disabled too.

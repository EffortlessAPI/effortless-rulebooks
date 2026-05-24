# Project rules — Naked Claude run

You are writing CODE ONLY. No Effortless skills, no rulebook, no Airtable
modifications.

## Hard rules
- DO NOT USE EFFORTLESS SKILLS at all.
- DO NOT modify Airtable in any way (no REST API writes, no OMNI, no
  Playwright). Treat the Airtable schema as read-only — the project owner
  changes it externally.
- DO NOT modify the rulebook (`effortless-rulebook.json`) or any generated
  files.
- DO NOT run `effortless build` or any pipeline transpiler.
- DO NOT leave background processes running. No `npm run dev`, no `vite`
  dev server left alive, no `node server.js &`.

## Required deliverables
- Source code for the app, written into the current directory.
- A `start.sh` at the repo root that, when run, brings up the app
  end-to-end (install deps if needed, run any DB migrations against the
  local `2026-04-20-1700-naked` postgres on localhost, start the
  backend, start the frontend build/dev server in the foreground or via
  clearly-tagged background processes that the script can also stop).
  The grader will run `./start.sh` to test the app — make it just work.

## When you are done
- Stop. Exit. Do not leave dev servers running.

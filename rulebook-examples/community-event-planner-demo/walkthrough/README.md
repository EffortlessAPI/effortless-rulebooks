# Community Event Planner — Walkthrough Plan

A scripted, narrated Playwright walkthrough of the demo app. The video is **generated** by `record-walkthrough.mjs` (Playwright drives a real browser, every click is real) and the voiceover is **generated** by `add-narration.mjs` (macOS `say` → AIFF → mixed onto the video with FFmpeg).

If something in this README disagrees with the script, the script wins — re-read it before changing the plan.

## What the app actually does

Six tables in the rulebook:

| Table       | Records | Key calculated/aggregated fields |
|-------------|---------|----------------------------------|
| Venues      | 3       | (just raw: capacity, location) |
| Speakers    | 5       | `AssignmentCount` (AGG), `IsOverbooked` (>3 assignments) |
| Events      | 4       | `TotalSpeakersAssigned`, `HasSpeakers`, `BookedCapacity`, `AvailableCapacity`, `AtCapacity`, `VenueConflictCount`, `HasVenueConflict`, `EventStatus`, `RegistrationDeadline`, `IsRegistrationOpen`, `CapacityHeadroom`, `RecommendedOverbookingFactor` |
| Assignments | 10      | `IsAvailable` (speaker available on event date?) |
| Attendees   | 5       | — |
| RSVPs       | 7       | — |

The headline calculated fields, in DAG order:

1. **`EventStatus`** — `IF(AND(HasSpeakers, NOT(AtCapacity), NOT(HasVenueConflict)), "ready", "issues")` — composite gate.
2. **`AvailableCapacity`** — `VenueCapacity (lookup) - BookedCapacity (agg over RSVPs)`.
3. **`HasVenueConflict`** — `VenueConflictCount > 1` (two events booked into same venue).
4. **`RegistrationDeadline`** — `EventDate - RegistrationCloseDaysBeforeEvent` (the "days before event" UI dropdown drives this).
5. **`IsRegistrationOpen`** — `RegistrationDeadline > NOW()`.
6. **`IsOverbooked`** — `AssignmentCount > 3`.

These are the things the walkthrough must surface; everything else is supporting context.

## UI surface

Routes the React app exposes:

- `/login` — picks one of 4 dev users (Admin / Organizer / Speaker / Attendee). Sets `X-User-Email` header.
- `/` — Dashboard: 4 stat boxes + event list. Each stat & metric is a `<DagCell>` (clickable when DAG mode is on).
- `/events/:id` — Event detail: status breakdown, speakers w/ availability, attendees, category, registration, **edit button** (admin only) → modal with Registration-Closes dropdown.
- `/speakers` — Speaker workload: overbooked warning section + available speakers w/ assignment counts.
- `/dag/:table/:field` — Explainer DAG for any calculated field.

The header has a **DAG Toggle** ("Explain mode") which is the *whole point of the demo* — when on, every derived value becomes a clickable link to its dependency graph.

## Walkthrough script (what each "click" actually does)

Total runtime target: ~2.5 minutes. 1280×800.

| # | Time   | Action                                                        | Why we show it                                                                                                  |
|---|--------|---------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| 1 | 0–6s   | Land on `/login`, hover the four role cards                   | Establishes the auth model: 4 dev roles, no real auth, header-based identity                                    |
| 2 | 6–12s  | Click **Admin** button → land on dashboard                    | Admin role is what unlocks the edit button later                                                                |
| 3 | 12–22s | Pause on dashboard, point at the four stat boxes              | These are *aggregations over Events* — `Total Events`, `Ready to Publish`, `Total Attendees`, `With Issues`     |
| 4 | 22–32s | Point at the event list — `meetup-1`, `tech-talk-1`, etc.     | Each row shows `TotalSpeakersAssigned`, `BookedCapacity`, `AvailableCapacity`, `EventStatus`, venue conflict ⚠️ |
| 5 | 32–40s | Click the **DAG Toggle** in the header                        | The Effortless headline: toggle "explain mode" — all derived values become clickable links to their DAG          |
| 6 | 40–52s | Click the ƒ glyph on the **Ready to Publish** stat box        | Opens the DAG for `Events.EventStatus` — shows the IF/AND tree of HasSpeakers + AtCapacity + HasVenueConflict. We click the stat box rather than an event row because event rows have a parent onClick that would steal the click. |
| 7 | 52–62s | Back to dashboard, click ƒ on **Total Attendees** stat box     | Opens `Events.BookedCapacity` — an aggregation over RSVPs filtered to confirmed status; great example of "this number was counted, not stored" |
| 8 | 62–70s | Back to dashboard, click on `tech-talk-1` row                 | Drill into event detail                                                                                          |
| 9 | 70–82s | Pan over the event status block (✓/✗ for speakers / capacity / venue conflict) | Shows the *components* of `EventStatus` as a checklist |
| 10| 82–92s | Scroll to Registration block, point at deadline + Open/Closed | This is the `RegistrationDeadline` and `IsRegistrationOpen` calc — driven by the days-before-event setting       |
| 11| 92–104s| Click the ✏️ edit button → modal opens                        | Admin-only edit modal                                                                                            |
| 12|104–116s| Open the "Registration Closes" dropdown, change selection      | Changes `RegistrationCloseDaysBeforeEvent` → cascades to `RegistrationDeadline` → cascades to `IsRegistrationOpen` |
| 13|116–122s| Click **Save** → modal closes → registration block updates    | Shows the rulebook-driven recompute round-tripping through Postgres → API → UI                                  |
| 14|122–130s| Navigate to **Speakers** tab in header                        | Shows the `Speakers` table's calculated fields                                                                  |
| 15|130–140s| Point at "Available Speakers" w/ assignment counts            | `AssignmentCount` is an aggregation over Assignments                                                            |
| 16|140–150s| Wrap-up: highlight that *every number we showed* came from the rulebook, not the UI | The core message of the demo                       |

## What we are NOT showing (intentionally cut)

- Venue detail page (only linked indirectly).
- RSVP create/cancel flow (the demo has no UI for it yet).
- Speaker login (the role exists but the schedule is the only differentiated page).
- The `Attendees` / `Assignments` / `RSVPs` raw tables (only seen through their effects on Events/Speakers).

## File layout

```
walkthrough/
├─ README.md                 ← this file (the plan)
├─ record-walkthrough.mjs    ← Playwright recorder — drives the clicks
├─ add-narration.mjs         ← generates voiceover with `say`, muxes onto video
├─ narration.txt             ← human-readable script with timestamps
├─ narration.srt             ← subtitle file (kept for accessibility)
├─ walkthrough.mp4           ← FINAL OUTPUT — video + voiceover
└─ screenshots/              ← per-step PNG snapshots (regenerated each run)
```

## How to regenerate

```bash
# 1. Make sure dev servers are running (server on :3045, web on :5188)
cd server && npm run dev &
cd web    && npm run dev &

# 2. Record the navigation (writes screenshots/, walkthrough-silent.webm)
node walkthrough/record-walkthrough.mjs

# 3. Add voiceover (writes walkthrough.mp4)
node walkthrough/add-narration.mjs
```

Output `walkthrough/walkthrough.mp4` is the artifact to share.

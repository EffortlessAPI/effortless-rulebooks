# 📘 Therapist Helper Portal — Docs

_Sessions and treatment progress: GoalUpdate → Goal.ProgressPct → Client.IsAtRisk three-hop DAG._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **User** | Application users. Therapists, supervisors, and clients log in via dev-login by user id (email). | — |
| Full Name | A defined attribute. | — |
| Role | A defined attribute. | _therapist \| supervisor \| client_ |
| Name | The same as its users ID. | — |
| Client Count | The number of clients related to the user. | — |

<details open>
<summary>Example data</summary>

| Full Name | Role | Name ƒ | Client Count ƒ |
|---|---|---|---|
| Dr. Tess Brennan | therapist | tess@example.com | 2 |
| Dr. Rob Singh | therapist | rob@example.com | 2 |
| Dr. Sue Chen (Supervisor) | supervisor | sue@example.com | 0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Client** | People in treatment. Each has an assigned therapist, a set of goals, and a stream of sessions. | — |
| Client Name | A defined attribute. | — |
| Therapist | A defined attribute. | _FK to Users.UsersId._ |
| Therapist Name | The full name of the client's therapist. | — |
| Start Date | A defined attribute. | — |
| Name | The same as its clients ID. | — |
| Session Count | The number of sessions related to the client. | — |
| Avg Mood Rating | The average mood rating across the sessions related to the client. | — |
| Goal Count | The number of goals related to the client. | — |
| Avg Goal Progress | The average progress pct across the goals related to the client. | _2nd-order: aggregates Goal.ProgressPct (a calc)._ |
| Last Session Label | The largest session label across the sessions related to the client. | _Most recent session label for this client._ |
| Is At Risk | True when at least one of the following holds: the avg mood rating is less than 5 or the avg goal progress is less than 50. | _3rd-order: low mood OR low avg goal progress._ |
| Status Label | Determined by priority: “At risk” if the at risk flag is set; in all other cases, “On track”. | _4th-order: human-readable client status derived from IsAtRisk._ |

<details open>
<summary>Example data</summary>

| Client Name | Therapist Name ƒ | Start Date | Name ƒ | Session Count ƒ | Avg Mood Rating ƒ | Goal Count ƒ | Avg Goal Progress ƒ | Last Session Label ƒ | Is At Risk ƒ | Status Label ƒ |
|---|---|---|---|---|---|---|---|---|---|---|
| Alex Rivera | Dr. Tess Brennan | 2026-01-15 | alex-r | 3 | 7.333333333333333 | 2 | 80.35714285714286 | 2026-04-16 am | false | On track |
| Blair Morgan | Dr. Tess Brennan | 2026-02-03 | blair-m | 3 | 4.0 | 2 | 49.99999999999999 | 2026-04-17 pm | true | At risk |
| Casey Lin | Dr. Rob Singh | 2025-11-22 | casey-l | 2 | 6.5 | 2 | 67.85714285714286 | 2026-04-11 am | false | On track |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Goal** | Treatment goals owned by a client. TargetScore is editable. | — |
| Title | A defined attribute. | — |
| Client | A defined attribute. | — |
| Client Name | Taken from the linked client. | — |
| Client Therapist | Taken from the linked client. | — |
| Target Score | A defined attribute. | _0-10. EDITABLE._ |
| Name | The same as its goals ID. | — |
| Update Count | The number of goal updates related to the goal. | — |
| Avg Score Achieved | The average score achieved across the goal updates related to the goal. | — |
| Latest Score | The largest score achieved across the goal updates related to the goal. | _Best score recorded against this goal._ |
| Progress Pct | Computed as the avg score achieved divided by the target score times 100 (or 0 if that can't be computed). | _2nd-order._ |
| Remaining Gap | Computed as the largest of 0 and the target score minus the avg score achieved. | _2nd-order: distance to target._ |
| Is on Track | True when the progress pct is at least 70. | _3rd-order._ |

<details open>
<summary>Example data</summary>

| Title | Client Name ƒ | Client Therapist ƒ | Target Score | Name ƒ | Update Count ƒ | Avg Score Achieved ƒ | Latest Score ƒ | Progress Pct ƒ | Remaining Gap ƒ | Is on Track ƒ |
|---|---|---|---|---|---|---|---|---|---|---|
| Reduce anxiety in social settings | Alex Rivera | tess@example.com | 8 | alex-anxiety | 3 | 6.0 | 7.0 | 75.0 | 2.0 | true |
| Improve sleep quality | Alex Rivera | tess@example.com | 7 | alex-sleep | 2 | 6.0 | 6.0 | 85.71428571428571 | 1.0 | true |
| Stabilize daily mood | Blair Morgan | tess@example.com | 8 | blair-mood | 3 | 2.6666666666666665 | 3.0 | 33.33333333333333 | 5.333333333333334 | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Session** | Therapy sessions. MoodRating is editable and feeds the client-level rollup. | — |
| Client | A defined attribute. | — |
| Client Name | Taken from the linked client. | — |
| Client Therapist | Taken from the linked client. | — |
| Session Label | A defined attribute. | — |
| Duration Minutes | A defined attribute. | — |
| Mood Rating | A defined attribute. | _1-10. EDITABLE._ |
| Notes | A defined attribute. | — |
| Name | The same as its sessions ID. | — |
| Update Count | The number of goal updates related to the session. | _Goal updates recorded this session._ |
| Avg Score Achieved | The average score achieved across the goal updates related to the session. | _Average update score for this session._ |
| Is Productive | True when all of the following hold: the update count is at least 2 and the avg score achieved is at least 5. | _2nd-order: session captured >=2 goal updates AND avg score >=5._ |
| Status Label | Determined by priority: “Productive” if the productive flag is set; in all other cases, “Light”. | _3rd-order: human-readable session status derived from IsProductive._ |

<details open>
<summary>Example data</summary>

| Client Name ƒ | Client Therapist ƒ | Session Label | Duration Minutes | Mood Rating | Notes | Name ƒ | Update Count ƒ | Avg Score Achieved ƒ | Is Productive ƒ | Status Label ƒ |
|---|---|---|---|---|---|---|---|---|---|---|
| Alex Rivera | tess@example.com | 2026-04-02 am | 50 | 7 | Good engagement. | ses-001 | 1 | 5.0 | false | Light |
| Alex Rivera | tess@example.com | 2026-04-09 am | 50 | 8 | Reported better sleep. | ses-002 | 2 | 6.0 | true | Productive |
| Alex Rivera | tess@example.com | 2026-04-16 am | 50 | 7 | — | ses-003 | 2 | 6.5 | true | Productive |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Goal Update** | Per-session goal progress entries. ScoreAchieved is editable. | — |
| Goal | A defined attribute. | — |
| Goal Title | Taken from the linked goal. | — |
| Goal Client | Taken from the linked goal. | — |
| Goal Target Score | Taken from the linked goal. | — |
| Goal Client Therapist | Taken from the linked goal. | — |
| Session | A defined attribute. | — |
| Session Label | Taken from the linked session. | — |
| Score Achieved | A defined attribute. | _0-10. EDITABLE._ |
| Name | The same as its goal updates ID. | — |

<details open>
<summary>Example data</summary>

| Goal Title ƒ | Goal Client ƒ | Goal Target Score ƒ | Goal Client Therapist ƒ | Session Label ƒ | Score Achieved | Name ƒ |
|---|---|---|---|---|---|---|
| Reduce anxiety in social settings | alex-r | 8.0 | tess@example.com | 2026-04-02 am | 5 | upd-001 |
| Reduce anxiety in social settings | alex-r | 8.0 | tess@example.com | 2026-04-09 am | 6 | upd-002 |
| Reduce anxiety in social settings | alex-r | 8.0 | tess@example.com | 2026-04-16 am | 7 | upd-003 |

_ƒ marks a computed column._

</details>

## 2 Fact Types

- a **client** references exactly one **user**
- a **goal** references exactly one **client**
- a **session** references exactly one **client**
- a **goal update** references exactly one **goal**
- a **goal update** references exactly one **session**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A user **must** have a full name and a role.
- A client **must** reference exactly one user as its therapist.
- A client **must** have a client name.
- A goal **must** reference exactly one client.
- A goal **must** have a title and a target score.
- A session **must** reference exactly one client.
- A session **must** have a session label and a mood rating.
- A goal update **must** reference exactly one goal.
- A goal update **must** reference exactly one session.
- A goal update **must** have a score achieved.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A user's name is the same as its users ID. |
| **DR-2 Client Count** | A user's client count is the number of clients related to the user. |
| **DR-3 Therapist Name** | A client's therapist name is the full name of the client's therapist. |
| **DR-4 Name** | A client's name is the same as its clients ID. |
| **DR-5 Session Count** | A client's session count is the number of sessions related to the client. |
| **DR-6 Avg Mood Rating** | A client's avg mood rating is the average mood rating across the sessions related to the client. |
| **DR-7 Goal Count** | A client's goal count is the number of goals related to the client. |
| **DR-8 Avg Goal Progress** | A client's avg goal progress is the average progress pct across the goals related to the client. |
| **DR-9 Last Session Label** | A client's last session label is the largest session label across the sessions related to the client. |
| **DR-10 Is At Risk** | A client is considered at-risk if at least one of the following holds: the avg mood rating is less than 5 or the avg goal progress is less than 50. |
| **DR-11 Status Label** | The client's status label is determined by the following priority:<br>1. “At risk”, if the at risk flag is set;<br>2. in all other cases, “On track”. |
| **DR-12 Client Name** | A goal's client name — taken from the linked client. |
| **DR-13 Client Therapist** | A goal's client therapist — taken from the linked client. |
| **DR-14 Name** | A goal's name is the same as its goals ID. |
| **DR-15 Update Count** | A goal's update count is the number of goal updates related to the goal. |
| **DR-16 Avg Score Achieved** | A goal's avg score achieved is the average score achieved across the goal updates related to the goal. |
| **DR-17 Latest Score** | A goal's latest score is the largest score achieved across the goal updates related to the goal. |
| **DR-18 Progress Pct** | A goal's progress pct is computed as the avg score achieved divided by the target score times 100 (or 0 if that can't be computed). |
| **DR-19 Remaining Gap** | A goal's remaining gap is computed as the largest of 0 and the target score minus the avg score achieved. |
| **DR-20 Is on Track** | A goal is considered on-track if the progress pct is at least 70. |
| **DR-21 Client Name** | A session's client name — taken from the linked client. |
| **DR-22 Client Therapist** | A session's client therapist — taken from the linked client. |
| **DR-23 Name** | A session's name is the same as its sessions ID. |
| **DR-24 Update Count** | A session's update count is the number of goal updates related to the session. |
| **DR-25 Avg Score Achieved** | A session's avg score achieved is the average score achieved across the goal updates related to the session. |
| **DR-26 Is Productive** | A session is considered productive if all of the following hold: the update count is at least 2 and the avg score achieved is at least 5. |
| **DR-27 Status Label** | The session's status label is determined by the following priority:<br>1. “Productive”, if the productive flag is set;<br>2. in all other cases, “Light”. |
| **DR-28 Goal Title** | A goal update's goal title — taken from the linked goal. |
| **DR-29 Goal Client** | A goal update's goal client — taken from the linked goal. |
| **DR-30 Goal Target Score** | A goal update's goal target score — taken from the linked goal. |
| **DR-31 Goal Client Therapist** | A goal update's goal client therapist — taken from the linked goal. |
| **DR-32 Session Label** | A goal update's session label — taken from the linked session. |
| **DR-33 Name** | A goal update's name is the same as its goal updates ID. |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Users.Name** | formula | `UsersId` |
| **Users.ClientCount** | rollup | `Count(Clients via Therapist)` |
| **Clients.TherapistName** | lookup | `Lookup(Users.FullName via Therapist)` |
| **Clients.Name** | formula | `ClientsId` |
| **Clients.SessionCount** | rollup | `Count(Sessions via Client)` |
| **Clients.AvgMoodRating** | rollup | `Average(Sessions.MoodRating via Client)` |
| **Clients.GoalCount** | rollup | `Count(Goals via Client)` |
| **Clients.AvgGoalProgress** | rollup | `Average(Goals.ProgressPct via Client)` |
| **Clients.LastSessionLabel** | rollup | `Max(Sessions.SessionLabel via Client)` |
| **Clients.IsAtRisk** | formula | `Or(AvgMoodRating < 5, AvgGoalProgress < 50)` |
| **Clients.StatusLabel** | formula | `If(IsAtRisk, "At risk", "On track")` |
| **Goals.ClientName** | lookup | `Lookup(Clients.ClientName via Client)` |
| **Goals.ClientTherapist** | lookup | `Lookup(Clients.Therapist via Client)` |
| **Goals.Name** | formula | `GoalsId` |
| **Goals.UpdateCount** | rollup | `Count(GoalUpdates via Goal)` |
| **Goals.AvgScoreAchieved** | rollup | `Average(GoalUpdates.ScoreAchieved via Goal)` |
| **Goals.LatestScore** | rollup | `Max(GoalUpdates.ScoreAchieved via Goal)` |
| **Goals.ProgressPct** | formula | `Iferror(AvgScoreAchieved / TargetScore * 100, 0)` |
| **Goals.RemainingGap** | formula | `Max(0, TargetScore - AvgScoreAchieved)` |
| **Goals.IsOnTrack** | formula | `ProgressPct >= 70` |
| **Sessions.ClientName** | lookup | `Lookup(Clients.ClientName via Client)` |
| **Sessions.ClientTherapist** | lookup | `Lookup(Clients.Therapist via Client)` |
| **Sessions.Name** | formula | `SessionsId` |
| **Sessions.UpdateCount** | rollup | `Count(GoalUpdates via Session)` |
| **Sessions.AvgScoreAchieved** | rollup | `Average(GoalUpdates.ScoreAchieved via Session)` |
| **Sessions.IsProductive** | formula | `And(UpdateCount >= 2, AvgScoreAchieved >= 5)` |
| **Sessions.StatusLabel** | formula | `If(IsProductive, "Productive", "Light")` |
| **GoalUpdates.GoalTitle** | lookup | `Lookup(Goals.Title via Goal)` |
| **GoalUpdates.GoalClient** | lookup | `Lookup(Goals.Client via Goal)` |
| **GoalUpdates.GoalTargetScore** | lookup | `Lookup(Goals.TargetScore via Goal)` |
| **GoalUpdates.GoalClientTherapist** | lookup | `Lookup(Goals.ClientTherapist via Goal)` |
| **GoalUpdates.SessionLabel** | lookup | `Lookup(Sessions.SessionLabel via Session)` |
| **GoalUpdates.Name** | formula | `GoalUpdatesId` |

---

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)

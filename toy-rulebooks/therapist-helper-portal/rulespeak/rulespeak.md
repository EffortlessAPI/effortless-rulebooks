# 📘 Therapist Helper Portal — RuleSpeak

_Sessions and treatment progress: GoalUpdate → Goal.ProgressPct → Client.IsAtRisk three-hop DAG._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **User** | Application users. Therapists, supervisors, and clients log in via dev-login by user id (email). |
| Client Count | Computed as `Countif(Clients.Therapist, Users.UsersId)`. |
| **Client** | People in treatment. Each has an assigned therapist, a set of goals, and a stream of sessions. |
| Therapist Name | The full name of the client's therapist. |
| Session Count | Computed as `Countif(Sessions.Client, Clients.ClientsId)`. |
| Avg Mood Rating | The average mood rating across the client's sessions. |
| Goal Count | Computed as `Countif(Goals.Client, Clients.ClientsId)`. |
| Avg Goal Progress | 2nd-order: aggregates Goal.ProgressPct (a calc). |
| Last Session Label | Most recent session label for this client. |
| Is At Risk | 3rd-order: low mood OR low avg goal progress. |
| Status Label | 4th-order: human-readable client status derived from IsAtRisk. |
| **Goal** | Treatment goals owned by a client. TargetScore is editable. |
| Client Name | The client name of the goal's client. |
| Client Therapist | The therapist of the goal's client. |
| Update Count | Computed as `Countif(GoalUpdates.Goal, Goals.GoalsId)`. |
| Avg Score Achieved | The average score achieved across the goal's goal updates. |
| Latest Score | Best score recorded against this goal. |
| Progress Pct | 2nd-order. |
| Remaining Gap | 2nd-order: distance to target. |
| Is on Track | 3rd-order. |
| **Session** | Therapy sessions. MoodRating is editable and feeds the client-level rollup. |
| Client Name | The client name of the session's client. |
| Client Therapist | The therapist of the session's client. |
| Update Count | Goal updates recorded this session. |
| Avg Score Achieved | Average update score for this session. |
| Is Productive | 2nd-order: session captured >=2 goal updates AND avg score >=5. |
| Status Label | 3rd-order: human-readable session status derived from IsProductive. |
| **Goal Update** | Per-session goal progress entries. ScoreAchieved is editable. |
| Goal Title | The title of the goal update's goal. |
| Goal Client | The client of the goal update's goal. |
| Goal Target Score | The target score of the goal update's goal. |
| Goal Client Therapist | The client therapist of the goal update's goal. |
| Session Label | The session label of the goal update's session. |

## 2 Fact Types

- a **client** references exactly one **user**
- a **goal** references exactly one **client**
- a **session** references exactly one **client**
- a **goal update** references exactly one **goal**
- a **goal update** references exactly one **session**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Client Count** | A user's client count is rolled up from its related records (`Countif(Clients.Therapist, Users.UsersId)`). |
| **DR-2 Therapist Name** | A client's therapist name is the full name of the client's therapist. |
| **DR-3 Session Count** | A client's session count is rolled up from its related records (`Countif(Sessions.Client, Clients.ClientsId)`). |
| **DR-4 Avg Mood Rating** | A client's avg mood rating is the average mood rating across the sessions related to the client. |
| **DR-5 Goal Count** | A client's goal count is rolled up from its related records (`Countif(Goals.Client, Clients.ClientsId)`). |
| **DR-6 Avg Goal Progress** | A client's avg goal progress is the average progress pct across the goals related to the client. |
| **DR-7 Last Session Label** | A client's last session label is rolled up from its related records (`Maxifs(Sessions.SessionLabel, Sessions.Client, Clients.ClientsId)`). |
| **DR-8 Is At Risk** | A client is considered at risk if at least one of the following holds: the avg mood rating is less than 5 or the avg goal progress is less than 50. |
| **DR-9 Status Label** | The client's status label is determined by the following priority:<br>1. the literal “At risk”, if the is at risk flag is set;<br>2. otherwise the literal “On track”. |
| **DR-10 Client Name** | A goal's client name is the client name of the goal's client. |
| **DR-11 Client Therapist** | A goal's client therapist is the therapist of the goal's client. |
| **DR-12 Update Count** | A goal's update count is rolled up from its related records (`Countif(GoalUpdates.Goal, Goals.GoalsId)`). |
| **DR-13 Avg Score Achieved** | A goal's avg score achieved is the average score achieved across the goal updates related to the goal. |
| **DR-14 Latest Score** | A goal's latest score is rolled up from its related records (`Maxifs(GoalUpdates.ScoreAchieved, GoalUpdates.Goal, Goals.GoalsId)`). |
| **DR-15 Progress Pct** | A goal's progress pct is computed as `Iferror(AvgScoreAchieved / TargetScore * 100, 0)`. |
| **DR-16 Remaining Gap** | A goal's remaining gap is computed as `Max(0, TargetScore - AvgScoreAchieved)`. |
| **DR-17 Is on Track** | A goal is considered on track if the progress pct is at least 70. |
| **DR-18 Client Name** | A session's client name is the client name of the session's client. |
| **DR-19 Client Therapist** | A session's client therapist is the therapist of the session's client. |
| **DR-20 Update Count** | A session's update count is the number of goal updates related to the session. |
| **DR-21 Avg Score Achieved** | A session's avg score achieved is the average score achieved across the goal updates related to the session. |
| **DR-22 Is Productive** | A session is considered a productive if all of the following hold: the update count is at least 2 and the avg score achieved is at least 5. |
| **DR-23 Status Label** | The session's status label is determined by the following priority:<br>1. the literal “Productive”, if the is productive flag is set;<br>2. otherwise the literal “Light”. |
| **DR-24 Goal Title** | A goal update's goal title is the title of the goal update's goal. |
| **DR-25 Goal Client** | A goal update's goal client is the client of the goal update's goal. |
| **DR-26 Goal Target Score** | A goal update's goal target score is the target score of the goal update's goal. |
| **DR-27 Goal Client Therapist** | A goal update's goal client therapist is the client therapist of the goal update's goal. |
| **DR-28 Session Label** | A goal update's session label is the session label of the goal update's session. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Users.ClientCount** | rollup | `Countif(Clients.Therapist, Users.UsersId)` |
| **Clients.TherapistName** | lookup | `Lookup(Users.FullName via Therapist)` |
| **Clients.SessionCount** | rollup | `Countif(Sessions.Client, Clients.ClientsId)` |
| **Clients.AvgMoodRating** | rollup | `Average(Sessions.MoodRating via Client)` |
| **Clients.GoalCount** | rollup | `Countif(Goals.Client, Clients.ClientsId)` |
| **Clients.AvgGoalProgress** | rollup | `Average(Goals.ProgressPct via Client)` |
| **Clients.LastSessionLabel** | rollup | `Maxifs(Sessions.SessionLabel, Sessions.Client, Clients.ClientsId)` |
| **Clients.IsAtRisk** | formula | `Or(AvgMoodRating < 5, AvgGoalProgress < 50)` |
| **Clients.StatusLabel** | formula | `If(IsAtRisk, "At risk", "On track")` |
| **Goals.ClientName** | lookup | `Lookup(Clients.ClientName via Client)` |
| **Goals.ClientTherapist** | lookup | `Lookup(Clients.Therapist via Client)` |
| **Goals.UpdateCount** | rollup | `Countif(GoalUpdates.Goal, Goals.GoalsId)` |
| **Goals.AvgScoreAchieved** | rollup | `Average(GoalUpdates.ScoreAchieved via Goal)` |
| **Goals.LatestScore** | rollup | `Maxifs(GoalUpdates.ScoreAchieved, GoalUpdates.Goal, Goals.GoalsId)` |
| **Goals.ProgressPct** | formula | `Iferror(AvgScoreAchieved / TargetScore * 100, 0)` |
| **Goals.RemainingGap** | formula | `Max(0, TargetScore - AvgScoreAchieved)` |
| **Goals.IsOnTrack** | formula | `ProgressPct >= 70` |
| **Sessions.ClientName** | lookup | `Lookup(Clients.ClientName via Client)` |
| **Sessions.ClientTherapist** | lookup | `Lookup(Clients.Therapist via Client)` |
| **Sessions.UpdateCount** | rollup | `Count(GoalUpdates via Session)` |
| **Sessions.AvgScoreAchieved** | rollup | `Average(GoalUpdates.ScoreAchieved via Session)` |
| **Sessions.IsProductive** | formula | `And(UpdateCount >= 2, AvgScoreAchieved >= 5)` |
| **Sessions.StatusLabel** | formula | `If(IsProductive, "Productive", "Light")` |
| **GoalUpdates.GoalTitle** | lookup | `Lookup(Goals.Title via Goal)` |
| **GoalUpdates.GoalClient** | lookup | `Lookup(Goals.Client via Goal)` |
| **GoalUpdates.GoalTargetScore** | lookup | `Lookup(Goals.TargetScore via Goal)` |
| **GoalUpdates.GoalClientTherapist** | lookup | `Lookup(Goals.ClientTherapist via Goal)` |
| **GoalUpdates.SessionLabel** | lookup | `Lookup(Sessions.SessionLabel via Session)` |

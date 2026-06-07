# 📘 Fantasy Football — RuleSpeak

_Multi-hop DAG: raw player stats → roster aggregations → matchup scoring → standings & seeding._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Dev User** | A dev user tracked by the business. |
| **Player** | A player tracked by the business. |
| Total Touchdowns | Sum of all TDs |
| Projected Score | Calculated fantasy points |
| **Roster** | A roster tracked by the business. |
| Total Passing Yards | Sum of QB passing yards on roster |
| Total Rushing Yards | Sum of rushing yards on roster |
| Total Receiving Yards | Sum of receiving yards on roster |
| Total Touchdowns | Sum of all TDs on roster |
| Total Interceptions | Sum of INTs on roster |
| Projected Score | Total roster fantasy points (2nd-order calc) |
| **Roster Assignment** | A roster assignment tracked by the business. |
| Player Name | Player display name |
| Position | Player position |
| Passing Yards | Player passing yards |
| Passing Touchdowns | Player passing TDs |
| Interceptions | Player INTs |
| Rushing Yards | Player rushing yards |
| Rushing Touchdowns | Player rushing TDs |
| Receptions | Player receptions |
| Received Yards | Player receiving yards |
| Received Touchdowns | Player receiving TDs |
| Total Touchdowns | Player total TDs |
| Projected Score | Player projected score |
| **Matchup** | A matchup tracked by the business. |
| Team1 Score | Team1's projected score |
| Team2 Score | Team2's projected score |
| Margin | Team1 score minus Team2 score |
| Winner | Winning roster ID |
| Loser | Losing roster ID |
| Is Tie Game | True if scores are equal |
| **Standing** | A standing tracked by the business. |
| Owner | Owner email |

## 2 Fact Types

- a **roster assignment** references exactly one **roster**
- a **roster assignment** references exactly one **player**
- a **matchup** references exactly one **roster**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Total Touchdowns** | A player's total touchdowns is computed as `PassingTouchdowns + RushingTouchdowns + ReceivedTouchdowns`. |
| **DR-2 Projected Score** | A player's projected score is computed as `Round((PassingYards / 25) + (TotalTouchdowns * 6) - (Interceptions * 2), 2)`. |
| **DR-3 Total Passing Yards** | A roster's total passing yards is the total passing yards across the roster assignments related to the roster. |
| **DR-4 Total Rushing Yards** | A roster's total rushing yards is the total rushing yards across the roster assignments related to the roster. |
| **DR-5 Total Receiving Yards** | A roster's total receiving yards is the total received yards across the roster assignments related to the roster. |
| **DR-6 Total Touchdowns** | A roster's total touchdowns is the total total touchdowns across the roster assignments related to the roster. |
| **DR-7 Total Interceptions** | A roster's total interceptions is the total interceptions across the roster assignments related to the roster. |
| **DR-8 Projected Score** | A roster's projected score is computed as `Round((TotalPassingYards / 25) + (TotalRushingYards / 10) + (TotalReceivingYards / 10) + (TotalTouchdowns * 6) - (TotalInterceptions * 2), 2)`. |
| **DR-9 Player Name** | A roster assignment's player name is the name of the roster assignment's player. |
| **DR-10 Position** | A roster assignment's position is the position of the roster assignment's player. |
| **DR-11 Passing Yards** | A roster assignment's passing yards is the passing yards of the roster assignment's player. |
| **DR-12 Passing Touchdowns** | A roster assignment's passing touchdowns is the passing touchdowns of the roster assignment's player. |
| **DR-13 Interceptions** | A roster assignment's interceptions is the interceptions of the roster assignment's player. |
| **DR-14 Rushing Yards** | A roster assignment's rushing yards is the rushing yards of the roster assignment's player. |
| **DR-15 Rushing Touchdowns** | A roster assignment's rushing touchdowns is the rushing touchdowns of the roster assignment's player. |
| **DR-16 Receptions** | A roster assignment's receptions is the receptions of the roster assignment's player. |
| **DR-17 Received Yards** | A roster assignment's received yards is the received yards of the roster assignment's player. |
| **DR-18 Received Touchdowns** | A roster assignment's received touchdowns is the received touchdowns of the roster assignment's player. |
| **DR-19 Total Touchdowns** | A roster assignment's total touchdowns is the total touchdowns of the roster assignment's player. |
| **DR-20 Projected Score** | A roster assignment's projected score is the projected score of the roster assignment's player. |
| **DR-21 Team1 Score** | A matchup's team1 score is the projected score of the matchup's team1. |
| **DR-22 Team2 Score** | A matchup's team2 score is the projected score of the matchup's team2. |
| **DR-23 Margin** | A matchup's margin is computed as `Team1Score - Team2Score`. |
| **DR-24 Winner** | The matchup's winner is determined by the following priority:<br>1. the team1, if the margin is greater than 0;<br>2. otherwise the team2. |
| **DR-25 Loser** | The matchup's loser is determined by the following priority:<br>1. the team2, if the margin is greater than 0;<br>2. otherwise the team1. |
| **DR-26 Is Tie Game** | A matchup is considered a tie game if the margin is 0. |
| **DR-27 Owner** | A standing's owner is the owner of the standing's standing ID. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Players.TotalTouchdowns** | formula | `PassingTouchdowns + RushingTouchdowns + ReceivedTouchdowns` |
| **Players.ProjectedScore** | formula | `Round((PassingYards / 25) + (TotalTouchdowns * 6) - (Interceptions * 2), 2)` |
| **Rosters.TotalPassingYards** | rollup | `Sum(RosterAssignments.PassingYards via Roster)` |
| **Rosters.TotalRushingYards** | rollup | `Sum(RosterAssignments.RushingYards via Roster)` |
| **Rosters.TotalReceivingYards** | rollup | `Sum(RosterAssignments.ReceivedYards via Roster)` |
| **Rosters.TotalTouchdowns** | rollup | `Sum(RosterAssignments.TotalTouchdowns via Roster)` |
| **Rosters.TotalInterceptions** | rollup | `Sum(RosterAssignments.Interceptions via Roster)` |
| **Rosters.ProjectedScore** | formula | `Round((TotalPassingYards / 25) + (TotalRushingYards / 10) + (TotalReceivingYards / 10) + (TotalTouchdowns * 6) - (TotalInterceptions * 2), 2)` |
| **RosterAssignments.PlayerName** | lookup | `Lookup(Players.Name via Player)` |
| **RosterAssignments.Position** | lookup | `Lookup(Players.Position via Player)` |
| **RosterAssignments.PassingYards** | lookup | `Lookup(Players.PassingYards via Player)` |
| **RosterAssignments.PassingTouchdowns** | lookup | `Lookup(Players.PassingTouchdowns via Player)` |
| **RosterAssignments.Interceptions** | lookup | `Lookup(Players.Interceptions via Player)` |
| **RosterAssignments.RushingYards** | lookup | `Lookup(Players.RushingYards via Player)` |
| **RosterAssignments.RushingTouchdowns** | lookup | `Lookup(Players.RushingTouchdowns via Player)` |
| **RosterAssignments.Receptions** | lookup | `Lookup(Players.Receptions via Player)` |
| **RosterAssignments.ReceivedYards** | lookup | `Lookup(Players.ReceivedYards via Player)` |
| **RosterAssignments.ReceivedTouchdowns** | lookup | `Lookup(Players.ReceivedTouchdowns via Player)` |
| **RosterAssignments.TotalTouchdowns** | lookup | `Lookup(Players.TotalTouchdowns via Player)` |
| **RosterAssignments.ProjectedScore** | lookup | `Lookup(Players.ProjectedScore via Player)` |
| **Matchups.Team1Score** | lookup | `Lookup(Rosters.ProjectedScore via Team1)` |
| **Matchups.Team2Score** | lookup | `Lookup(Rosters.ProjectedScore via Team2)` |
| **Matchups.Margin** | formula | `Team1Score - Team2Score` |
| **Matchups.Winner** | formula | `If(Margin > 0, Team1, Team2)` |
| **Matchups.Loser** | formula | `If(Margin > 0, Team2, Team1)` |
| **Matchups.IsTieGame** | formula | `Margin = 0` |
| **Standings.Owner** | lookup | `Lookup(Rosters.Owner via StandingId)` |

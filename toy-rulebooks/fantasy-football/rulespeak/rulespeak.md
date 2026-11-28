# 📘 Fantasy Football — Docs

_Multi-hop DAG: raw player stats → roster aggregations → matchup scoring → standings & seeding._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Dev User** | A dev user is identified by its name. | — |
| Email | A defined attribute. | _User email (also PK)_ |
| Name | The same as its email. | _Display name_ |
| Role | A defined attribute. | _commissioner, owner, or spectator_ |

<details open>
<summary>Example data</summary>

| Email | Name ƒ | Role |
|---|---|---|
| commissioner@league.local | commissioner@league.local | commissioner |
| owner@league.local | owner@league.local | owner |
| spectator@league.local | spectator@league.local | spectator |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Player** | A player is identified by its name. | — |
| Name | The same as its player ID. | _Display name_ |
| Position | A defined attribute. | _QB, RB, WR, TE_ |
| Passing Yards | A defined attribute. | _Season passing yards_ |
| Passing Touchdowns | A defined attribute. | _Season passing TDs_ |
| Interceptions | A defined attribute. | _Season INTs_ |
| Rushing Yards | A defined attribute. | _Season rushing yards_ |
| Rushing Touchdowns | A defined attribute. | _Season rushing TDs_ |
| Receptions | A defined attribute. | _Season receptions_ |
| Received Yards | A defined attribute. | _Season receiving yards_ |
| Received Touchdowns | A defined attribute. | _Season receiving TDs_ |
| Total Touchdowns | Computed as the passing touchdowns plus the rushing touchdowns plus the received touchdowns. | _Sum of all TDs_ |
| Projected Score | Computed as the passing yards divided by 25 plus the total touchdowns times 6 minus the interceptions times 2 rounded to 2 decimal place(s). | _Calculated fantasy points_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Position | Passing Yards | Passing Touchdowns | Interceptions | Rushing Yards | Rushing Touchdowns | Receptions | Received Yards | Received Touchdowns | Total Touchdowns ƒ | Projected Score ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|
| patrick-mahomes | QB | 5500 | 48 | 8 | 510 | 3 | 0 | 0 | 0 | 51 | 510.0 |
| travis-kelce | TE | 0 | 0 | 0 | 0 | 0 | 110 | 1200 | 12 | 12 | 72.0 |
| christian-mccaffrey | RB | 0 | 0 | 0 | 1500 | 15 | 90 | 650 | 3 | 18 | 108.0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Roster** | A roster is identified by its name. | — |
| Name | The same as its roster ID. | _Display name_ |
| Owner | A defined attribute. | _Owner email_ |
| Total Passing Yards | The total passing yards across the roster assignments related to the roster. | _Sum of QB passing yards on roster_ |
| Total Rushing Yards | The total rushing yards across the roster assignments related to the roster. | _Sum of rushing yards on roster_ |
| Total Receiving Yards | The total received yards across the roster assignments related to the roster. | _Sum of receiving yards on roster_ |
| Total Touchdowns | The total touchdowns across the roster assignments related to the roster. | _Sum of all TDs on roster_ |
| Total Interceptions | The total interceptions across the roster assignments related to the roster. | _Sum of INTs on roster_ |
| Projected Score | Computed as the total passing yards divided by 25 plus the total rushing yards divided by 10 plus the total receiving yards divided by 10 plus the total touchdowns times 6 minus the total interceptions times 2 rounded to 2 decimal place(s). | _Total roster fantasy points (2nd-order calc)_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Owner | Total Passing Yards ƒ | Total Rushing Yards ƒ | Total Receiving Yards ƒ | Total Touchdowns ƒ | Total Interceptions ƒ | Projected Score ƒ |
|---|---|---|---|---|---|---|---|
| mahomes-mob | commissioner@league.local | 5500 | 2010 | 1850 | 81 | 8 | 1076.0 |
| hurts-hustlers | owner@league.local | 3800 | 3020 | 1720 | 71 | 5 | 1042.0 |
| allens-arsenal | owner@league.local | 4500 | 2100 | 1950 | 61 | 8 | 935.0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Roster Assignment** | A roster assignment is identified by its name and is related to a roster and a player. | — |
| Name | The same as its assignment ID. | _Display name_ |
| Roster | A defined attribute. | _Roster FK_ |
| Player | A defined attribute. | _Player FK_ |
| Player Name | Taken from the linked player. | _Player display name_ |
| Position | Taken from the linked player. | _Player position_ |
| Passing Yards | Taken from the linked player. | _Player passing yards_ |
| Passing Touchdowns | Taken from the linked player. | _Player passing TDs_ |
| Interceptions | Taken from the linked player. | _Player INTs_ |
| Rushing Yards | Taken from the linked player. | _Player rushing yards_ |
| Rushing Touchdowns | Taken from the linked player. | _Player rushing TDs_ |
| Receptions | Taken from the linked player. | _Player receptions_ |
| Received Yards | Taken from the linked player. | _Player receiving yards_ |
| Received Touchdowns | Taken from the linked player. | _Player receiving TDs_ |
| Total Touchdowns | Taken from the linked player. | _Player total TDs_ |
| Projected Score | Taken from the linked player. | _Player projected score_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Player Name ƒ | Position ƒ | Passing Yards ƒ | Passing Touchdowns ƒ | Interceptions ƒ | Rushing Yards ƒ | Rushing Touchdowns ƒ | Receptions ƒ | Received Yards ƒ | Received Touchdowns ƒ | Total Touchdowns ƒ | Projected Score ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| mahomes-mob-qb | patrick-mahomes | QB | 5500 | 48 | 8 | 510 | 3 | 0 | 0 | 0 | 51 | 510.0 |
| mahomes-mob-rb | christian-mccaffrey | RB | 0 | 0 | 0 | 1500 | 15 | 90 | 650 | 3 | 18 | 108.0 |
| mahomes-mob-te | travis-kelce | TE | 0 | 0 | 0 | 0 | 0 | 110 | 1200 | 12 | 12 | 72.0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Matchup** | A matchup is identified by its name and is related to a roster (its team1) and a roster (its team2). | — |
| Name | The same as its matchup ID. | _Display name_ |
| Week | A defined attribute. | _Week number_ |
| Team1 | A defined attribute. | _First roster_ |
| Team2 | A defined attribute. | _Second roster_ |
| Team1 Score | The projected score of the matchup's team1. | _Team1's projected score_ |
| Team2 Score | The projected score of the matchup's team2. | _Team2's projected score_ |
| Margin | Computed as the team1 score minus the team2 score. | _Team1 score minus Team2 score_ |
| Winner | Determined by priority: the team1 if the margin is greater than 0; in all other cases, the team2. | _Winning roster ID_ |
| Loser | Determined by priority: the team2 if the margin is greater than 0; in all other cases, the team1. | _Losing roster ID_ |
| Is Tie Game | True when the margin is 0. | _True if scores are equal_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Week | Team1 Score ƒ | Team2 Score ƒ | Margin ƒ | Winner ƒ | Loser ƒ | Is Tie Game ƒ |
|---|---|---|---|---|---|---|---|
| week1-mob-hustlers | 1 | 1076.0 | 1042.0 | 34.0 | mahomes-mob | hurts-hustlers | false |
| week1-arsenal-dynasty | 1 | 935.0 | 775.0 | 160.0 | allens-arsenal | davantes-dynasty | false |
| week1-limit-wins | 1 | 383.0 | 657.0 | -274.0 | defense-wins | hurts-limit | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Standing** | A standing is identified by its name. | — |
| Name | Taken from the linked standing ID. | _Roster name_ |
| Owner | Taken from the linked standing ID. | _Owner email_ |
| Wins | A defined attribute. | _Count of matchups won_ |
| Losses | A defined attribute. | _Count of matchups lost_ |
| Ties | A defined attribute. | _Count of tied matchups_ |
| Win Pct | A defined attribute. | _Win percentage_ |
| Points for | A defined attribute. | _Total points scored by this roster_ |
| Points Against | A defined attribute. | _Total points allowed_ |
| Seed Rank | A defined attribute. | _Playoff seed (manually set)_ |
| Playoff Bound | True when an empty string. | _True if seeded 1-6_ |
| Champs Path | A defined attribute. | _If playoff-bound, path to championship_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Owner ƒ | Wins | Losses | Ties | Win Pct | Points for | Points Against | Seed Rank | Playoff Bound | Champs Path |
|---|---|---|---|---|---|---|---|---|---|---|
| mahomes-mob | commissioner@league.local | 6 | 0 | 0 | 1.0 | 6456.0 | 4727.0 | 1 | true | — |
| hurts-hustlers | owner@league.local | 5 | 1 | 0 | 0.8333 | 6252.0 | 4761.0 | 2 | true | — |
| davantes-dynasty | owner@league.local | 3 | 3 | 0 | 0.5 | 4650.0 | 4476.0 | 3 | true | — |

_ƒ marks a computed column._

</details>

## 2 Fact Types

- a **roster assignment** references exactly one **roster**
- a **roster assignment** references exactly one **player**
- a **matchup** references exactly one **roster**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A dev user **must** have an email and a role.
- A player **must** have a position; a passing yards; a passing touchdowns; an interceptions; a rushing yards; a rushing touchdowns; a receptions; a received yards; and a received touchdowns.
- A roster **must** have an owner.
- A roster assignment **must** reference exactly one roster.
- A roster assignment **must** reference exactly one player.
- A matchup **must** reference exactly one roster as its team1.
- A matchup **must** reference exactly one roster as its team2.
- A matchup **must** have a week.
- A standing **must** have a wins; a losses; a ties; a win pct; a points for; and a points against, and record whether it is playoff bound.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A dev user's name is the same as its email. |
| **DR-2 Name** | A player's name is the same as its player ID. |
| **DR-3 Total Touchdowns** | A player's total touchdowns is computed as the passing touchdowns plus the rushing touchdowns plus the received touchdowns. |
| **DR-4 Projected Score** | A player's projected score is computed as the passing yards divided by 25 plus the total touchdowns times 6 minus the interceptions times 2 rounded to 2 decimal place(s). |
| **DR-5 Name** | A roster's name is the same as its roster ID. |
| **DR-6 Total Passing Yards** | A roster's total passing yards is the total passing yards across the roster assignments related to the roster. |
| **DR-7 Total Rushing Yards** | A roster's total rushing yards is the total rushing yards across the roster assignments related to the roster. |
| **DR-8 Total Receiving Yards** | A roster's total receiving yards is the total received yards across the roster assignments related to the roster. |
| **DR-9 Total Touchdowns** | A roster's total touchdowns is the total touchdowns across the roster assignments related to the roster. |
| **DR-10 Total Interceptions** | A roster's total interceptions is the total interceptions across the roster assignments related to the roster. |
| **DR-11 Projected Score** | A roster's projected score is computed as the total passing yards divided by 25 plus the total rushing yards divided by 10 plus the total receiving yards divided by 10 plus the total touchdowns times 6 minus the total interceptions times 2 rounded to 2 decimal place(s). |
| **DR-12 Name** | A roster assignment's name is the same as its assignment ID. |
| **DR-13 Player Name** | A roster assignment's player name — taken from the linked player. |
| **DR-14 Position** | A roster assignment's position — taken from the linked player. |
| **DR-15 Passing Yards** | A roster assignment's passing yards — taken from the linked player. |
| **DR-16 Passing Touchdowns** | A roster assignment's passing touchdowns — taken from the linked player. |
| **DR-17 Interceptions** | A roster assignment's interceptions — taken from the linked player. |
| **DR-18 Rushing Yards** | A roster assignment's rushing yards — taken from the linked player. |
| **DR-19 Rushing Touchdowns** | A roster assignment's rushing touchdowns — taken from the linked player. |
| **DR-20 Receptions** | A roster assignment's receptions — taken from the linked player. |
| **DR-21 Received Yards** | A roster assignment's received yards — taken from the linked player. |
| **DR-22 Received Touchdowns** | A roster assignment's received touchdowns — taken from the linked player. |
| **DR-23 Total Touchdowns** | A roster assignment's total touchdowns — taken from the linked player. |
| **DR-24 Projected Score** | A roster assignment's projected score — taken from the linked player. |
| **DR-25 Name** | A matchup's name is the same as its matchup ID. |
| **DR-26 Team1 Score** | A matchup's team1 score is the projected score of the matchup's team1. |
| **DR-27 Team2 Score** | A matchup's team2 score is the projected score of the matchup's team2. |
| **DR-28 Margin** | A matchup's margin is computed as the team1 score minus the team2 score. |
| **DR-29 Winner** | The matchup's winner is determined by the following priority:<br>1. the team1, if the margin is greater than 0;<br>2. in all other cases, the team2. |
| **DR-30 Loser** | The matchup's loser is determined by the following priority:<br>1. the team2, if the margin is greater than 0;<br>2. in all other cases, the team1. |
| **DR-31 Is Tie Game** | A matchup is considered a tie game if the margin is 0. |
| **DR-32 Name** | A standing's name — taken from the linked standing ID. |
| **DR-33 Owner** | A standing's owner — taken from the linked standing ID. |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **DevUsers.Name** | formula | `Email` |
| **Players.Name** | formula | `PlayerId` |
| **Players.TotalTouchdowns** | formula | `PassingTouchdowns + RushingTouchdowns + ReceivedTouchdowns` |
| **Players.ProjectedScore** | formula | `Round(PassingYards / 25 + TotalTouchdowns * 6 - Interceptions * 2, 2)` |
| **Rosters.Name** | formula | `RosterId` |
| **Rosters.TotalPassingYards** | rollup | `Sum(RosterAssignments.PassingYards via Roster)` |
| **Rosters.TotalRushingYards** | rollup | `Sum(RosterAssignments.RushingYards via Roster)` |
| **Rosters.TotalReceivingYards** | rollup | `Sum(RosterAssignments.ReceivedYards via Roster)` |
| **Rosters.TotalTouchdowns** | rollup | `Sum(RosterAssignments.TotalTouchdowns via Roster)` |
| **Rosters.TotalInterceptions** | rollup | `Sum(RosterAssignments.Interceptions via Roster)` |
| **Rosters.ProjectedScore** | formula | `Round(TotalPassingYards / 25 + TotalRushingYards / 10 + TotalReceivingYards / 10 + TotalTouchdowns * 6 - TotalInterceptions * 2, 2)` |
| **RosterAssignments.Name** | formula | `AssignmentId` |
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
| **Matchups.Name** | formula | `MatchupId` |
| **Matchups.Team1Score** | lookup | `Lookup(Rosters.ProjectedScore via Team1)` |
| **Matchups.Team2Score** | lookup | `Lookup(Rosters.ProjectedScore via Team2)` |
| **Matchups.Margin** | formula | `Team1Score - Team2Score` |
| **Matchups.Winner** | formula | `If(Margin > 0, Team1, Team2)` |
| **Matchups.Loser** | formula | `If(Margin > 0, Team2, Team1)` |
| **Matchups.IsTieGame** | formula | `Margin = 0` |
| **Standings.Name** | formula | `Lookup(Rosters.Name via StandingId)` |
| **Standings.Owner** | lookup | `Lookup(Rosters.Owner via StandingId)` |

---

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)

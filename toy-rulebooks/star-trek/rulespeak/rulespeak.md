# 📘 Star Trek — RuleSpeak®

_Aggregations over nested series → seasons → episodes (avg rating roll-ups). The canonical aggregation demo._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Sery** | A sery is identified by its name and is related to optionally a rating (its series ratings). | — |
| Name | Computed as the series number, followed by a hyphen, followed by the show code. | — |
| Series Number | A defined attribute. | — |
| Show Code | A defined attribute. | — |
| Title | A defined attribute. | — |
| Description | A defined attribute. | — |
| Network | A defined attribute. | — |
| Premiere Date | A defined attribute. | — |
| Finale Date | A defined attribute. | — |
| Total Seasons | The number of seasons related to the sery. | — |
| Total Episodes | The total episode count across the seasons related to the sery. | — |
| Is Long Running | True when the total episodes is greater than 50. | — |
| Seasons | A defined attribute. | — |
| Series Ratings | A defined attribute. | — |
| Rating | The total rating across the ratings related to the sery. | — |
| Is Good | True when the rating is at least 4.5. | — |
| **Season** | A season is identified by its name. | — |
| Name | Computed as the series, followed by “-Season-”, followed by the season number. | — |
| Series | A defined attribute. | — |
| Show Title | Taken from the linked series. | — |
| Show Description | Taken from the linked series. | — |
| Show Network | Taken from the linked series. | — |
| Season Number | A defined attribute. | — |
| Title | A defined attribute. | — |
| Description | A defined attribute. | — |
| Start Airdate | A defined attribute. | — |
| End Airdate | A defined attribute. | — |
| Episode Count | The number of episodes related to the season. | — |
| Episodes | A defined attribute. | — |
| Mock Data Notes | A defined attribute. | — |
| **Episode** | An episode is identified by its name and is related to optionally a season. | — |
| Name | Computed as the season, followed by “-Episode-”, followed by “0”, followed by the episode number. | — |
| Season | A defined attribute. | — |
| Season Number | Taken from the linked season. | — |
| Season Title | Taken from the linked season. | — |
| Season Description | Taken from the linked season. | — |
| Episode Number | A defined attribute. | — |
| Description | A defined attribute. | — |
| Airdate | A defined attribute. | — |
| Writers | A defined attribute. | — |
| Director | A defined attribute. | — |
| Runtime Minutes | A defined attribute. | — |
| Ratings | A defined attribute. | — |
| One Sentence Summary | A defined attribute. | — |
| Favorite Color | A defined attribute. | — |
| **Crew Type** | A crew type is identified by its name. | — |
| Name | A defined attribute. | — |
| Description | A defined attribute. | — |
| **People** | A people is identified by its name. | — |
| Name | A defined attribute. | — |
| Birth Year | A defined attribute. | — |
| Nationality | A defined attribute. | — |
| Bio | A defined attribute. | — |
| Crew Assignments | A defined attribute. | — |
| **Movy** | A movy is identified by its name. | — |
| Name | Computed as the movie number, followed by a hyphen, followed by the title. | — |
| Movie Number | A defined attribute. | — |
| Title | A defined attribute. | — |
| Description | A defined attribute. | — |
| Release Date | A defined attribute. | — |
| Runtime Minutes | A defined attribute. | — |
| Budget | A defined attribute. | — |
| Box Office | A defined attribute. | — |
| Crew Assignments | A defined attribute. | — |
| Crew Count | The number of crew assignments related to the movy. | — |
| Director Name | A defined attribute. | — |
| **Crew Assignment** | A crew assignment is identified by its name and is related to optionally a people (its person); optionally a movy (its movie); and optionally a crew type. | — |
| Name | Computed as the person name, followed by a hyphen, followed by the crew type name, followed by a hyphen, followed by the movie title. | — |
| Person | A defined attribute. | — |
| Person Name | Taken from the linked person. | — |
| Movie | A defined attribute. | — |
| Movie Title | Taken from the linked movie. | — |
| Crew Type | A defined attribute. | — |
| Crew Type Name | Taken from the linked crew type. | — |
| Notes | A defined attribute. | — |
| **Rating** | A rating is identified by its name and is related to optionally an episode. | — |
| Name | Computed as the display name, followed by a hyphen, followed by the users name. | — |
| Display Name | Determined by priority: “Episode: ”, followed by the episode name if the series is blank; in all other cases, “Series: ”, followed by the series name. | — |
| Rating | A defined attribute. | — |
| Series | A defined attribute. | — |
| Series Name | Taken from the linked series. | — |
| Episode | A defined attribute. | — |
| Episode Name | Taken from the linked episode. | — |
| Episode Season Title | Taken from the linked episode. | — |
| Users Name | A defined attribute. | — |
| Email Address | A defined attribute. | — |
| Phone Nmber | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Created At | A defined attribute. | — |
| **ERB Version** | An ERB version is identified by its name. | — |
| Name | A defined attribute. | — |
| Message | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Commit Date | A defined attribute. | — |
| Is Published | True when an empty string. | — |
| **ERB Customization** | An ERB customization is identified by its name. | — |
| Name | A defined attribute. | — |
| Title | A defined attribute. | — |
| SQL Code | A defined attribute. | — |
| SQL Target | A defined attribute. | — |
| Customization Type | A defined attribute. | — |

## 2 Fact Types

- a **sery** may reference one **rating**
- an **episode** may reference one **season**
- a **crew assignment** may reference one **people**
- a **crew assignment** may reference one **movy**
- a **crew assignment** may reference one **crew type**
- a **rating** may reference one **episode**

## 3 Operative Rules

_No operative rules yet. Required fields and foreign keys imply structural
`must`-rules automatically; to declare semantic obligations (`must` / `must not` / `should`), add a **Constraints** table whose rows point at
boolean calculated fields. See the tool README for the column contract._

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A sery's name is computed as the series number, followed by a hyphen, followed by the show code. |
| **DR-2 Total Seasons** | A sery's total seasons is the number of seasons related to the sery. |
| **DR-3 Total Episodes** | A sery's total episodes is the total episode count across the seasons related to the sery. |
| **DR-4 Is Long Running** | A sery is considered long-running if the total episodes is greater than 50. |
| **DR-5 Rating** | A sery's rating is the total rating across the ratings related to the sery. |
| **DR-6 Is Good** | A sery is considered a good if the rating is at least 4.5. |
| **DR-7 Name** | A season's name is computed as the series, followed by “-Season-”, followed by the season number. |
| **DR-8 Show Title** | A season's show title — taken from the linked series. |
| **DR-9 Show Description** | A season's show description — taken from the linked series. |
| **DR-10 Show Network** | A season's show network — taken from the linked series. |
| **DR-11 Episode Count** | A season's episode count is the number of episodes related to the season. |
| **DR-12 Name** | An episode's name is computed as the season, followed by “-Episode-”, followed by “0”, followed by the episode number. |
| **DR-13 Season Number** | An episode's season number — taken from the linked season. |
| **DR-14 Season Title** | An episode's season title — taken from the linked season. |
| **DR-15 Season Description** | An episode's season description — taken from the linked season. |
| **DR-16 Name** | A movy's name is computed as the movie number, followed by a hyphen, followed by the title. |
| **DR-17 Crew Count** | A movy's crew count is the number of crew assignments related to the movy. |
| **DR-18 Name** | A crew assignment's name is computed as the person name, followed by a hyphen, followed by the crew type name, followed by a hyphen, followed by the movie title. |
| **DR-19 Person Name** | A crew assignment's person name — taken from the linked person. |
| **DR-20 Movie Title** | A crew assignment's movie title — taken from the linked movie. |
| **DR-21 Crew Type Name** | A crew assignment's crew type name — taken from the linked crew type. |
| **DR-22 Name** | A rating's name is computed as the display name, followed by a hyphen, followed by the users name. |
| **DR-23 Display Name** | The rating's display name is determined by the following priority:<br>1. “Episode: ”, followed by the episode name, if the series is blank;<br>2. in all other cases, “Series: ”, followed by the series name. |
| **DR-24 Series Name** | A rating's series name — taken from the linked series. |
| **DR-25 Episode Name** | A rating's episode name — taken from the linked episode. |
| **DR-26 Episode Season Title** | A rating's episode season title — taken from the linked episode. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Series.Name** | formula | `SeriesNumber & "-" & ShowCode` |
| **Series.TotalSeasons** | rollup | `Count(Seasons via Series)` |
| **Series.TotalEpisodes** | rollup | `Sum(Seasons.EpisodeCount via Series)` |
| **Series.IsLongRunning** | formula | `TotalEpisodes > 50` |
| **Series.Rating** | rollup | `Sum(Ratings.Rating via Series)` |
| **Series.IsGood** | formula | `Rating >= 4.5` |
| **Seasons.Name** | formula | `Series & "-Season-" & SeasonNumber` |
| **Seasons.ShowTitle** | lookup | `Lookup(Series.Title via Series)` |
| **Seasons.ShowDescription** | lookup | `Lookup(Series.Description via Series)` |
| **Seasons.ShowNetwork** | lookup | `Lookup(Series.Network via Series)` |
| **Seasons.EpisodeCount** | rollup | `Count(Episodes via Season)` |
| **Episodes.Name** | formula | `Season & "-Episode-" & If(EpisodeNumber < 10, "0", "") & EpisodeNumber` |
| **Episodes.SeasonNumber** | lookup | `Lookup(Seasons.SeasonNumber via Season)` |
| **Episodes.SeasonTitle** | lookup | `Lookup(Seasons.Title via Season)` |
| **Episodes.SeasonDescription** | lookup | `Lookup(Seasons.Description via Season)` |
| **Movies.Name** | formula | `MovieNumber & "-" & Title` |
| **Movies.CrewCount** | rollup | `Count(CrewAssignments via Movie)` |
| **CrewAssignments.Name** | formula | `PersonName & "-" & CrewTypeName & "-" & MovieTitle` |
| **CrewAssignments.PersonName** | lookup | `Lookup(People.Name via Person)` |
| **CrewAssignments.MovieTitle** | lookup | `Lookup(Movies.Title via Movie)` |
| **CrewAssignments.CrewTypeName** | lookup | `Lookup(CrewTypes.Name via CrewType)` |
| **Ratings.Name** | formula | `DisplayName & "-" & UsersName` |
| **Ratings.DisplayName** | formula | `If(Series = Blank(), "Episode: " & EpisodeName, "Series: " & SeriesName)` |
| **Ratings.SeriesName** | lookup | `Lookup(Series.Name via Series)` |
| **Ratings.EpisodeName** | lookup | `Lookup(Episodes.Name via Episode)` |
| **Ratings.EpisodeSeasonTitle** | lookup | `Lookup(Episodes.SeasonTitle via Episode)` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._

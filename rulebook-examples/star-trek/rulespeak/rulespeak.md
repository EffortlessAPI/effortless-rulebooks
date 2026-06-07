# 📘 Star Trek — RuleSpeak

_Aggregations over nested series → seasons → episodes (avg rating roll-ups). The canonical aggregation demo._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Sery** | A sery tracked by the business. |
| Total Seasons | The number of seasons related to the sery. |
| Total Episodes | The total episode count across the sery's seasons. |
| Is Long Running | True when the total episodes is greater than 50. |
| Rating | The total rating across the sery's ratings. |
| Is Good | True when the rating is at least 4.5. |
| **Season** | A season tracked by the business. |
| Show Title | The title of the season's series. |
| Show Description | The description of the season's series. |
| Show Network | The network of the season's series. |
| Episode Count | The number of episodes related to the season. |
| **Episode** | An episode tracked by the business. |
| Season Number | The season number of the episode's season. |
| Season Title | The title of the episode's season. |
| Season Description | The description of the episode's season. |
| **Crew Type** | A crew type tracked by the business. |
| **People** | A people tracked by the business. |
| **Movy** | A movy tracked by the business. |
| Crew Count | The number of crew assignments related to the movy. |
| **Crew Assignment** | A crew assignment tracked by the business. |
| Person Name | The name of the crew assignment's person. |
| Movie Title | The title of the crew assignment's movie. |
| Crew Type Name | The name of the crew assignment's crew type. |
| **Rating** | A rating tracked by the business. |
| Display Name | Computed: `If(Series = Blank(), "Episode: " & EpisodeName, "Series: " &SeriesName)`. |
| Series Name | The name of the rating's series. |
| Episode Name | The name of the rating's episode. |
| Episode Season Title | The season title of the rating's episode. |
| **ERB Version** | An ERB version tracked by the business. |
| **ERB Customization** | An ERB customization tracked by the business. |

## 2 Fact Types

- a **sery** may reference one **rating**
- an **episode** may reference one **season**
- a **crew assignment** may reference one **people**
- a **crew assignment** may reference one **movy**
- a **crew assignment** may reference one **crew type**
- a **rating** may reference one **episode**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Total Seasons** | A sery's total seasons is the number of seasons related to the sery. |
| **DR-2 Total Episodes** | A sery's total episodes is the total episode count across the seasons related to the sery. |
| **DR-3 Is Long Running** | A sery is considered long running if the total episodes is greater than 50. |
| **DR-4 Rating** | A sery's rating is the total rating across the ratings related to the sery. |
| **DR-5 Is Good** | A sery is considered a good if the rating is at least 4.5. |
| **DR-6 Show Title** | A season's show title is the title of the season's series. |
| **DR-7 Show Description** | A season's show description is the description of the season's series. |
| **DR-8 Show Network** | A season's show network is the network of the season's series. |
| **DR-9 Episode Count** | A season's episode count is the number of episodes related to the season. |
| **DR-10 Season Number** | An episode's season number is the season number of the episode's season. |
| **DR-11 Season Title** | An episode's season title is the title of the episode's season. |
| **DR-12 Season Description** | An episode's season description is the description of the episode's season. |
| **DR-13 Crew Count** | A movy's crew count is the number of crew assignments related to the movy. |
| **DR-14 Person Name** | A crew assignment's person name is the name of the crew assignment's person. |
| **DR-15 Movie Title** | A crew assignment's movie title is the title of the crew assignment's movie. |
| **DR-16 Crew Type Name** | A crew assignment's crew type name is the name of the crew assignment's crew type. |
| **DR-17 Display Name** | The rating's display name is determined by the following priority:<br>1. the literal “Episode: ”, followed by the episode name, if the series is `Blank()`;<br>2. otherwise the literal “Series: ”, followed by the series name. |
| **DR-18 Series Name** | A rating's series name is the name of the rating's series. |
| **DR-19 Episode Name** | A rating's episode name is the name of the rating's episode. |
| **DR-20 Episode Season Title** | A rating's episode season title is the season title of the rating's episode. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Series.TotalSeasons** | rollup | `Count(Seasons via Series)` |
| **Series.TotalEpisodes** | rollup | `Sum(Seasons.EpisodeCount via Series)` |
| **Series.IsLongRunning** | formula | `TotalEpisodes > 50` |
| **Series.Rating** | rollup | `Sum(Ratings.Rating via Series)` |
| **Series.IsGood** | formula | `Rating >= 4.5` |
| **Seasons.ShowTitle** | lookup | `Lookup(Series.Title via Series)` |
| **Seasons.ShowDescription** | lookup | `Lookup(Series.Description via Series)` |
| **Seasons.ShowNetwork** | lookup | `Lookup(Series.Network via Series)` |
| **Seasons.EpisodeCount** | rollup | `Count(Episodes via Season)` |
| **Episodes.SeasonNumber** | lookup | `Lookup(Seasons.SeasonNumber via Season)` |
| **Episodes.SeasonTitle** | lookup | `Lookup(Seasons.Title via Season)` |
| **Episodes.SeasonDescription** | lookup | `Lookup(Seasons.Description via Season)` |
| **Movies.CrewCount** | rollup | `Count(CrewAssignments via Movie)` |
| **CrewAssignments.PersonName** | lookup | `Lookup(People.Name via Person)` |
| **CrewAssignments.MovieTitle** | lookup | `Lookup(Movies.Title via Movie)` |
| **CrewAssignments.CrewTypeName** | lookup | `Lookup(CrewTypes.Name via CrewType)` |
| **Ratings.DisplayName** | formula | `If(Series = Blank(), "Episode: " & EpisodeName, "Series: " &SeriesName)` |
| **Ratings.SeriesName** | lookup | `Lookup(Series.Name via Series)` |
| **Ratings.EpisodeName** | lookup | `Lookup(Episodes.Name via Episode)` |
| **Ratings.EpisodeSeasonTitle** | lookup | `Lookup(Episodes.SeasonTitle via Episode)` |

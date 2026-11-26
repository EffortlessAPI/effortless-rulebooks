# 📘 Star Trek — Docs

_Aggregations over nested series → seasons → episodes (avg rating roll-ups). The canonical aggregation demo._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Series** | A series is identified by its name and is related to optionally a rating (its series ratings). | — |
| Name | Computed as the series number, followed by a hyphen, followed by the show code. | — |
| Series Number | A defined attribute. | — |
| Show Code | A defined attribute. | — |
| Title | A defined attribute. | — |
| Description | A defined attribute. | — |
| Network | A defined attribute. | — |
| Premiere Date | A defined attribute. | — |
| Finale Date | A defined attribute. | — |
| Total Seasons | The number of seasons related to the series. | — |
| Total Episodes | The total episode count across the seasons related to the series. | — |
| Is Long Running | True when the total episodes is greater than 50. | — |
| Seasons | A defined attribute. | — |
| Series Ratings | A defined attribute. | — |
| Rating Count | The number of ratings related to the series. | _How many fan ratings this series has._ |
| Rating Total | The total rating across the ratings related to the series. | _Sum of every fan rating attached to this series._ |
| Rating | Determined by priority: 0 if the rating count is 0; in all other cases, the rating total divided by the rating count. | _The average fan rating: RatingTotal / RatingCount, and 0 for a series nobody has rated yet. It was =SUMIFS(...) until 2026-11-26, which made it the total, not the average this table and IsGood describe (TOS read 13 where the rulebook says 4.33)._ |
| Is Good | True when the rating is at least 4.5. | — |

<details open>
<summary>Example data</summary>

| Name ƒ | Series Number | Show Code | Title | Description | Network | Premiere Date | Finale Date | Total Seasons ƒ | Total Episodes ƒ | Is Long Running ƒ | Rating Count ƒ | Rating Total ƒ | Rating ƒ | Is Good ƒ |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1-TOS | 1 | TOS | Star Trek: The Original Series | The voyages of the starship Enterprise under Captain James T. Kirk. | NBC | 1966-09-08 | 1969-06-03 | 3 | 76.0 | true | 3 | 13.0 | 4.333333333333333 | false |
| 2-TNG | 2 | TNG | Star Trek: The Next Generation | The next generation of Starfleet explores the galaxy under Captain Jean-Luc Picard. | Syndication | 1987-09-28 | 1994-05-23 | 7 | 178.0 | true | 1 | 5.0 | 5.0 | true |
| 3-DS9 | 3 | DS9 | Star Trek: Deep Space Nine | A space station at the edge of the final frontier faces political intrigue and war. | Syndication | 1993-01-03 | — | 7 | 176.0 | true | 0 | 0.0 | 0.0 | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Show Title ƒ | Show Description ƒ | Show Network ƒ | Season Number | Title | Description | Start Airdate | End Airdate | Episode Count ƒ | Mock Data Notes |
|---|---|---|---|---|---|---|---|---|---|---|
| 1-tos-Season-1 | Star Trek: The Original Series | The voyages of the starship Enterprise under Captain James T. Kirk. | NBC | 1 | The Original Series Season 1 | Season 1 of Star Trek: The Original Series | 1966-09-01 | 1967-09-01 | 26 | — |
| 1-tos-Season-2 | Star Trek: The Original Series | The voyages of the starship Enterprise under Captain James T. Kirk. | NBC | 2 | The Original Series Season 2 | Season 2 of Star Trek: The Original Series | 1967-09-01 | 1968-08-31 | 26 | — |
| 1-tos-Season-3 | Star Trek: The Original Series | The voyages of the starship Enterprise under Captain James T. Kirk. | NBC | 3 | The Original Series Season 3 | Season 3 of Star Trek: The Original Series | 1968-09-01 | 1969-09-01 | 24 | — |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Season Number ƒ | Season Title ƒ | Season Description ƒ | Episode Number | Description | Airdate | Writers | Director | Runtime Minutes | One Sentence Summary | Favorite Color |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 1-tos-season-1-Episode-01 | 1 | The Original Series Season 1 | Season 1 of Star Trek: The Original Series | 1 | Episode 1 of season 1 | 1966-09-01 | Story by Starfleet | Starfleet Director | 48 | An adventure on the USS Enterprise. | — |
| 1-tos-season-1-Episode-02 | 1 | The Original Series Season 1 | Season 1 of Star Trek: The Original Series | 2 | Episode 2 of season 1 | 1966-09-08 | Story by Starfleet | Starfleet Director | 48 | An adventure on the USS Enterprise. | — |
| 1-tos-season-1-Episode-03 | 1 | The Original Series Season 1 | Season 1 of Star Trek: The Original Series | 3 | Episode 3 of season 1 | 1966-09-15 | Story by Starfleet | Starfleet Director | 48 | An adventure on the USS Enterprise. | — |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Crew Type** | A crew type is identified by its name. | — |
| Name | A defined attribute. | — |
| Description | A defined attribute. | — |

<details open>
<summary>Example data</summary>

| Name | Description |
|---|---|
| Actor | Performer in front of the camera |
| Director | Director of the production |
| Writer | Screenplay or story writer |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Person** | A person is identified by its name. | — |
| Name | A defined attribute. | — |
| Birth Year | A defined attribute. | — |
| Nationality | A defined attribute. | — |
| Bio | A defined attribute. | — |
| Crew Assignments | A defined attribute. | — |

<details open>
<summary>Example data</summary>

| Name | Birth Year | Nationality | Bio |
|---|---|---|---|
| William Shatner | 1931 | Canadian | Actor famous for playing Captain James T. Kirk |
| Leonard Nimoy | 1931 | American | Actor famous for playing Spock |
| Patrick Stewart | 1940 | British | Actor famous for playing Captain Jean-Luc Picard |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Movie Number | Title | Description | Release Date | Runtime Minutes | Budget | Box Office | Crew Count ƒ | Director Name |
|---|---|---|---|---|---|---|---|---|---|
| 1-Star Trek: Generations | 1 | Star Trek: Generations | The seventh and final film of the original series cast, featuring the Next Generation crew | 1994-11-18 | 118 | 40000000 | 118159941 | 3 | — |
| 2-Star Trek: First Contact | 2 | Star Trek: First Contact | The Enterprise-E travels back in time to prevent a Borg invasion of Earth | 1996-11-22 | 111 | 46000000 | 92262550 | 1 | — |
| 3-Star Trek: Insurrection | 3 | Star Trek: Insurrection | The Enterprise-E crew fights to protect a peaceful planet from exploitation | 1998-11-25 | 103 | 70000000 | 70252769 | 0 | — |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Crew Assignment** | A crew assignment is identified by its name and is related to optionally a person; optionally a movy (its movie); and optionally a crew type. | — |
| Name | Computed as the person name, followed by a hyphen, followed by the crew type name, followed by a hyphen, followed by the movie title. | — |
| Person | A defined attribute. | — |
| Person Name | Taken from the linked person. | — |
| Movie | A defined attribute. | — |
| Movie Title | Taken from the linked movie. | — |
| Crew Type | A defined attribute. | — |
| Crew Type Name | Taken from the linked crew type. | — |
| Notes | A defined attribute. | — |

<details open>
<summary>Example data</summary>

| Name ƒ | Person Name ƒ | Movie Title ƒ | Crew Type Name ƒ | Notes |
|---|---|---|---|---|
| William Shatner-Actor-Star Trek: Generations | William Shatner | Star Trek: Generations | Actor | Reprised role as Captain Kirk |
| Patrick Stewart-Actor-Star Trek: Generations | Patrick Stewart | Star Trek: Generations | Actor | Captain Picard |
| Patrick Stewart-Actor-Star Trek: First Contact | Patrick Stewart | Star Trek: First Contact | Actor | Captain Picard |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Display Name ƒ | Rating | Series Name ƒ | Episode Name ƒ | Episode Season Title ƒ | Users Name | Email Address | Phone Nmber | Notes | Created At |
|---|---|---|---|---|---|---|---|---|---|---|
| Series: 1-TOS-Riley Shaw | Series: 1-TOS | 5 | 1-TOS | -Episode-0 | — | Riley Shaw | riley.shaw@example.com | 555-0107 | — | 2026-01-12T19:46:59Z |
| Series: 1-TOS-Alex Mercer | Series: 1-TOS | 5 | 1-TOS | -Episode-0 | — | Alex Mercer | alex.mercer@example.com | 555-0101 | — | 2026-01-12T19:46:59Z |
| Series: 1-TOS-Drew Patel | Series: 1-TOS | 3 | 1-TOS | -Episode-0 | — | Drew Patel | drew.patel@example.com | 555-0108 | — | 2026-01-12T19:46:59Z |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **ERB Version** | An ERB version is identified by its name. | — |
| Name | A defined attribute. | — |
| Message | A defined attribute. | — |
| Notes | A defined attribute. | — |
| Commit Date | A defined attribute. | — |
| Is Published | True when an empty string. | — |

<details open>
<summary>Example data</summary>

| Name | Message | Notes | Commit Date | Is Published |
|---|---|---|---|---|
| v2026-01-16T20:44:58.844Z | Initial demo | — | 2026-01-16 | true |
| v2026-01-16T20:47:35.227Z | We added a one sentence summary to episodes. | — | 2026-01-16 | true |
| v2026-01-16T20:49:06.117Z | Added Favorite Color | — | 2026-01-16 | true |

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **ERB Customization** | An ERB customization is identified by its name. | — |
| Name | A defined attribute. | — |
| Title | A defined attribute. | — |
| SQL Code | A defined attribute. | — |
| SQL Target | A defined attribute. | — |
| Customization Type | A defined attribute. | — |

<details open>
<summary>Example data</summary>

| Name | Title | SQL Code | SQL Target | Customization Type |
|---|---|---|---|---|
| 03a-customize-schema.sql | Customize Schema | -- ============================================================================<br>-- CUSTOMIZE SCHEMA - User-defined tables and schema modifications<br>-- ============================================================================<br>-- This file is for YOUR custom schema changes that should persist across<br>-- regeneration of the base ERB files.<br>--<br>-- USE THIS FILE FOR:<br>--   - Additional tables not defined in the rulebook<br>--   - Extra columns on existing tables (ALTER TABLE)<br>--   - Custom indexes for performance tuning<br>--   - Custom constraints or triggers<br>--<br>-- IMPORTANT:<br>--   - This file runs AFTER 01-drop-and-create-tables.sql<br>--   - The base tables already exist when this runs<br>--   - This file will NOT be overwritten by ERB regeneration<br>--<br>-- EXAMPLES:<br>--<br>--   -- Add a custom table<br>--   CREATE TABLE IF NOT EXISTS audit_log (<br>--       audit_log_id    TEXT PRIMARY KEY DEFAULT gen_random_uuid()::TEXT,<br>--       table_name      TEXT NOT NULL,<br>--       record_id       TEXT NOT NULL,<br>--       action          TEXT NOT NULL,<br>--       changed_at      TIMESTAMPTZ DEFAULT NOW()<br>--   );<br>--<br>--   -- Add a column to an existing table<br>--   ALTER TABLE erb_versions ADD COLUMN IF NOT EXISTS custom_field TEXT;<br>--<br>--   -- Add an index for performance<br>--   CREATE INDEX IF NOT EXISTS idx_custom ON some_table(some_column);<br>--<br>-- ============================================================================<br><br>-- Your custom schema changes go here:<br> | Postgres | Schema |
| 03b-customize-functions.sql | Customize Functions | -- ============================================================================<br>-- CUSTOMIZE FUNCTIONS - User-defined calculation functions<br>-- ============================================================================<br>-- This file is for YOUR custom PostgreSQL functions that should persist<br>-- across regeneration of the base ERB files.<br>--<br>-- USE THIS FILE FOR:<br>--   - Additional calc_* functions for computed fields<br>--   - Helper functions for complex business logic<br>--   - Utility functions for data transformation<br>--   - Trigger functions for custom automation<br>--<br>-- IMPORTANT:<br>--   - This file runs AFTER 02-create-functions.sql<br>--   - Base ERB calc functions already exist when this runs<br>--   - This file will NOT be overwritten by ERB regeneration<br>--   - Functions defined here can be used in 03b-customize-views.sql<br>--<br>-- ERB FUNCTION NAMING CONVENTIONS:<br>--   - calc_tablename_fieldname() - Returns a computed value for a view field<br>--   - Helper functions can use any naming convention<br>--<br>-- EXAMPLES:<br>--<br>--   -- Custom calculation function for a view field<br>--   CREATE OR REPLACE FUNCTION calc_orders_total_with_tax(p_order_id TEXT)<br>--   RETURNS NUMERIC AS $$<br>--   BEGIN<br>--       RETURN (SELECT subtotal * 1.08 FROM orders WHERE order_id = p_order_id);<br>--   END;<br>--   $$ LANGUAGE plpgsql STABLE;<br>--<br>--   -- Utility function<br>--   CREATE OR REPLACE FUNCTION format_currency(p_amount NUMERIC)<br>--   RETURNS TEXT AS $$<br>--   BEGIN<br>--       RETURN '$' \|\| TO_CHAR(p_amount, 'FM999,999,990.00');<br>--   END;<br>--   $$ LANGUAGE plpgsql IMMUTABLE;<br>--<br>-- ============================================================================<br><br>-- Your custom functions go here:<br> | Postgres | Functions |
| 03c-customize-views.sql | Customize Views | -- ============================================================================<br>-- CUSTOMIZE VIEWS - User-defined views and view extensions<br>-- ============================================================================<br>-- This file is for YOUR custom views that should persist across<br>-- regeneration of the base ERB files.<br>--<br>-- USE THIS FILE FOR:<br>--   - Additional views not defined in the rulebook<br>--   - Specialized views for reporting or dashboards<br>--   - Views that combine data from multiple base views<br>--   - Materialized views for performance optimization<br>--<br>-- IMPORTANT:<br>--   - This file runs AFTER 03-create-views.sql<br>--   - All base vw_* views already exist when this runs<br>--   - This file will NOT be overwritten by ERB regeneration<br>--   - If you need to EXTEND a base view, create a new view that selects from it<br>--<br>-- ERB VIEW PATTERN:<br>--   - Base views (vw_*) contain all denormalized data + calculated fields<br>--   - READ from views, WRITE to base tables<br>--   - Never JOIN in application code - extend views instead<br>--<br>-- EXAMPLES:<br>--<br>--   -- Custom reporting view<br>--   CREATE OR REPLACE VIEW vw_monthly_summary AS<br>--   SELECT<br>--       DATE_TRUNC('month', created_at) AS month,<br>--       COUNT(*) AS record_count,<br>--       SUM(amount) AS total_amount<br>--   FROM vw_orders<br>--   GROUP BY DATE_TRUNC('month', created_at);<br>--<br>--   -- Extended view with additional computed fields<br>--   CREATE OR REPLACE VIEW vw_orders_extended AS<br>--   SELECT<br>--       o.*,<br>--       calc_orders_total_with_tax(o.order_id) AS total_with_tax<br>--   FROM vw_orders o;<br>--<br>--   -- Materialized view for heavy queries<br>--   CREATE MATERIALIZED VIEW IF NOT EXISTS mv_dashboard_stats AS<br>--   SELECT ... FROM vw_orders ...;<br>--<br>-- ============================================================================<br><br>-- Your custom views go here:<br> | Postgres | Views |

</details>

## 2 Fact Types

- a **series** may reference one **rating**
- an **episode** may reference one **season**
- a **crew assignment** may reference one **person**
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
| **DR-1 Name** | A series's name is computed as the series number, followed by a hyphen, followed by the show code. |
| **DR-2 Total Seasons** | A series's total seasons is the number of seasons related to the series. |
| **DR-3 Total Episodes** | A series's total episodes is the total episode count across the seasons related to the series. |
| **DR-4 Is Long Running** | A series is considered long-running if the total episodes is greater than 50. |
| **DR-5 Rating Count** | A series's rating count is the number of ratings related to the series. |
| **DR-6 Rating Total** | A series's rating total is the total rating across the ratings related to the series. |
| **DR-7 Rating** | The series's rating is determined by the following priority:<br>1. 0, if the rating count is 0;<br>2. in all other cases, the rating total divided by the rating count. |
| **DR-8 Is Good** | A series is considered a good if the rating is at least 4.5. |
| **DR-9 Name** | A season's name is computed as the series, followed by “-Season-”, followed by the season number. |
| **DR-10 Show Title** | A season's show title — taken from the linked series. |
| **DR-11 Show Description** | A season's show description — taken from the linked series. |
| **DR-12 Show Network** | A season's show network — taken from the linked series. |
| **DR-13 Episode Count** | A season's episode count is the number of episodes related to the season. |
| **DR-14 Name** | An episode's name is computed as the season, followed by “-Episode-”, followed by “0”, followed by the episode number. |
| **DR-15 Season Number** | An episode's season number — taken from the linked season. |
| **DR-16 Season Title** | An episode's season title — taken from the linked season. |
| **DR-17 Season Description** | An episode's season description — taken from the linked season. |
| **DR-18 Name** | A movy's name is computed as the movie number, followed by a hyphen, followed by the title. |
| **DR-19 Crew Count** | A movy's crew count is the number of crew assignments related to the movy. |
| **DR-20 Name** | A crew assignment's name is computed as the person name, followed by a hyphen, followed by the crew type name, followed by a hyphen, followed by the movie title. |
| **DR-21 Person Name** | A crew assignment's person name — taken from the linked person. |
| **DR-22 Movie Title** | A crew assignment's movie title — taken from the linked movie. |
| **DR-23 Crew Type Name** | A crew assignment's crew type name — taken from the linked crew type. |
| **DR-24 Name** | A rating's name is computed as the display name, followed by a hyphen, followed by the users name. |
| **DR-25 Display Name** | The rating's display name is determined by the following priority:<br>1. “Episode: ”, followed by the episode name, if the series is blank;<br>2. in all other cases, “Series: ”, followed by the series name. |
| **DR-26 Series Name** | A rating's series name — taken from the linked series. |
| **DR-27 Episode Name** | A rating's episode name — taken from the linked episode. |
| **DR-28 Episode Season Title** | A rating's episode season title — taken from the linked episode. |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Series.Name** | formula | `SeriesNumber & "-" & ShowCode` |
| **Series.TotalSeasons** | rollup | `Count(Seasons via Series)` |
| **Series.TotalEpisodes** | rollup | `Sum(Seasons.EpisodeCount via Series)` |
| **Series.IsLongRunning** | formula | `TotalEpisodes > 50` |
| **Series.RatingCount** | rollup | `Count(Ratings via Series)` |
| **Series.RatingTotal** | rollup | `Sum(Ratings.Rating via Series)` |
| **Series.Rating** | formula | `If(RatingCount = 0, 0, RatingTotal / RatingCount)` |
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

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)

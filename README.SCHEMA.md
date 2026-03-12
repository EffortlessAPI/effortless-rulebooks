# Effortless Rulebook

### Generated from:

> This builds ssotme tools including those that are disabled which regenerates this file.

```
$ cd ./docs
$ ssotme -build -id
```


> Rulebook generated from Airtable base 'DEMO: StarTrek'.

**Model ID:** ``

---

## Formal Arguments

The following logical argument establishes that "language" can be formalized as a computable classification, and demonstrates that not everything qualifies as a language under this definition.


## Execution Substrates

The following formats have been evaluated as execution substrates for this rulebook.

See the `execution-substrates/` directory for available format implementations.

---

## Table Schemas

### Table: Series

> Table: Series

#### Schema

| Field | Type | Data Type | Nullable | Description |
|-------|------|-----------|----------|-------------|
| `SerieId` | raw | string | No | - |
| `Name` | calculated | string | Yes | - |
| `SeriesNumber` | raw | integer | Yes | - |
| `ShowCode` | raw | string | Yes | - |
| `Title` | raw | string | Yes | - |
| `Description` | raw | string | Yes | - |
| `Network` | raw | string | Yes | - |
| `PremiereDate` | raw | datetime | Yes | - |
| `FinaleDate` | raw | datetime | Yes | - |
| `TotalSeasons` | aggregation | integer | Yes | - |
| `TotalEpisodes` | aggregation | number | Yes | - |
| `IsLongRunning` | calculated | boolean | Yes | - |
| `Seasons` | relationship | string | Yes | - |
| `SeriesRatings` | relationship | string | Yes | - |
| `Rating` | aggregation | number | Yes | - |
| `IsGood` | calculated | boolean | Yes | - |

**Formula for `Name`:**
```
={{SeriesNumber}} & "-" & {{ShowCode}}
```

**Formula for `TotalSeasons`:**
```
=COUNTIFS(Seasons!{{Series}}, Series!{{SerieId}})
```

**Formula for `TotalEpisodes`:**
```
=SUMIFS(Seasons!{{EpisodeCount}}, Seasons!{{Series}}, Series!{{SerieId}})
```

**Formula for `IsLongRunning`:**
```
={{TotalEpisodes}} > 50
```

**Formula for `Rating`:**
```
=SUMIFS(Ratings!{{Rating}}, Ratings!{{Series}}, Series!{{SerieId}})
```

**Formula for `IsGood`:**
```
={{Rating}} >= 4.5
```


#### Sample Data (10 records)

| Field | Value |
|-------|-------|
| `SerieId` | 1-tos |
| `Name` | 1-TOS |
| `Title` | Star Trek: The Original Series |
| `Description` | The voyages of the starship Enterprise under Captain James T. Kirk. |
| `Network` | NBC |
| `PremiereDate` | 1966-09-08 |
| `FinaleDate` | 1969-06-03 |
| `TotalSeasons` | 3 |
| `TotalEpisodes` | 76 |
| `Seasons` | 1-tos-season-1, 1-tos-season-2, 1-tos-season-3 |
| `SeriesNumber` | 1 |
| `ShowCode` | TOS |
| `SeriesRatings` | series-1-tos-riley-shaw, series-1-tos-alex-mercer, series-1-tos-drew-patel |
| `Rating` | 4.333333333333333 |
| `IsLongRunning` | true |
| `IsGood` | false |

---

### Table: Seasons

> Table: Seasons

#### Schema

| Field | Type | Data Type | Nullable | Description |
|-------|------|-----------|----------|-------------|
| `SeasonId` | raw | string | No | - |
| `Name` | calculated | string | Yes | - |
| `Series` | relationship | string | Yes | - |
| `ShowTitle` | lookup | string | Yes | - |
| `ShowDescription` | lookup | string | Yes | - |
| `ShowNetwork` | lookup | string | Yes | - |
| `SeasonNumber` | raw | integer | Yes | - |
| `Title` | raw | string | Yes | - |
| `Description` | raw | string | Yes | - |
| `StartAirdate` | raw | datetime | Yes | - |
| `EndAirdate` | raw | datetime | Yes | - |
| `EpisodeCount` | aggregation | integer | Yes | - |
| `Episodes` | relationship | string | Yes | - |
| `MockDataNotes` | raw | string | Yes | - |

**Formula for `Name`:**
```
={{Series}} & "-Season-" & {{SeasonNumber}}
```

**Formula for `ShowTitle`:**
```
=INDEX(Series!{{Title}}, MATCH(Seasons!{{Series}}, Series!{{SerieId}}, 0))
```

**Formula for `ShowDescription`:**
```
=INDEX(Series!{{Description}}, MATCH(Seasons!{{Series}}, Series!{{SerieId}}, 0))
```

**Formula for `ShowNetwork`:**
```
=INDEX(Series!{{Network}}, MATCH(Seasons!{{Series}}, Series!{{SerieId}}, 0))
```

**Formula for `EpisodeCount`:**
```
=COUNTIFS(Episodes!{{Season}}, Seasons!{{SeasonId}})
```


#### Sample Data (3 records)

| Field | Value |
|-------|-------|
| `SeasonId` | 1-tos-season-1 |
| `Name` | 1-TOS-Season-1 |
| `Series` | 1-tos |
| `SeasonNumber` | 1 |
| `Title` | TOS Season 1 |
| `Description` | Season 1 of TOS |
| `StartAirdate` | 1966-09-08 |
| `EndAirdate` | 1967-03-23 |
| `EpisodeCount` | 26 |
| `Episodes` | 1-tos-season-1-episode-01, 1-tos-season-1-episode-02, 1-tos-season-1-episode-03, 1-tos-season-1-episode-07, 1-tos-season-1-episode-08, 1-tos-season-1-episode-09, 1-tos-season-1-episode-10, 1-tos-season-1-episode-11, 1-tos-season-1-episode-12, 1-tos-season-1-episode-13, 1-tos-season-1-episode-14, 1-tos-season-1-episode-15, 1-tos-season-1-episode-16, 1-tos-season-1-episode-17, 1-tos-season-1-episode-18, 1-tos-season-1-episode-19, 1-tos-season-1-episode-20, 1-tos-season-1-episode-21, 1-tos-season-1-episode-22, 1-tos-season-1-episode-23, 1-tos-season-1-episode-24, 1-tos-season-1-episode-25, 1-tos-season-1-episode-26, 1-tos-season-1-episode-27, 1-tos-season-1-episode-28, 1-tos-season-1-episode-29 |
| `ShowNetwork` | NBC |
| `ShowDescription` | The voyages of the starship Enterprise under Captain James T. Kirk. |
| `ShowTitle` | Star Trek: The Original Series |
| `MockDataNotes` |  |

---

### Table: Episodes

> Table: Episodes

#### Schema

| Field | Type | Data Type | Nullable | Description |
|-------|------|-----------|----------|-------------|
| `EpisodeId` | raw | string | No | - |
| `Name` | calculated | string | Yes | - |
| `Season` | relationship | string | Yes | - |
| `SeasonNumber` | lookup | integer | Yes | - |
| `SeasonTitle` | lookup | string | Yes | - |
| `SeasonDescription` | lookup | string | Yes | - |
| `EpisodeNumber` | raw | integer | Yes | - |
| `Description` | raw | string | Yes | - |
| `Airdate` | raw | datetime | Yes | - |
| `Writers` | raw | string | Yes | - |
| `Director` | raw | string | Yes | - |
| `RuntimeMinutes` | raw | integer | Yes | - |
| `Ratings` | relationship | string | Yes | - |
| `OneSentenceSummary` | raw | string | Yes | - |
| `FavoriteColor` | raw | string | Yes | - |

**Formula for `Name`:**
```
={{Season}} & "-Episode-" & IF({{EpisodeNumber}} < 10, '0', '') & {{EpisodeNumber}}
```

**Formula for `SeasonNumber`:**
```
=INDEX(Seasons!{{SeasonNumber}}, MATCH(Episodes!{{Season}}, Seasons!{{SeasonId}}, 0))
```

**Formula for `SeasonTitle`:**
```
=INDEX(Seasons!{{Title}}, MATCH(Episodes!{{Season}}, Seasons!{{SeasonId}}, 0))
```

**Formula for `SeasonDescription`:**
```
=INDEX(Seasons!{{Description}}, MATCH(Episodes!{{Season}}, Seasons!{{SeasonId}}, 0))
```


#### Sample Data (292 records)

| Field | Value |
|-------|-------|
| `EpisodeId` | 1-tos-season-1-episode-01 |
| `Name` | 1-TOS-Season-1-Episode-01 |
| `EpisodeNumber` | 1 |
| `Description` | Episode 1 of season 1 of TOS. |
| `Airdate` | 1966-09-08 |
| `Writers` | Staff Writer |
| `Director` | Series Director |
| `RuntimeMinutes` | 48 |
| `Season` | 1-tos-season-1 |
| `SeasonDescription` | Season 1 of TOS |
| `SeasonTitle` | TOS Season 1 |
| `SeasonNumber` | 1 |
| `Ratings` | episode-1-tos-season-1-episode-01-alexis-cruz |
| `OneSentenceSummary` | what |
| `FavoriteColor` |  |

---

### Table: Ratings

> Table: Ratings

#### Schema

| Field | Type | Data Type | Nullable | Description |
|-------|------|-----------|----------|-------------|
| `RatingId` | raw | string | No | - |
| `Name` | calculated | string | Yes | - |
| `DisplayName` | calculated | string | Yes | - |
| `Rating` | raw | string | Yes | - |
| `Series` | relationship | string | Yes | - |
| `SeriesName` | lookup | string | Yes | - |
| `Episode` | relationship | string | Yes | - |
| `EpisodeName` | lookup | string | Yes | - |
| `EpisodeSeasonTitle` | lookup | string | Yes | - |
| `UsersName` | raw | string | Yes | - |
| `EmailAddress` | raw | string | Yes | - |
| `PhoneNmber` | raw | string | Yes | - |
| `Notes` | raw | string | Yes | - |
| `CreatedAt` | raw | datetime | Yes | - |

**Formula for `Name`:**
```
={{DisplayName}} & "-" & {{UsersName}}
```

**Formula for `DisplayName`:**
```
=IF({{Series}} = BLANK(), "Episode: " & {{EpisodeName}}, "Series: " &{{SeriesName}})
```

**Formula for `SeriesName`:**
```
=INDEX(Series!{{Name}}, MATCH(Ratings!{{Series}}, Series!{{SerieId}}, 0))
```

**Formula for `EpisodeName`:**
```
=INDEX(Episodes!{{Name}}, MATCH(Ratings!{{Episode}}, Episodes!{{EpisodeId}}, 0))
```

**Formula for `EpisodeSeasonTitle`:**
```
=INDEX(Episodes!{{SeasonTitle}}, MATCH(Ratings!{{Episode}}, Episodes!{{EpisodeId}}, 0))
```


#### Sample Data (13 records)

| Field | Value |
|-------|-------|
| `RatingId` | series-1-tos-riley-shaw |
| `Name` | Series: 1-TOS-Riley Shaw |
| `Rating` | 5 |
| `Series` | 1-tos |
| `EmailAddress` | riley.shaw@example.com |
| `PhoneNmber` | 555-0107 |
| `SeriesName` | 1-TOS |
| `DisplayName` | Series: 1-TOS |
| `UsersName` | Riley Shaw |
| `CreatedAt` | 2026-01-12T19:46:59Z |
| `Episode` |  |
| `EpisodeName` |  |
| `EpisodeSeasonTitle` |  |
| `Notes` |  |

---

### Table: ERBVersions

> Table: ERBVersions

#### Schema

| Field | Type | Data Type | Nullable | Description |
|-------|------|-----------|----------|-------------|
| `ERBVersionId` | raw | string | No | - |
| `Name` | raw | string | Yes | - |
| `Message` | raw | string | Yes | - |
| `Notes` | raw | string | Yes | - |
| `CommitDate` | raw | datetime | Yes | - |
| `IsPublished` | raw | boolean | Yes | - |


#### Sample Data (8 records)

| Field | Value |
|-------|-------|
| `ERBVersionId` | v2026-01-16t20-44-58-844z |
| `Name` | v2026-01-16T20:44:58.844Z |
| `Message` | Initial demo |
| `CommitDate` | 2026-01-16 |
| `IsPublished` | true |
| `Notes` |  |

---

### Table: ERBCustomizations

> Table: ERBCustomizations

#### Schema

| Field | Type | Data Type | Nullable | Description |
|-------|------|-----------|----------|-------------|
| `ERBCustomizationId` | raw | string | No | - |
| `Name` | raw | string | Yes | - |
| `Title` | raw | string | Yes | - |
| `SQLCode` | raw | string | Yes | - |
| `SQLTarget` | raw | string | Yes | - |
| `CustomizationType` | raw | string | Yes | - |


#### Sample Data (5 records)

| Field | Value |
|-------|-------|
| `ERBCustomizationId` | 03a-customize-schema-sql |
| `Name` | 03a-customize-schema.sql |
| `Title` | Customize Schema |
| `SQLCode` | -- ============================================================================
-- CUSTOMIZE SCHEMA - User-defined tables and schema modifications
-- ============================================================================
-- This file is for YOUR custom schema changes that should persist across
-- regeneration of the base ERB files.
--
-- USE THIS FILE FOR:
--   - Additional tables not defined in the rulebook
--   - Extra columns on existing tables (ALTER TABLE)
--   - Custom indexes for performance tuning
--   - Custom constraints or triggers
--
-- IMPORTANT:
--   - This file runs AFTER 01-drop-and-create-tables.sql
--   - The base tables already exist when this runs
--   - This file will NOT be overwritten by ERB regeneration
--
-- EXAMPLES:
--
--   -- Add a custom table
--   CREATE TABLE IF NOT EXISTS audit_log (
--       audit_log_id    TEXT PRIMARY KEY DEFAULT gen_random_uuid()::TEXT,
--       table_name      TEXT NOT NULL,
--       record_id       TEXT NOT NULL,
--       action          TEXT NOT NULL,
--       changed_at      TIMESTAMPTZ DEFAULT NOW()
--   );
--
--   -- Add a column to an existing table
--   ALTER TABLE erb_versions ADD COLUMN IF NOT EXISTS custom_field TEXT;
--
--   -- Add an index for performance
--   CREATE INDEX IF NOT EXISTS idx_custom ON some_table(some_column);
--
-- ============================================================================

-- Your custom schema changes go here:
 |
| `SQLTarget` | Postgres |
| `CustomizationType` | Schema |

---


## Metadata

**Summary:** Airtable export with schema-first type mapping: Schemas, Data, Relationships (FK links), Lookups (INDEX/MATCH), Aggregations (SUMIFS/COUNTIFS/Rollups), and Calculated fields (formulas) in Excel dialect. Field types are determined from Airtable's schema metadata FIRST (no coercion), with intelligent fallback to formula/data analysis only when schema is unavailable.

### Conversion Details

| Property | Value |
|----------|-------|
| Source Base ID | `appqwWQxIWFtyDsiL` |
| Table Count | 6 |
| Tool Version | 2.0.0 |
| Export Mode | schema_first_type_mapping |
| Field Type Mapping | checkbox→boolean, number→number/integer, multipleRecordLinks→relationship, multipleLookupValues→lookup, rollup→aggregation, count→aggregation, formula→calculated |

### Type Inference

- **Enabled:** true
- **Priority:** airtable_metadata (NO COERCION) → formula_analysis → reference_resolution → data_analysis (fallback only)
- **Airtable Type Mapping:** checkbox→boolean, singleLineText→string, multilineText→string, number→number/integer, datetime→datetime, singleSelect→string, email→string, url→string, phoneNumber→string
- **Data Coercion Hierarchy:** Only used as fallback when Airtable schema unavailable: datetime → number → integer → boolean → base64 → json → string
- **Nullable Support:** true
- **Error Value Handling:** #NUM!, #ERROR!, #N/A, #REF!, #DIV/0!, #VALUE!, #NAME? are treated as NULL

---

*Generated from effortless-rulebook.json*


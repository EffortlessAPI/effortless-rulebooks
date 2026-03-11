# Specification Document for DEMO: StarTrek Rulebook

## Overview
This document provides a detailed specification for the calculated fields within the DEMO: StarTrek rulebook. The rulebook includes data about various Star Trek series, their seasons, episodes, and ratings. The calculated fields derive values based on existing raw data, allowing for insights such as total episodes, series ratings, and whether a series is considered long-running or good.

---

## Entities and Calculated Fields

### 1. Series

#### Input Fields
- **SerieId**
  - **Type**: String
  - **Description**: Unique identifier for the series.

- **SeriesNumber**
  - **Type**: Integer
  - **Description**: The numerical order of the series in the Star Trek franchise.

- **ShowCode**
  - **Type**: String
  - **Description**: A short code representing the series.

#### Calculated Fields
- **Name**
  - **Description**: Combines the SeriesNumber and ShowCode to create a formatted name for the series.
  - **Formula**: `={{SeriesNumber}} & "-" & {{ShowCode}}`
  - **Example**: For SeriesNumber `1` and ShowCode `TOS`, the Name would be `1-TOS`.

- **TotalSeasons**
  - **Description**: Counts the number of seasons associated with the series.
  - **Formula**: `=COUNTIFS(Seasons!{{Series}}, Series!{{SerieId}})`
  - **Example**: For SerieId `1-tos`, if there are 3 seasons, TotalSeasons would be `3`.

- **TotalEpisodes**
  - **Description**: Sums the episode counts from all seasons associated with the series.
  - **Formula**: `=SUMIFS(Seasons!{{EpisodeCount}}, Seasons!{{Series}}, Series!{{SerieId}})`
  - **Example**: If the series has 3 seasons with episode counts of 26, 26, and 24, TotalEpisodes would be `76`.

- **IsLongRunning**
  - **Description**: Determines if the series has more than 50 episodes.
  - **Formula**: `={{TotalEpisodes}} > 50`
  - **Example**: For TotalEpisodes `76`, IsLongRunning would be `true`.

- **Rating**
  - **Description**: Sums the ratings given to the series by users.
  - **Formula**: `=SUMIFS(Ratings!{{Rating}}, Ratings!{{Series}}, Series!{{SerieId}})`
  - **Example**: If the series has ratings of 5, 5, and 3, the Rating would be `13`.

- **IsGood**
  - **Description**: Checks if the average rating is 4.5 or higher.
  - **Formula**: `={{Rating}} >= 4.5`
  - **Example**: For a Rating of `4.333`, IsGood would be `false`.

---

### 2. Seasons

#### Input Fields
- **SeasonId**
  - **Type**: String
  - **Description**: Unique identifier for the season.

- **Series**
  - **Type**: String
  - **Description**: Identifier linking to the associated series.

- **SeasonNumber**
  - **Type**: Integer
  - **Description**: The numerical order of the season within its series.

#### Calculated Fields
- **Name**
  - **Description**: Combines the Series identifier and SeasonNumber to create a formatted name for the season.
  - **Formula**: `={{Series}} & "-Season-" & {{SeasonNumber}}`
  - **Example**: For Series `1-tos` and SeasonNumber `1`, the Name would be `1-tos-Season-1`.

- **EpisodeCount**
  - **Description**: Counts the number of episodes in the season.
  - **Formula**: `=COUNTIFS(Episodes!{{Season}}, Seasons!{{SeasonId}})`
  - **Example**: If there are 26 episodes in Season `1-tos-season-1`, EpisodeCount would be `26`.

---

### 3. Episodes

#### Input Fields
- **EpisodeId**
  - **Type**: String
  - **Description**: Unique identifier for the episode.

- **Season**
  - **Type**: String
  - **Description**: Identifier linking to the associated season.

- **EpisodeNumber**
  - **Type**: Integer
  - **Description**: The numerical order of the episode within its season.

#### Calculated Fields
- **Name**
  - **Description**: Combines the Season identifier and EpisodeNumber to create a formatted name for the episode.
  - **Formula**: `={{Season}} & "-Episode-" & IF({{EpisodeNumber}} < 10, '0', '') & {{EpisodeNumber}}`
  - **Example**: For Season `1-tos-season-1` and EpisodeNumber `1`, the Name would be `1-tos-season-1-Episode-01`.

---

### 4. Ratings

#### Input Fields
- **RatingId**
  - **Type**: String
  - **Description**: Unique identifier for the rating.

- **Rating**
  - **Type**: String
  - **Description**: The rating value given by a user.

- **Series**
  - **Type**: String
  - **Description**: Identifier linking to the associated series.

- **Episode**
  - **Type**: String
  - **Description**: Identifier linking to the associated episode.

#### Calculated Fields
- **DisplayName**
  - **Description**: Creates a display name based on whether the rating is for a series or an episode.
  - **Formula**: `=IF({{Series}} = BLANK(), "Episode: " & {{EpisodeName}}, "Series: " & {{SeriesName}})`
  - **Example**: For Series `1-tos`, the DisplayName would be `Series: 1-TOS`.

- **Name**
  - **Description**: Combines the DisplayName and UsersName to create a unique name for the rating.
  - **Formula**: `={{DisplayName}} & "-" & {{UsersName}}`
  - **Example**: For DisplayName `Series: 1-TOS` and UsersName `Riley Shaw`, the Name would be `Series: 1-TOS-Riley Shaw`.

---

This specification document outlines the necessary steps and formulas to compute calculated fields for the DEMO: StarTrek rulebook, allowing users to derive meaningful insights from the data.
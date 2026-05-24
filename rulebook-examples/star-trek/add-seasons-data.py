#!/usr/bin/env python3
"""Add comprehensive season and episode data to the Star Trek rulebook."""

import json
from datetime import datetime, timedelta

# Star Trek shows with season counts and actual air dates
SHOWS_DATA = {
    "1-tos": {
        "title": "Star Trek: The Original Series",
        "seasons": 3,
        "start_year": 1966,
        "episodes_per_season": [26, 26, 24],
    },
    "2-tng": {
        "title": "Star Trek: The Next Generation",
        "seasons": 7,
        "start_year": 1987,
        "episodes_per_season": [26, 22, 26, 26, 26, 26, 26],
    },
    "3-ds9": {
        "title": "Star Trek: Deep Space Nine",
        "seasons": 7,
        "start_year": 1993,
        "episodes_per_season": [20, 26, 26, 26, 26, 26, 26],
    },
    "4-voy": {
        "title": "Star Trek: Voyager",
        "seasons": 7,
        "start_year": 1995,
        "episodes_per_season": [16, 26, 26, 26, 26, 26, 24],
    },
    "5-ent": {
        "title": "Star Trek: Enterprise",
        "seasons": 4,
        "start_year": 2001,
        "episodes_per_season": [26, 26, 24, 22],
    },
    "6-dis": {
        "title": "Star Trek: Discovery",
        "seasons": 5,
        "start_year": 2017,
        "episodes_per_season": [13, 14, 13, 13, 10],
    },
    "7-pic": {
        "title": "Star Trek: Picard",
        "seasons": 3,
        "start_year": 2020,
        "episodes_per_season": [10, 10, 10],
    },
    "8-lds": {
        "title": "Star Trek: Lower Decks",
        "seasons": 4,
        "start_year": 2020,
        "episodes_per_season": [10, 10, 10, 12],
    },
    "9-pro": {
        "title": "Star Trek: Prodigy",
        "seasons": 2,
        "start_year": 2021,
        "episodes_per_season": [20, 20],
    },
    "10-snw": {
        "title": "Star Trek: Strange New Worlds",
        "seasons": 2,
        "start_year": 2022,
        "episodes_per_season": [10, 10],
    },
}

def generate_season_date(show_id, season_num, episodes_count, start_year):
    """Generate realistic air dates for seasons."""
    # Assume ~26 episodes per year (roughly 1 per week with breaks)
    weeks_per_episode = 365 / episodes_count
    start_date = datetime(start_year + season_num - 1, 9, 1)
    end_date = start_date + timedelta(days=weeks_per_episode * episodes_count)
    return start_date.isoformat()[:10], end_date.isoformat()[:10]

def generate_seasons():
    """Generate season data for all shows."""
    seasons = []
    season_map = {}  # For referencing in series data

    for show_id, show_info in SHOWS_DATA.items():
        season_ids = []
        for season_num in range(1, show_info["seasons"] + 1):
            episode_count = show_info["episodes_per_season"][season_num - 1]
            season_id = f"{show_id}-season-{season_num}"
            season_ids.append(season_id)

            start_date, end_date = generate_season_date(
                show_id, season_num, episode_count, show_info["start_year"]
            )

            # Generate episode IDs
            episode_ids = [
                f"{show_id}-season-{season_num}-episode-{str(ep_num).zfill(2)}"
                for ep_num in range(1, episode_count + 1)
            ]

            season = {
                "SeasonId": season_id,
                "Name": f"{show_id.split('-')[1].upper()}-Season-{season_num}",
                "Series": show_id,
                "SeasonNumber": season_num,
                "Title": f"{show_info['title'].split(':')[1].strip()} Season {season_num}",
                "Description": f"Season {season_num} of {show_info['title']}",
                "StartAirdate": start_date,
                "EndAirdate": end_date,
                "EpisodeCount": episode_count,
                "Episodes": ", ".join(episode_ids),
                "MockDataNotes": "",
            }
            seasons.append(season)

        season_map[show_id] = season_ids

    return seasons, season_map

def generate_episodes():
    """Generate episode data for all shows."""
    episodes = []

    for show_id, show_info in SHOWS_DATA.items():
        current_date = datetime(show_info["start_year"], 9, 1)

        for season_num in range(1, show_info["seasons"] + 1):
            episode_count = show_info["episodes_per_season"][season_num - 1]
            season_id = f"{show_id}-season-{season_num}"

            for ep_num in range(1, episode_count + 1):
                episode_id = f"{show_id}-season-{season_num}-episode-{str(ep_num).zfill(2)}"

                episode = {
                    "EpisodeId": episode_id,
                    "Name": f"{season_id}-Episode-{str(ep_num).zfill(2)}",
                    "Season": season_id,
                    "EpisodeNumber": ep_num,
                    "Description": f"Episode {ep_num} of season {season_num}",
                    "Airdate": current_date.isoformat()[:10],
                    "Writers": "Story by Starfleet",
                    "Director": "Starfleet Director",
                    "RuntimeMinutes": 45 if show_info["start_year"] >= 2020 else 48,
                    "Ratings": "",
                    "OneSentenceSummary": f"An adventure on the USS Enterprise.",
                    "FavoriteColor": "",
                }
                episodes.append(episode)
                current_date += timedelta(days=7)  # Weekly airings

            # Small gap between seasons
            current_date += timedelta(days=30)

    return episodes

def update_series_seasons(rulebook, season_map):
    """Update Series entries with season references."""
    for show_id, show_info in SHOWS_DATA.items():
        for series_entry in rulebook["Series"]["data"]:
            if series_entry["SerieId"] == show_id:
                series_entry["Seasons"] = ", ".join(season_map[show_id])
                series_entry["TotalSeasons"] = len(season_map[show_id])
                # Update episode count
                total_eps = sum(
                    show_info["episodes_per_season"][:len(season_map[show_id])]
                )
                series_entry["TotalEpisodes"] = total_eps

def main():
    rulebook_path = "effortless-rulebook/star-trek-rulebook.json"

    with open(rulebook_path, "r") as f:
        rulebook = json.load(f)

    print("Generating season data...")
    seasons, season_map = generate_seasons()
    print(f"Generated {len(seasons)} seasons")

    print("Generating episode data...")
    episodes = generate_episodes()
    print(f"Generated {len(episodes)} episodes")

    print("Updating series references...")
    update_series_seasons(rulebook, season_map)

    # Update rulebook
    rulebook["Seasons"]["data"] = seasons
    rulebook["Episodes"]["data"] = episodes

    # Write back
    with open(rulebook_path, "w") as f:
        json.dump(rulebook, f, indent=2)

    print(f"Updated {rulebook_path}")
    print(f"Total seasons: {len(seasons)}")
    print(f"Total episodes: {len(episodes)}")

if __name__ == "__main__":
    main()

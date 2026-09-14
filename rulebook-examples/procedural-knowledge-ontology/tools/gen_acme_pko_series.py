#!/usr/bin/env python3
"""Scaffold the "Effortless ACME Corp: PKO" video series in the producer repo.

Copies table SCHEMA (field defs) verbatim from the reference video (03-closure)
so every video in the new series matches its exact structural shape, and fills
in DATA per video from the content spec below.
"""
from __future__ import annotations

import json
import uuid
from collections import OrderedDict
from pathlib import Path

PRODUCER = Path("/Users/eejai42/development/effortless-videos")
REF = PRODUCER / "series/03-effortless-demos/03-closure/effortless-rulebook/effortless-rulebook.json"
SERIES_DIR = PRODUCER / "series/17-effortless-acme-corp-pko"

with REF.open() as fh:
    ref = json.load(fh, object_pairs_hook=OrderedDict)

TABLE_SCHEMAS = {t: ref[t]["schema"] for t in
                 ["Storyboards", "Scenes", "Acts", "Shorts", "Assets", "Clips", "MusicCues"]}
META_SCHEMA = ref["__meta__"]["schema"]


def scene_name(storyboard_title: str, order: int, title: str) -> str:
    return f"{storyboard_title} / {order:02d} - {title}"


def build_rulebook(spec: dict) -> OrderedDict:
    title = spec["title"]
    rb = OrderedDict()
    rb["$schema"] = ref["$schema"]
    rb["Name"] = title
    rb["Description"] = spec["description"]

    rb["Storyboards"] = OrderedDict([
        ("schema", TABLE_SCHEMAS["Storyboards"]),
        ("data", [OrderedDict([
            ("Title", title),
            ("Logline", spec["logline"]),
            ("WorkingTitle", spec["working_title"]),
            ("TargetLength", spec["target_length"]),
            ("Voice", spec["voice"]),
            ("RunningExample", spec["running_example"]),
            ("HowToUse", spec["how_to_use"]),
            ("OutputFileName", spec["output_file"]),
            ("Status", spec["status"]),
        ])]),
    ])

    rb["Acts"] = OrderedDict([
        ("schema", TABLE_SCHEMAS["Acts"]),
        ("data", [OrderedDict([
            ("ActId", a["id"]), ("Storyboard", title), ("ActOrder", a["order"]),
            ("ActNumeral", a["numeral"]), ("ActTitle", a["act_title"]),
            ("Timecode", a["timecode"]), ("Tagline", a["tagline"]),
        ]) for a in spec["acts"]]),
    ])

    scenes_data = []
    assets_data = []
    clips_data = []
    for sc in spec["scenes"]:
        scenes_data.append(OrderedDict([
            ("Storyboard", title), ("SceneOrder", sc["order"]), ("SceneTitle", sc["title"]),
            ("Purpose", sc["purpose"]), ("Act", sc["act"]), ("Timecode", sc["timecode"]),
            ("Script", sc["script"]),
        ]))
        vo_key = f"vo-{sc['order']:02d}"
        visual_key = f"visual-{sc['order']:02d}"
        assets_data.append(OrderedDict([
            ("AssetKey", vo_key), ("Kind", "Voiceover"),
            ("FilePath", f"assets/vo/{vo_key}.wav"),
            ("SourceCommand", ""), ("IsReady", False),
        ]))
        assets_data.append(OrderedDict([
            ("AssetKey", visual_key), ("Kind", "ScreenRecording"),
            ("FilePath", f"assets/screencasts/{visual_key}.mp4"),
            ("SourceCommand", ""), ("IsReady", False),
        ]))
        clips_data.append(OrderedDict([
            ("Scene", scene_name(title, sc["order"], sc["title"])),
            ("Asset", visual_key), ("ClipOrder", 1),
            ("TrimInSeconds", None), ("TrimOutSeconds", None), ("Transition", "Fade"),
        ]))

    rb["Scenes"] = OrderedDict([("schema", TABLE_SCHEMAS["Scenes"]), ("data", scenes_data)])
    rb["Shorts"] = OrderedDict([("schema", TABLE_SCHEMAS["Shorts"]), ("data", [])])
    rb["Assets"] = OrderedDict([("schema", TABLE_SCHEMAS["Assets"]), ("data", assets_data)])
    rb["Clips"] = OrderedDict([("schema", TABLE_SCHEMAS["Clips"]), ("data", clips_data)])
    rb["MusicCues"] = OrderedDict([("schema", TABLE_SCHEMAS["MusicCues"]), ("data", [])])

    rb["__meta__"] = OrderedDict([
        ("schema", META_SCHEMA),
        ("data", [
            OrderedDict([("Name", "render.fps"), ("Value", "30"), ("Notes", "Frames per second for the final render.")]),
            OrderedDict([("Name", "render.width"), ("Value", "1280"), ("Notes", "Output width in pixels.")]),
            OrderedDict([("Name", "render.height"), ("Value", "720"), ("Notes", "Output height in pixels.")]),
            OrderedDict([("Name", "vo.words_per_minute"), ("Value", "150"), ("Notes", "Assumed narration speaking rate, used to estimate voiceover timing from WordCount.")]),
            OrderedDict([("Name", "vo.voice"), ("Value", "11labs:rsSs1fIfZ5iK01FBtCIr"), ("Notes", "Narration voice, same as the reference video (03-closure), pending a season-specific choice.")]),
            OrderedDict([("Name", "story.version"), ("Value", "v0.01-placeholder"), ("Notes", "First-pass scripts capturing the arc. Expect revisions once assembly starts.")]),
            OrderedDict([("Name", "story.running_example"), ("Value", spec["running_example"]), ("Notes", "The real ACME Corp procedure/finding this episode is built around.")]),
            OrderedDict([("Name", "story.assets_mode"), ("Value", "placeholder"), ("Notes", "No visuals or VO recorded yet. Every Asset is IsReady:false.")]),
            OrderedDict([("Name", "story.structure"), ("Value", spec["structure"]), ("Notes", "")]),
        ]),
    ])
    return rb


def write_video(slug: str, spec: dict):
    video_dir = SERIES_DIR / slug
    (video_dir / "effortless-rulebook").mkdir(parents=True, exist_ok=True)
    rb = build_rulebook(spec)
    with (video_dir / "effortless-rulebook/effortless-rulebook.json").open("w") as fh:
        json.dump(rb, fh, indent=2, ensure_ascii=False)
        fh.write("\n")

    project_name = f"effortless-vid-acme-pko-{slug}"
    ej = OrderedDict([
        ("ShowHidden", False), ("ShowAllFiles", False), ("CurrentPath", None),
        ("SSoTmeProjectFiles", None), ("SSoTmeProjectId", str(uuid.uuid4())),
        ("Name", project_name),
        ("ProjectSettings", [OrderedDict([("Name", "project-name"), ("Value", project_name)])]),
        ("ProjectTranspilers", [
            OrderedDict([
                ("IsSSoTTranspiler", False), ("Name", "rulebooktorulespeak"),
                ("RelativePath", "/rulespeak"),
                ("CommandLine", "rulebook-to-rulespeak -i ../effortless-rulebook/effortless-rulebook.json"),
                ("IsDisabled", False),
            ]),
            OrderedDict([
                ("IsSSoTTranspiler", False), ("Name", "JsonHbarsTransform"),
                ("RelativePath", "/storyboard-doc"),
                ("CommandLine", "json-hbars-transform -i ../effortless-rulebook/effortless-rulebook.json -i storyboard.hbars -o ../STORYBOARD.md"),
                ("IsDisabled", False),
            ]),
        ]),
    ])
    with (video_dir / "effortless.json").open("w") as fh:
        json.dump(ej, fh, indent=2, ensure_ascii=False)
        fh.write("\n")
    print(f"wrote {video_dir}")


if __name__ == "__main__":
    import series_content
    SERIES_DIR.mkdir(parents=True, exist_ok=True)
    with (SERIES_DIR / "series.json").open("w") as fh:
        json.dump(OrderedDict([
            ("Title", "Effortless ACME Corp: PKO"),
            ("Description", "A dramatized but honest walkthrough of ACME Corporation's Procedure "
                             "Register: two real procedures (the quarter-end close, a workforce "
                             "policy rollout), told role by role, then a two-part finale on the "
                             "rulebook and tooling that generates it. Every finding shown is a real "
                             "witnessed value from procedural-knowledge-ontology; UI not yet built "
                             "is only shown when the access rules and data behind it already exist."),
        ]), fh, indent=2, ensure_ascii=False)
        fh.write("\n")
    for slug, spec in series_content.VIDEOS.items():
        write_video(slug, spec)
    print("done")

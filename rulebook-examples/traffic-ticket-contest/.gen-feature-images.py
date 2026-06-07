#!/usr/bin/env python3
"""
Generate a cartoon illustration for each ERBFeatures row using gpt-image-1,
downscale to 300x300, and save under assets/feature-images/<ERBFeatureId>.png.

Reuses the per-feature image prompts authored in .author-data.py (the _img_prompt
field on each feature) so the imagery stays in lockstep with the data.

Idempotent: skips a feature whose PNG already exists unless --force is given.
"""
import os, sys, io, base64, importlib.util, time

HERE = os.path.dirname(os.path.abspath(__file__))
OUT_DIR = os.path.join(HERE, "assets", "feature-images")
os.makedirs(OUT_DIR, exist_ok=True)

# Import ROWS from the authoring module to reuse the _img_prompt definitions.
spec = importlib.util.spec_from_file_location("author_data", os.path.join(HERE, ".author-data.py"))
author = importlib.util.module_from_spec(spec)
# author-data's main() runs only under __main__, so importing is side-effect-free for ROWS.
spec.loader.exec_module(author)
FEATURES = author.ROWS["ERBFeatures"]

STYLE = ("Flat 2D cartoon illustration, bold clean outlines, bright cheerful colors, "
         "simple friendly characters, white background, centered composition, no text labels, "
         "modern flat vector sticker style. Subject: ")

from openai import OpenAI
from PIL import Image
client = OpenAI()

def gen_one(feature, force=False):
    fid = feature["ERBFeatureId"]
    out_path = os.path.join(OUT_DIR, f"{fid}.png")
    if os.path.exists(out_path) and not force:
        return ("skip", fid, os.path.getsize(out_path))
    prompt = STYLE + feature["_img_prompt"]
    resp = client.images.generate(model="gpt-image-1", prompt=prompt, size="1024x1024", n=1)
    raw = base64.b64decode(resp.data[0].b64_json)
    img = Image.open(io.BytesIO(raw)).convert("RGBA").resize((300, 300), Image.LANCZOS)
    img.save(out_path, "PNG", optimize=True)
    return ("ok", fid, os.path.getsize(out_path))

def main():
    force = "--force" in sys.argv
    only = None
    for a in sys.argv[1:]:
        if a.startswith("--only="):
            only = set(a.split("=", 1)[1].split(","))
    todo = [f for f in FEATURES if not only or f["ERBFeatureId"] in only]
    print(f"Generating {len(todo)} feature image(s) -> {OUT_DIR}")
    results = []
    for i, f in enumerate(todo, 1):
        fid = f["ERBFeatureId"]
        try:
            status, _, size = gen_one(f, force=force)
            print(f"  [{i}/{len(todo)}] {status:4}  {fid:32} {size/1024:6.1f} KB")
            results.append((fid, status))
        except Exception as e:
            print(f"  [{i}/{len(todo)}] ERR   {fid:32} {type(e).__name__}: {str(e)[:160]}")
            results.append((fid, "error"))
    ok = sum(1 for _, s in results if s in ("ok", "skip"))
    print(f"\nDone: {ok}/{len(todo)} present. errors: {[fid for fid,s in results if s=='error']}")

if __name__ == "__main__":
    main()

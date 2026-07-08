#!/usr/bin/env python3
"""
Generate thumbnail for Article 11 — ABox Materialization.

Steps:
  1. Call DALL-E 3 to generate the base image.
  2. Overlay series text, part label, and Effortless logo via PIL.
  3. Save final PNG to thumbnails/final/11-abox-materialization.png

Usage:
  OPENAI_API_KEY=sk-... python3 generate-11.py
"""

import os
import sys
import urllib.request
import io
from pathlib import Path

try:
    from PIL import Image, ImageDraw, ImageFont
except ImportError:
    sys.exit("Install Pillow first: pip install Pillow")

try:
    import openai
except ImportError:
    sys.exit("Install openai first: pip install openai")

# ── Paths ──────────────────────────────────────────────────────────────────────
HERE = Path(__file__).parent
RAW_PATH   = HERE / "11-abox-materialization.png"
FINAL_PATH = HERE / "final" / "11-abox-materialization.png"
LOGO_PATH  = HERE / "effortless-logo-extracted.png"

FINAL_PATH.parent.mkdir(parents=True, exist_ok=True)

# ── 1. Generate base image with DALL-E 3 ─────────────────────────────────────
DALLE_PROMPT = (
    "A dramatic, dark cinematic photograph: a vast ancient library seen from above, "
    "its shelves stretching to infinity, lit by warm amber lantern light against deep "
    "black shadow. In the foreground, an enormous open book rests on a stone lectern; "
    "its pages glow with softly luminous text that has already been filled in — the "
    "answers written before any reader arrived. Fine lines of golden light connect the "
    "book to distant shelves like a web of pre-computed knowledge. The mood is quiet, "
    "certain, complete. No people. Photorealistic, ultra-detailed, 16:9 ratio."
)

print("Calling DALL-E 3…")
client = openai.OpenAI(api_key=os.environ["OPENAI_API_KEY"])

response = client.images.generate(
    model="gpt-image-1",
    prompt=DALLE_PROMPT,
    size="1536x1024",
    quality="high",
    n=1,
)

import base64
raw_bytes = base64.b64decode(response.data[0].b64_json)
print("Image received (base64).")

img = Image.open(io.BytesIO(raw_bytes)).convert("RGB")
if img.size != (1536, 1024):
    img = img.resize((1536, 1024), Image.LANCZOS)
img.save(RAW_PATH)
print(f"Raw image saved → {RAW_PATH}")

# ── 2. Overlay text + logo ─────────────────────────────────────────────────────
W, H = 1536, 1024
draw = ImageDraw.Draw(img)

# -- Fonts: try system fonts, fall back to default --
def load_font(size, bold=False):
    candidates = []
    if bold:
        candidates = [
            "/System/Library/Fonts/Supplemental/Georgia Bold.ttf",
            "/System/Library/Fonts/Helvetica.ttc",
            "/usr/share/fonts/truetype/dejavu/DejaVuSerif-Bold.ttf",
            "/usr/share/fonts/truetype/liberation/LiberationSerif-Bold.ttf",
        ]
    else:
        candidates = [
            "/System/Library/Fonts/Supplemental/Georgia.ttf",
            "/System/Library/Fonts/Helvetica.ttc",
            "/usr/share/fonts/truetype/dejavu/DejaVuSerif.ttf",
            "/usr/share/fonts/truetype/liberation/LiberationSerif-Regular.ttf",
        ]
    for path in candidates:
        if Path(path).exists():
            return ImageFont.truetype(path, size)
    return ImageFont.load_default()

font_small  = load_font(28, bold=False)
font_part   = load_font(38, bold=True)
font_hook   = load_font(128, bold=True)

# -- Dark gradient at bottom for legibility --
from PIL import ImageFilter
gradient = Image.new("RGBA", (W, H), (0, 0, 0, 0))
gd = ImageDraw.Draw(gradient)
for y in range(H // 2, H):
    alpha = int(220 * ((y - H // 2) / (H // 2)) ** 1.5)
    gd.line([(0, y), (W, y)], fill=(0, 0, 0, alpha))
img = img.convert("RGBA")
img = Image.alpha_composite(img, gradient)

draw = ImageDraw.Draw(img)

# -- Series text top-left --
SERIES_LINE1 = "Trust the Artifact, Not the Autocomplete"
SERIES_LINE2 = "Part 11 of 10 — Bonus"

draw.text((36, 30), SERIES_LINE1, font=font_small, fill=(255, 255, 255, 220))
draw.text((36, 66), SERIES_LINE2,  font=font_part,  fill=(255, 255, 255, 240))

# -- Logo top-right --
if LOGO_PATH.exists():
    logo = Image.open(LOGO_PATH).convert("RGBA")
    logo_size = 110
    logo = logo.resize((logo_size, logo_size), Image.LANCZOS)
    margin = 24
    img.paste(logo, (W - logo_size - margin, margin), logo)
else:
    print(f"Warning: logo not found at {LOGO_PATH}, skipping logo overlay")

draw = ImageDraw.Draw(img)

# -- Big hook text bottom-left --
HOOK_LINE1 = "ABox"
HOOK_LINE2 = "Hydrated."

shadow_offset = 4
for line, y_pos in [(HOOK_LINE1, H - 260), (HOOK_LINE2, H - 140)]:
    draw.text((36 + shadow_offset, y_pos + shadow_offset), line,
              font=font_hook, fill=(0, 0, 0, 160))
    draw.text((36, y_pos), line, font=font_hook, fill=(255, 255, 255, 255))

# ── 3. Save final ─────────────────────────────────────────────────────────────
final = img.convert("RGB")
final.save(FINAL_PATH)
print(f"Final thumbnail saved → {FINAL_PATH}")

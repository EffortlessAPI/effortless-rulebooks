#!/usr/bin/env python3
"""Generate tiling-the-plane-rulebook.json — the hand-authored SSoT.

Domain: tiling the plane. Two halves that form one DAG:
  CATALOG  — the mathematics of which tilings exist and why they're valid.
  GENERATIVE — placing prototiles into a bounded region and measuring coverage.

DAG rule: 1-to-many only, no cycles, no many-to-many. Junctions are first-class
entities (TilingPrototiles, VertexFigures, Placements).
"""
import json, os, math

# ---- field helpers -------------------------------------------------------
def raw(name, dt, desc, nullable=True, pk=False):
    f = {"name": name, "datatype": dt, "type": "raw", "nullable": nullable, "Description": desc}
    if pk:
        f["is_primary_key"] = True
        f["nullable"] = False
    return f

def rel(name, target, desc, nullable=True):
    return {"name": name, "datatype": "string", "type": "relationship",
            "nullable": nullable, "RelatedTo": target, "Description": desc}

def calc(name, dt, formula, desc, nullable=True):
    return {"name": name, "datatype": dt, "type": "calculated",
            "nullable": nullable, "formula": formula, "Description": desc}

def lookup(name, dt, formula, desc, nullable=True):
    return {"name": name, "datatype": dt, "type": "lookup",
            "nullable": nullable, "formula": formula, "Description": desc}

def lk(name, dt, this_tbl, fk, target_tbl, target_field, target_pk, desc, nullable=True):
    """Lookup via the transpiler-supported INDEX/MATCH idiom (LOOKUP() is NOT compiled)."""
    formula = (f"=INDEX({target_tbl}!{{{{{target_field}}}}}, "
               f"MATCH({this_tbl}!{{{{{fk}}}}}, {target_tbl}!{{{{{target_pk}}}}}, 0))")
    return lookup(name, dt, formula, desc, nullable)

def agg(name, dt, formula, desc, nullable=True):
    return {"name": name, "datatype": dt, "type": "aggregation",
            "nullable": nullable, "formula": formula, "Description": desc}

def table(desc, schema, data, important=True):
    return {"Description": desc, "important": important, "schema": schema, "data": data}

rb = {
    "$schema": "https://example.com/cmcc-schema/v1",
    "Name": "Tiling the Plane",
    "Description": ("A library for tiling the Euclidean plane. The catalog half models which "
                    "tilings exist and why each vertex figure is valid (interior angles summing to "
                    "360deg); the generative half places prototiles into a bounded region and measures "
                    "coverage, gaps, and overlaps. One DAG of Schema/Data/Lookups/Aggregations/Formulas."),
}

# ========================================================================
# __meta__  (typed-row hybrid — project-level metadata)
# ========================================================================
meta_schema = [
    raw("MetaKey", "string", "The metadata key. Unique within the table.", nullable=False, pk=True),
    calc("Name", "string", "={{MetaKey}}", "Mirrors MetaKey so the row is addressable by Name.", nullable=False),
    raw("ValueType", "string", "How to interpret the value: 'string' | 'object' | 'array'.", nullable=False),
    raw("StringValue", "string", "Plain string value (when ValueType=='string')."),
    raw("JsonValue", "string", "JSON-encoded value (when ValueType is 'object' or 'array')."),
]
def m_s(k, v): return {"MetaKey": k, "ValueType": "string", "StringValue": v, "JsonValue": None}
def m_j(k, v, kind="array"): return {"MetaKey": k, "ValueType": kind, "StringValue": None, "JsonValue": json.dumps(v)}
meta_data = [
    m_s("tagline", "Tile the plane, then prove the tiling holds."),
    m_s("motif", "isometric-lattice"),
    m_j("motif_palette", ["#1f6feb", "#2ea043", "#d29922", "#a371f7"]),
    m_s("description_rich", "A two-sided rulebook: a catalog of plane tilings (regular, "
        "semiregular/Archimedean) whose validity is *derived* from vertex angle sums, plus a "
        "generative engine that places tiles in a region and computes coverage/gaps/overlaps live."),
    m_j("use_cases", [
        "Browse the regular and Archimedean tilings and see WHY each vertex configuration closes to 360deg.",
        "Lay tiles into a region and watch coverage, gaps, and overlaps recompute from the views.",
        "Use as a teaching surface for symmetry groups and edge-to-edge tilings.",
    ]),
    m_j("substrates", ["postgres"]),
]
rb["__meta__"] = table("Project-level metadata that travels with the rulebook.", meta_schema, meta_data, important=False)

# ========================================================================
# SymmetryGroups  (reference: the 17 wallpaper groups + a few used here)
# ========================================================================
sg_schema = [
    raw("SymmetryGroupId", "string", "Stable id (IUC short notation, slugified).", nullable=False, pk=True),
    calc("Name", "string", '={{SymmetryGroupId}}', "Mirrors the id.", nullable=False),
    raw("Notation", "string", "IUC / crystallographic short notation (e.g. 'p6m').", nullable=False),
    raw("Orbifold", "string", "Conway orbifold notation (e.g. '*632')."),
    raw("DisplayName", "string", "Human-friendly name."),
    raw("Description", "string", "What this wallpaper group is."),
    agg("TilingCount", "integer",
        "=COUNTIFS(Tilings!{{SymmetryGroup}}, SymmetryGroups!{{SymmetryGroupId}})",
        "How many catalogued tilings have this symmetry group."),
]
sg_data = [
    {"SymmetryGroupId": "p4m", "Notation": "p4m", "Orbifold": "*442", "DisplayName": "Square symmetry",
     "Description": "Full symmetry of the square lattice; reflections + 4-fold rotations."},
    {"SymmetryGroupId": "p6m", "Notation": "p6m", "Orbifold": "*632", "DisplayName": "Hexagonal symmetry",
     "Description": "Full symmetry of the hexagonal lattice; reflections + 6-fold rotations."},
    {"SymmetryGroupId": "p3", "Notation": "p3", "Orbifold": "333", "DisplayName": "3-fold rotation",
     "Description": "Rotations of order 3, no reflections."},
    {"SymmetryGroupId": "cmm", "Notation": "cmm", "Orbifold": "2*22", "DisplayName": "Centered rhombic",
     "Description": "Two perpendicular reflection axes plus 2-fold rotations."},
]
rb["SymmetryGroups"] = table("The wallpaper symmetry groups a tiling can belong to.", sg_schema, sg_data)

# ========================================================================
# Prototiles  (the shapes — regular polygons by number of sides)
# ========================================================================
pt_schema = [
    raw("PrototileId", "string", "Stable id.", nullable=False, pk=True),
    calc("Name", "string", '=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")', "Slug from display name.", nullable=False),
    raw("DisplayName", "string", "Human name (e.g. 'Equilateral Triangle').", nullable=False),
    raw("Sides", "integer", "Number of edges of this regular polygon.", nullable=False),
    raw("EdgeLength", "number", "Edge length in unit cells (1.0 = unit edge).", nullable=False),
    # Area of a regular n-gon with edge s = (1/4) n s^2 cot(pi/n). Trig is outside the
    # transpiler's portable formula vocabulary, so the calc_prototiles_area() function is
    # hand-implemented in postgres-bootstrap/02b-customize-functions.sql (Postgres tan()/pi()).
    calc("Area", "number", "=0.25*{{Sides}}*POWER({{EdgeLength}},2)/TAN(PI()/{{Sides}})",
         "Area of one tile (regular n-gon). Trig override lives in 02b-customize-functions.sql.",
         nullable=True),
    # Interior angle of a regular n-gon = (n-2)*180/n
    calc("InteriorAngleDeg", "number", "=({{Sides}}-2)*180/{{Sides}}",
         "Interior angle in degrees of this regular polygon."),
    calc("VertexAngleTurns", "number", "={{InteriorAngleDeg}}/360",
         "Fraction of a full turn occupied at one vertex by this tile."),
    calc("IsRegular", "boolean", "={{Sides}}>=3",
         "A regular polygon needs at least 3 sides."),
    agg("UsedInTilingsCount", "integer",
        "=COUNTIFS(TilingPrototiles!{{Prototile}}, Prototiles!{{PrototileId}})",
        "How many catalogued tilings use this prototile."),
]
def proto(pid, disp, sides):
    return {"PrototileId": pid, "DisplayName": disp, "Sides": sides, "EdgeLength": 1.0}
pt_data = [
    proto("triangle", "Equilateral Triangle", 3),
    proto("square", "Square", 4),
    proto("hexagon", "Regular Hexagon", 6),
    proto("octagon", "Regular Octagon", 8),
    proto("dodecagon", "Regular Dodecagon", 12),
]
rb["Prototiles"] = table("The prototiles (regular polygons) available for tilings.", pt_schema, pt_data)

# ========================================================================
# Tilings  (catalogued tilings of the plane)
# ========================================================================
ti_schema = [
    raw("TilingId", "string", "Stable id.", nullable=False, pk=True),
    calc("Name", "string", '=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")', "Slug.", nullable=False),
    raw("DisplayName", "string", "Human name (e.g. 'Hexagonal tiling').", nullable=False),
    raw("VertexConfig", "string", "Vertex configuration / Schlafli symbol string (e.g. '3.4.6.4').", nullable=False),
    raw("Kind", "string", "'regular' | 'semiregular' (Archimedean).", nullable=False),
    raw("IsEdgeToEdge", "boolean", "Whether tiles meet edge-to-edge.", nullable=False),
    rel("SymmetryGroup", "SymmetryGroups", "The wallpaper symmetry group of this tiling."),
    lk("SymmetryNotation", "string", "Tilings", "SymmetryGroup",
       "SymmetryGroups", "Notation", "SymmetryGroupId",
       "Pulled symmetry notation for display."),
    agg("DistinctPrototileCount", "integer",
        "=COUNTIFS(TilingPrototiles!{{Tiling}}, Tilings!{{TilingId}})",
        "How many distinct prototiles this tiling uses."),
    agg("VertexFigureCount", "integer",
        "=COUNTIFS(VertexFigures!{{Tiling}}, Tilings!{{TilingId}})",
        "How many distinct vertex figures this tiling has."),
    agg("ValidVertexFigureCount", "integer",
        "=COUNTIFS(VertexFigures!{{Tiling}}, Tilings!{{TilingId}}, VertexFigures!{{IsValid}}, TRUE)",
        "How many of its vertex figures close to 360deg."),
    calc("IsRegularTiling", "boolean", '={{DistinctPrototileCount}}=1',
         "Regular tiling = built from a single prototile."),
    calc("AllVerticesValid", "boolean", '={{ValidVertexFigureCount}}={{VertexFigureCount}}',
         "True when every vertex figure of this tiling closes to a full turn."),
]
def tiling(tid, disp, vc, kind, e2e, sg):
    return {"TilingId": tid, "DisplayName": disp, "VertexConfig": vc, "Kind": kind,
            "IsEdgeToEdge": e2e, "SymmetryGroup": sg}
ti_data = [
    tiling("triangular", "Triangular tiling", "3.3.3.3.3.3", "regular", True, "p6m"),
    tiling("square", "Square tiling", "4.4.4.4", "regular", True, "p4m"),
    tiling("hexagonal", "Hexagonal tiling", "6.6.6", "regular", True, "p6m"),
    tiling("truncated-square", "Truncated square tiling", "4.8.8", "semiregular", True, "p4m"),
    tiling("trihexagonal", "Trihexagonal tiling", "3.6.3.6", "semiregular", True, "p6m"),
    tiling("rhombitrihexagonal", "Rhombitrihexagonal tiling", "3.4.6.4", "semiregular", True, "p6m"),
]
rb["Tilings"] = table("Catalogued tilings of the plane.", ti_schema, ti_data)

# ========================================================================
# TilingPrototiles  (first-class junction: which prototiles a tiling uses)
# ========================================================================
tp_schema = [
    raw("TilingPrototileId", "string", "Stable id.", nullable=False, pk=True),
    calc("Name", "string", '=CONCAT({{Tiling}}, "/", {{Prototile}})', "Composite name.", nullable=False),
    rel("Tiling", "Tilings", "The tiling.", nullable=False),
    rel("Prototile", "Prototiles", "A prototile used by that tiling.", nullable=False),
    raw("CountAtVertex", "integer", "How many of this prototile meet at a typical vertex.", nullable=False),
    lk("PrototileSides", "integer", "TilingPrototiles", "Prototile",
       "Prototiles", "Sides", "PrototileId",
       "Sides of the referenced prototile."),
    lk("PrototileInteriorAngle", "number", "TilingPrototiles", "Prototile",
       "Prototiles", "InteriorAngleDeg", "PrototileId",
       "Interior angle of the referenced prototile."),
]
def tp(tid, tiling_id, proto_id, n):
    return {"TilingPrototileId": tid, "Tiling": tiling_id, "Prototile": proto_id, "CountAtVertex": n}
tp_data = [
    tp("tri-1", "triangular", "triangle", 6),
    tp("sq-1", "square", "square", 4),
    tp("hex-1", "hexagonal", "hexagon", 3),
    tp("tsq-1", "truncated-square", "square", 1),
    tp("tsq-2", "truncated-square", "octagon", 2),
    tp("trihex-1", "trihexagonal", "triangle", 2),
    tp("trihex-2", "trihexagonal", "hexagon", 2),
    tp("rtri-1", "rhombitrihexagonal", "triangle", 1),
    tp("rtri-2", "rhombitrihexagonal", "square", 2),
    tp("rtri-3", "rhombitrihexagonal", "hexagon", 1),
]
rb["TilingPrototiles"] = table("First-class junction: prototiles used by each tiling.", tp_schema, tp_data)

# ========================================================================
# VertexFigures  (first-class: a vertex type, its angle sum, validity)
# ========================================================================
vf_schema = [
    raw("VertexFigureId", "string", "Stable id.", nullable=False, pk=True),
    calc("Name", "string", '=CONCAT({{Tiling}}, "@", {{Config}})', "Composite name.", nullable=False),
    rel("Tiling", "Tilings", "The tiling this vertex figure belongs to.", nullable=False),
    raw("Config", "string", "Vertex configuration at this vertex (e.g. '3.4.6.4').", nullable=False),
    # Sum of the listed interior angles, supplied as data (the angle contributions),
    # and a derived check that they sum to a full turn.
    raw("AngleSumDeg", "number", "Sum of interior angles meeting at this vertex (degrees).", nullable=False),
    calc("TurnsAtVertex", "number", "={{AngleSumDeg}}/360", "Angle sum as a fraction of a full turn."),
    calc("AngleGapDeg", "number", "=ABS({{AngleSumDeg}}-360)", "Distance from a full 360deg turn."),
    calc("IsValid", "boolean", "={{AngleGapDeg}}<=0.0001",
         "Valid vertex figure: interior angles close to a full turn."),
]
def vf(vid, tiling_id, config, angle_sum):
    return {"VertexFigureId": vid, "Tiling": tiling_id, "Config": config, "AngleSumDeg": angle_sum}
# angle sums: triangle 60, square 90, hexagon 120, octagon 135, dodecagon 150
vf_data = [
    vf("vf-tri", "triangular", "3.3.3.3.3.3", 6*60),
    vf("vf-sq", "square", "4.4.4.4", 4*90),
    vf("vf-hex", "hexagonal", "6.6.6", 3*120),
    vf("vf-tsq", "truncated-square", "4.8.8", 90 + 2*135),
    vf("vf-trihex", "trihexagonal", "3.6.3.6", 2*60 + 2*120),
    vf("vf-rtri", "rhombitrihexagonal", "3.4.6.4", 60 + 2*90 + 120),
]
rb["VertexFigures"] = table("First-class vertex figures; validity derived from angle sum = 360.", vf_schema, vf_data)

# ========================================================================
# GENERATIVE HALF
# ========================================================================
# Regions  (a bounded patch of the plane to be tiled)
rg_schema = [
    raw("RegionId", "string", "Stable id.", nullable=False, pk=True),
    calc("Name", "string", '=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")', "Slug.", nullable=False),
    raw("DisplayName", "string", "Human name.", nullable=False),
    raw("Width", "number", "Region width in unit cells.", nullable=False),
    raw("Height", "number", "Region height in unit cells.", nullable=False),
    rel("TargetTiling", "Tilings", "Which catalogued tiling this region is trying to realize."),
    calc("Area", "number", "={{Width}}*{{Height}}", "Total area of the region."),
    agg("PlacementCount", "integer",
        "=COUNTIFS(Placements!{{Region}}, Regions!{{RegionId}})",
        "How many tiles have been placed in this region."),
    agg("CoveredArea", "number",
        "=SUMIFS(Placements!{{TileArea}}, Placements!{{Region}}, Regions!{{RegionId}})",
        "Sum of placed-tile areas (counts overlaps twice — see CoveragePct)."),
    agg("OverlapCount", "integer",
        "=COUNTIFS(Placements!{{Region}}, Regions!{{RegionId}}, Placements!{{IsOverlapping}}, TRUE)",
        "How many placed tiles are flagged as overlapping a neighbour."),
    calc("CoveragePct", "number", "=100*{{CoveredArea}}/{{Area}}",
         "Covered area as a percentage of region area."),
    calc("IsFullyCovered", "boolean", "={{CoveragePct}}>=100",
         "Whether placed tiles cover at least the whole region area."),
    calc("IsCleanTiling", "boolean", "=AND({{IsFullyCovered}}, {{OverlapCount}}=0)",
         "Fully covered with no overlaps = a clean tiling of the region."),
]
def region(rid, disp, w, h, target):
    return {"RegionId": rid, "DisplayName": disp, "Width": w, "Height": h, "TargetTiling": target}
rg_data = [
    region("demo-square-4x4", "Demo Square 4x4", 4, 4, "square"),
    region("demo-hex-strip", "Demo Hex Strip", 6, 2, "hexagonal"),
    region("demo-empty", "Demo Empty Plot", 3, 3, "triangular"),
]
rb["Regions"] = table("Bounded patches of the plane to be tiled.", rg_schema, rg_data)

# Placements  (a prototile dropped into a region at x,y with a rotation)
pl_schema = [
    raw("PlacementId", "string", "Stable id.", nullable=False, pk=True),
    calc("Name", "string", '=CONCAT({{Region}}, "#", {{PlacementId}})', "Composite name.", nullable=False),
    rel("Region", "Regions", "The region this tile is placed in.", nullable=False),
    rel("Prototile", "Prototiles", "The prototile being placed.", nullable=False),
    raw("X", "number", "X coordinate of the tile's anchor (unit cells).", nullable=False),
    raw("Y", "number", "Y coordinate of the tile's anchor (unit cells).", nullable=False),
    raw("RotationDeg", "number", "Rotation of the tile in degrees.", nullable=False),
    raw("IsOverlapping", "boolean", "Flag: this placement overlaps a neighbour (authored/validated).", nullable=False),
    lk("TileArea", "number", "Placements", "Prototile",
       "Prototiles", "Area", "PrototileId",
       "Area of the placed prototile (for coverage rollups)."),
    lk("TileSides", "integer", "Placements", "Prototile",
       "Prototiles", "Sides", "PrototileId",
       "Sides of the placed prototile."),
    lk("RegionWidth", "number", "Placements", "Region",
       "Regions", "Width", "RegionId",
       "Width of the containing region."),
    lk("RegionHeight", "number", "Placements", "Region",
       "Regions", "Height", "RegionId",
       "Height of the containing region."),
    calc("IsInsideRegion", "boolean",
         "=AND({{X}}>=0, {{Y}}>=0, {{X}}<{{RegionWidth}}, {{Y}}<{{RegionHeight}})",
         "Whether the anchor sits within the region bounds."),
]
def place(pid, region_id, proto_id, x, y, rot=0, overlap=False):
    return {"PlacementId": pid, "Region": region_id, "Prototile": proto_id,
            "X": x, "Y": y, "RotationDeg": rot, "IsOverlapping": overlap}
pl_data = []
# Fill the 4x4 demo square cleanly with 16 unit squares.
i = 0
for yy in range(4):
    for xx in range(4):
        i += 1
        pl_data.append(place(f"sq-{i:02d}", "demo-square-4x4", "square", xx, yy))
# Hex strip — a few hexagons (illustrative, partial coverage).
for j, (hx, hy) in enumerate([(0, 0), (1, 0), (2, 0), (3, 0)], start=1):
    pl_data.append(place(f"hex-{j:02d}", "demo-hex-strip", "hexagon", hx, hy))
# demo-empty intentionally has no placements (0% coverage).
rb["Placements"] = table("Individual tile placements within a region.", pl_schema, pl_data)

# ------------------------------------------------------------------------
out = os.path.join(os.path.dirname(__file__), "effortless-rulebook", "tiling-the-plane-rulebook.json")
os.makedirs(os.path.dirname(out), exist_ok=True)
with open(out, "w") as fh:
    json.dump(rb, fh, indent=2)
    fh.write("\n")

# Sanity report
tables = [k for k, v in rb.items() if isinstance(v, dict) and "schema" in v]
print(f"Wrote {out}")
print(f"Tables ({len(tables)}):")
for k in tables:
    nf = len(rb[k]["schema"]); nd = len(rb[k]["data"])
    print(f"  {k:20s} {nf:2d} fields  {nd:3d} rows")

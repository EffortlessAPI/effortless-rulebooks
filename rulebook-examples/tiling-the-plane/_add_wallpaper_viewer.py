#!/usr/bin/env python3
"""Additive rulebook edit: give SymmetryGroups a full symmetry recipe and add a
WallpaperDesigns table. PURELY ADDITIVE — existing fields/data are never touched.

Run once; safe to re-run (idempotent: it skips fields/tables that already exist).

Symmetry model
--------------
Each wallpaper group is described in FRACTIONAL (lattice) coordinates. A point in
the plane is p = u*B1 + v*B2 where B1,B2 are the lattice basis vectors (the
viewer derives B1,B2 from LatticeType + LatticeAngleDeg + LatticeRatio). The
point group is given as a list of COSET REPRESENTATIVE affine ops:

    {"k": "<kind>", "m": [a,b,c,d], "t": [tx,ty]}

mapping (u,v) -> (a*u + b*v + tx,  c*u + d*v + ty), where (tx,ty) are fractions of
the lattice. The viewer then tiles by integer lattice translations i*B1 + j*B2.
The identity is always included implicitly; `Generators` lists the NON-identity
coset reps (so the full motif copy-count in one cell = len(Generators)+1).
"""
import json, os

RB = os.path.join(os.path.dirname(__file__),
                  "effortless-rulebook", "tiling-the-plane-rulebook.json")

# ---- transform helpers (fractional coords) -------------------------------
def op(kind, m, t=(0, 0)):
    return {"k": kind, "m": list(m), "t": [t[0], t[1]]}

# rotations about the origin in a basis where they're integer matrices.
# For square/rectangular/oblique lattices the 2-fold is [-1,0,0,-1].
ID   = (1, 0, 0, 1)
R2   = (-1, 0, 0, -1)                 # 180
# 4-fold on a SQUARE lattice: (u,v) -> (-v, u)
R4   = (0, -1, 1, 0)
R4_3 = (0, 1, -1, 0)
# 3-fold on a HEX lattice (basis 120 apart): (u,v) -> (-v, u-v)
R3   = (0, -1, 1, -1)
R3_2 = (-1, 1, -1, 0)
# 6-fold on a HEX lattice: (u,v) -> (u-v, u)
R6   = (1, -1, 1, 0)
R6_5 = (0, 1, -1, 1)
# mirrors (square/rect): across u-axis, v-axis, diagonals
MX   = (1, 0, 0, -1)                  # mirror in u-axis (flip v)
MY   = (-1, 0, 0, 1)                  # mirror in v-axis (flip u)
MD   = (0, 1, 1, 0)                   # mirror across u=v diagonal
MA   = (0, -1, -1, 0)                 # mirror across u=-v anti-diagonal
# hex mirrors
MH1  = (0, -1, -1, 0)                 # one hex mirror
MH2  = (-1, 1, 0, 1)                  # another hex mirror line
MH3  = (1, 0, 1, -1)

def compose(a, b):
    """matrix a∘b (4-tuples)."""
    a0, a1, a2, a3 = a
    b0, b1, b2, b3 = b
    return (a0 * b0 + a1 * b2, a0 * b1 + a1 * b3,
            a2 * b0 + a3 * b2, a2 * b1 + a3 * b3)

# ---- per-group recipe ----------------------------------------------------
# lattice: type, angle between basis vectors (deg), |B2|/|B1| ratio.
# generators: list of non-identity coset-rep ops.
RECIPE = {
    "p1":   dict(lattice=("oblique", 73, 1.30), ops=[]),
    "p2":   dict(lattice=("oblique", 73, 1.30), ops=[op("rot2", R2)]),
    "pm":   dict(lattice=("rectangular", 90, 1.40), ops=[op("mirror", MY)]),
    "pg":   dict(lattice=("rectangular", 90, 1.40),
                 ops=[op("glide", MY, (0, 0.5))]),
    "cm":   dict(lattice=("rhombic", 70, 1.0),
                 ops=[op("mirror", MD)]),
    "pmm":  dict(lattice=("rectangular", 90, 1.40),
                 ops=[op("rot2", R2), op("mirror", MX), op("mirror", MY)]),
    "pmg":  dict(lattice=("rectangular", 90, 1.40),
                 ops=[op("rot2", R2), op("mirror", MY),
                      op("glide", MX, (0, 0.5))]),
    "pgg":  dict(lattice=("rectangular", 90, 1.40),
                 ops=[op("rot2", R2), op("glide", MX, (0.5, 0.5)),
                      op("glide", MY, (0.5, 0.5))]),
    "cmm":  dict(lattice=("rhombic", 70, 1.0),
                 ops=[op("rot2", R2), op("mirror", MD), op("mirror", MA)]),
    "p4":   dict(lattice=("square", 90, 1.0),
                 ops=[op("rot4", R4), op("rot2", R2), op("rot4", R4_3)]),
    "p4m":  dict(lattice=("square", 90, 1.0),
                 ops=[op("rot4", R4), op("rot2", R2), op("rot4", R4_3),
                      op("mirror", MX), op("mirror", MY),
                      op("mirror", MD), op("mirror", MA)]),
    "p4g":  dict(lattice=("square", 90, 1.0),
                 ops=[op("rot4", R4), op("rot2", R2), op("rot4", R4_3),
                      op("glide", MX, (0.5, 0.5)), op("glide", MY, (0.5, 0.5)),
                      op("mirror", MD), op("mirror", MA)]),
    "p3":   dict(lattice=("hexagonal", 120, 1.0),
                 ops=[op("rot3", R3), op("rot3", R3_2)]),
    "p3m1": dict(lattice=("hexagonal", 120, 1.0),
                 ops=[op("rot3", R3), op("rot3", R3_2),
                      op("mirror", MH1), op("mirror", MH2), op("mirror", MH3)]),
    "p31m": dict(lattice=("hexagonal", 120, 1.0),
                 ops=[op("rot3", R3), op("rot3", R3_2),
                      op("mirror", compose(R3, MH1)),
                      op("mirror", compose(R3_2, MH1)),
                      op("mirror", MH1)]),
    "p6":   dict(lattice=("hexagonal", 120, 1.0),
                 ops=[op("rot6", R6), op("rot3", R3), op("rot2", R2),
                      op("rot3", R3_2), op("rot6", R6_5)]),
    "p6m":  dict(lattice=("hexagonal", 120, 1.0),
                 ops=[op("rot6", R6), op("rot3", R3), op("rot2", R2),
                      op("rot3", R3_2), op("rot6", R6_5),
                      op("mirror", MH1), op("mirror", compose(R6, MH1)),
                      op("mirror", compose(R3, MH1)),
                      op("mirror", compose(R2, MH1)),
                      op("mirror", compose(R3_2, MH1)),
                      op("mirror", compose(R6_5, MH1))]),
}

ROT_ORDER = {"p1": 1, "p2": 2, "pm": 1, "pg": 1, "cm": 1, "pmm": 2, "pmg": 2,
             "pgg": 2, "cmm": 2, "p4": 4, "p4m": 4, "p4g": 4, "p3": 3,
             "p3m1": 3, "p31m": 3, "p6": 6, "p6m": 6}


def field(name, dt, typ, desc, nullable=True, formula=None):
    f = {"name": name, "datatype": dt, "type": typ, "nullable": nullable,
         "Description": desc}
    if formula:
        f["formula"] = formula
    return f


def ensure_field(schema, f):
    if not any(x["name"] == f["name"] for x in schema):
        schema.append(f)
        return True
    return False


def main():
    rb = json.load(open(RB))
    sg = rb["SymmetryGroups"]
    schema, data = sg["schema"], sg["data"]

    # 1) additive fields on SymmetryGroups -------------------------------
    added = []
    for f in [
        field("LatticeType", "string", "raw",
              "Bravais lattice class: oblique | rectangular | rhombic | square | hexagonal."),
        field("LatticeAngleDeg", "number", "raw",
              "Angle (deg) between the two lattice basis vectors B1,B2."),
        field("LatticeRatio", "number", "raw",
              "Length ratio |B2|/|B1| of the lattice basis."),
        field("Generators", "string", "raw",
              "JSON array of non-identity coset-rep affine ops in fractional "
              "coords: {k,m:[a,b,c,d],t:[tx,ty]} mapping (u,v)->(a u+b v+tx, c u+d v+ty)."),
        field("RotationOrder", "integer", "raw",
              "Highest order of rotational symmetry (1,2,3,4,6)."),
        field("OperationCount", "integer", "raw",
              "Number of point-group coset reps including the identity "
              "(= motif copies per lattice cell)."),
        field("HasMirrors", "boolean", "raw",
              "Whether the group contains reflection (mirror) symmetry."),
        field("HasGlides", "boolean", "raw",
              "Whether the group contains glide-reflection symmetry."),
    ]:
        if ensure_field(schema, f):
            added.append(f["name"])

    # 2) populate the recipe on each group row ---------------------------
    for row in data:
        gid = row["SymmetryGroupId"]
        rec = RECIPE.get(gid)
        if not rec:
            print("  WARN no recipe for", gid)
            continue
        lt, ang, ratio = rec["lattice"]
        ops = rec["ops"]
        row["LatticeType"] = lt
        row["LatticeAngleDeg"] = ang
        row["LatticeRatio"] = ratio
        row["Generators"] = json.dumps(ops, separators=(",", ":"))
        row["RotationOrder"] = ROT_ORDER[gid]
        row["OperationCount"] = len(ops) + 1
        row["HasMirrors"] = any(o["k"] == "mirror" for o in ops)
        row["HasGlides"] = any(o["k"] == "glide" for o in ops)

    # 3) WallpaperDesigns table ------------------------------------------
    if "WallpaperDesigns" not in rb:
        rb["WallpaperDesigns"] = {
            "Description": "User-authored wallpaper designs: a fundamental-domain "
                           "motif (drawn strokes) replicated under a chosen symmetry group.",
            "important": True,
            "schema": [
                field("WallpaperDesignId", "string", "raw",
                      "Stable id.", nullable=False),
                field("Name", "string", "calculated",
                      "Slug.", nullable=False,
                      formula='=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")'),
                field("DisplayName", "string", "raw",
                      "Human name for this design.", nullable=False),
                {"name": "SymmetryGroup", "datatype": "string", "type": "relationship",
                 "nullable": False, "RelatedTo": "SymmetryGroups",
                 "Description": "The wallpaper group whose symmetry replicates the motif."},
                field("Strokes", "string", "raw",
                      "JSON array of polylines drawn in the fundamental domain, in "
                      "fractional cell coords: [{color,width,points:[[u,v],...]},...].",
                      nullable=False),
                field("StrokeColor", "string", "raw",
                      "Default stroke color (hex) for new strokes."),
                field("BackgroundColor", "string", "raw",
                      "Fill color (hex) behind the tiling."),
                # lookups from the chosen group (INDEX/MATCH idiom — the supported one)
                field("GroupNotation", "string", "lookup",
                      "IUC notation of the chosen group.",
                      formula="=INDEX(SymmetryGroups!{{Notation}}, "
                              "MATCH(WallpaperDesigns!{{SymmetryGroup}}, "
                              "SymmetryGroups!{{SymmetryGroupId}}, 0))"),
                field("GroupOrbifold", "string", "lookup",
                      "Orbifold notation of the chosen group.",
                      formula="=INDEX(SymmetryGroups!{{Orbifold}}, "
                              "MATCH(WallpaperDesigns!{{SymmetryGroup}}, "
                              "SymmetryGroups!{{SymmetryGroupId}}, 0))"),
                field("GroupLatticeType", "string", "lookup",
                      "Lattice class of the chosen group.",
                      formula="=INDEX(SymmetryGroups!{{LatticeType}}, "
                              "MATCH(WallpaperDesigns!{{SymmetryGroup}}, "
                              "SymmetryGroups!{{SymmetryGroupId}}, 0))"),
                field("GroupRotationOrder", "integer", "lookup",
                      "Highest rotation order of the chosen group.",
                      formula="=INDEX(SymmetryGroups!{{RotationOrder}}, "
                              "MATCH(WallpaperDesigns!{{SymmetryGroup}}, "
                              "SymmetryGroups!{{SymmetryGroupId}}, 0))"),
                field("GroupOperationCount", "integer", "lookup",
                      "Motif copies per cell for the chosen group.",
                      formula="=INDEX(SymmetryGroups!{{OperationCount}}, "
                              "MATCH(WallpaperDesigns!{{SymmetryGroup}}, "
                              "SymmetryGroups!{{SymmetryGroupId}}, 0))"),
            ],
            "data": _seed_designs(),
        }
        print("  added table WallpaperDesigns with", len(rb["WallpaperDesigns"]["data"]), "seed rows")
    else:
        print("  WallpaperDesigns already present — leaving data as-is")

    # also add WallpaperDesignCount aggregation onto SymmetryGroups
    if ensure_field(schema, field("WallpaperDesignCount", "integer", "aggregation",
                    "How many saved designs use this group.",
                    formula="=COUNTIFS(WallpaperDesigns!{{SymmetryGroup}}, "
                            "SymmetryGroups!{{SymmetryGroupId}})")):
        added.append("WallpaperDesignCount")

    json.dump(rb, open(RB, "w"), indent=2)
    open(RB, "a").write("\n")
    print("Added SymmetryGroups fields:", added or "(none new)")
    print("SymmetryGroups now has", len(schema), "fields;",
          "WallpaperDesigns has", len(rb["WallpaperDesigns"]["schema"]), "fields.")


def _seed_designs():
    """A couple of starter designs so the viewer isn't empty on first load."""
    # A simple diagonal stroke + an L, in fractional cell coords [0,1]x[0,1].
    diag = [{"color": "#1f6feb", "width": 3,
             "points": [[0.15, 0.15], [0.85, 0.85]]}]
    elbow = [{"color": "#2ea043", "width": 3,
              "points": [[0.2, 0.8], [0.2, 0.2], [0.8, 0.2]]},
             {"color": "#d29922", "width": 3,
              "points": [[0.5, 0.5], [0.8, 0.65]]}]
    return [
        {"WallpaperDesignId": "starter-p4m", "DisplayName": "Starter p4m",
         "SymmetryGroup": "p4m", "Strokes": json.dumps(elbow, separators=(",", ":")),
         "StrokeColor": "#1f6feb", "BackgroundColor": "#0a0d12"},
        {"WallpaperDesignId": "starter-p6m", "DisplayName": "Starter p6m",
         "SymmetryGroup": "p6m", "Strokes": json.dumps(diag, separators=(",", ":")),
         "StrokeColor": "#a371f7", "BackgroundColor": "#0a0d12"},
        {"WallpaperDesignId": "starter-pgg", "DisplayName": "Starter pgg",
         "SymmetryGroup": "pgg", "Strokes": json.dumps(elbow, separators=(",", ":")),
         "StrokeColor": "#2ea043", "BackgroundColor": "#0a0d12"},
    ]


if __name__ == "__main__":
    main()

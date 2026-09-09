# 📘 Tiling the Plane — RuleSpeak®

_A library for tiling the Euclidean plane. The catalog half models which tilings exist and why each vertex figure is valid (interior angles summing to 360deg); the generative half places prototiles into a bounded region and measures coverage, gaps, and overlaps. One DAG of Schema/Data/Lookups/Aggregations/Formulas._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Symmetry Group** | The wallpaper symmetry groups a tiling can belong to. | — |
| Name | The same as its symmetry group ID. | _Mirrors the id._ |
| Notation | A defined attribute. | _IUC / crystallographic short notation (e.g. 'p6m')._ |
| Orbifold | A defined attribute. | _Conway orbifold notation (e.g. '*632')._ |
| Display Name | A defined attribute. | _Human-friendly name._ |
| Description | A defined attribute. | _What this wallpaper group is._ |
| Tiling Count | The number of tilings related to the symmetry group. | _How many catalogued tilings have this symmetry group._ |
| Lattice Type | A defined attribute. | _Bravais lattice class: oblique \| rectangular \| rhombic \| square \| hexagonal._ |
| Lattice Angle Deg | A defined attribute. | _Angle (deg) between the two lattice basis vectors B1,B2._ |
| Lattice Ratio | A defined attribute. | _Length ratio \|B2\|/\|B1\| of the lattice basis._ |
| Generators | A defined attribute. | _JSON array of non-identity coset-rep affine ops in fractional coords: {k,m:[a,b,c,d],t:[tx,ty]} mapping (u,v)->(a u+b v+tx, c u+d v+ty)._ |
| Rotation Order | A defined attribute. | _Highest order of rotational symmetry (1,2,3,4,6)._ |
| Operation Count | A defined attribute. | _Number of point-group coset reps including the identity (= motif copies per lattice cell)._ |
| Has Mirrors | True when an empty string. | _Whether the group contains reflection (mirror) symmetry._ |
| Has Glides | True when an empty string. | _Whether the group contains glide-reflection symmetry._ |
| Wallpaper Design Count | The number of wallpaper designs related to the symmetry group. | _How many saved designs use this group._ |
| **Prototile** | The prototiles (regular polygons) available for tilings. | — |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Slug from display name._ |
| Display Name | A defined attribute. | _Human name (e.g. 'Equilateral Triangle')._ |
| Sides | A defined attribute. | _Number of edges of this regular polygon._ |
| Edge Length | A defined attribute. | _Edge length in unit cells (1.0 = unit edge)._ |
| Area | Computed as 0.25 times the sides times the edge length raised to the power of 2 divided by the tangent of pi divided by the sides. ⚠︎ mechanical <!-- rulespeak:reword --> | _Area of one tile (regular n-gon). Trig override lives in 02b-customize-functions.sql._ |
| Interior Angle Deg | Computed as the sides minus 2 times 180 divided by the sides. | _Interior angle in degrees of this regular polygon._ |
| Vertex Angle Turns | Computed as the interior angle deg divided by 360. | _Fraction of a full turn occupied at one vertex by this tile._ |
| Is Regular | True when the sides is at least 3. | _A regular polygon needs at least 3 sides._ |
| Is Triangle | True when the sides is 3. | _True when this prototile is a triangle (3 sides)._ |
| Is Square | True when the sides is 4. | _True when this prototile is a square (4 sides)._ |
| Used in Tilings Count | The number of tiling prototiles related to the prototile. | _How many catalogued tilings use this prototile._ |
| **Tiling** | Catalogued tilings of the plane. | — |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Slug._ |
| Display Name | A defined attribute. | _Human name (e.g. 'Hexagonal tiling')._ |
| Vertex Config | A defined attribute. | _Vertex configuration / Schlafli symbol string (e.g. '3.4.6.4')._ |
| Kind | A defined attribute. | _'regular' \| 'semiregular' (Archimedean)._ |
| Is Edge to Edge | True when an empty string. | _Whether tiles meet edge-to-edge._ |
| Symmetry Group | A defined attribute. | _The wallpaper symmetry group of this tiling._ |
| Symmetry Notation | Taken from the linked symmetry group. | _Pulled symmetry notation for display._ |
| Distinct Prototile Count | The number of tiling prototiles related to the tiling. | _How many distinct prototiles this tiling uses._ |
| Vertex Figure Count | The number of vertex figures related to the tiling. | _How many distinct vertex figures this tiling has._ |
| Valid Vertex Figure Count | The number of the tiling's vertex figures that are valid. | _How many of its vertex figures close to 360deg._ |
| Is Regular Tiling | True when the distinct prototile count is 1. | _Regular tiling = built from a single prototile._ |
| All Vertices Valid | True when the valid vertex figure count is the vertex figure count. | _True when every vertex figure of this tiling closes to a full turn._ |
| T1x | A defined attribute. | _X component of the first lattice translation vector (unit cells). Repeating the fundamental domain along T1 and T2 fills the plane._ |
| T1y | A defined attribute. | _Y component of the first lattice translation vector (unit cells)._ |
| T2x | A defined attribute. | _X component of the second lattice translation vector (unit cells)._ |
| T2y | A defined attribute. | _Y component of the second lattice translation vector (unit cells)._ |
| Lattice Determinant | Computed as the t1x times the t2y minus the t1y times the t2x. | _Cross product T1 x T2. Equals the signed area of the fundamental domain (unit cell)._ |
| Fundamental Domain Area | Computed as the absolute value of the lattice determinant. | _Area of the fundamental domain spanned by the two translation vectors._ |
| Has Lattice | True when the absolute value of the lattice determinant is greater than 0.0001. | _True when the two translation vectors are present and linearly independent (span the plane)._ |
| Does Tile Plane | True when all of the following hold: the all vertices valid flag is set and the lattice flag is set. | _THE ULTIMATE QUESTION. Yes/no: does this tiling fill the plane? True iff every vertex figure closes to 360deg AND a non-degenerate lattice of translation vectors repeats the fundamental domain across the plane._ |
| **Tiling Prototile** | First-class junction: prototiles used by each tiling. | — |
| Name | Computed as the tiling, followed by a slash, followed by the prototile. | _Composite name._ |
| Tiling | A defined attribute. | _The tiling._ |
| Prototile | A defined attribute. | _A prototile used by that tiling._ |
| Count At Vertex | A defined attribute. | _How many of this prototile meet at a typical vertex._ |
| Prototile Sides | Taken from the linked prototile. | _Sides of the referenced prototile._ |
| Prototile Interior Angle | The interior angle deg of the tiling prototile's prototile. | _Interior angle of the referenced prototile._ |
| **Vertex Figure** | First-class vertex figures; validity derived from angle sum = 360. | — |
| Name | Computed as the tiling, followed by “@”, followed by the config. | _Composite name._ |
| Tiling | A defined attribute. | _The tiling this vertex figure belongs to._ |
| Config | A defined attribute. | _Vertex configuration at this vertex (e.g. '3.4.6.4')._ |
| Angle Sum Deg | A defined attribute. | _Sum of interior angles meeting at this vertex (degrees)._ |
| Turns At Vertex | Computed as the angle sum deg divided by 360. | _Angle sum as a fraction of a full turn._ |
| Angle Gap Deg | Computed as the absolute value of the angle sum deg minus 360. | _Distance from a full 360deg turn._ |
| Is Valid | True when the angle gap deg is at most 0.0001. | _Valid vertex figure: interior angles close to a full turn._ |
| **Region** | Bounded patches of the plane to be tiled. | — |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Slug._ |
| Display Name | A defined attribute. | _Human name._ |
| Width | A defined attribute. | _Region width in unit cells._ |
| Height | A defined attribute. | _Region height in unit cells._ |
| Target Tiling | A defined attribute. | _Which catalogued tiling this region is trying to realize._ |
| Area | Computed as the width times the height. | _Total area of the region._ |
| Placement Count | The number of placements related to the region. | _How many tiles have been placed in this region._ |
| Covered Area | The total tile area across the placements related to the region. | _Sum of placed-tile areas (counts overlaps twice — see CoveragePct)._ |
| Overlap Count | The number of the region's placements that are overlapping. | _How many placed tiles are flagged as overlapping a neighbour._ |
| Coverage Pct | Computed as 100 times the covered area divided by the area. | _Covered area as a percentage of region area._ |
| Is Fully Covered | True when the coverage pct is at least 100. | _Whether placed tiles cover at least the whole region area._ |
| Is Clean Tiling | True when all of the following hold: the fully covered flag is set and the overlap count is 0. | _Fully covered with no overlaps = a clean tiling of the region._ |
| **Placement** | Individual tile placements within a region. | — |
| Name | Computed as the region, followed by “#”, followed by the placement ID. | _Composite name._ |
| Region | A defined attribute. | _The region this tile is placed in._ |
| Prototile | A defined attribute. | _The prototile being placed._ |
| X | A defined attribute. | _X coordinate of the tile's anchor (unit cells)._ |
| Y | A defined attribute. | _Y coordinate of the tile's anchor (unit cells)._ |
| Rotation Deg | A defined attribute. | _Rotation of the tile in degrees._ |
| Is Overlapping | True when an empty string. | _Flag: this placement overlaps a neighbour (authored/validated)._ |
| Tile Area | Taken from the linked prototile. | _Area of the placed prototile (for coverage rollups)._ |
| Tile Sides | Taken from the linked prototile. | _Sides of the placed prototile._ |
| Region Width | Taken from the linked region. | _Width of the containing region._ |
| Region Height | Taken from the linked region. | _Height of the containing region._ |
| Is Inside Region | True when all of the following hold: the x is at least 0; the y is at least 0; the x is less than the region width; and the y is less than the region height. | _Whether the anchor sits within the region bounds._ |
| **Wallpaper Design** | User-authored wallpaper designs: a fundamental-domain motif (drawn strokes) replicated under a chosen symmetry group. | — |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Slug._ |
| Display Name | A defined attribute. | _Human name for this design._ |
| Symmetry Group | A defined attribute. | _The wallpaper group whose symmetry replicates the motif._ |
| Strokes | A defined attribute. | _JSON array of polylines drawn in the fundamental domain, in fractional cell coords: [{color,width,points:[[u,v],...]},...]._ |
| Stroke Color | A defined attribute. | _Default stroke color (hex) for new strokes._ |
| Background Color | A defined attribute. | _Fill color (hex) behind the tiling._ |
| Group Notation | Taken from the linked symmetry group. | _IUC notation of the chosen group._ |
| Group Orbifold | Taken from the linked symmetry group. | _Orbifold notation of the chosen group._ |
| Group Lattice Type | Taken from the linked symmetry group. | _Lattice class of the chosen group._ |
| Group Rotation Order | Taken from the linked symmetry group. | _Highest rotation order of the chosen group._ |
| Group Operation Count | Taken from the linked symmetry group. | _Motif copies per cell for the chosen group._ |

## 2 Fact Types

- a **tiling** may reference one **symmetry group**
- a **tiling prototile** references exactly one **tiling**
- a **tiling prototile** references exactly one **prototile**
- a **vertex figure** references exactly one **tiling**
- a **region** may reference one **tiling**
- a **placement** references exactly one **region**
- a **placement** references exactly one **prototile**
- a **wallpaper design** references exactly one **symmetry group**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A symmetry group **must** have a notation.
- A prototile **must** have a display name, a sides, and an edge length.
- A tiling **must** have a display name, a vertex config, and a kind, and record whether it is an edge to edge.
- A tiling prototile **must** reference exactly one tiling.
- A tiling prototile **must** reference exactly one prototile.
- A tiling prototile **must** have a count at vertex.
- A vertex figure **must** reference exactly one tiling.
- A vertex figure **must** have a config and an angle sum deg.
- A region **must** have a display name, a width, and a height.
- A placement **must** reference exactly one region.
- A placement **must** reference exactly one prototile.
- A placement **must** have a x, a y, and a rotation deg, and record whether it is overlapping.
- A wallpaper design **must** reference exactly one symmetry group.
- A wallpaper design **must** have a display name and a strokes.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A symmetry group's name is the same as its symmetry group ID. |
| **DR-2 Tiling Count** | A symmetry group's tiling count is the number of tilings related to the symmetry group. |
| **DR-3 Wallpaper Design Count** | A symmetry group's wallpaper design count is the number of wallpaper designs related to the symmetry group. |
| **DR-4 Name** | A prototile's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-5 Area** | A prototile's area is computed as 0.25 times the sides times the edge length raised to the power of 2 divided by the tangent of pi divided by the sides. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-6 Interior Angle Deg** | A prototile's interior angle deg is computed as the sides minus 2 times 180 divided by the sides. |
| **DR-7 Vertex Angle Turns** | A prototile's vertex angle turns is computed as the interior angle deg divided by 360. |
| **DR-8 Is Regular** | A prototile is considered a regular if the sides is at least 3. |
| **DR-9 Is Triangle** | A prototile is considered a triangle if the sides is 3. |
| **DR-10 Is Square** | A prototile is considered a square if the sides is 4. |
| **DR-11 Used in Tilings Count** | A prototile's used in tilings count is the number of tiling prototiles related to the prototile. |
| **DR-12 Name** | A tiling's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-13 Symmetry Notation** | A tiling's symmetry notation — taken from the linked symmetry group. |
| **DR-14 Distinct Prototile Count** | A tiling's distinct prototile count is the number of tiling prototiles related to the tiling. |
| **DR-15 Vertex Figure Count** | A tiling's vertex figure count is the number of vertex figures related to the tiling. |
| **DR-16 Valid Vertex Figure Count** | A tiling's valid vertex figure count is the number of the tiling's vertex figures that are valid. |
| **DR-17 Is Regular Tiling** | A tiling is considered regular-tiling if the distinct prototile count is 1. |
| **DR-18 All Vertices Valid** | A tiling is flagged all vertices valid if the valid vertex figure count is the vertex figure count. |
| **DR-19 Lattice Determinant** | A tiling's lattice determinant is computed as the t1x times the t2y minus the t1y times the t2x. |
| **DR-20 Fundamental Domain Area** | A tiling's fundamental domain area is computed as the absolute value of the lattice determinant. |
| **DR-21 Has Lattice** | A tiling is considered to have a lattice if the absolute value of the lattice determinant is greater than 0.0001. |
| **DR-22 Does Tile Plane** | A tiling is considered to doe a tile plane if all of the following hold: the all vertices valid flag is set and the lattice flag is set. |
| **DR-23 Name** | A tiling prototile's name is computed as the tiling, followed by a slash, followed by the prototile. |
| **DR-24 Prototile Sides** | A tiling prototile's prototile sides — taken from the linked prototile. |
| **DR-25 Prototile Interior Angle** | A tiling prototile's prototile interior angle is the interior angle deg of the tiling prototile's prototile. |
| **DR-26 Name** | A vertex figure's name is computed as the tiling, followed by “@”, followed by the config. |
| **DR-27 Turns At Vertex** | A vertex figure's turns at vertex is computed as the angle sum deg divided by 360. |
| **DR-28 Angle Gap Deg** | A vertex figure's angle gap deg is computed as the absolute value of the angle sum deg minus 360. |
| **DR-29 Is Valid** | A vertex figure is considered valid if the angle gap deg is at most 0.0001. |
| **DR-30 Name** | A region's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-31 Area** | A region's area is computed as the width times the height. |
| **DR-32 Placement Count** | A region's placement count is the number of placements related to the region. |
| **DR-33 Covered Area** | A region's covered area is the total tile area across the placements related to the region. |
| **DR-34 Overlap Count** | A region's overlap count is the number of the region's placements that are overlapping. |
| **DR-35 Coverage Pct** | A region's coverage pct is computed as 100 times the covered area divided by the area. |
| **DR-36 Is Fully Covered** | A region is considered fully-covered if the coverage pct is at least 100. |
| **DR-37 Is Clean Tiling** | A region is considered clean-tiling if all of the following hold: the fully covered flag is set and the overlap count is 0. |
| **DR-38 Name** | A placement's name is computed as the region, followed by “#”, followed by the placement ID. |
| **DR-39 Tile Area** | A placement's tile area — taken from the linked prototile. |
| **DR-40 Tile Sides** | A placement's tile sides — taken from the linked prototile. |
| **DR-41 Region Width** | A placement's region width — taken from the linked region. |
| **DR-42 Region Height** | A placement's region height — taken from the linked region. |
| **DR-43 Is Inside Region** | A placement is considered an inside region if all of the following hold: the x is at least 0; the y is at least 0; the x is less than the region width; and the y is less than the region height. |
| **DR-44 Name** | A wallpaper design's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-45 Group Notation** | A wallpaper design's group notation — taken from the linked symmetry group. |
| **DR-46 Group Orbifold** | A wallpaper design's group orbifold — taken from the linked symmetry group. |
| **DR-47 Group Lattice Type** | A wallpaper design's group lattice type — taken from the linked symmetry group. |
| **DR-48 Group Rotation Order** | A wallpaper design's group rotation order — taken from the linked symmetry group. |
| **DR-49 Group Operation Count** | A wallpaper design's group operation count — taken from the linked symmetry group. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **SymmetryGroups.Name** | formula | `SymmetryGroupId` |
| **SymmetryGroups.TilingCount** | rollup | `Count(Tilings via SymmetryGroup)` |
| **SymmetryGroups.WallpaperDesignCount** | rollup | `Count(WallpaperDesigns via SymmetryGroup)` |
| **Prototiles.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **Prototiles.Area** | formula | `0.25 * Sides * Power(EdgeLength, 2) / Tan(Pi() / Sides)` |
| **Prototiles.InteriorAngleDeg** | formula | `Sides - 2 * 180 / Sides` |
| **Prototiles.VertexAngleTurns** | formula | `InteriorAngleDeg / 360` |
| **Prototiles.IsRegular** | formula | `Sides >= 3` |
| **Prototiles.IsTriangle** | formula | `Sides = 3` |
| **Prototiles.IsSquare** | formula | `Sides = 4` |
| **Prototiles.UsedInTilingsCount** | rollup | `Count(TilingPrototiles via Prototile)` |
| **Tilings.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **Tilings.SymmetryNotation** | lookup | `Lookup(SymmetryGroups.Notation via SymmetryGroup)` |
| **Tilings.DistinctPrototileCount** | rollup | `Count(TilingPrototiles via Tiling)` |
| **Tilings.VertexFigureCount** | rollup | `Count(VertexFigures via Tiling)` |
| **Tilings.ValidVertexFigureCount** | rollup | `Count(VertexFigures via Tiling)` |
| **Tilings.IsRegularTiling** | formula | `DistinctPrototileCount = 1` |
| **Tilings.AllVerticesValid** | formula | `ValidVertexFigureCount = VertexFigureCount` |
| **Tilings.LatticeDeterminant** | formula | `T1x * T2y - T1y * T2x` |
| **Tilings.FundamentalDomainArea** | formula | `Abs(LatticeDeterminant)` |
| **Tilings.HasLattice** | formula | `Abs(LatticeDeterminant) > 0.0001` |
| **Tilings.DoesTilePlane** | formula | `And(AllVerticesValid, HasLattice)` |
| **TilingPrototiles.Name** | formula | `Concat(Tiling, "/", Prototile)` |
| **TilingPrototiles.PrototileSides** | lookup | `Lookup(Prototiles.Sides via Prototile)` |
| **TilingPrototiles.PrototileInteriorAngle** | lookup | `Lookup(Prototiles.InteriorAngleDeg via Prototile)` |
| **VertexFigures.Name** | formula | `Concat(Tiling, "@", Config)` |
| **VertexFigures.TurnsAtVertex** | formula | `AngleSumDeg / 360` |
| **VertexFigures.AngleGapDeg** | formula | `Abs(AngleSumDeg - 360)` |
| **VertexFigures.IsValid** | formula | `AngleGapDeg <= 0.0001` |
| **Regions.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **Regions.Area** | formula | `Width * Height` |
| **Regions.PlacementCount** | rollup | `Count(Placements via Region)` |
| **Regions.CoveredArea** | rollup | `Sum(Placements.TileArea via Region)` |
| **Regions.OverlapCount** | rollup | `Count(Placements via Region)` |
| **Regions.CoveragePct** | formula | `100 * CoveredArea / Area` |
| **Regions.IsFullyCovered** | formula | `CoveragePct >= 100` |
| **Regions.IsCleanTiling** | formula | `And(IsFullyCovered, OverlapCount = 0)` |
| **Placements.Name** | formula | `Concat(Region, "#", PlacementId)` |
| **Placements.TileArea** | lookup | `Lookup(Prototiles.Area via Prototile)` |
| **Placements.TileSides** | lookup | `Lookup(Prototiles.Sides via Prototile)` |
| **Placements.RegionWidth** | lookup | `Lookup(Regions.Width via Region)` |
| **Placements.RegionHeight** | lookup | `Lookup(Regions.Height via Region)` |
| **Placements.IsInsideRegion** | formula | `And(X >= 0, Y >= 0, X < RegionWidth, Y < RegionHeight)` |
| **WallpaperDesigns.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **WallpaperDesigns.GroupNotation** | lookup | `Lookup(SymmetryGroups.Notation via SymmetryGroup)` |
| **WallpaperDesigns.GroupOrbifold** | lookup | `Lookup(SymmetryGroups.Orbifold via SymmetryGroup)` |
| **WallpaperDesigns.GroupLatticeType** | lookup | `Lookup(SymmetryGroups.LatticeType via SymmetryGroup)` |
| **WallpaperDesigns.GroupRotationOrder** | lookup | `Lookup(SymmetryGroups.RotationOrder via SymmetryGroup)` |
| **WallpaperDesigns.GroupOperationCount** | lookup | `Lookup(SymmetryGroups.OperationCount via SymmetryGroup)` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._

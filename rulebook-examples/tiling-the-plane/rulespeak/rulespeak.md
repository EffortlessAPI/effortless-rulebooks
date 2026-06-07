# 📘 Tiling the Plane — RuleSpeak

_A library for tiling the Euclidean plane. The catalog half models which tilings exist and why each vertex figure is valid (interior angles summing to 360deg); the generative half places prototiles into a bounded region and measures coverage, gaps, and overlaps. One DAG of Schema/Data/Lookups/Aggregations/Formulas._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Symmetry Group** | The wallpaper symmetry groups a tiling can belong to. |
| Tiling Count | How many catalogued tilings have this symmetry group. |
| Wallpaper Design Count | How many saved designs use this group. |
| **Prototile** | The prototiles (regular polygons) available for tilings. |
| Area | Area of one tile (regular n-gon). Trig override lives in 02b-customize-functions.sql. |
| Interior Angle Deg | Interior angle in degrees of this regular polygon. |
| Vertex Angle Turns | Fraction of a full turn occupied at one vertex by this tile. |
| Is Regular | A regular polygon needs at least 3 sides. |
| Is Triangle | True when this prototile is a triangle (3 sides). |
| Is Square | True when this prototile is a square (4 sides). |
| Used in Tilings Count | How many catalogued tilings use this prototile. |
| **Tiling** | Catalogued tilings of the plane. |
| Symmetry Notation | Pulled symmetry notation for display. |
| Distinct Prototile Count | How many distinct prototiles this tiling uses. |
| Vertex Figure Count | How many distinct vertex figures this tiling has. |
| Valid Vertex Figure Count | How many of its vertex figures close to 360deg. |
| Is Regular Tiling | Regular tiling = built from a single prototile. |
| All Vertices Valid | True when every vertex figure of this tiling closes to a full turn. |
| Lattice Determinant | Cross product T1 x T2. Equals the signed area of the fundamental domain (unit cell). |
| Fundamental Domain Area | Area of the fundamental domain spanned by the two translation vectors. |
| Has Lattice | True when the two translation vectors are present and linearly independent (span the plane). |
| Does Tile Plane | THE ULTIMATE QUESTION. Yes/no: does this tiling fill the plane? True iff every vertex figure closes to 360deg AND a non-degenerate lattice of translation vectors repeats the fundamental domain across the plane. |
| **Tiling Prototile** | First-class junction: prototiles used by each tiling. |
| Prototile Sides | Sides of the referenced prototile. |
| Prototile Interior Angle | Interior angle of the referenced prototile. |
| **Vertex Figure** | First-class vertex figures; validity derived from angle sum = 360. |
| Turns At Vertex | Angle sum as a fraction of a full turn. |
| Angle Gap Deg | Distance from a full 360deg turn. |
| Is Valid | Valid vertex figure: interior angles close to a full turn. |
| **Region** | Bounded patches of the plane to be tiled. |
| Area | Total area of the region. |
| Placement Count | How many tiles have been placed in this region. |
| Covered Area | Sum of placed-tile areas (counts overlaps twice — see CoveragePct). |
| Overlap Count | How many placed tiles are flagged as overlapping a neighbour. |
| Coverage Pct | Covered area as a percentage of region area. |
| Is Fully Covered | Whether placed tiles cover at least the whole region area. |
| Is Clean Tiling | Fully covered with no overlaps = a clean tiling of the region. |
| **Placement** | Individual tile placements within a region. |
| Tile Area | Area of the placed prototile (for coverage rollups). |
| Tile Sides | Sides of the placed prototile. |
| Region Width | Width of the containing region. |
| Region Height | Height of the containing region. |
| Is Inside Region | Whether the anchor sits within the region bounds. |
| **Wallpaper Design** | User-authored wallpaper designs: a fundamental-domain motif (drawn strokes) replicated under a chosen symmetry group. |
| Group Notation | IUC notation of the chosen group. |
| Group Orbifold | Orbifold notation of the chosen group. |
| Group Lattice Type | Lattice class of the chosen group. |
| Group Rotation Order | Highest rotation order of the chosen group. |
| Group Operation Count | Motif copies per cell for the chosen group. |

## 2 Fact Types

- a **tiling** may reference one **symmetry group**
- a **tiling prototile** references exactly one **tiling**
- a **tiling prototile** references exactly one **prototile**
- a **vertex figure** references exactly one **tiling**
- a **region** may reference one **tiling**
- a **placement** references exactly one **region**
- a **placement** references exactly one **prototile**
- a **wallpaper design** references exactly one **symmetry group**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Tiling Count** | A symmetry group's tiling count is the number of tilings related to the symmetry group. |
| **DR-2 Wallpaper Design Count** | A symmetry group's wallpaper design count is the number of wallpaper designs related to the symmetry group. |
| **DR-3 Area** | A prototile's area is computed as `0.25*Sides*Power(EdgeLength,2)/Tan(Pi()/Sides)`. |
| **DR-4 Interior Angle Deg** | A prototile's interior angle deg is computed as `(Sides-2)*180/Sides`. |
| **DR-5 Vertex Angle Turns** | A prototile's vertex angle turns is computed as `InteriorAngleDeg/360`. |
| **DR-6 Is Regular** | A prototile is considered a regular if the sides is at least 3. |
| **DR-7 Is Triangle** | A prototile is considered a triangle if the sides is 3. |
| **DR-8 Is Square** | A prototile is considered a square if the sides is 4. |
| **DR-9 Used in Tilings Count** | A prototile's used in tilings count is the number of tiling prototiles related to the prototile. |
| **DR-10 Symmetry Notation** | A tiling's symmetry notation is the notation of the tiling's symmetry group. |
| **DR-11 Distinct Prototile Count** | A tiling's distinct prototile count is the number of tiling prototiles related to the tiling. |
| **DR-12 Vertex Figure Count** | A tiling's vertex figure count is the number of vertex figures related to the tiling. |
| **DR-13 Valid Vertex Figure Count** | A tiling's valid vertex figure count is the number of the tiling's vertex figures that are valid. |
| **DR-14 Is Regular Tiling** | A tiling is considered regular tiling if the distinct prototile count is 1. |
| **DR-15 All Vertices Valid** | A tiling is flagged all vertices valid if the valid vertex figure count is the vertex figure count. |
| **DR-16 Lattice Determinant** | A tiling's lattice determinant is computed as `T1x*T2y-T1y*T2x`. |
| **DR-17 Fundamental Domain Area** | A tiling's fundamental domain area is computed as `Abs(LatticeDeterminant)`. |
| **DR-18 Has Lattice** | A tiling is considered to have a lattice if `Abs(LatticeDeterminant)` is greater than 0.0001. |
| **DR-19 Does Tile Plane** | A tiling is considered to doe tile plane if all of the following hold: the all vertices valid flag is set and the has lattice flag is set. |
| **DR-20 Prototile Sides** | A tiling prototile's prototile sides is the sides of the tiling prototile's prototile. |
| **DR-21 Prototile Interior Angle** | A tiling prototile's prototile interior angle is the interior angle deg of the tiling prototile's prototile. |
| **DR-22 Turns At Vertex** | A vertex figure's turns at vertex is computed as `AngleSumDeg/360`. |
| **DR-23 Angle Gap Deg** | A vertex figure's angle gap deg is computed as `Abs(AngleSumDeg-360)`. |
| **DR-24 Is Valid** | A vertex figure is considered valid if the angle gap deg is at most 0.0001. |
| **DR-25 Area** | A region's area is computed as `Width*Height`. |
| **DR-26 Placement Count** | A region's placement count is the number of placements related to the region. |
| **DR-27 Covered Area** | A region's covered area is the total tile area across the placements related to the region. |
| **DR-28 Overlap Count** | A region's overlap count is the number of the region's placements that are overlapping. |
| **DR-29 Coverage Pct** | A region's coverage pct is computed as `100*CoveredArea/Area`. |
| **DR-30 Is Fully Covered** | A region is considered fully covered if the coverage pct is at least 100. |
| **DR-31 Is Clean Tiling** | A region is considered clean tiling if all of the following hold: the is fully covered flag is set and the overlap count is 0. |
| **DR-32 Tile Area** | A placement's tile area is the area of the placement's prototile. |
| **DR-33 Tile Sides** | A placement's tile sides is the sides of the placement's prototile. |
| **DR-34 Region Width** | A placement's region width is the width of the placement's region. |
| **DR-35 Region Height** | A placement's region height is the height of the placement's region. |
| **DR-36 Is Inside Region** | A placement is considered an inside region if all of the following hold: the x is at least 0; the y is at least 0; the x is less than the region width; and the y is less than the region height. |
| **DR-37 Group Notation** | A wallpaper design's group notation is the notation of the wallpaper design's symmetry group. |
| **DR-38 Group Orbifold** | A wallpaper design's group orbifold is the orbifold of the wallpaper design's symmetry group. |
| **DR-39 Group Lattice Type** | A wallpaper design's group lattice type is the lattice type of the wallpaper design's symmetry group. |
| **DR-40 Group Rotation Order** | A wallpaper design's group rotation order is the rotation order of the wallpaper design's symmetry group. |
| **DR-41 Group Operation Count** | A wallpaper design's group operation count is the operation count of the wallpaper design's symmetry group. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **SymmetryGroups.TilingCount** | rollup | `Count(Tilings via SymmetryGroup)` |
| **SymmetryGroups.WallpaperDesignCount** | rollup | `Count(WallpaperDesigns via SymmetryGroup)` |
| **Prototiles.Area** | formula | `0.25*Sides*Power(EdgeLength,2)/Tan(Pi()/Sides)` |
| **Prototiles.InteriorAngleDeg** | formula | `(Sides-2)*180/Sides` |
| **Prototiles.VertexAngleTurns** | formula | `InteriorAngleDeg/360` |
| **Prototiles.IsRegular** | formula | `Sides>=3` |
| **Prototiles.IsTriangle** | formula | `Sides=3` |
| **Prototiles.IsSquare** | formula | `Sides=4` |
| **Prototiles.UsedInTilingsCount** | rollup | `Count(TilingPrototiles via Prototile)` |
| **Tilings.SymmetryNotation** | lookup | `Lookup(SymmetryGroups.Notation via SymmetryGroup)` |
| **Tilings.DistinctPrototileCount** | rollup | `Count(TilingPrototiles via Tiling)` |
| **Tilings.VertexFigureCount** | rollup | `Count(VertexFigures via Tiling)` |
| **Tilings.ValidVertexFigureCount** | rollup | `Count(VertexFigures.Tiling, Tilings.TilingId, VertexFigures.IsValid, TRUE)` |
| **Tilings.IsRegularTiling** | formula | `DistinctPrototileCount=1` |
| **Tilings.AllVerticesValid** | formula | `ValidVertexFigureCount=VertexFigureCount` |
| **Tilings.LatticeDeterminant** | formula | `T1x*T2y-T1y*T2x` |
| **Tilings.FundamentalDomainArea** | formula | `Abs(LatticeDeterminant)` |
| **Tilings.HasLattice** | formula | `Abs(LatticeDeterminant)>0.0001` |
| **Tilings.DoesTilePlane** | formula | `And(AllVerticesValid, HasLattice)` |
| **TilingPrototiles.PrototileSides** | lookup | `Lookup(Prototiles.Sides via Prototile)` |
| **TilingPrototiles.PrototileInteriorAngle** | lookup | `Lookup(Prototiles.InteriorAngleDeg via Prototile)` |
| **VertexFigures.TurnsAtVertex** | formula | `AngleSumDeg/360` |
| **VertexFigures.AngleGapDeg** | formula | `Abs(AngleSumDeg-360)` |
| **VertexFigures.IsValid** | formula | `AngleGapDeg<=0.0001` |
| **Regions.Area** | formula | `Width*Height` |
| **Regions.PlacementCount** | rollup | `Count(Placements via Region)` |
| **Regions.CoveredArea** | rollup | `Sum(Placements.TileArea via Region)` |
| **Regions.OverlapCount** | rollup | `Count(Placements.Region, Regions.RegionId, Placements.IsOverlapping, TRUE)` |
| **Regions.CoveragePct** | formula | `100*CoveredArea/Area` |
| **Regions.IsFullyCovered** | formula | `CoveragePct>=100` |
| **Regions.IsCleanTiling** | formula | `And(IsFullyCovered, OverlapCount=0)` |
| **Placements.TileArea** | lookup | `Lookup(Prototiles.Area via Prototile)` |
| **Placements.TileSides** | lookup | `Lookup(Prototiles.Sides via Prototile)` |
| **Placements.RegionWidth** | lookup | `Lookup(Regions.Width via Region)` |
| **Placements.RegionHeight** | lookup | `Lookup(Regions.Height via Region)` |
| **Placements.IsInsideRegion** | formula | `And(X>=0, Y>=0, X<RegionWidth, Y<RegionHeight)` |
| **WallpaperDesigns.GroupNotation** | lookup | `Lookup(SymmetryGroups.Notation via SymmetryGroup)` |
| **WallpaperDesigns.GroupOrbifold** | lookup | `Lookup(SymmetryGroups.Orbifold via SymmetryGroup)` |
| **WallpaperDesigns.GroupLatticeType** | lookup | `Lookup(SymmetryGroups.LatticeType via SymmetryGroup)` |
| **WallpaperDesigns.GroupRotationOrder** | lookup | `Lookup(SymmetryGroups.RotationOrder via SymmetryGroup)` |
| **WallpaperDesigns.GroupOperationCount** | lookup | `Lookup(SymmetryGroups.OperationCount via SymmetryGroup)` |

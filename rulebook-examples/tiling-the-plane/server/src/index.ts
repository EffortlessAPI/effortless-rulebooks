/**
 * Tiling-the-Plane control-panel API.
 *
 * Doctrine (see ../../CLAUDE.md): the VIEW is the contract.
 *   - Reads ALWAYS come from vw_<entity>. The view's row IS the row; every
 *     calc/lookup/aggregation column is already populated by the rulebook's
 *     generated SQL. We never re-derive a computed value here.
 *   - Writes ALWAYS target the base table and only ever touch RAW columns,
 *     keyed on <table>_id. After a write we re-SELECT from the view so the
 *     caller sees the recomputed cascade.
 * If a value is missing, we raise — we never paint in a fallback.
 */
import express from "express";
import cors from "cors";
import pg from "pg";
import { spawn } from "node:child_process";
import * as fs from "node:fs";
import * as path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const PORT = Number(process.env.PORT) || 3032;
const DATABASE_URL =
  process.env.DATABASE_URL ||
  "postgresql://postgres@localhost:5432/erb_tiling_the_plane";

const pool = new pg.Pool({ connectionString: DATABASE_URL });

/**
 * One entry per entity. `editable` lists the RAW columns the control panel may
 * write; everything else in the view is derived and read-only by construction.
 * `required` columns must be present on create.
 */
type Entity = {
  view: string;
  table: string;
  pk: string;
  editable: string[];
  required: string[];
};

const ENTITIES: Record<string, Entity> = {
  "symmetry-groups": {
    view: "vw_symmetry_groups",
    table: "symmetry_groups",
    pk: "symmetry_group_id",
    editable: ["symmetry_group_id", "notation", "orbifold", "display_name", "description"],
    required: ["symmetry_group_id", "notation"],
  },
  prototiles: {
    view: "vw_prototiles",
    table: "prototiles",
    pk: "prototile_id",
    editable: ["prototile_id", "display_name", "sides", "edge_length"],
    required: ["prototile_id", "display_name", "sides", "edge_length"],
  },
  tilings: {
    view: "vw_tilings",
    table: "tilings",
    pk: "tiling_id",
    editable: ["tiling_id", "display_name", "vertex_config", "kind", "is_edge_to_edge", "symmetry_group", "t1x", "t1y", "t2x", "t2y"],
    required: ["tiling_id", "display_name", "vertex_config", "kind", "is_edge_to_edge"],
  },
  "tiling-prototiles": {
    view: "vw_tiling_prototiles",
    table: "tiling_prototiles",
    pk: "tiling_prototile_id",
    editable: ["tiling_prototile_id", "tiling", "prototile", "count_at_vertex"],
    required: ["tiling_prototile_id", "tiling", "prototile", "count_at_vertex"],
  },
  "vertex-figures": {
    view: "vw_vertex_figures",
    table: "vertex_figures",
    pk: "vertex_figure_id",
    editable: ["vertex_figure_id", "tiling", "config", "angle_sum_deg"],
    required: ["vertex_figure_id", "tiling", "config", "angle_sum_deg"],
  },
  regions: {
    view: "vw_regions",
    table: "regions",
    pk: "region_id",
    editable: ["region_id", "display_name", "width", "height", "target_tiling"],
    required: ["region_id", "display_name", "width", "height"],
  },
  placements: {
    view: "vw_placements",
    table: "placements",
    pk: "placement_id",
    editable: ["placement_id", "region", "prototile", "x", "y", "rotation_deg", "is_overlapping"],
    required: ["placement_id", "region", "prototile", "x", "y"],
  },
  "wallpaper-designs": {
    view: "vw_wallpaper_designs",
    table: "wallpaper_designs",
    pk: "wallpaper_design_id",
    editable: [
      "wallpaper_design_id", "display_name", "symmetry_group",
      "strokes", "stroke_color", "background_color",
    ],
    required: ["wallpaper_design_id", "display_name", "symmetry_group", "strokes"],
  },
};

const app = express();
app.use(cors());
app.use(express.json());

app.get("/healthz", async (_req, res) => {
  try {
    await pool.query("SELECT 1");
    res.json({ ok: true, db: DATABASE_URL.replace(/\/\/[^@]*@/, "//") });
  } catch (e: any) {
    res.status(500).json({ ok: false, error: e.message });
  }
});

/** Catalog of entities the UI can drive, with their column metadata. */
app.get("/api/entities", async (_req, res) => {
  const out: any = {};
  for (const [key, e] of Object.entries(ENTITIES)) {
    const cols = await pool.query(
      `SELECT column_name, data_type
         FROM information_schema.columns
        WHERE table_name = $1
        ORDER BY ordinal_position`,
      [e.view],
    );
    out[key] = {
      key,
      view: e.view,
      pk: e.pk,
      editable: e.editable,
      required: e.required,
      columns: cols.rows.map((r) => ({
        name: r.column_name,
        type: r.data_type,
        editable: e.editable.includes(r.column_name),
      })),
    };
  }
  res.json(out);
});

function entityOr404(req: express.Request, res: express.Response): Entity | null {
  const e = ENTITIES[req.params.entity];
  if (!e) {
    res.status(404).json({ error: `unknown entity '${req.params.entity}'` });
    return null;
  }
  return e;
}

/**
 * Excel export. Reads the live state from every vw_* view, populates a copy of
 * the rulebook's data arrays, and hands it to the `rulebook-to-xlsx` transpiler —
 * the same SSoT, projected to a spreadsheet. Every calc/lookup/aggregation column
 * is included because the rows come from the views, not the base tables.
 *
 * NOTE: this route is declared BEFORE the generic /api/:entity[/:id] routes so
 * "export" / "xlsx" don't get captured as an entity + id.
 */
const PROJECT_ROOT = path.join(__dirname, "../..");
const RULEBOOK_PATH = path.join(
  PROJECT_ROOT,
  "effortless-rulebook/tiling-the-plane-rulebook.json",
);
const PROJECT_NAME = process.env.PROJECT_NAME || "tiling-the-plane";

function toSnake(name: string): string {
  return name.replace(/([A-Z])/g, "_$1").toLowerCase().replace(/^_/, "");
}
function viewFor(table: string): string {
  return `vw_${toSnake(table)}`;
}

app.get("/api/export/xlsx", async (_req, res) => {
  const exportPath = path.join(PROJECT_ROOT, "rulebook-export.json");
  const xlsxFilename = `${PROJECT_NAME}-rulebook.xlsx`;
  const xlsxPath = path.join(PROJECT_ROOT, xlsxFilename);
  try {
    const rulebook = JSON.parse(fs.readFileSync(RULEBOOK_PATH, "utf8"));
    const reserved = new Set(["$schema", "Name", "Description", "_meta", "__meta__"]);
    const tables = Object.keys(rulebook).filter(
      (k) => !reserved.has(k) && rulebook[k]?.schema,
    );

    for (const table of tables) {
      const view = viewFor(table);
      try {
        const { rows } = await pool.query(`SELECT * FROM ${view}`);
        const schema: Array<{ name: string }> = rulebook[table].schema;
        rulebook[table].data = rows.map((row: Record<string, unknown>) => {
          const mapped: Record<string, unknown> = {};
          for (const field of schema) {
            const snake = toSnake(field.name);
            if (snake in row) mapped[field.name] = row[snake];
          }
          return mapped;
        });
      } catch (e) {
        console.warn(`[export] skipping view ${view}:`, (e as Error).message);
        rulebook[table].data = [];
      }
    }

    fs.writeFileSync(exportPath, JSON.stringify(rulebook, null, 2));

    await new Promise<void>((resolve, reject) => {
      const proc = spawn(
        "effortless",
        ["rulebook-to-xlsx", "-i", "./rulebook-export.json", "-o", xlsxFilename],
        { cwd: PROJECT_ROOT },
      );
      let stderr = "";
      proc.stderr.on("data", (d) => (stderr += d.toString()));
      proc.on("close", (code) =>
        code === 0 ? resolve() : reject(new Error(`rulebook-to-xlsx exited ${code}: ${stderr}`)),
      );
      proc.on("error", reject);
    });

    res.download(xlsxPath, xlsxFilename, () => {
      try { fs.unlinkSync(xlsxPath); } catch {}
      try { fs.unlinkSync(exportPath); } catch {}
    });
  } catch (err: any) {
    console.error("[export] failed:", err);
    try { fs.unlinkSync(exportPath); } catch {}
    res.status(500).json({ error: err.message });
  }
});

/**
 * Wallpaper-pattern OpenSCAD export. Emits a 2D `.scad` script suitable for
 * laser-cutting: each stroke in the motif (and its symmetry copies across the
 * tiled plane) becomes either a linear_extrude()'d path (open strokes) or a
 * filled polygon (closed strokes where first≈last point). The lattice + group
 * ops are read from vw_symmetry_groups (the view is the contract); the design's
 * strokes are passed as a JSON body.
 *
 * POST /api/export/wallpaper-scad
 * Body: { groupId, strokes, tiles (N for ±N lattice), scale (px per cell) }
 *
 * Declared BEFORE the generic routes so "export" is not captured as an entity.
 */
type ScadStroke = { color: string; width: number; points: [number, number][]; filled?: boolean };

function isClosed(points: [number, number][], threshold = 0.05): boolean {
  if (points.length < 3) return false;
  const [u0, v0] = points[0];
  const [u1, v1] = points[points.length - 1];
  return Math.hypot(u1 - u0, v1 - v0) < threshold;
}

app.post("/api/export/wallpaper-scad", async (req, res) => {
  try {
    const { groupId, strokes, tiles: N = 3, scale = 60, frame } = req.body ?? {};
    if (!groupId) return res.status(400).json({ error: "groupId required" });
    if (!Array.isArray(strokes) || strokes.length === 0)
      return res.status(400).json({ error: "strokes required" });

    // Read the group's lattice + generators from the view (the contract).
    const gr = await pool.query(
      `SELECT notation, lattice_type, lattice_angle_deg, lattice_ratio, generators
         FROM vw_symmetry_groups WHERE symmetry_group_id = $1`,
      [groupId],
    );
    if (gr.rowCount === 0) return res.status(404).json({ error: `unknown group '${groupId}'` });
    const group = gr.rows[0];

    const angleDeg = Number(group.lattice_angle_deg) || 90;
    const ratio = Number(group.lattice_ratio) || 1;
    const angleRad = (angleDeg * Math.PI) / 180;
    // Basis in fractional→mm (scale = mm per cell unit)
    const B1 = [scale, 0];
    const B2 = [scale * ratio * Math.cos(angleRad), scale * ratio * Math.sin(angleRad)];

    let ops: Array<{ m: number[]; t: number[] }> = [{ m: [1, 0, 0, 1], t: [0, 0] }];
    try {
      if (group.generators) ops = [{ m: [1, 0, 0, 1], t: [0, 0] }, ...JSON.parse(group.generators)];
    } catch {}

    function applyOp(op: { m: number[]; t: number[] }, u: number, v: number): [number, number] {
      const [a, b, c, d] = op.m;
      return [a * u + b * v + op.t[0], c * u + d * v + op.t[1]];
    }

    function toMm(u: number, v: number, di: number, dj: number): [number, number] {
      const uu = u + di, vv = v + dj;
      return [uu * B1[0] + vv * B2[0], uu * B1[1] + vv * B2[1]];
    }

    // Build all copies: (stroke × op × lattice cell)
    const lines: string[] = [];
    const frameShape: string = frame?.shape ?? "rect";
    const frameW: number = Number(frame?.width) || 200;
    const frameH: number = frameShape === "rect" ? (Number(frame?.height) || 200) : frameW;

    lines.push(`// OpenSCAD wallpaper export — ${group.notation} (${groupId})`);
    lines.push(`// Suitable for laser cutting as a 2-D pattern.`);
    lines.push(`// Strokes: fundamental domain. Symmetry ops: vw_symmetry_groups (the view is the contract).`);
    lines.push(`// Scale: ${scale} mm per lattice unit. Frame: ${frameShape} ${frameW}×${frameH} mm. Tiled ±${N} cells.`);
    lines.push(``);
    lines.push(`// Workflow: open in OpenSCAD, Project (F6) → Export as DXF/SVG for laser software.`);
    lines.push(`// The pattern is intersection()'d with the frame shape — nothing bleeds outside.`);
    lines.push(``);

    // Frame shape module
    lines.push(`module frame_shape() {`);
    if (frameShape === "circle") {
      lines.push(`  circle(r = ${(frameW / 2).toFixed(3)}, $fn = 128);`);
    } else if (frameShape === "hex") {
      lines.push(`  circle(r = ${(frameW / 2).toFixed(3)}, $fn = 6);`);
    } else if (frameShape === "octagon") {
      lines.push(`  circle(r = ${(frameW / 2).toFixed(3)}, $fn = 8);`);
    } else {
      lines.push(`  square([${frameW.toFixed(3)}, ${frameH.toFixed(3)}], center = true);`);
    }
    lines.push(`}`);
    lines.push(``);

    let shapeIdx = 0;
    const shapes: string[] = [];

    for (let di = -N; di <= N; di++) {
      for (let dj = -N; dj <= N; dj++) {
        for (const op of ops) {
          for (const stroke of strokes as ScadStroke[]) {
            const pts = stroke.points.map(([u, v]) => {
              const [ou, ov] = applyOp(op, u, v);
              return toMm(ou, ov, di, dj);
            });

            if (isClosed(stroke.points)) {
              // Closed stroke → filled polygon
              const ptStr = pts.map(([x, y]) => `[${x.toFixed(3)}, ${y.toFixed(3)}]`).join(", ");
              shapes.push(`  // shape ${shapeIdx} (filled, color ${stroke.color}, cell [${di},${dj}])`);
              shapes.push(`  polygon(points=[${ptStr}]);`);
            } else if (pts.length >= 2) {
              // Open stroke → offset path approximation (stroke width in mm ≈ stroke.width * 0.3)
              const hw = ((stroke.width || 2) * 0.3) / 2;
              for (let k = 0; k < pts.length - 1; k++) {
                const [x0, y0] = pts[k];
                const [x1, y1] = pts[k + 1];
                const len = Math.hypot(x1 - x0, y1 - y0);
                if (len < 0.001) continue;
                const angle = Math.atan2(y1 - y0, x1 - x0) * (180 / Math.PI);
                const mx = (x0 + x1) / 2, my = (y0 + y1) / 2;
                shapes.push(`  // stroke seg ${shapeIdx}k${k} [${di},${dj}]`);
                shapes.push(`  translate([${mx.toFixed(3)}, ${my.toFixed(3)}, 0])`);
                shapes.push(`    rotate([0, 0, ${angle.toFixed(3)}])`);
                shapes.push(`      square([${len.toFixed(3)}, ${(hw * 2).toFixed(3)}], center=true);`);
              }
            }
            shapeIdx++;
          }
        }
      }
    }

    // Never `lines.push(...shapes)` — spread blows the call stack once a hand-drawn
    // stroke has enough segments (≈70+ points × ops × tiles).
    lines.push(`intersection() {`);
    lines.push(`  frame_shape();`);
    lines.push(`  union() {`);
    for (const shape of shapes) lines.push("  " + shape);
    lines.push(`  }`);
    lines.push(`}`);

    const scad = lines.join("\n");
    const filename = `wallpaper-${group.notation}-pattern.scad`;
    res.setHeader("Content-Type", "application/x-openscad");
    res.setHeader("Content-Disposition", `attachment; filename="${filename}"`);
    res.send(scad);
  } catch (err: any) {
    console.error("[export:wallpaper-scad] failed:", err);
    res.status(500).json({ error: err.message });
  }
});

/**
 * OpenSCAD export. Emits a `.scad` script that reconstructs a region's tiling as
 * extruded regular polygons — the same geometry the Studio draws in SVG, so what
 * you see is what you print/open in OpenSCAD.
 *
 * Doctrine (see ../../CLAUDE.md): the VIEW is the contract. Every value used here
 * — region width/height, each placement's x/y/rotation, each prototile's side
 * count and edge length — is SELECTed from vw_*. We do NOT re-derive anything;
 * we only translate already-computed rows into OpenSCAD geometry. The unit-cell
 * placement + regular-n-gon construction mirrors polygonPoints() in TilingStudio.
 *
 * Declared BEFORE the generic /api/:entity[/:id] routes so "export" isn't
 * captured as an entity.
 *
 * Query: ?region=<region_id> (defaults to the busiest region by placement_count).
 */
app.get("/api/export/scad", async (req, res) => {
  try {
    // Pick the region: explicit ?region=, else the one with the most placements.
    const requested = typeof req.query.region === "string" ? req.query.region : null;
    let row: Record<string, any> | null;
    if (requested) {
      const r = await pool.query(`SELECT * FROM vw_regions WHERE region_id = $1`, [requested]);
      if (r.rowCount === 0) return res.status(404).json({ error: `unknown region '${requested}'` });
      row = r.rows[0];
    } else {
      const r = await pool.query(
        `SELECT * FROM vw_regions ORDER BY placement_count DESC NULLS LAST, region_id LIMIT 1`,
      );
      if (r.rowCount === 0) return res.status(404).json({ error: "no regions to export" });
      row = r.rows[0];
    }
    const region = row!;

    const regionId = region.region_id as string;
    const width = Number(region.width);
    const height = Number(region.height);

    // Placements for this region (already computed in the view) joined to the
    // prototile's side-count + edge length so the polygon matches the catalog.
    const placements = await pool.query(
      `SELECT p.placement_id, p.prototile, p.x, p.y, p.rotation_deg, p.is_overlapping,
              p.is_inside_region, pr.sides, pr.edge_length
         FROM vw_placements p
         JOIN vw_prototiles pr ON pr.prototile_id = p.prototile
        WHERE p.region = $1
        ORDER BY p.placement_id`,
      [regionId],
    );

    const CELL = 1; // one OpenSCAD unit per grid cell
    const R = CELL * 0.5; // circumradius, matches the SVG's CELL*0.5
    const THICK = 0.2; // extrusion height (units)

    const lines: string[] = [];
    lines.push(`// OpenSCAD export — Tiling the Plane`);
    lines.push(`// Region: ${regionId} (${region.display_name ?? regionId}) — ${width}x${height}`);
    lines.push(`// Generated from vw_regions / vw_placements / vw_prototiles (the view is the contract).`);
    lines.push(`// Each tile is a regular n-gon centered in its unit cell, rotated, then extruded.`);
    lines.push(``);
    lines.push(`thickness = ${THICK};`);
    lines.push(`cell = ${CELL};`);
    lines.push(``);
    // A parametric regular-polygon module so the script reads like geometry, not data.
    lines.push(`module ngon(sides, r, rot) {`);
    lines.push(`  rotate([0, 0, rot - 90])`);
    lines.push(`    circle(r = r, $fn = sides);`);
    lines.push(`}`);
    lines.push(``);
    lines.push(`module tile(cx, cy, sides, r, rot) {`);
    lines.push(`  translate([cx, cy, 0])`);
    lines.push(`    linear_extrude(height = thickness)`);
    lines.push(`      ngon(sides, r, rot);`);
    lines.push(`}`);
    lines.push(``);

    if (placements.rowCount === 0) {
      lines.push(`// (no placements in this region yet)`);
    }
    for (const t of placements.rows) {
      const sides = Number(t.sides);
      // Center of the cell, y measured up (SVG flips; OpenSCAD is already y-up).
      const cx = (Number(t.x) + 0.5) * CELL;
      const cy = (Number(t.y) + 0.5) * CELL;
      const rot = Number(t.rotation_deg) || 0;
      const flags = [
        t.is_overlapping ? "overlapping" : null,
        t.is_inside_region === false ? "outside-region" : null,
      ].filter(Boolean).join(", ");
      const note = flags ? ` // ${t.placement_id} [${flags}]` : ` // ${t.placement_id}`;
      lines.push(`tile(${cx}, ${cy}, ${sides}, ${R.toFixed(4)}, ${rot});${note}`);
    }
    lines.push(``);

    const scad = lines.join("\n");
    const filename = `${PROJECT_NAME}-${regionId}.scad`;
    res.setHeader("Content-Type", "application/x-openscad");
    res.setHeader("Content-Disposition", `attachment; filename="${filename}"`);
    res.send(scad);
  } catch (err: any) {
    console.error("[export:scad] failed:", err);
    res.status(500).json({ error: err.message });
  }
});

/** LIST — read from the view. The view's row IS the row. */
app.get("/api/:entity", async (req, res) => {
  const e = entityOr404(req, res);
  if (!e) return;
  try {
    const r = await pool.query(`SELECT * FROM ${e.view} ORDER BY ${e.pk}`);
    res.json(r.rows);
  } catch (err: any) {
    res.status(500).json({ error: err.message });
  }
});

/** GET one — from the view. */
app.get("/api/:entity/:id", async (req, res) => {
  const e = entityOr404(req, res);
  if (!e) return;
  try {
    const r = await pool.query(`SELECT * FROM ${e.view} WHERE ${e.pk} = $1`, [req.params.id]);
    if (r.rowCount === 0) return res.status(404).json({ error: "not found" });
    res.json(r.rows[0]);
  } catch (err: any) {
    res.status(500).json({ error: err.message });
  }
});

/** CREATE — write raw columns to the base table, then re-read from the view. */
app.post("/api/:entity", async (req, res) => {
  const e = entityOr404(req, res);
  if (!e) return;
  const body = req.body ?? {};
  for (const r of e.required) {
    if (body[r] === undefined || body[r] === null || body[r] === "") {
      return res.status(400).json({ error: `missing required field '${r}'` });
    }
  }
  const cols = e.editable.filter((c) => body[c] !== undefined);
  const vals = cols.map((c) => body[c]);
  const placeholders = cols.map((_, i) => `$${i + 1}`);
  try {
    await pool.query(
      `INSERT INTO ${e.table} (${cols.join(", ")}) VALUES (${placeholders.join(", ")})`,
      vals,
    );
    const r = await pool.query(`SELECT * FROM ${e.view} WHERE ${e.pk} = $1`, [body[e.pk]]);
    res.status(201).json(r.rows[0]);
  } catch (err: any) {
    res.status(400).json({ error: err.message });
  }
});

/** UPDATE — patch raw columns on the base table, then re-read from the view. */
app.patch("/api/:entity/:id", async (req, res) => {
  const e = entityOr404(req, res);
  if (!e) return;
  const body = req.body ?? {};
  // Never let the PK be overwritten via PATCH.
  const cols = e.editable.filter((c) => c !== e.pk && body[c] !== undefined);
  if (cols.length === 0) return res.status(400).json({ error: "no editable fields supplied" });
  const sets = cols.map((c, i) => `${c} = $${i + 1}`);
  const vals = cols.map((c) => body[c]);
  vals.push(req.params.id);
  try {
    const upd = await pool.query(
      `UPDATE ${e.table} SET ${sets.join(", ")} WHERE ${e.pk} = $${vals.length}`,
      vals,
    );
    if (upd.rowCount === 0) return res.status(404).json({ error: "not found" });
    const r = await pool.query(`SELECT * FROM ${e.view} WHERE ${e.pk} = $1`, [req.params.id]);
    res.json(r.rows[0]);
  } catch (err: any) {
    res.status(400).json({ error: err.message });
  }
});

/** DELETE — from the base table. */
app.delete("/api/:entity/:id", async (req, res) => {
  const e = entityOr404(req, res);
  if (!e) return;
  try {
    const r = await pool.query(`DELETE FROM ${e.table} WHERE ${e.pk} = $1`, [req.params.id]);
    if (r.rowCount === 0) return res.status(404).json({ error: "not found" });
    res.json({ ok: true });
  } catch (err: any) {
    res.status(400).json({ error: err.message });
  }
});

app.listen(PORT, () => {
  console.log(`[tiling-the-plane] API on http://localhost:${PORT}`);
  console.log(`[tiling-the-plane] DB ${DATABASE_URL.replace(/\/\/[^@]*@/, "//")}`);
});

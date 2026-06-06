import { useMemo, useState } from "react";
import { api, Row } from "../lib/api";
import { useAsync } from "../lib/useApi";

/**
 * Tiling Studio — the full management surface for regions and their placements.
 *
 * Doctrine (see ../../CLAUDE.md): the VIEW is the contract.
 *   - Every value rendered here comes from vw_regions / vw_placements / vw_prototiles.
 *   - Every edit (drag, rotate, retype, toggle, region create/resize) writes only
 *     RAW columns to the base table via the API, then re-reads the view. Coverage,
 *     overlap count, TileArea, IsInsideRegion, the clean-tiling flag — none of it is
 *     computed in this browser. The database recomputes the cascade on every write.
 */

const CELL = 56; // px per unit cell
const PAD = 24;

const SHAPE_FILL: Record<number, string> = {
  3: "#2ea043",
  4: "#1f6feb",
  6: "#d29922",
  8: "#a371f7",
  12: "#db61a2",
};

/** Vertices of a regular n-gon centered in a unit cell at (cx, cy). */
function polygonPoints(sides: number, cx: number, cy: number, rotDeg: number): string {
  const r = CELL * 0.5;
  const rot = (rotDeg * Math.PI) / 180 - Math.PI / 2;
  const pts: string[] = [];
  for (let i = 0; i < sides; i++) {
    const a = rot + (i * 2 * Math.PI) / sides;
    pts.push(`${(cx + r * Math.cos(a)).toFixed(1)},${(cy + r * Math.sin(a)).toFixed(1)}`);
  }
  return pts.join(" ");
}

function fmt(n: any, d = 1): string {
  if (n === null || n === undefined) return "—";
  const v = Number(n);
  return Number.isNaN(v) ? String(n) : v.toFixed(d);
}

type DragState =
  | { kind: "palette"; prototile: string }
  | { kind: "tile"; placementId: string };

export default function TilingStudio() {
  const regions = useAsync(() => api.list("regions"), []);
  const placements = useAsync(() => api.list("placements"), []);
  const prototiles = useAsync(() => api.list("prototiles"), []);
  const tilings = useAsync(() => api.list("tilings"), []);

  const [sel, setSel] = useState<string | null>(null); // active region id
  const [selTile, setSelTile] = useState<string | null>(null); // selected placement id
  const [msg, setMsg] = useState<string | null>(null);
  const [err, setErr] = useState<string | null>(null);
  const [brush, setBrush] = useState<string>("");
  const [drag, setDrag] = useState<DragState | null>(null);
  const [hoverCell, setHoverCell] = useState<string | null>(null);
  const [showRegionForm, setShowRegionForm] = useState(false);

  const protoBySides = useMemo(() => {
    const m: Record<string, number> = {};
    for (const p of prototiles.data ?? []) m[p.prototile_id] = p.sides;
    return m;
  }, [prototiles.data]);

  if (regions.error || placements.error)
    return <div className="page"><div className="error">{regions.error || placements.error}</div></div>;
  if (regions.loading || placements.loading || prototiles.loading)
    return <div className="page">Loading…</div>;

  const R = regions.data ?? [];
  const busiest = [...R].sort((a, b) => (b.placement_count ?? 0) - (a.placement_count ?? 0))[0];
  const active = sel ?? busiest?.region_id ?? R[0]?.region_id;
  const region = R.find((r) => r.region_id === active);
  const tiles = (placements.data ?? []).filter((p) => p.region === active);
  const selectedTile = tiles.find((t) => t.placement_id === selTile) ?? null;

  function flash(m: string) {
    setMsg(m);
    setErr(null);
  }

  function reloadAll() {
    placements.reload();
    regions.reload();
  }

  // ── placement mutations ───────────────────────────────────────────────
  async function addTile(prototileId: string, x: number, y: number) {
    if (!region) return;
    setErr(null);
    try {
      const id = `${region.region_id}-${prototileId}-${x}-${y}-${Date.now() % 100000}`;
      await api.create("placements", {
        placement_id: id,
        region: region.region_id,
        prototile: prototileId,
        x,
        y,
        rotation_deg: 0,
        is_overlapping: false,
      });
      flash("Tile placed — coverage recomputed by the database.");
      setSelTile(id);
      reloadAll();
    } catch (e: any) {
      setErr(e.message);
    }
  }

  async function patchTile(id: string, body: Row, note: string) {
    setErr(null);
    try {
      await api.patch("placements", id, body);
      flash(note);
      reloadAll();
    } catch (e: any) {
      setErr(e.message);
    }
  }

  async function delTile(id: string) {
    setErr(null);
    try {
      await api.remove("placements", id);
      flash("Tile removed — coverage recomputed.");
      if (selTile === id) setSelTile(null);
      reloadAll();
    } catch (e: any) {
      setErr(e.message);
    }
  }

  // ── drag & drop (snap to grid cells) ──────────────────────────────────
  function onCellDrop(gx: number, gy: number) {
    setHoverCell(null);
    if (!drag) return;
    if (drag.kind === "palette") {
      // dropping a new tile from the palette onto a cell
      if (!occupied.has(`${gx},${gy}`)) addTile(drag.prototile, gx, gy);
    } else {
      // moving an existing tile to a new cell
      const t = tiles.find((p) => p.placement_id === drag.placementId);
      if (t && (Number(t.x) !== gx || Number(t.y) !== gy)) {
        patchTile(drag.placementId, { x: gx, y: gy }, "Tile moved — IsInsideRegion / coverage recomputed.");
      }
    }
    setDrag(null);
  }

  const W = region ? Number(region.width) * CELL + PAD * 2 : 0;
  const H = region ? Number(region.height) * CELL + PAD * 2 : 0;
  const occupied = new Set(tiles.map((t) => `${t.x},${t.y}`));
  const brushProto = brush || (prototiles.data ?? [])[0]?.prototile_id || "";

  return (
    <div className="page">
      <div className="page-head">
        <h1>Tiling Studio</h1>
        <button className="btn" onClick={() => setShowRegionForm((s) => !s)}>
          {showRegionForm ? "Cancel" : "+ New region"}
        </button>
      </div>
      <p className="muted">
        Drag a tile from the palette onto an empty cell, or drag a placed tile to
        move it. Click a tile to select it and edit its properties on the right.
        Coverage, overlaps, tile area, and the “clean tiling” flag are all computed
        by the database and refresh on every edit — nothing runs in this browser.
      </p>

      {showRegionForm && (
        <NewRegionForm
          tilings={tilings.data ?? []}
          onCreated={(id) => {
            setShowRegionForm(false);
            setSel(id);
            setSelTile(null);
            flash("Region created.");
            regions.reload();
          }}
          onError={setErr}
        />
      )}

      <div className="region-tabs">
        {R.map((r) => (
          <button
            key={r.region_id}
            className={`tab ${r.region_id === active ? "active" : ""}`}
            onClick={() => {
              setSel(r.region_id);
              setSelTile(null);
            }}
          >
            {r.display_name}
            <span className="tab-sub">{r.width}×{r.height}</span>
          </button>
        ))}
      </div>

      {msg && <div className="toast ok">{msg}</div>}
      {err && <div className="toast no">{err}</div>}

      {region && (
        <div className="viz-layout">
          <div className="viz-canvas">
            <svg width={W} height={H} className="tiling-svg">
              {Array.from({ length: Number(region.height) }).map((_, gy) =>
                Array.from({ length: Number(region.width) }).map((_, gx) => {
                  const key = `${gx},${gy}`;
                  const cx = PAD + gx * CELL;
                  const cy = PAD + (Number(region.height) - 1 - gy) * CELL;
                  const isOcc = occupied.has(key);
                  const isHover = hoverCell === key;
                  return (
                    <rect
                      key={key}
                      x={cx}
                      y={cy}
                      width={CELL}
                      height={CELL}
                      className={`cell ${isOcc ? "" : "cell-empty"} ${isHover && !isOcc ? "cell-drop" : ""}`}
                      onClick={() => !isOcc && addTile(brushProto, gx, gy)}
                      onDragOver={(e) => {
                        if (!drag) return;
                        e.preventDefault();
                        setHoverCell(key);
                      }}
                      onDragLeave={() => setHoverCell((h) => (h === key ? null : h))}
                      onDrop={(e) => {
                        e.preventDefault();
                        onCellDrop(gx, gy);
                      }}
                    />
                  );
                }),
              )}
              {tiles.map((t) => {
                const sides = protoBySides[t.prototile] ?? 4;
                const cx = PAD + Number(t.x) * CELL + CELL / 2;
                const cy = PAD + (Number(region.height) - 1 - Number(t.y)) * CELL + CELL / 2;
                const isSel = t.placement_id === selTile;
                const outside = t.is_inside_region === false;
                return (
                  <polygon
                    key={t.placement_id}
                    points={polygonPoints(sides, cx, cy, Number(t.rotation_deg))}
                    fill={SHAPE_FILL[sides] ?? "#888"}
                    fillOpacity={t.is_overlapping ? 0.5 : 0.85}
                    stroke={isSel ? "#f0f6fc" : t.is_overlapping ? "#f85149" : outside ? "#f0883e" : "#0d1117"}
                    strokeWidth={isSel ? 3 : t.is_overlapping ? 2.5 : 1}
                    className={`tile ${isSel ? "tile-sel" : ""}`}
                    {...({ draggable: true } as any)}
                    onDragStart={() => {
                      setDrag({ kind: "tile", placementId: t.placement_id });
                      setSelTile(t.placement_id);
                    }}
                    onDragEnd={() => {
                      setDrag(null);
                      setHoverCell(null);
                    }}
                    onClick={(e) => {
                      e.stopPropagation();
                      setSelTile(t.placement_id);
                    }}
                  >
                    <title>{t.placement_id}</title>
                  </polygon>
                );
              })}
            </svg>

            <div className="brush-bar">
              <span className="muted">Drag onto the grid (or click a cell to place):</span>
              {(prototiles.data ?? []).map((p: Row) => (
                <button
                  key={p.prototile_id}
                  className={`brush ${brushProto === p.prototile_id ? "active" : ""}`}
                  style={{ borderColor: SHAPE_FILL[p.sides] ?? "#888" }}
                  draggable
                  onDragStart={() => setDrag({ kind: "palette", prototile: p.prototile_id })}
                  onDragEnd={() => {
                    setDrag(null);
                    setHoverCell(null);
                  }}
                  onClick={() => setBrush(p.prototile_id)}
                  title="Drag onto a cell, or click to make this the click-to-place brush"
                >
                  <span className="swatch" style={{ background: SHAPE_FILL[p.sides] ?? "#888" }} />
                  {p.display_name}
                </button>
              ))}
            </div>
          </div>

          <div className="viz-side">
            {selectedTile ? (
              <TileInspector
                key={selectedTile.placement_id}
                tile={selectedTile}
                prototiles={prototiles.data ?? []}
                onPatch={(body, note) => patchTile(selectedTile.placement_id, body, note)}
                onDelete={() => delTile(selectedTile.placement_id)}
                onClose={() => setSelTile(null)}
              />
            ) : (
              <RegionInspector
                region={region}
                tilings={tilings.data ?? []}
                onResize={(body) => resizeRegion(region.region_id, body)}
                onDelete={() => deleteRegion(region.region_id)}
              />
            )}
          </div>
        </div>
      )}
    </div>
  );

  // ── region mutations (closures over reloadAll/setSel) ─────────────────
  async function resizeRegion(id: string, body: Row) {
    setErr(null);
    try {
      await api.patch("regions", id, body);
      flash("Region updated — area & coverage recomputed.");
      regions.reload();
    } catch (e: any) {
      setErr(e.message);
    }
  }

  async function deleteRegion(id: string) {
    if (!confirm(`Delete region ${id} and all its placements?`)) return;
    setErr(null);
    try {
      // remove child placements first so the FK doesn't block the delete
      for (const t of tiles) await api.remove("placements", t.placement_id);
      await api.remove("regions", id);
      flash("Region deleted.");
      setSel(null);
      setSelTile(null);
      reloadAll();
    } catch (e: any) {
      setErr(e.message);
    }
  }
}

// ── Tile property editor ────────────────────────────────────────────────
function TileInspector({
  tile,
  prototiles,
  onPatch,
  onDelete,
  onClose,
}: {
  tile: Row;
  prototiles: Row[];
  onPatch: (body: Row, note: string) => void;
  onDelete: () => void;
  onClose: () => void;
}) {
  const [rot, setRot] = useState<number>(Number(tile.rotation_deg) || 0);
  const [x, setX] = useState<string>(String(tile.x));
  const [y, setY] = useState<string>(String(tile.y));

  return (
    <aside className="inspector">
      <div className="inspector-head">
        <h3>Tile</h3>
        <button className="btn-link" onClick={onClose}>close</button>
      </div>
      <div className="mono small muted">{tile.placement_id}</div>

      <label className="ins-field">
        <span>Prototile (shape)</span>
        <select
          value={tile.prototile}
          onChange={(e) => onPatch({ prototile: e.target.value }, "Shape changed — TileArea & coverage recomputed.")}
        >
          {prototiles.map((p) => (
            <option key={p.prototile_id} value={p.prototile_id}>
              {p.display_name} ({p.sides} sides)
            </option>
          ))}
        </select>
      </label>

      <label className="ins-field">
        <span>Rotation: {rot}°</span>
        <input
          type="range"
          min={0}
          max={359}
          step={1}
          value={rot}
          onChange={(e) => setRot(Number(e.target.value))}
          onMouseUp={() => onPatch({ rotation_deg: rot }, "Rotation saved.")}
          onTouchEnd={() => onPatch({ rotation_deg: rot }, "Rotation saved.")}
        />
      </label>

      <div className="ins-row">
        <label className="ins-field">
          <span>X</span>
          <input
            type="number"
            value={x}
            onChange={(e) => setX(e.target.value)}
            onBlur={() => Number(x) !== Number(tile.x) && onPatch({ x: Number(x) }, "X saved — IsInsideRegion recomputed.")}
          />
        </label>
        <label className="ins-field">
          <span>Y</span>
          <input
            type="number"
            value={y}
            onChange={(e) => setY(e.target.value)}
            onBlur={() => Number(y) !== Number(tile.y) && onPatch({ y: Number(y) }, "Y saved — IsInsideRegion recomputed.")}
          />
        </label>
      </div>

      <label className="ins-check">
        <input
          type="checkbox"
          checked={!!tile.is_overlapping}
          onChange={(e) => onPatch({ is_overlapping: e.target.checked }, "Overlap flag saved — OverlapCount recomputed.")}
        />
        <span>Marked overlapping (feeds <code>OverlapCount</code>)</span>
      </label>

      <div className="ins-derived">
        <div className="ins-derived-title">Derived (from <code>vw_placements</code>)</div>
        <Stat k="Tile area" v={fmt(tile.tile_area, 4)} />
        <Stat k="Tile sides" v={String(tile.tile_sides ?? "—")} />
        <Stat k="Inside region?" v={tile.is_inside_region ? "yes ✓" : "no"} good={tile.is_inside_region} />
      </div>

      <button className="btn btn-danger full" onClick={onDelete}>Delete tile</button>
    </aside>
  );
}

// ── Region rollups + resize/delete ──────────────────────────────────────
function RegionInspector({
  region,
  tilings,
  onResize,
  onDelete,
}: {
  region: Row;
  tilings: Row[];
  onResize: (body: Row) => void;
  onDelete: () => void;
}) {
  const [w, setW] = useState<string>(String(region.width));
  const [h, setH] = useState<string>(String(region.height));
  const [name, setName] = useState<string>(region.display_name ?? "");
  const [tgt, setTgt] = useState<string>(region.target_tiling ?? "");

  return (
    <aside className="inspector">
      <div className="inspector-head">
        <h3>{region.display_name}</h3>
      </div>
      <p className="muted small">Select a tile to edit it, or manage the region here.</p>

      <div className="ins-derived">
        <div className="ins-derived-title">Rollups (from <code>vw_regions</code>)</div>
        <Stat k="Region area" v={fmt(region.area, 0)} />
        <Stat k="Tiles placed" v={String(region.placement_count)} />
        <Stat k="Covered area" v={fmt(region.covered_area, 3)} />
        <Stat k="Coverage" v={`${fmt(region.coverage_pct, 1)}%`} highlight />
        <Stat k="Overlaps" v={String(region.overlap_count)} />
        <Stat k="Fully covered?" v={region.is_fully_covered ? "yes" : "no"} />
        <Stat k="Clean tiling?" v={region.is_clean_tiling ? "yes ✓" : "no"} highlight good={region.is_clean_tiling} />
      </div>
      <div className="bar-track">
        <div className="bar-fill" style={{ width: `${Math.min(100, Number(region.coverage_pct) || 0)}%` }} />
      </div>

      <div className="ins-derived-title" style={{ marginTop: 14 }}>Edit region (raw inputs)</div>
      <label className="ins-field">
        <span>Display name</span>
        <input
          value={name}
          onChange={(e) => setName(e.target.value)}
          onBlur={() => name !== region.display_name && name.trim() && onResize({ display_name: name })}
        />
      </label>
      <div className="ins-row">
        <label className="ins-field">
          <span>Width</span>
          <input
            type="number"
            min={1}
            value={w}
            onChange={(e) => setW(e.target.value)}
            onBlur={() => Number(w) !== Number(region.width) && Number(w) > 0 && onResize({ width: Number(w) })}
          />
        </label>
        <label className="ins-field">
          <span>Height</span>
          <input
            type="number"
            min={1}
            value={h}
            onChange={(e) => setH(e.target.value)}
            onBlur={() => Number(h) !== Number(region.height) && Number(h) > 0 && onResize({ height: Number(h) })}
          />
        </label>
      </div>
      <label className="ins-field">
        <span>Target tiling</span>
        <select
          value={tgt}
          onChange={(e) => {
            setTgt(e.target.value);
            onResize({ target_tiling: e.target.value || null });
          }}
        >
          <option value="">— none —</option>
          {tilings.map((t) => (
            <option key={t.tiling_id} value={t.tiling_id}>{t.display_name}</option>
          ))}
        </select>
      </label>

      <button className="btn btn-danger full" onClick={onDelete}>Delete region</button>
    </aside>
  );
}

// ── New-region form ─────────────────────────────────────────────────────
function NewRegionForm({
  tilings,
  onCreated,
  onError,
}: {
  tilings: Row[];
  onCreated: (id: string) => void;
  onError: (m: string) => void;
}) {
  const [name, setName] = useState("");
  const [w, setW] = useState("4");
  const [h, setH] = useState("4");
  const [tgt, setTgt] = useState("");

  async function submit() {
    try {
      if (!name.trim()) throw new Error("display name is required");
      const id = name.trim().toLowerCase().replace(/[^a-z0-9]+/g, "-").replace(/(^-|-$)/g, "");
      await api.create("regions", {
        region_id: id,
        display_name: name.trim(),
        width: Number(w),
        height: Number(h),
        ...(tgt ? { target_tiling: tgt } : {}),
      });
      onCreated(id);
    } catch (e: any) {
      onError(e.message);
    }
  }

  return (
    <div className="new-row">
      <label className="field">
        <span>Display name <b className="req">*</b></span>
        <input value={name} onChange={(e) => setName(e.target.value)} placeholder="e.g. Studio Test 5x5" />
      </label>
      <label className="field">
        <span>Width <b className="req">*</b></span>
        <input type="number" min={1} value={w} onChange={(e) => setW(e.target.value)} />
      </label>
      <label className="field">
        <span>Height <b className="req">*</b></span>
        <input type="number" min={1} value={h} onChange={(e) => setH(e.target.value)} />
      </label>
      <label className="field">
        <span>Target tiling</span>
        <select value={tgt} onChange={(e) => setTgt(e.target.value)}>
          <option value="">— none —</option>
          {tilings.map((t) => (
            <option key={t.tiling_id} value={t.tiling_id}>{t.display_name}</option>
          ))}
        </select>
      </label>
      <button className="btn" onClick={submit}>Create</button>
    </div>
  );
}

function Stat({ k, v, highlight, good }: { k: string; v: string; highlight?: boolean; good?: boolean }) {
  return (
    <div className={`stat-row ${highlight ? "hl" : ""}`}>
      <dt>{k}</dt>
      <dd className={good ? "good" : ""}>{v}</dd>
    </div>
  );
}

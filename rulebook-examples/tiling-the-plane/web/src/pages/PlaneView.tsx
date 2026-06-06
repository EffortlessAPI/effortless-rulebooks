import { useEffect, useMemo, useRef, useState } from "react";
import { api, Row } from "../lib/api";
import { useAsync } from "../lib/useApi";

/**
 * Plane View — "does it tile the plane, and HOW?"
 *
 * Doctrine (see ../../CLAUDE.md): the VIEW is the contract.
 *   - The yes/no verdict is read straight from vw_tilings.does_tile_plane.
 *     It is NOT recomputed here. The database derived it from
 *       AllVerticesValid  (every vertex figure closes to 360deg)   AND
 *       HasLattice        (two independent translation vectors exist).
 *   - The animation is pure presentation: it takes the lattice vectors
 *     (t1x,t1y,t2x,t2y) that the database stores and repeats the tiling's
 *     prototile motif across the plane along those vectors. The geometry is
 *     drawn from view values; nothing about validity is decided in this browser.
 *
 * If a tiling tiles the plane, you watch the fundamental motif propagate
 * outward along its lattice and fill the canvas. If it does not (regular
 * pentagon: vertex figure sums to 324deg, no lattice), there is nothing to
 * propagate — the verdict is NO and a single motif sits alone.
 */

const SCALE = 34; // px per unit cell
const SHAPE_FILL: Record<number, string> = {
  3: "#2ea043",
  4: "#1f6feb",
  5: "#db61a2",
  6: "#d29922",
  8: "#a371f7",
  12: "#56d4dd",
};

/** Vertices of a regular n-gon centered at (cx,cy) with circumradius r. */
function ngon(sides: number, cx: number, cy: number, r: number, rotDeg: number): string {
  const rot = (rotDeg * Math.PI) / 180 - Math.PI / 2;
  const pts: string[] = [];
  for (let i = 0; i < sides; i++) {
    const a = rot + (i * 2 * Math.PI) / sides;
    pts.push(`${(cx + r * Math.cos(a)).toFixed(2)},${(cy + r * Math.sin(a)).toFixed(2)}`);
  }
  return pts.join(" ");
}

function fmt(n: any, d = 3): string {
  if (n === null || n === undefined) return "—";
  const v = Number(n);
  return Number.isNaN(v) ? String(n) : v.toFixed(d);
}

type Cell = { i: number; j: number; dist: number; x: number; y: number };

export default function PlaneView() {
  const tilings = useAsync(() => api.list("tilings"), []);
  const tilingPrototiles = useAsync(() => api.list("tiling-prototiles"), []);
  const prototiles = useAsync(() => api.list("prototiles"), []);

  const [selId, setSelId] = useState<string | null>(null);
  const [playing, setPlaying] = useState(true);
  const [t, setT] = useState(0); // animation progress 0..1
  const raf = useRef<number | null>(null);

  // Sides lookup for prototiles.
  const sidesById = useMemo(() => {
    const m: Record<string, number> = {};
    for (const p of prototiles.data ?? []) m[p.prototile_id] = Number(p.sides);
    return m;
  }, [prototiles.data]);

  const T = tilings.data ?? [];
  const sel = T.find((x) => x.tiling_id === selId) ?? T[0] ?? null;

  // Which prototiles make up the selected tiling's motif (from the junction table).
  const motif = useMemo(() => {
    if (!sel) return [];
    return (tilingPrototiles.data ?? [])
      .filter((tp) => tp.tiling === sel.tiling_id)
      .map((tp) => ({ prototile: tp.prototile, sides: sidesById[tp.prototile] ?? 4 }));
  }, [tilingPrototiles.data, sel, sidesById]);

  // Restart the animation whenever the selected tiling changes.
  useEffect(() => {
    setT(0);
    setPlaying(true);
  }, [selId]);

  // Animation loop: ease t from 0 -> 1 over ~2.4s, then hold.
  useEffect(() => {
    if (!playing) return;
    let start: number | null = null;
    const DUR = 2400;
    const step = (ts: number) => {
      if (start === null) start = ts;
      const p = Math.min(1, (ts - start) / DUR);
      setT(p);
      if (p < 1) {
        raf.current = requestAnimationFrame(step);
      } else {
        setPlaying(false);
      }
    };
    raf.current = requestAnimationFrame(step);
    return () => {
      if (raf.current) cancelAnimationFrame(raf.current);
    };
  }, [playing]);

  if (tilings.error) return <div className="page"><div className="error">{tilings.error}</div></div>;
  if (tilings.loading || prototiles.loading) return <div className="page">Loading…</div>;

  const VIEW = 640;
  const HALF = VIEW / 2;
  const tiles = !sel ? false : sel.does_tile_plane === true;

  // Lattice vectors (unit cells -> px). Null/degenerate => no propagation.
  const t1 = sel ? { x: Number(sel.t1x) * SCALE, y: -Number(sel.t1y) * SCALE } : null;
  const t2 = sel ? { x: Number(sel.t2x) * SCALE, y: -Number(sel.t2y) * SCALE } : null;
  const hasLattice = sel?.has_lattice === true && t1 && t2 &&
    Number.isFinite(t1.x) && Number.isFinite(t1.y) && Number.isFinite(t2.x) && Number.isFinite(t2.y);

  // Build the set of lattice cells that fit on the canvas.
  const cells: Cell[] = [];
  if (sel && hasLattice && t1 && t2) {
    const N = 9; // range of lattice indices each way
    let maxDist = 0.0001;
    const raw: Cell[] = [];
    for (let i = -N; i <= N; i++) {
      for (let j = -N; j <= N; j++) {
        const x = HALF + i * t1.x + j * t2.x;
        const y = HALF + i * t1.y + j * t2.y;
        if (x < -SCALE || x > VIEW + SCALE || y < -SCALE || y > VIEW + SCALE) continue;
        const dist = Math.hypot(x - HALF, y - HALF);
        raw.push({ i, j, dist, x, y });
        if (dist > maxDist) maxDist = dist;
      }
    }
    // Normalize distance so the wavefront (t) reveals cells from center outward.
    for (const c of raw) cells.push({ ...c, dist: c.dist / maxDist });
  }

  // Motif radius: pack the distinct prototiles inside one cell footprint.
  const motifR = SCALE * 0.5;

  function MotifAt({ cx, cy, opacity }: { cx: number; cy: number; opacity: number }) {
    // For a single-prototile (regular) tiling, draw one centered polygon.
    // For multi-prototile (Archimedean) tilings, fan the distinct shapes around
    // the lattice point so the repeating unit is legible.
    if (motif.length <= 1) {
      const sides = motif[0]?.sides ?? 4;
      return (
        <polygon
          points={ngon(sides, cx, cy, motifR, 0)}
          fill={SHAPE_FILL[sides] ?? "#888"}
          fillOpacity={0.85 * opacity}
          stroke="#0d1117"
          strokeWidth={1}
        />
      );
    }
    return (
      <g>
        {motif.map((m, k) => {
          const a = (k / motif.length) * 2 * Math.PI - Math.PI / 2;
          const off = motifR * 0.55;
          const mx = cx + off * Math.cos(a);
          const my = cy + off * Math.sin(a);
          return (
            <polygon
              key={k}
              points={ngon(m.sides, mx, my, motifR * 0.5, 0)}
              fill={SHAPE_FILL[m.sides] ?? "#888"}
              fillOpacity={0.85 * opacity}
              stroke="#0d1117"
              strokeWidth={1}
            />
          );
        })}
      </g>
    );
  }

  return (
    <div className="page">
      <div className="page-head">
        <h1>Plane View</h1>
        <button className="btn" onClick={() => { setT(0); setPlaying(true); }}>
          ↻ Replay
        </button>
      </div>
      <p className="muted">
        Pick a tiling and watch how its fundamental motif repeats across the plane
        along the lattice translation vectors stored in <code>vw_tilings</code>.
        The verdict below is the database’s answer — <code>does_tile_plane</code> —
        not anything this browser decided.
      </p>

      <div className="region-tabs">
        {T.map((x) => (
          <button
            key={x.tiling_id}
            className={`tab ${x.tiling_id === (sel?.tiling_id) ? "active" : ""}`}
            onClick={() => setSelId(x.tiling_id)}
          >
            {x.display_name}
            <span className="tab-sub">
              {x.does_tile_plane === true ? "tiles ✓" : "does not tile"}
            </span>
          </button>
        ))}
      </div>

      {sel && (
        <>
          <div
            className="toast"
            style={{
              fontSize: 18,
              fontWeight: 700,
              padding: "14px 18px",
              background: tiles ? "rgba(46,160,67,0.14)" : "rgba(248,81,73,0.14)",
              border: `1px solid ${tiles ? "var(--green)" : "var(--red)"}`,
              color: tiles ? "#7ee787" : "#ffa198",
            }}
          >
            Does <b>{sel.display_name}</b> tile the plane?&nbsp;
            {tiles ? "YES ✓" : "NO ✗"}
            <span style={{ display: "block", fontSize: 12, fontWeight: 400, marginTop: 4, opacity: 0.9 }}>
              {tiles
                ? "Every vertex figure closes to 360° and two independent translation vectors repeat the motif across the whole plane."
                : sel.all_vertices_valid === false
                  ? "Its vertex figure does not close to 360° — the tiles leave a gap (or overlap) at every vertex, so it cannot fill the plane."
                  : "No non-degenerate lattice of translation vectors — the motif cannot be repeated to fill the plane."}
            </span>
          </div>

          <div className="viz-layout">
            <div className="viz-canvas">
              <svg width={VIEW} height={VIEW} className="tiling-svg">
                {/* lattice vector arrows from center */}
                {hasLattice && t1 && t2 && (
                  <g opacity={0.5}>
                    <line x1={HALF} y1={HALF} x2={HALF + t1.x} y2={HALF + t1.y} stroke="#79c0ff" strokeWidth={2} markerEnd="url(#arrow)" />
                    <line x1={HALF} y1={HALF} x2={HALF + t2.x} y2={HALF + t2.y} stroke="#f0883e" strokeWidth={2} markerEnd="url(#arrow)" />
                    <defs>
                      <marker id="arrow" markerWidth="8" markerHeight="8" refX="6" refY="3" orient="auto">
                        <path d="M0,0 L6,3 L0,6 Z" fill="#c9d1d9" />
                      </marker>
                    </defs>
                  </g>
                )}

                {hasLattice ? (
                  cells.map((c) => {
                    // wavefront: reveal a cell once t passes its normalized distance.
                    const lead = 0.18;
                    const op = Math.max(0, Math.min(1, (t - c.dist + lead) / lead));
                    if (op <= 0) return null;
                    return <MotifAt key={`${c.i},${c.j}`} cx={c.x} cy={c.y} opacity={op} />;
                  })
                ) : (
                  // No lattice: a single lonely motif, nothing to propagate.
                  <MotifAt cx={HALF} cy={HALF} opacity={1} />
                )}

                {!hasLattice && (
                  <text x={HALF} y={HALF + SCALE * 1.6} textAnchor="middle" fill="#ffa198" fontSize={13}>
                    no lattice — cannot repeat to fill the plane
                  </text>
                )}
              </svg>
            </div>

            <div className="viz-side">
              <aside className="inspector">
                <div className="inspector-head"><h3>{sel.display_name}</h3></div>
                <div className="ins-derived">
                  <div className="ins-derived-title">From <code>vw_tilings</code></div>
                  <Stat k="Vertex config" v={String(sel.vertex_config)} />
                  <Stat k="Kind" v={String(sel.kind)} />
                  <Stat k="Symmetry" v={String(sel.symmetry_notation ?? sel.symmetry_group ?? "—")} />
                  <Stat k="Vertex figures" v={String(sel.vertex_figure_count ?? "—")} />
                  <Stat k="…valid" v={String(sel.valid_vertex_figure_count ?? "—")} />
                  <Stat k="All vertices valid?" v={sel.all_vertices_valid ? "yes ✓" : "no"} good={sel.all_vertices_valid} />
                  <Stat k="T1" v={`(${fmt(sel.t1x, 2)}, ${fmt(sel.t1y, 2)})`} />
                  <Stat k="T2" v={`(${fmt(sel.t2x, 2)}, ${fmt(sel.t2y, 2)})`} />
                  <Stat k="Fund. domain area" v={fmt(sel.fundamental_domain_area, 3)} />
                  <Stat k="Has lattice?" v={sel.has_lattice ? "yes ✓" : "no"} good={sel.has_lattice} />
                  <Stat k="Tiles the plane?" v={tiles ? "YES ✓" : "NO ✗"} good={tiles} highlight />
                </div>
                <p className="muted small">
                  <span style={{ color: "#79c0ff" }}>━▸</span> T1&nbsp;&nbsp;
                  <span style={{ color: "#f0883e" }}>━▸</span> T2 — the two
                  translation vectors. The plane is the integer combinations
                  i·T1 + j·T2 of these.
                </p>
              </aside>
            </div>
          </div>
        </>
      )}
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

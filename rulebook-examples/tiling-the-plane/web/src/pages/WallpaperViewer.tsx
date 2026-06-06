import { useEffect, useMemo, useRef, useState } from "react";
import { api, Row } from "../lib/api";
import { useAsync } from "../lib/useApi";

/**
 * Wallpaper Viewer — pick one of the 17 wallpaper groups, draw a motif in the
 * fundamental domain, and watch the group's symmetry replicate it across the plane.
 *
 * Doctrine (see ../../CLAUDE.md): the VIEW is the contract.
 *   - The symmetry RECIPE (lattice type/angle/ratio + the generator coset-rep ops)
 *     is read straight from vw_symmetry_groups. Nothing about the symmetry is
 *     decided in this browser — we only *apply* the ops the database stores.
 *   - Saved designs round-trip through vw_wallpaper_designs (reads) and the
 *     wallpaper_designs base table (writes), like every other entity.
 * The drawing pad + animation are pure presentation (a runtime concern the SSoT
 * explicitly doesn't model), but every transform it draws comes from view data.
 */

// ---- geometry types ------------------------------------------------------
type Mat = [number, number, number, number]; // a,b,c,d
type Op = { k: string; m: Mat; t: [number, number] };
type Stroke = { color: string; width: number; points: [number, number][]; filled?: boolean };

/** A stroke is "closed" when its first and last points are within threshold fractional units. */
function isClosed(points: [number, number][], threshold = 0.06): boolean {
  if (points.length < 3) return false;
  const [u0, v0] = points[0];
  const [u1, v1] = points[points.length - 1];
  return Math.hypot(u1 - u0, v1 - v0) < threshold;
}

const CANVAS = 560;
const PAD_CANVAS = 320;

// Basis vectors (in px, before centering) for each lattice class.
function basis(latticeType: string, angleDeg: number, ratio: number, scale: number) {
  const a = (angleDeg * Math.PI) / 180;
  const B1: [number, number] = [scale, 0];
  const B2: [number, number] = [scale * ratio * Math.cos(a), scale * ratio * Math.sin(a)];
  return { B1, B2 };
}

/** fractional (u,v) -> pixel (x,y) under basis. */
function toPx(u: number, v: number, B1: [number, number], B2: [number, number]): [number, number] {
  return [u * B1[0] + v * B2[0], u * B1[1] + v * B2[1]];
}

/** apply an affine op (fractional coords) to a fractional point. */
function applyOp(op: Op, u: number, v: number): [number, number] {
  const [a, b, c, d] = op.m;
  return [a * u + b * v + op.t[0], c * u + d * v + op.t[1]];
}

const IDENTITY: Op = { k: "id", m: [1, 0, 0, 1], t: [0, 0] };

const PALETTE = ["#1f6feb", "#2ea043", "#d29922", "#a371f7", "#db61a2", "#e6edf3"];

export default function WallpaperViewer() {
  const groups = useAsync(() => api.list("symmetry-groups"), []);
  const designs = useAsync(() => api.list("wallpaper-designs"), []);

  const [groupId, setGroupId] = useState<string>("");
  const [strokes, setStrokes] = useState<Stroke[]>([]);
  const [current, setCurrent] = useState<[number, number][]>([]);
  const [color, setColor] = useState<string>(PALETTE[0]);
  const [width, setWidth] = useState<number>(3);
  const [designName, setDesignName] = useState<string>("");
  const [loadedId, setLoadedId] = useState<string | null>(null);
  const [showGrid, setShowGrid] = useState(true);
  const [showFundamental, setShowFundamental] = useState(true);
  const [showFrame, setShowFrame] = useState(true);
  const [fillMode, setFillMode] = useState(false);
  // Frame / export controls
  const [frameShape, setFrameShape] = useState<"rect" | "circle" | "hex" | "octagon">("rect");
  const [frameW, setFrameW] = useState(200);   // mm
  const [frameH, setFrameH] = useState(200);   // mm
  const [cellScale, setCellScale] = useState(60); // mm per lattice unit
  const [msg, setMsg] = useState<string | null>(null);
  const [err, setErr] = useState<string | null>(null);
  const drawing = useRef(false);

  const G = groups.data ?? [];
  // default to a rich group once data lands
  useEffect(() => {
    if (!groupId && G.length) {
      setGroupId(G.find((g) => g.notation === "p4m")?.symmetry_group_id ?? G[0].symmetry_group_id);
    }
  }, [G, groupId]);

  const group = G.find((g) => g.symmetry_group_id === groupId);

  const ops: Op[] = useMemo(() => {
    if (!group?.generators) return [IDENTITY];
    try {
      const gen: Op[] = JSON.parse(group.generators);
      return [IDENTITY, ...gen];
    } catch {
      return [IDENTITY];
    }
  }, [group]);

  if (groups.error) return <div className="page"><div className="error">{groups.error}</div></div>;
  if (groups.loading) return <div className="page">Loading…</div>;

  // ---- drawing pad handlers (fractional coords in [0,1]) -----------------
  function padXY(e: React.MouseEvent): [number, number] {
    const rect = (e.currentTarget as SVGElement).getBoundingClientRect();
    const u = (e.clientX - rect.left) / rect.width;
    const v = 1 - (e.clientY - rect.top) / rect.height; // flip y so up is +v
    return [Math.min(1, Math.max(0, u)), Math.min(1, Math.max(0, v))];
  }
  function onPadDown(e: React.MouseEvent) {
    drawing.current = true;
    const pt = padXY(e);
    setCurrent([pt]);
  }
  function onPadMove(e: React.MouseEvent) {
    if (!drawing.current) return;
    const pt = padXY(e); // capture before setState closure runs
    setCurrent((c) => [...c, pt]);
  }
  function onPadUp() {
    if (drawing.current && current.length > 1) {
      // In fill mode, snap the last point to the first to close the loop automatically.
      const pts: [number, number][] = fillMode
        ? [...current, current[0]]
        : current;
      const closed = isClosed(pts);
      setStrokes((s) => [...s, { color, width, points: pts, filled: fillMode && closed }]);
    }
    drawing.current = false;
    setCurrent([]);
  }

  function undo() { setStrokes((s) => s.slice(0, -1)); }
  function clearAll() { setStrokes([]); setCurrent([]); }

  async function save() {
    setErr(null); setMsg(null);
    if (!group) return;
    if (strokes.length === 0) { setErr("Draw something in the fundamental domain first."); return; }
    const name = designName.trim() || `${group.notation} design`;
    const id = (loadedId ?? `${group.notation}-${name}`)
      .toLowerCase().replace(/[^a-z0-9]+/g, "-").replace(/^-|-$/g, "");
    const body: Row = {
      wallpaper_design_id: id,
      display_name: name,
      symmetry_group: group.symmetry_group_id,
      strokes: JSON.stringify(strokes),
      stroke_color: color,
      background_color: "#0a0d12",
    };
    try {
      // upsert: try patch, fall back to create
      if (loadedId) {
        await api.patch("wallpaper-designs", id, body);
      } else {
        try { await api.create("wallpaper-designs", body); }
        catch { await api.patch("wallpaper-designs", id, body); }
      }
      setLoadedId(id);
      setMsg(`Saved “${name}”.`);
      designs.reload();
    } catch (e: any) { setErr(e.message); }
  }

  function load(d: Row) {
    try {
      setStrokes(JSON.parse(d.strokes || "[]"));
    } catch { setStrokes([]); }
    setGroupId(d.symmetry_group);
    setDesignName(d.display_name);
    setColor(d.stroke_color || PALETTE[0]);
    setLoadedId(d.wallpaper_design_id);
    setMsg(`Loaded “${d.display_name}”.`);
  }

  function newDesign() {
    setStrokes([]); setCurrent([]); setLoadedId(null); setDesignName("");
    setMsg(null); setErr(null);
  }

  /** Toggle fill on/off for the current stroke being drawn, or the last stroke. */
  function toggleFill(idx: number) {
    setStrokes((s) =>
      s.map((st, i) => i === idx ? { ...st, filled: !st.filled } : st),
    );
  }

  async function downloadScad() {
    setErr(null);
    if (!group) return;
    if (strokes.length === 0) { setErr("Draw something first."); return; }
    try {
      const r = await fetch("/api/export/wallpaper-scad", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          groupId: group.symmetry_group_id,
          strokes,
          tiles: Math.ceil(Math.max(frameW, frameH) / cellScale) + 2,
          scale: cellScale,
          frame: { shape: frameShape, width: frameW, height: frameH },
        }),
      });
      if (!r.ok) {
        const j = await r.json().catch(() => ({}));
        throw new Error(j.error || `export failed (${r.status})`);
      }
      const cd = r.headers.get("content-disposition") ?? "";
      const m = /filename="?([^"]+)"?/.exec(cd);
      const filename = m?.[1] ?? `wallpaper-${group.notation}.scad`;
      const blob = await r.blob();
      const a = document.createElement("a");
      a.href = URL.createObjectURL(blob);
      a.download = filename;
      a.click();
      URL.revokeObjectURL(a.href);
    } catch (e: any) { setErr(e.message); }
  }

  async function del(d: Row) {
    if (!confirm(`Delete “${d.display_name}”?`)) return;
    try {
      await api.remove("wallpaper-designs", d.wallpaper_design_id);
      if (loadedId === d.wallpaper_design_id) newDesign();
      designs.reload();
    } catch (e: any) { setErr(e.message); }
  }

  // ---- plane rendering ---------------------------------------------------
  // SVG pixels per lattice unit — scale the view so the frame fits comfortably.
  const svgScale = Math.min(70, (CANVAS * 0.8) / Math.max(frameW / cellScale, frameH / cellScale));
  const { B1, B2 } = group
    ? basis(group.lattice_type ?? "square", Number(group.lattice_angle_deg) || 90, Number(group.lattice_ratio) || 1, svgScale)
    : basis("square", 90, 1, svgScale);

  // one motif copy = each op applied to every stroke; then tile by lattice.
  function cellStrokes(): Stroke[] {
    const all: Stroke[] = [];
    for (const op of ops) {
      for (const st of strokes) {
        all.push({
          color: st.color, width: st.width, filled: st.filled,
          points: st.points.map(([u, v]) => applyOp(op, u, v)),
        });
      }
    }
    return all;
  }

  // NOTE: plain call, not useMemo — this runs AFTER the early returns above, so a
  // hook here would violate the Rules of Hooks. The work is cheap (a few strokes).
  const reps = cellStrokes();

  // tile enough to cover the frame + bleed
  const N = Math.ceil(Math.max(frameW, frameH) / cellScale) + 2;
  const tiles: [number, number][] = [];
  for (let i = -N; i <= N; i++) for (let j = -N; j <= N; j++) tiles.push([i, j]);
  const cx = CANVAS / 2, cy = CANVAS / 2;

  // Frame in SVG coordinates: frameW/cellScale lattice units → pixels via svgScale
  const fwPx = (frameW / cellScale) * svgScale;
  const fhPx = (frameH / cellScale) * svgScale;

  // Build the SVG clip/overlay path for the chosen frame shape.
  function frameClipPath(): string {
    const hw = fwPx / 2, hh = fhPx / 2;
    if (frameShape === "circle") {
      const r = Math.min(hw, hh);
      return `M${cx - r},${cy} a${r},${r} 0 1,0 ${r * 2},0 a${r},${r} 0 1,0 -${r * 2},0`;
    }
    if (frameShape === "hex") {
      const r = Math.min(hw, hh);
      const pts = Array.from({ length: 6 }, (_, i) => {
        const a = (i * 60 - 30) * Math.PI / 180;
        return `${cx + r * Math.cos(a)},${cy + r * Math.sin(a)}`;
      });
      return `M${pts[0]} L${pts.slice(1).join(" L")} Z`;
    }
    if (frameShape === "octagon") {
      const r = Math.min(hw, hh);
      const pts = Array.from({ length: 8 }, (_, i) => {
        const a = (i * 45) * Math.PI / 180;
        return `${cx + r * Math.cos(a)},${cy + r * Math.sin(a)}`;
      });
      return `M${pts[0]} L${pts.slice(1).join(" L")} Z`;
    }
    // rect
    return `M${cx - hw},${cy - hh} h${fwPx} v${fhPx} h${-fwPx} Z`;
  }

  function strokePath(st: Stroke, i: number, j: number): string {
    return st.points
      .map(([u, v], k) => {
        const [px, py] = toPx(u + i, v + j, B1, B2);
        // y flips so +v is up on screen
        return `${k === 0 ? "M" : "L"}${(cx + px).toFixed(1)},${(cy - py).toFixed(1)}`;
      })
      .join(" ");
  }

  return (
    <div className="page">
      <h1>Wallpaper Viewer</h1>
      <p className="muted">
        Pick one of the 17 wallpaper groups, draw a motif in the fundamental
        domain on the left, and watch the group’s symmetry repeat it across the
        plane on the right. The symmetry recipe — lattice and the{" "}
        {group ? <b>{group.operation_count}</b> : "…"} point-group operations — is
        read straight from <code>vw_symmetry_groups</code>; this page only applies
        what the database stores.
      </p>

      {/* group picker */}
      <div className="group-grid">
        {[...G]
          .sort((a, b) => (a.rotation_order - b.rotation_order) || a.notation.localeCompare(b.notation))
          .map((g) => (
            <button
              key={g.symmetry_group_id}
              className={`group-chip ${g.symmetry_group_id === groupId ? "active" : ""}`}
              onClick={() => setGroupId(g.symmetry_group_id)}
              title={g.description ?? ""}
            >
              <span className="g-notation">{g.notation}</span>
              <span className="g-orbifold">{g.orbifold}</span>
            </button>
          ))}
      </div>

      {msg && <div className="toast ok">{msg}</div>}
      {err && <div className="toast no">{err}</div>}

      <div className="wp-layout">
        {/* ---- fundamental-domain drawing pad ---- */}
        <div className="wp-pad-col">
          <h3>Fundamental domain</h3>
          <p className="muted small">Click-drag to draw. Your strokes are the seed motif.</p>
          <svg
            className="wp-pad"
            width={PAD_CANVAS}
            height={PAD_CANVAS}
            onMouseDown={onPadDown}
            onMouseMove={onPadMove}
            onMouseUp={onPadUp}
            onMouseLeave={onPadUp}
          >
            <rect x={0} y={0} width={PAD_CANVAS} height={PAD_CANVAS} fill="#0a0d12" />
            {/* fractional grid */}
            {[0.25, 0.5, 0.75].map((f) => (
              <g key={f} stroke="#21262d" strokeWidth={1}>
                <line x1={f * PAD_CANVAS} y1={0} x2={f * PAD_CANVAS} y2={PAD_CANVAS} />
                <line x1={0} y1={f * PAD_CANVAS} x2={PAD_CANVAS} y2={f * PAD_CANVAS} />
              </g>
            ))}
            <rect x={0} y={0} width={PAD_CANVAS} height={PAD_CANVAS} fill="none" stroke="#30363d" />
            {/* saved strokes — closed+filled ones render as polygon, open ones as polyline */}
            {strokes.map((st, i) => {
              const ptStr = st.points.map(([u, v]) => `${u * PAD_CANVAS},${(1 - v) * PAD_CANVAS}`).join(" ");
              return st.filled ? (
                <polygon
                  key={i}
                  points={ptStr}
                  fill={st.color + "55"}
                  stroke={st.color}
                  strokeWidth={st.width}
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  style={{ cursor: "pointer" }}
                  onClick={() => toggleFill(i)}
                  {...({ title: "Click to toggle fill" } as any)}
                />
              ) : (
                <polyline
                  key={i}
                  points={ptStr}
                  fill="none"
                  stroke={st.color}
                  strokeWidth={st.width}
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  style={{ cursor: isClosed(st.points) ? "pointer" : undefined }}
                  onClick={() => isClosed(st.points) && toggleFill(i)}
                  {...(isClosed(st.points) ? ({ title: "Click to fill shape" } as any) : {})}
                />
              );
            })}
            {/* in-progress stroke */}
            {current.length > 1 && (
              <polyline
                points={current.map(([u, v]) => `${u * PAD_CANVAS},${(1 - v) * PAD_CANVAS}`).join(" ")}
                fill="none"
                stroke={color}
                strokeWidth={width}
                strokeLinecap="round"
                strokeLinejoin="round"
                opacity={0.7}
              />
            )}
          </svg>

          <div className="pad-tools">
            <div className="swatches">
              {PALETTE.map((c) => (
                <button
                  key={c}
                  className={`swatch-btn ${c === color ? "active" : ""}`}
                  style={{ background: c }}
                  onClick={() => setColor(c)}
                />
              ))}
            </div>
            <label className="width-ctl">
              width
              <input type="range" min={1} max={8} value={width} onChange={(e) => setWidth(Number(e.target.value))} />
            </label>
            <button
              className={`btn-link ${fillMode ? "fill-active" : ""}`}
              onClick={() => setFillMode((f) => !f)}
              title="Fill mode: closed shapes are filled with the stroke color"
            >
              {fillMode ? "◆ fill on" : "◇ fill off"}
            </button>
            <button className="btn-link" onClick={undo} disabled={!strokes.length}>undo</button>
            <button className="btn-link danger" onClick={clearAll} disabled={!strokes.length}>clear</button>
          </div>

          <div className="save-row">
            <input
              className="cell-input"
              placeholder="design name"
              value={designName}
              onChange={(e) => setDesignName(e.target.value)}
            />
            <button className="btn" onClick={save}>{loadedId ? "Update" : "Save"}</button>
            <button className="btn btn-secondary" onClick={newDesign}>New</button>
            <button
              className="btn btn-secondary"
              onClick={downloadScad}
              disabled={!strokes.length}
              title="Download as OpenSCAD 2D pattern for laser cutting"
            >
              ⬇ .scad
            </button>
          </div>
        </div>

        {/* ---- the tiled plane ---- */}
        <div className="wp-plane-col">
          <div className="plane-head">
            <h3>{group ? `${group.display_name} (${group.notation})` : "—"}</h3>
            <div className="plane-toggles">
              <label><input type="checkbox" checked={showGrid} onChange={(e) => setShowGrid(e.target.checked)} /> lattice</label>
              <label><input type="checkbox" checked={showFundamental} onChange={(e) => setShowFundamental(e.target.checked)} /> base cell</label>
              <label><input type="checkbox" checked={showFrame} onChange={(e) => setShowFrame(e.target.checked)} /> frame</label>
            </div>
          </div>

          {/* Frame / export controls */}
          <div className="frame-controls">
            <div className="frame-shapes">
              {(["rect", "circle", "hex", "octagon"] as const).map((s) => (
                <button
                  key={s}
                  className={`frame-shape-btn ${frameShape === s ? "active" : ""}`}
                  onClick={() => setFrameShape(s)}
                  title={s}
                >
                  {s === "rect" ? "▭" : s === "circle" ? "○" : s === "hex" ? "⬡" : "⯃"}
                </button>
              ))}
            </div>
            <label className="frame-dim">
              <span>W</span>
              <input type="number" min={20} max={1000} step={10} value={frameW}
                onChange={(e) => setFrameW(Math.max(20, Number(e.target.value)))} />
              <span>mm</span>
            </label>
            {frameShape === "rect" && (
              <label className="frame-dim">
                <span>H</span>
                <input type="number" min={20} max={1000} step={10} value={frameH}
                  onChange={(e) => setFrameH(Math.max(20, Number(e.target.value)))} />
                <span>mm</span>
              </label>
            )}
            <label className="frame-dim">
              <span>cell</span>
              <input type="number" min={10} max={300} step={5} value={cellScale}
                onChange={(e) => setCellScale(Math.max(10, Number(e.target.value)))} />
              <span>mm</span>
            </label>
          </div>

          <svg className="wp-plane" width={CANVAS} height={CANVAS}>
            <defs>
              <clipPath id="frame-clip">
                <path d={frameClipPath()} />
              </clipPath>
            </defs>
            <rect x={0} y={0} width={CANVAS} height={CANVAS} fill="#0a0d12" />
            {/* tiling clipped to frame */}
            <g clipPath={showFrame ? "url(#frame-clip)" : undefined}>
              {/* lattice grid */}
              {showGrid && tiles.map(([i, j]) => {
                const [ox, oy] = toPx(i, j, B1, B2);
                const [e1x, e1y] = toPx(i + 1, j, B1, B2);
                const [e2x, e2y] = toPx(i, j + 1, B1, B2);
                return (
                  <g key={`g${i}-${j}`} stroke="#1b2230" strokeWidth={1}>
                    <line x1={cx + ox} y1={cy - oy} x2={cx + e1x} y2={cy - e1y} />
                    <line x1={cx + ox} y1={cy - oy} x2={cx + e2x} y2={cy - e2y} />
                  </g>
                );
              })}
              {/* fundamental cell outline at origin */}
              {showFundamental && (() => {
                const corners = [[0, 0], [1, 0], [1, 1], [0, 1]].map(([u, v]) => toPx(u, v, B1, B2));
                return (
                  <polygon
                    points={corners.map(([x, y]) => `${cx + x},${cy - y}`).join(" ")}
                    fill="rgba(31,111,235,0.07)"
                    stroke="#1f6feb"
                    strokeWidth={1.5}
                    strokeDasharray="4 3"
                  />
                );
              })()}
              {/* replicated motif */}
              {tiles.map(([i, j]) =>
                reps.map((st, k) => {
                  const pts = st.points
                    .map(([u, v]) => {
                      const [px, py] = toPx(u + i, v + j, B1, B2);
                      return `${(cx + px).toFixed(1)},${(cy - py).toFixed(1)}`;
                    })
                    .join(" ");
                  return st.filled ? (
                    <polygon
                      key={`${i}-${j}-${k}`}
                      points={pts}
                      fill={st.color + "55"}
                      stroke={st.color}
                      strokeWidth={st.width * 0.6}
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  ) : (
                    <path
                      key={`${i}-${j}-${k}`}
                      d={strokePath(st, i, j)}
                      fill="none"
                      stroke={st.color}
                      strokeWidth={st.width}
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  );
                }),
              )}
            </g>
            {/* frame border overlay — always on top of the tiling */}
            {showFrame && (
              <>
                {/* dim the outside */}
                <path
                  d={`M0,0 h${CANVAS} v${CANVAS} h${-CANVAS} Z M${frameClipPath().slice(1)}`}
                  fill="rgba(0,0,0,0.55)"
                  fillRule="evenodd"
                />
                {/* crisp border */}
                <path
                  d={frameClipPath()}
                  fill="none"
                  stroke="#e6edf3"
                  strokeWidth={1.5}
                  strokeDasharray="6 3"
                />
                {/* size label */}
                <text
                  x={cx}
                  y={cy + fhPx / 2 + 14}
                  textAnchor="middle"
                  fill="#8b949e"
                  fontSize={10}
                >
                  {frameShape === "circle" || frameShape === "hex" || frameShape === "octagon"
                    ? `⌀ ${frameW} mm`
                    : `${frameW} × ${frameH} mm`}
                </text>
              </>
            )}
          </svg>
          <p className="muted small">
            {strokes.length === 0
              ? "Draw in the fundamental domain to see the wallpaper fill in."
              : `${strokes.length} stroke${strokes.length > 1 ? "s" : ""} × ${ops.length} op${ops.length > 1 ? "s" : ""} = ${reps.length} copies/cell · cell ${cellScale} mm`}
          </p>
        </div>
      </div>

      {/* saved designs */}
      <section>
        <h2>Saved designs</h2>
        {designs.loading ? (
          <p className="muted">Loading…</p>
        ) : (designs.data ?? []).length === 0 ? (
          <p className="muted">No saved designs yet — draw one and hit Save.</p>
        ) : (
          <div className="design-grid">
            {(designs.data ?? []).map((d: Row) => (
              <div key={d.wallpaper_design_id} className={`design-card ${loadedId === d.wallpaper_design_id ? "active" : ""}`}>
                <DesignThumb design={d} groups={G} />
                <div className="design-meta">
                  <div className="design-name">{d.display_name}</div>
                  <div className="design-sub">
                    <code>{d.group_notation}</code> · {d.group_orbifold} · {d.group_lattice_type}
                  </div>
                  <div className="design-actions">
                    <button className="btn-link" onClick={() => load(d)}>load</button>
                    <button className="btn-link danger" onClick={() => del(d)}>delete</button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}

/** Small static thumbnail of a saved design's tiled plane. */
function DesignThumb({ design, groups }: { design: Row; groups: Row[] }) {
  const g = groups.find((x) => x.symmetry_group_id === design.symmetry_group);
  const S = 120, sc = 26, cx = S / 2, cy = S / 2;
  let strokes: Stroke[] = [];
  try { strokes = JSON.parse(design.strokes || "[]"); } catch {}
  let ops: Op[] = [IDENTITY];
  try { if (g?.generators) ops = [IDENTITY, ...JSON.parse(g.generators)]; } catch {}
  const { B1, B2 } = basis(g?.lattice_type ?? "square", Number(g?.lattice_angle_deg) || 90, Number(g?.lattice_ratio) || 1, sc);
  const reps: Stroke[] = [];
  for (const op of ops) for (const st of strokes)
    reps.push({ color: st.color, width: 1.5, points: st.points.map(([u, v]) => applyOp(op, u, v)) });
  const tiles: [number, number][] = [];
  for (let i = -3; i <= 3; i++) for (let j = -3; j <= 3; j++) tiles.push([i, j]);
  return (
    <svg width={S} height={S} className="design-thumb">
      <rect x={0} y={0} width={S} height={S} fill="#0a0d12" />
      {tiles.map(([i, j]) =>
        reps.map((st, k) => (
          <path
            key={`${i}-${j}-${k}`}
            d={st.points.map(([u, v], n) => {
              const px = (u + i) * B1[0] + (v + j) * B2[0];
              const py = (u + i) * B1[1] + (v + j) * B2[1];
              return `${n === 0 ? "M" : "L"}${(cx + px).toFixed(1)},${(cy - py).toFixed(1)}`;
            }).join(" ")}
            fill="none"
            stroke={st.color}
            strokeWidth={st.width}
            strokeLinecap="round"
          />
        )),
      )}
    </svg>
  );
}

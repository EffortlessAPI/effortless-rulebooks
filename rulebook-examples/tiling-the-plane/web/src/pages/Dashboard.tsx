import { Link } from "react-router-dom";
import { api, Row } from "../lib/api";
import { useAsync } from "../lib/useApi";
import { DagCell } from "../explainer-dag";

function fmt(n: any, digits = 2): string {
  if (n === null || n === undefined) return "—";
  const v = Number(n);
  return Number.isNaN(v) ? String(n) : v.toFixed(digits);
}

export default function Dashboard() {
  const tilings = useAsync(() => api.list("tilings"), []);
  const prototiles = useAsync(() => api.list("prototiles"), []);
  const vfs = useAsync(() => api.list("vertex-figures"), []);
  const regions = useAsync(() => api.list("regions"), []);

  const loading =
    tilings.loading || prototiles.loading || vfs.loading || regions.loading;
  const error = tilings.error || prototiles.error || vfs.error || regions.error;

  if (error)
    return (
      <div className="page">
        <div className="error">Could not reach the API: {error}</div>
        <p>Is the server running? Try <code>./start.sh all</code>.</p>
      </div>
    );
  if (loading) return <div className="page">Loading…</div>;

  const T = tilings.data ?? [];
  const validTilings = T.filter((t) => t.all_vertices_valid).length;
  const regular = T.filter((t) => t.is_regular_tiling).length;
  const tilesPlane = T.filter((t) => t.does_tile_plane).length;
  const validVF = (vfs.data ?? []).filter((v) => v.is_valid).length;
  const totalVF = (vfs.data ?? []).length;

  return (
    <div className="page">
      <h1>Tiling the Plane</h1>
      <p className="lede">
        A catalog of plane tilings whose validity is <em>derived</em> from the
        angles meeting at each vertex, plus a generative engine that lays tiles
        into a region and measures how much it covers. Every number below is read
        straight from the database views — nothing is computed in the browser.
      </p>

      <div className="stat-grid">
        <Stat label="Tilings catalogued" value={String(T.length)} to="/t/tilings" />
        <Stat
          label="…that tile the plane"
          value={`${tilesPlane} / ${T.length}`}
          hint="valid vertices AND a lattice"
          to="/plane"
        />
        <Stat
          label="Valid vertex figures"
          value={`${validVF} / ${totalVF}`}
          hint="interior angles sum to 360°"
          to="/t/vertex-figures"
        />
        <Stat label="Prototiles" value={String((prototiles.data ?? []).length)} to="/t/prototiles" />
      </div>

      <section>
        <h2>Tilings</h2>
        <p className="muted">
          A tiling fills the plane when <strong>two</strong> things hold, both
          computed — not typed in: every vertex figure closes to a full 360° turn
          (<code>AllVerticesValid</code>) <em>and</em> two independent translation
          vectors exist (<code>HasLattice</code>). Open the{" "}
          <Link to="/plane">Plane View</Link> to watch a tiling propagate along its
          lattice — or see the lone motif of one that can’t.
        </p>
        <table className="grid">
          <thead>
            <tr>
              <th>Tiling</th>
              <th>Vertex config</th>
              <th>Kind</th>
              <th>Symmetry</th>
              <th className="num">Prototiles</th>
              <th>Regular?</th>
              <th>Vertices valid?</th>
              <th>Has lattice?</th>
              <th>Tiles the plane?</th>
            </tr>
          </thead>
          <tbody>
            {T.map((t) => (
              <tr key={t.tiling_id}>
                <td><Link to="/t/tilings">{t.display_name}</Link></td>
                <td><code>{t.vertex_config}</code></td>
                <td>{t.kind}</td>
                <td><DagCell table="Tilings" field="SymmetryNotation">{t.symmetry_notation ?? "—"}</DagCell></td>
                <td className="num"><DagCell table="Tilings" field="DistinctPrototileCount">{t.distinct_prototile_count}</DagCell></td>
                <td><DagCell table="Tilings" field="IsRegularTiling"><Bool v={t.is_regular_tiling} /></DagCell></td>
                <td><DagCell table="Tilings" field="AllVerticesValid"><Bool v={t.all_vertices_valid} /></DagCell></td>
                <td><DagCell table="Tilings" field="HasLattice"><Bool v={t.has_lattice} /></DagCell></td>
                <td>
                  <Link to="/plane" className="verdict-link">
                    <DagCell table="Tilings" field="DoesTilePlane"><Bool v={t.does_tile_plane} /></DagCell>
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>

      <section>
        <h2>Prototiles</h2>
        <p className="muted">
          Area uses real trigonometry (¼·n·s²·cot(π/n)) implemented in the
          database's <code>02b</code> customization seam — beyond the spreadsheet
          formula language, but still part of the model.
        </p>
        <table className="grid">
          <thead>
            <tr>
              <th>Shape</th>
              <th className="num">Sides</th>
              <th className="num">Interior angle</th>
              <th className="num">Area (unit edge)</th>
              <th className="num">Used in tilings</th>
            </tr>
          </thead>
          <tbody>
            {(prototiles.data ?? []).map((p: Row) => (
              <tr key={p.prototile_id}>
                <td>{p.display_name}</td>
                <td className="num">{p.sides}</td>
                <td className="num"><DagCell table="Prototiles" field="InteriorAngleDeg">{fmt(p.interior_angle_deg, 0)}°</DagCell></td>
                <td className="num"><DagCell table="Prototiles" field="Area">{fmt(p.area, 4)}</DagCell></td>
                <td className="num"><DagCell table="Prototiles" field="UsedInTilingsCount">{p.used_in_tilings_count}</DagCell></td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>

      <section>
        <h2>Regions — coverage</h2>
        <p className="muted">
          Lay tiles into a region; coverage, overlaps, and the “clean tiling”
          flag roll up automatically. Open the{" "}
          <Link to="/studio">Tiling Studio</Link> to see them and edit live.
        </p>
        <table className="grid">
          <thead>
            <tr>
              <th>Region</th>
              <th className="num">Area</th>
              <th className="num">Tiles</th>
              <th className="num">Coverage</th>
              <th className="num">Overlaps</th>
              <th>Clean tiling?</th>
            </tr>
          </thead>
          <tbody>
            {(regions.data ?? []).map((r: Row) => (
              <tr key={r.region_id}>
                <td><Link to="/studio">{r.display_name}</Link></td>
                <td className="num"><DagCell table="Regions" field="Area">{fmt(r.area, 0)}</DagCell></td>
                <td className="num"><DagCell table="Regions" field="PlacementCount">{r.placement_count}</DagCell></td>
                <td className="num"><DagCell table="Regions" field="CoveragePct">{fmt(r.coverage_pct, 1)}%</DagCell></td>
                <td className="num"><DagCell table="Regions" field="OverlapCount">{r.overlap_count}</DagCell></td>
                <td><DagCell table="Regions" field="IsCleanTiling"><Bool v={r.is_clean_tiling} /></DagCell></td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>
    </div>
  );
}

function Stat({ label, value, hint, to }: { label: string; value: string; hint?: string; to?: string }) {
  const body = (
    <>
      <div className="stat-value">{value}</div>
      <div className="stat-label">{label}</div>
      {hint && <div className="stat-hint">{hint}</div>}
    </>
  );
  return to ? <Link to={to} className="stat">{body}</Link> : <div className="stat">{body}</div>;
}

function Bool({ v }: { v: any }) {
  return v ? <span className="pill ok">yes</span> : <span className="pill no">no</span>;
}

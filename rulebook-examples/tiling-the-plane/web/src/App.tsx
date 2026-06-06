import { Link, NavLink, Navigate, Route, Routes, useNavigate, useParams } from "react-router-dom";
import Dashboard from "./pages/Dashboard";
import EntityPage from "./pages/EntityPage";
import TilingStudio from "./pages/TilingStudio";
import PlaneView from "./pages/PlaneView";
import WallpaperViewer from "./pages/WallpaperViewer";
import { DagToggle, FieldDag, RoutingContext } from "./explainer-dag";
import "./explainer-dag/dag.css";

/** Routing adapter so the explainer DAG navigates via react-router. */
function useDagRouting() {
  const navigate = useNavigate();
  return {
    FieldLink: ({ table, field, className, children }: {
      table: string; field: string; className?: string; children: React.ReactNode;
    }) => (
      <Link to={`/dag/${table}/${field}`} className={className}>
        {children}
      </Link>
    ),
    onBack: () => navigate(-1),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
  };
}

function DagRoute() {
  const { table = "", field = "" } = useParams();
  return <FieldDag table={table} field={field} routing={useDagRouting()} />;
}

const CATALOG = [
  { key: "symmetry-groups", label: "Symmetry Groups" },
  { key: "prototiles", label: "Prototiles" },
  { key: "tilings", label: "Tilings" },
  { key: "tiling-prototiles", label: "Tiling ↔ Prototiles" },
  { key: "vertex-figures", label: "Vertex Figures" },
];
const GENERATIVE = [
  { key: "regions", label: "Regions" },
  { key: "placements", label: "Placements" },
];

/** Stream a server export endpoint to a file download. */
function downloadExport(url: string, fallbackName: string) {
  fetch(url)
    .then(async (r) => {
      if (!r.ok) {
        let detail = "";
        try { detail = (await r.json()).error; } catch { /* not JSON */ }
        throw new Error(detail || `export failed (${r.status})`);
      }
      // Honor the server's filename when it supplies one.
      const cd = r.headers.get("content-disposition") || "";
      const m = /filename="?([^"]+)"?/.exec(cd);
      return { blob: await r.blob(), name: m?.[1] || fallbackName };
    })
    .then(({ blob, name }) => {
      const a = document.createElement("a");
      const objUrl = URL.createObjectURL(blob);
      a.href = objUrl;
      a.download = name;
      a.click();
      URL.revokeObjectURL(objUrl);
    })
    .catch((e) => alert(e.message));
}

function exportXlsx() {
  downloadExport("/api/export/xlsx", "tiling-the-plane-rulebook.xlsx");
}

function exportScad() {
  downloadExport("/api/export/scad", "tiling-the-plane.scad");
}

export default function App() {
  const routing = useDagRouting();
  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark">▦</span>
          <div>
            <div className="brand-title">Tiling the Plane</div>
            <div className="brand-sub">control panel</div>
          </div>
        </div>

        <nav>
          <NavLink to="/" end className="nav-item">
            ◆ Dashboard
          </NavLink>
          <NavLink to="/studio" className="nav-item">
            ▣ Tiling Studio
          </NavLink>
          <NavLink to="/plane" className="nav-item">
            ⬡ Plane View — does it tile?
          </NavLink>
          <NavLink to="/wallpaper" className="nav-item">
            ▤ Wallpaper Viewer
          </NavLink>

          <div className="nav-group">Catalog</div>
          {CATALOG.map((e) => (
            <NavLink key={e.key} to={`/t/${e.key}`} className="nav-item">
              {e.label}
            </NavLink>
          ))}

          <div className="nav-group">Generative</div>
          {GENERATIVE.map((e) => (
            <NavLink key={e.key} to={`/t/${e.key}`} className="nav-item">
              {e.label}
            </NavLink>
          ))}
        </nav>

        <div className="sidebar-foot">
          Reads come from <code>vw_*</code>. Edits write raw columns; the
          derived values you see are recomputed by the database.
        </div>
      </aside>

      <main className="content">
        <div className="topbar">
          <button className="btn btn-secondary" onClick={exportXlsx} title="Download every view as an Excel workbook">
            ⬇ Export to Excel
          </button>
          <button className="btn btn-secondary" onClick={exportScad} title="Download the busiest region's tiling as an OpenSCAD .scad model">
            ⬇ Open in OpenSCAD (.scad)
          </button>
          <DagToggle />
        </div>
        <RoutingContext.Provider value={routing}>
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/studio" element={<TilingStudio />} />
            <Route path="/plane" element={<PlaneView />} />
            <Route path="/wallpaper" element={<WallpaperViewer />} />
            <Route path="/visualizer" element={<Navigate to="/studio" replace />} />
            <Route path="/t/:entity" element={<EntityPage />} />
            <Route path="/dag/:table/:field" element={<DagRoute />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </RoutingContext.Provider>
      </main>
    </div>
  );
}

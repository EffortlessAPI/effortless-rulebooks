import { HashRouter, Routes, Route, Navigate } from "react-router-dom";
import { usePortal } from "./hooks/usePortal.js";
import { roleMeetsMin } from "./lib/nav.js";
import TopBar from "./components/TopBar.jsx";
import Sidebar from "./components/Sidebar.jsx";
import Toast from "./components/Toast.jsx";

// Main area
import HomeScreen     from "./screens/main/HomeScreen.jsx";
import FlavorsScreen  from "./screens/main/FlavorsScreen.jsx";
import FeaturesScreen from "./screens/main/FeaturesScreen.jsx";

// Developer area
import EntitiesScreen       from "./screens/developer/EntitiesScreen.jsx";
import FormulasScreen       from "./screens/developer/FormulasScreen.jsx";
import RelationshipsScreen  from "./screens/developer/RelationshipsScreen.jsx";
import SampleDataScreen     from "./screens/developer/SampleDataScreen.jsx";
import SubstratesScreen     from "./screens/developer/SubstratesScreen.jsx";
import BuildsScreen         from "./screens/developer/BuildsScreen.jsx";
import TestsScreen          from "./screens/developer/TestsScreen.jsx";
import SpokesScreen         from "./screens/developer/SpokesScreen.jsx";
import AddToolScreen        from "./screens/developer/AddToolScreen.jsx";
import { TechPostgresScreen, TechProxyScreen, TechFilesScreen, TechJsonScreen, TechResetScreen }
  from "./screens/developer/TechScreens.jsx";

// Admin area
import AdminLandingScreen   from "./screens/admin/AdminLandingScreen.jsx";
import UsersScreen          from "./screens/admin/UsersScreen.jsx";
import RolesScreen          from "./screens/admin/RolesScreen.jsx";
import PermissionsScreen    from "./screens/admin/PermissionsScreen.jsx";
import NavigationScreen     from "./screens/admin/NavigationScreen.jsx";
import ScreensAdminScreen   from "./screens/admin/ScreensAdminScreen.jsx";

// Docs area
import DocsHomeScreen    from "./screens/docs/DocsHomeScreen.jsx";
import FramingScreen     from "./screens/docs/FramingScreen.jsx";
import MethodologyScreen from "./screens/docs/MethodologyScreen.jsx";
import FieldTypesScreen  from "./screens/docs/FieldTypesScreen.jsx";
import GlossaryScreen    from "./screens/docs/GlossaryScreen.jsx";

const PORTAL_REQUIRED = ["UserRoles", "AppUsers", "AppPermissions", "AppNavigation", "AppScreens"];

function PortalSetupNeeded({ projectRulebook, reload }) {
  const missing = PORTAL_REQUIRED.filter(
    (t) => !projectRulebook?.[t] || !Array.isArray(projectRulebook[t].data) || !projectRulebook[t].data.length
  );
  return (
    <div style={{ padding: 32, maxWidth: 820, margin: "40px auto", fontFamily: "system-ui" }}>
      <h1 style={{ marginTop: 0 }}>⚠ ERB Admin Portal — setup needed</h1>
      <p>The portal requires portal-config tables in the <strong>PROJECT</strong> rulebook. Missing:</p>
      <ul>{missing.map((t) => <li key={t}><code>{t}</code></li>)}</ul>
      <button onClick={reload}>↻ Reload</button>
    </div>
  );
}

function Portal() {
  const { me, projectRulebook, rulebook, projects, reload } = usePortal();

  if (!projectRulebook || !rulebook || !me) {
    return <div style={{ padding: 30 }}>Loading ERB Admin Portal…</div>;
  }

  const broken = PORTAL_REQUIRED.some(
    (t) => !projectRulebook[t] || !Array.isArray(projectRulebook[t].data) || !projectRulebook[t].data.length
  );
  if (broken || me.error) {
    return <PortalSetupNeeded projectRulebook={projectRulebook} reload={reload} />;
  }

  const screens = projectRulebook.AppScreens?.data || [];
  const sc = (path) => screens.find((s) => s.Path === path);
  const props = { projectRulebook, rulebook, me, projects, reload };

  return (
    <>
      <TopBar me={me} projects={projects} rulebook={rulebook} projectRulebook={projectRulebook} reload={reload} />
      <div className="layout">
        <Sidebar projectRulebook={projectRulebook} me={me} />
        <div className="main">
          <Routes>
            {/* Main area */}
            <Route path="/"                 element={<HomeScreen    {...props} screen={sc("/")} />} />
            <Route path="/projects/flavors" element={<FlavorsScreen {...props} screen={sc("/projects/flavors")} />} />
            <Route path="/features"         element={<FeaturesScreen {...props} screen={sc("/features")} />} />

            {/* Developer area */}
            <Route path="/developer/rulebook/entities"      element={<EntitiesScreen      {...props} screen={sc("/developer/rulebook/entities")} />} />
            <Route path="/developer/rulebook/formulas"      element={<FormulasScreen      {...props} screen={sc("/developer/rulebook/formulas")} />} />
            <Route path="/developer/rulebook/relationships" element={<RelationshipsScreen {...props} screen={sc("/developer/rulebook/relationships")} />} />
            <Route path="/developer/rulebook/data"          element={<SampleDataScreen    {...props} screen={sc("/developer/rulebook/data")} />} />
            <Route path="/developer/substrates"             element={<SubstratesScreen    {...props} screen={sc("/developer/substrates")} />} />
            <Route path="/developer/builds"                 element={<BuildsScreen        {...props} screen={sc("/developer/builds")} />} />
            <Route path="/developer/tests"                  element={<TestsScreen         {...props} screen={sc("/developer/tests")} />} />
            <Route path="/developer/spokes"                 element={<SpokesScreen        {...props} screen={sc("/developer/spokes")} />} />
            <Route path="/developer/tools/add"              element={<AddToolScreen       {...props} screen={sc("/developer/tools/add")} />} />
            <Route path="/developer/tech/postgres"          element={<TechPostgresScreen  screen={sc("/developer/tech/postgres")} />} />
            <Route path="/developer/tech/proxy"             element={<TechProxyScreen     screen={sc("/developer/tech/proxy")} />} />
            <Route path="/developer/tech/files"             element={<TechFilesScreen     screen={sc("/developer/tech/files")} />} />
            <Route path="/developer/tech/rulebook-json"     element={<TechJsonScreen      screen={sc("/developer/tech/rulebook-json")} />} />
            <Route path="/developer/tech/reset"             element={<TechResetScreen     screen={sc("/developer/tech/reset")} />} />

            {/* Admin area */}
            <Route path="/admin"                element={<AdminLandingScreen {...props} screen={sc("/admin")} />} />
            <Route path="/admin/users"          element={<UsersScreen        {...props} screen={sc("/admin/users")} />} />
            <Route path="/admin/users/roles"    element={<RolesScreen        {...props} screen={sc("/admin/users/roles")} />} />
            <Route path="/admin/permissions"    element={<PermissionsScreen  {...props} screen={sc("/admin/permissions")} />} />
            <Route path="/admin/navigation"     element={<NavigationScreen   {...props} screen={sc("/admin/navigation")} />} />
            <Route path="/admin/screens"        element={<ScreensAdminScreen {...props} screen={sc("/admin/screens")} />} />

            {/* Docs area */}
            <Route path="/docs"             element={<DocsHomeScreen    {...props} />} />
            <Route path="/docs/framing"     element={<FramingScreen     {...props} screen={sc("/docs/framing")} />} />
            <Route path="/docs/methodology" element={<MethodologyScreen {...props} screen={sc("/docs/methodology")} />} />
            <Route path="/docs/field-types" element={<FieldTypesScreen  {...props} screen={sc("/docs/field-types")} />} />
            <Route path="/docs/glossary"    element={<GlossaryScreen    {...props} screen={sc("/docs/glossary")} />} />

            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </div>
      </div>
      <Toast />
    </>
  );
}

export default function App() {
  return (
    <HashRouter>
      <Portal />
    </HashRouter>
  );
}

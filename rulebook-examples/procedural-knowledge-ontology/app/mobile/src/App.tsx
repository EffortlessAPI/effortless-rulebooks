import { Navigate, Route, Routes, useLocation } from "react-router-dom";
import { useSession } from "./session";
import { Device, Loading } from "./ui/kit";
import SignIn from "./pages/SignIn";
import Lockout from "./pages/Lockout";
import SafetyDesk from "./pages/SafetyDesk";
import Floor from "./pages/Floor";
import Workbench from "./pages/Workbench";
import Sourcing from "./pages/Sourcing";
import ChangeBoard from "./pages/ChangeBoard";
import ReleaseGate from "./pages/ReleaseGate";
import Admin from "./pages/Admin";
import RoleHome from "./pages/RoleHome";

// Routes are AppRoleProfiles.HomeRoute values. A role with no hand-built home lands on RoleHome,
// which is generated from the tables in that sign-in's schema.
const TABLET = ["/knowledge-engineer", "/sourcing-manager", "/release-manager", "/ontology-authority", "/admin"];

export default function App() {
  const { shell, loading } = useSession();
  const { pathname } = useLocation();
  if (loading) return <Device kind="phone"><Loading /></Device>;
  if (!shell) return <Device kind="tablet"><SignIn /></Device>;
  const home = shell.profile?.home_route || `/${shell.claims.role}`;
  const device = pathname.startsWith("/admin") || TABLET.some((p) => pathname.startsWith(p)) ? "tablet"
    : pathname === "/" ? (shell.profile?.device || "tablet") : (shell.profile?.device === "phone" ? "phone" : "tablet");
  return (
    <Device kind={device as "phone" | "tablet"}>
      <Routes>
        <Route path="/" element={<Navigate to={home} replace />} />
        <Route path="/maintenance-technician/my-lockout" element={<Lockout />} />
        <Route path="/plant-safety-officer/desk" element={<SafetyDesk />} />
        <Route path="/plant-operations-manager/floor" element={<Floor />} />
        <Route path="/knowledge-engineer/workbench" element={<Workbench />} />
        <Route path="/sourcing-manager/what-we-still-know" element={<Sourcing />} />
        <Route path="/ontology-authority/change-board" element={<ChangeBoard />} />
        <Route path="/release-manager/release-gate" element={<ReleaseGate />} />
        <Route path="/admin/*" element={shell.claims.is_admin ? <Admin /> : <Navigate to={home} replace />} />
        <Route path="*" element={<RoleHome />} />
      </Routes>
    </Device>
  );
}

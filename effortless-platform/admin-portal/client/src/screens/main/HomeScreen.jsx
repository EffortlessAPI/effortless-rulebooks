import { useNavigate } from "react-router-dom";
import ScreenHeader from "../../components/ScreenHeader.jsx";
import TagChip, { tagsForFlavor } from "../../components/TagChip.jsx";

export default function HomeScreen({ screen, projectRulebook, rulebook, projects, me }) {
  const navigate = useNavigate();
  const subs    = projectRulebook?.ExecutionSubstrates?.data || [];
  const spokes  = projectRulebook?.RulebookSourceSpokes?.data || [];
  const flows   = projectRulebook?.CoreDataFlows?.data || [];
  const meta    = projectRulebook?.ProjectMetadata?.data?.[0] || {};
  const flavors      = projectRulebook?.RulebookFlavors?.data || [];
  const flavorTags   = projectRulebook?.FlavorTags?.data || [];
  const rulebookTags = projectRulebook?.RulebookTags?.data || [];

  const flav = flavors.find((f) => f.ProjectSlug === projects?.active);
  const tags = flav ? tagsForFlavor(flav.FlavorId, flavorTags, rulebookTags) : [];

  const entityCount = Object.keys(rulebook || {}).filter(
    (k) => !k.startsWith("$") && !k.startsWith("_") && rulebook[k]?.schema
  ).length;

  const role = me?.role;

  return (
    <>
      <ScreenHeader screen={screen} />
      {role?.Tagline && (
        <div className="story-banner" style={{ borderLeftColor: role.ColorTheme || "#888" }}>
          <b>{role.Name}:</b> {role.Tagline}
        </div>
      )}

      <div className="cards">
        <div className="card">
          <h3>Active project</h3>
          <div className="big">{projects?.active}</div>
          <div style={{ marginTop: 6, display: "flex", flexWrap: "wrap", gap: 4 }}>
            {tags.map((t) => <TagChip key={t.TagId} tag={t} />)}
          </div>
          {flav && <div className="sub" style={{ marginTop: 4 }}>{flav.LearningFocus}</div>}
        </div>

        <div className="card clickable" onClick={() => navigate("/developer/rulebook/entities")}>
          <h3>Rulebook entities</h3>
          <div className="big">{entityCount}</div>
        </div>

        <div className="card clickable" onClick={() => navigate("/developer/substrates")}>
          <h3>Substrates</h3>
          <div className="big">{subs.length}</div>
          <div className="sub">{subs.filter((s) => s.CanBeAnswerKey).length} eligible as answer key</div>
        </div>

        <div className="card clickable" onClick={() => navigate("/developer/spokes")}>
          <h3>Input spokes</h3>
          <div className="big">{spokes.length}</div>
        </div>

        <div className="card">
          <h3>Core flows</h3>
          <div className="big">{flows.length}</div>
        </div>

        <div className="card clickable" onClick={() => navigate(`/admin/users/roles?role=${encodeURIComponent(me?.RoleId || "")}`)}>
          <h3>Signed in as</h3>
          <div className="big">{me?.DisplayName}</div>
          <div className="sub">{role?.Name}</div>
        </div>

        <div className="card">
          <h3>Portal URL</h3>
          <div className="big mono">{window.location.host}</div>
          <div className="sub">{meta.ProxyUrl || ""}</div>
        </div>

        <div className="card">
          <h3>Repo root</h3>
          <div className="big mono" style={{ fontSize: 13 }}>{meta.RepositoryRoot || ""}</div>
        </div>
      </div>

      <h3 style={{ marginTop: 28 }}>Where to go</h3>
      <div className="cards" style={{ gridTemplateColumns: "repeat(auto-fill, minmax(200px, 1fr))" }}>
        <div className="card clickable" onClick={() => navigate("/developer/rulebook/entities")}>
          <h3>🔧 Developer</h3>
          <div className="sub">Edit rulebook, substrates, builds, tests, and tech tools.</div>
        </div>
        <div className="card clickable" onClick={() => navigate("/admin/users")}>
          <h3>🛡 Admin</h3>
          <div className="sub">Manage users, roles, and portal configuration.</div>
        </div>
        <div className="card clickable" onClick={() => navigate("/docs/framing")}>
          <h3>📖 Docs</h3>
          <div className="sub">Framing invariants, axioms, field types, and methodology.</div>
        </div>
        <div className="card clickable" onClick={() => navigate("/projects/flavors")}>
          <h3>🏷 Flavours</h3>
          <div className="sub">Browse and filter the demo rulebook catalogue.</div>
        </div>
      </div>
    </>
  );
}

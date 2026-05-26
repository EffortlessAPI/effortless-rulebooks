import { useNavigate, useParams } from "react-router-dom";
import ScreenHeader from "../../components/ScreenHeader.jsx";

export default function DeveloperDomainScreen({ screen, rulebook, projects }) {
  const navigate = useNavigate();
  const { domain } = useParams();
  const dom = (projects?.projects || []).find((p) => p.id === domain);

  const tables = Object.keys(rulebook || {}).filter(
    (k) => !k.startsWith("$") && !k.startsWith("_") && rulebook[k]?.schema,
  );

  const groups = [
    { label: "Author the rulebook", links: [
      ["Entities",        `/developer/${domain}/entities`],
      ["Formulas",        `/developer/${domain}/formulas`],
      ["Relationships",   `/developer/${domain}/relationships`],
      ["Sample Data",     `/developer/${domain}/data`],
    ]},
    { label: "Build & verify", links: [
      ["Substrates",      `/developer/${domain}/substrates`],
      ["Builds",          `/developer/${domain}/builds`],
      ["Tests",           `/developer/${domain}/tests`],
      ["Input Spokes",    `/developer/${domain}/spokes`],
    ]},
    { label: "Inspect", links: [
      ["Explorer (SQL + data)", `/developer/${domain}/explorer`],
      ["Files",                 `/developer/${domain}/files`],
      ["Rulebook JSON",         `/developer/${domain}/rulebook-json`],
      ["Reset Editor",          `/developer/${domain}/reset`],
    ]},
  ];

  return (
    <>
      <ScreenHeader screen={screen} />
      <div className="story-banner" style={{ borderLeftColor: "#b48cff", display: "flex", alignItems: "center", gap: 12 }}>
        {dom?.logoUrl && (
          <img
            src={dom.logoUrl}
            alt=""
            style={{ width: 48, height: 48, borderRadius: 8, objectFit: "cover", flexShrink: 0 }}
          />
        )}
        <div>
          <b>{dom?.displayName || dom?.name || domain}</b> — <span className="muted">{tables.length} tables</span>
          {dom?.tagline ? ` · ${dom.tagline}` : (dom?.description ? ` · ${dom.description}` : "")}
        </div>
      </div>

      {groups.map((g) => (
        <div key={g.label}>
          <h3 className="muted small" style={{ marginTop: 24 }}>{g.label.toUpperCase()}</h3>
          <div className="cards">
            {g.links.map(([label, to]) => (
              <div key={to} className="card clickable" onClick={() => navigate(to)}>
                <div className="big" style={{ fontSize: 15 }}>{label}</div>
              </div>
            ))}
          </div>
        </div>
      ))}
    </>
  );
}

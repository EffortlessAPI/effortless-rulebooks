import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import ScreenHeader from "../../components/ScreenHeader.jsx";

export default function DeveloperHomeScreen({ screen, projects, projectRulebook }) {
  const navigate = useNavigate();
  const role     = (projectRulebook?.UserRoles?.data || []).find((r) => r.RoleId === "role-developer");
  const domains  = (projects?.projects || []).filter((d) => d.id !== "__top__");
  const platform = (projects?.projects || []).find((d) => d.id === "__top__");
  const active   = projects?.active;

  // If there's already an active non-top domain, jump to its landing
  useEffect(() => {
    if (active && active !== "__top__") {
      navigate(`/developer/${active}`);
    }
  }, [active]);

  return (
    <>
      <ScreenHeader screen={screen} />
      {role?.Tagline && (
        <div className="story-banner" style={{ borderLeftColor: role.ColorTheme }}>
          <b>{role.Name}:</b> {role.Tagline}
        </div>
      )}

      <h3 className="muted small" style={{ marginTop: 24 }}>PICK A DOMAIN TO WORK ON</h3>
      <div className="cards">
        {domains.map((d) => (
          <div
            key={d.id}
            className="card clickable domain-card"
            onClick={() => navigate(`/developer/${d.id}`)}
          >
            {d.logoUrl && (
              <img
                src={d.logoUrl}
                alt=""
                className="domain-card-logo"
                onError={(e) => { e.currentTarget.style.display = "none"; }}
              />
            )}
            <div className="domain-card-text">
              <h3 style={{ color: "#b48cff" }}>{d.id}</h3>
              <div className="big" style={{ fontSize: 16 }}>{d.name}</div>
              <div className="sub">{d.description || "—"}</div>
            </div>
          </div>
        ))}
      </div>

      {platform && (
        <>
          <h3 className="muted small" style={{ marginTop: 32 }}>OR EDIT THE PLATFORM</h3>
          <div className="cards">
            <div
              className="card clickable domain-card"
              style={{ borderColor: "#b48cff" }}
              onClick={() => navigate(`/developer/${platform.id}`)}
            >
              {platform.logoUrl && (
                <img
                  src={platform.logoUrl}
                  alt=""
                  className="domain-card-logo"
                  onError={(e) => { e.currentTarget.style.display = "none"; }}
                />
              )}
              <div className="domain-card-text">
                <h3 style={{ color: "#b48cff" }}>{platform.id}</h3>
                <div className="big" style={{ fontSize: 16 }}>{platform.name}</div>
                <div className="sub">The rulebook describing ERB itself.</div>
              </div>
            </div>
          </div>
        </>
      )}
    </>
  );
}

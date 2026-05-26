import { useNavigate } from "react-router-dom";
import ScreenHeader from "../../components/ScreenHeader.jsx";
import DomainTile from "../../components/DomainTile.jsx";
import PickerChips from "../../components/PickerChips.jsx";

export default function DeveloperHomeScreen({ screen, projects, projectRulebook, domainState }) {
  const navigate = useNavigate();
  const role     = (projectRulebook?.UserRoles?.data || []).find((r) => r.RoleId === "role-developer");
  const domains  = (projects?.projects || []).filter((d) => d.id !== "__top__");
  const platform = (projects?.projects || []).find((d) => d.id === "__top__");

  return (
    <>
      <ScreenHeader screen={screen} />
      {role?.Tagline && (
        <div className="story-banner" style={{ borderLeftColor: role.ColorTheme }}>
          <b>{role.Name}:</b> {role.Tagline}
        </div>
      )}

      <PickerChips domainState={domainState} domains={domains} areaPath="/developer" />

      <h3 className="muted small" style={{ marginTop: 24 }}>PICK A DOMAIN TO WORK ON</h3>
      <div className="domain-gallery">
        {domains.map((d) => (
          <DomainTile
            key={d.id}
            d={d}
            accentColor="#b48cff"
            onClick={() => navigate(`/developer/${d.id}`)}
          />
        ))}
      </div>

      {platform && (
        <>
          <h3 className="muted small" style={{ marginTop: 32 }}>OR EDIT THE PLATFORM</h3>
          <div className="domain-gallery">
            <DomainTile
              d={platform}
              accentColor="#6ea8fe"
              onClick={() => navigate(`/developer/${platform.id}`)}
            />
          </div>
        </>
      )}
    </>
  );
}

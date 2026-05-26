import { useNavigate } from "react-router-dom";
import ScreenHeader from "../../components/ScreenHeader.jsx";
import DomainTile from "../../components/DomainTile.jsx";

export default function DeveloperDomainsScreen({ screen, projects }) {
  const navigate = useNavigate();
  const domains  = projects?.projects || [];

  return (
    <>
      <ScreenHeader screen={screen} />
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
    </>
  );
}

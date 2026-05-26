import { useNavigate } from "react-router-dom";
import ScreenHeader from "../../components/ScreenHeader.jsx";

export default function DeveloperDomainsScreen({ screen, projects }) {
  const navigate = useNavigate();
  const domains  = projects?.projects || [];

  return (
    <>
      <ScreenHeader screen={screen} />
      <div className="cards">
        {domains.map((d) => (
          <div
            key={d.id}
            className="card clickable"
            onClick={() => navigate(`/developer/${d.id}`)}
          >
            <h3 style={{ color: "#b48cff" }}>{d.id}</h3>
            <div className="big" style={{ fontSize: 16 }}>{d.name}</div>
            <div className="sub">{d.description || "—"}</div>
          </div>
        ))}
      </div>
    </>
  );
}

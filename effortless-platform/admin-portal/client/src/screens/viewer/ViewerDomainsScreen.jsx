import { useNavigate } from "react-router-dom";
import ScreenHeader from "../../components/ScreenHeader.jsx";

export default function ViewerDomainsScreen({ screen, projects }) {
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
            onClick={() => navigate(`/viewer/${d.id}`)}
          >
            <h3 style={{ color: "#7280ad" }}>{d.id}</h3>
            <div className="big" style={{ fontSize: 16 }}>{d.name}</div>
            <div className="sub">{d.description || "—"}</div>
          </div>
        ))}
      </div>
    </>
  );
}

import { Link } from "react-router-dom";
import { Me } from "../types";

export default function Placeholder({ me, title }: { me: Me; title: string }) {
  return (
    <div className="placeholder">
      <h2>{title} — {me.role} view (placeholder)</h2>
      <p className="muted">
        This demo wires the <b>coordinator</b> role end-to-end. The {me.role} view would
        normally show a read-only seating chart from {me.role}'s side, with the ability
        to flag must-not pairs and add prefers-near requests.
      </p>
      <p>
        <Link to="/tables">View the seating chart →</Link>
      </p>
    </div>
  );
}

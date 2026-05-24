import { Link } from "react-router-dom";
import { Me } from "../types";

export default function Placeholder({ me, title }: { me: Me; title: string }) {
  return (
    <div className="placeholder">
      <h2>{title} — {me.role} view (placeholder)</h2>
      <p className="muted">
        This demo wires the <b>Trainer</b> role end-to-end. The {me.role} view would
        normally show {me.role === "client"
          ? "your own invoices and a payment portal."
          : "every trainer, every client, and admin tools for editing rates and trainers."}
      </p>
      <p>
        <Link to="/invoices">View your invoices</Link>
      </p>
    </div>
  );
}

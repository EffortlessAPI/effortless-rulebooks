import { useState } from "react";
import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Session } from "../types";
import { api } from "../lib/api";
import { money, date, dateInput } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

export default function Sessions() {
  const { data, loading, reload } = useApi<Session[]>("/api/sessions");
  const [editing, setEditing] = useState<string | null>(null);

  if (loading) return <div>Loading…</div>;
  const ss = data ?? [];

  return (
    <>
      <h2>Sessions</h2>
      <p className="muted">
        Edit a session's <b>hours</b> or <b>rate override</b> below and watch the line total
        (and its rolled-up invoice subtotal + total) update on the next read.
      </p>
      <div className="panel">
        <table>
          <thead>
            <tr><th>Session</th><th>Client</th><th>Date</th><th className="right">Hours</th><th className="right">Rate override</th><th className="right">Effective</th><th className="right">Line total</th><th>Invoice</th><th></th></tr>
          </thead>
          <tbody>
            {ss.map((s) => editing === s.session_id ? (
              <EditRow key={s.session_id} s={s} onDone={() => { setEditing(null); reload(); }} onCancel={() => setEditing(null)} />
            ) : (
              <tr key={s.session_id}>
                <td><DagCell table="Sessions" field="Name">{s.name}</DagCell><div className="muted" style={{ fontSize: 12 }}><DagCell table="Sessions" field="Notes">{s.notes}</DagCell></div></td>
                <td><DagCell table="Sessions" field="ClientName">{s.client_name}</DagCell></td>
                <td><DagCell table="Sessions" field="SessionDate">{date(s.session_date)}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="DurationHours">{s.duration_hours}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="RateOverride">{Number(s.rate_override) > 0 ? money(s.rate_override) : <span className="muted">—</span>}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="EffectiveRate">{money(s.effective_rate)}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="LineTotal"><b>{money(s.line_total)}</b></DagCell></td>
                <td><DagCell table="Sessions" field="InvoiceName">{s.invoice_name ? <Link to={`/invoices/${s.invoice}`}>{s.invoice_name}</Link> : <span className="muted">unbilled</span>}</DagCell></td>
                <td><button onClick={() => setEditing(s.session_id)}>Edit</button></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}

function EditRow({ s, onDone, onCancel }: { s: Session; onDone: () => void; onCancel: () => void }) {
  const [hours, setHours] = useState(String(s.duration_hours));
  const [override, setOverride] = useState(String(s.rate_override ?? 0));
  const [d, setD] = useState(dateInput(s.session_date));
  const [busy, setBusy] = useState(false);
  const save = async () => {
    setBusy(true);
    try {
      await api(`/api/sessions/${s.session_id}`, {
        method: "PATCH",
        body: JSON.stringify({
          duration_hours: Number(hours),
          rate_override: Number(override),
          session_date: d,
        }),
      });
      onDone();
    } catch (e: any) {
      alert(e.message);
      setBusy(false);
    }
  };
  return (
    <tr>
      <td>{s.name}</td>
      <td>{s.client_name}</td>
      <td><input type="date" value={d} onChange={(e) => setD(e.target.value)} /></td>
      <td className="right"><input style={{ width: 70 }} value={hours} onChange={(e) => setHours(e.target.value)} /></td>
      <td className="right"><input style={{ width: 70 }} value={override} onChange={(e) => setOverride(e.target.value)} /></td>
      <td colSpan={3} className="muted">recalculated after save</td>
      <td>
        <button className="primary" onClick={save} disabled={busy}>Save</button>
        <button onClick={onCancel} disabled={busy}>Cancel</button>
      </td>
    </tr>
  );
}

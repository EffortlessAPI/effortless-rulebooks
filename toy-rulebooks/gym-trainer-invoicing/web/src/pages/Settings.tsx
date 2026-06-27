import { useEffect, useState } from "react";
import { useApi } from "../lib/useApi";
import { Me, Trainer } from "../types";
import { api } from "../lib/api";
import { money } from "../lib/fmt";

export default function Settings({ me }: { me: Me }) {
  const { data, loading, reload } = useApi<Trainer[]>("/api/trainers");
  const mine = (data ?? []).find((t) => t.trainer_id === me.trainer_id);
  const [rate, setRate] = useState("");
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    if (mine) setRate(String(mine.hourly_rate));
  }, [mine?.hourly_rate]);

  if (loading || !mine) return <div>Loading…</div>;

  const save = async () => {
    setBusy(true);
    try {
      await api(`/api/trainers/${mine.trainer_id}`, {
        method: "PATCH",
        body: JSON.stringify({ hourly_rate: Number(rate) }),
      });
      reload();
    } catch (e: any) {
      alert(e.message);
    }
    setBusy(false);
  };

  return (
    <>
      <h2>Settings</h2>
      <div className="panel" style={{ maxWidth: 500 }}>
        <h3>Default hourly rate</h3>
        <p className="muted">
          Changing this updates every <i>future</i> session's effective rate for clients of yours.
          (Per-session overrides on existing sessions are preserved.)
        </p>
        <div className="field">
          <label>Hourly rate (USD)</label>
          <input value={rate} onChange={(e) => setRate(e.target.value)} />
        </div>
        <div className="actions">
          <button className="primary" onClick={save} disabled={busy}>Save</button>
        </div>
        <p className="muted" style={{ marginTop: 16 }}>
          Current: {money(mine.hourly_rate)}/hr
        </p>
      </div>
    </>
  );
}

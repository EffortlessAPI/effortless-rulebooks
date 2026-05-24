import { useApi } from '../lib/useApi';
import { DagCell } from '../explainer-dag';
import { num } from '../lib/fmt';
import type { Capability } from '../types';

export default function Capabilities() {
  const { data, loading, error } = useApi<Capability[]>('/capabilities');
  if (loading) return <p>Loading…</p>;
  if (error) return <p className="err">{String(error)}</p>;
  const rows = data ?? [];
  return (
    <div className="page">
      <h1>Capabilities</h1>
      <p className="lede">
        The cognitive primitives being assessed. Each capability has a tier
        (foundational / composite / emergent) and an importance weight that
        scales every Assessment of that capability.
      </p>
      <table className="table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Tier</th>
            <th className="num">Weight</th>
            <th>Description</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((c) => (
            <tr key={c.capabilities_id}>
              <td>
                <DagCell table="Capabilities" field="Name">
                  {c.name}
                </DagCell>
              </td>
              <td>
                <span className={`tier tier-${c.tier}`}>{c.tier}</span>
              </td>
              <td className="num">{num(c.weight, 1)}</td>
              <td>{c.description}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

import { Link } from 'react-router-dom';
import { useApi } from '../lib/useApi';
import { DagCell } from '../explainer-dag';
import { num } from '../lib/fmt';
import type { Intelligence } from '../types';

export default function Intelligences() {
  const { data, loading, error } = useApi<Intelligence[]>('/intelligences');
  if (loading) return <p>Loading…</p>;
  if (error) return <p className="err">{String(error)}</p>;
  const rows = data ?? [];
  return (
    <div className="page">
      <h1>Intelligences</h1>
      <p className="lede">All agents in the taxonomy. Click a row to see its assessments.</p>
      <table className="table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Substrate</th>
            <th>Description</th>
            <th className="num">Total Weighted</th>
            <th>Class</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((r) => (
            <tr key={r.intelligences_id}>
              <td>
                <DagCell table="Intelligences" field="Name">
                  <Link to={`/intelligences/${r.intelligences_id}`}>{r.name}</Link>
                </DagCell>
              </td>
              <td>{r.substrate}</td>
              <td>{r.description}</td>
              <td className="num">
                <DagCell table="Intelligences" field="TotalWeightedScore">
                  {num(r.total_weighted_score, 1)}
                </DagCell>
              </td>
              <td>
                <DagCell table="Intelligences" field="TaxonomyClass">
                  <span className={`badge badge-${r.taxonomy_class.toLowerCase()}`}>
                    {r.taxonomy_class}
                  </span>
                </DagCell>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

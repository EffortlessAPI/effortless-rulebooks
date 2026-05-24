import { Link } from 'react-router-dom';
import { useApi } from '../lib/useApi';
import { DagCell } from '../explainer-dag';
import { num } from '../lib/fmt';
import type { Intelligence } from '../types';

export default function Dashboard() {
  const { data, loading, error } = useApi<Intelligence[]>('/intelligences');

  if (loading) return <p>Loading…</p>;
  if (error) return <p className="err">Failed to load: {String(error)}</p>;
  const rows = data ?? [];

  const counts = rows.reduce<Record<string, number>>((acc, r) => {
    acc[r.taxonomy_class] = (acc[r.taxonomy_class] ?? 0) + 1;
    return acc;
  }, {});

  return (
    <div className="page">
      <h1>Dashboard</h1>
      <p className="lede">
        Each intelligence is classified by summing its weighted assessment scores and bucketing
        the total. Edit any <strong>RawScore</strong> on the{' '}
        <Link to="/assessments">Assessments</Link> page to watch the cascade.
      </p>

      <section className="stats">
        {(['Generalist', 'Broad', 'Narrow'] as const).map((k) => (
          <div key={k} className="stat">
            <div className="stat-label">{k}</div>
            <div className="stat-value">{counts[k] ?? 0}</div>
          </div>
        ))}
      </section>

      <section>
        <h2>Intelligences</h2>
        <table className="table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Substrate</th>
              <th>Assessments</th>
              <th className="num">Total Weighted</th>
              <th>Taxonomy Class</th>
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
                <td className="num">
                  <DagCell table="Intelligences" field="AssessmentCount">
                    {r.assessment_count}
                  </DagCell>
                </td>
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
      </section>
    </div>
  );
}

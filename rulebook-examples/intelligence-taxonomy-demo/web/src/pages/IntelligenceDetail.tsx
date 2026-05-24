import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useApi } from '../lib/useApi';
import { api } from '../lib/api';
import { DagCell } from '../explainer-dag';
import { num } from '../lib/fmt';
import type { Assessment, Intelligence } from '../types';

export default function IntelligenceDetail() {
  const { id = '' } = useParams();
  const intel = useApi<Intelligence>(`/intelligences/${id}`);
  const assessments = useApi<Assessment[]>(`/assessments?intelligence=${encodeURIComponent(id)}`);

  if (intel.loading || assessments.loading) return <p>Loading…</p>;
  if (intel.error)
    return (
      <p className="err">
        Could not load intelligence “{id}”. <Link to="/intelligences">Back</Link>
      </p>
    );
  const i = intel.data!;
  const rows = assessments.data ?? [];

  const reloadAll = async () => {
    await Promise.all([intel.reload(), assessments.reload()]);
  };

  const saveScore = async (assessId: string, raw_score: number) => {
    await api(`/assessments/${assessId}`, {
      method: 'PATCH',
      body: JSON.stringify({ raw_score }),
    });
    await reloadAll();
  };

  return (
    <div className="page">
      <p>
        <Link to="/intelligences">← Intelligences</Link>
      </p>
      <h1>{i.name}</h1>
      <p className="lede">{i.description}</p>

      <section className="kvs">
        <Kv label="Substrate" value={i.substrate} />
        <Kv
          label="Total Weighted Score"
          dag={{ table: 'Intelligences', field: 'TotalWeightedScore' }}
          value={num(i.total_weighted_score, 1)}
        />
        <Kv
          label="Taxonomy Class"
          dag={{ table: 'Intelligences', field: 'TaxonomyClass' }}
          value={
            <span className={`badge badge-${String(i.taxonomy_class).toLowerCase()}`}>
              {i.taxonomy_class}
            </span>
          }
        />
        <Kv
          label="Assessment Count"
          dag={{ table: 'Intelligences', field: 'AssessmentCount' }}
          value={i.assessment_count}
        />
      </section>

      <section>
        <h2>Assessments</h2>
        <p className="hint">
          Edit a <code>RawScore</code> and watch <code>WeightedScore</code> recompute on the row,
          then the totals at the top of the page roll up after the save completes.
        </p>
        <table className="table">
          <thead>
            <tr>
              <th>Capability</th>
              <th>Tier</th>
              <th className="num">Weight</th>
              <th className="num">RawScore</th>
              <th className="num">WeightedScore</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((a) => (
              <AssessmentRow key={a.assessments_id} a={a} onSave={saveScore} />
            ))}
          </tbody>
        </table>
      </section>
    </div>
  );
}

function Kv({
  label,
  value,
  dag,
}: {
  label: string;
  value: React.ReactNode;
  dag?: { table: string; field: string };
}) {
  return (
    <div className="kv">
      <div className="kv-label">{label}</div>
      <div className="kv-value">
        {dag ? (
          <DagCell table={dag.table} field={dag.field}>
            {value}
          </DagCell>
        ) : (
          value
        )}
      </div>
    </div>
  );
}

function AssessmentRow({
  a,
  onSave,
}: {
  a: Assessment;
  onSave: (id: string, score: number) => Promise<void>;
}) {
  const [val, setVal] = useState<string>(String(a.raw_score));
  const [busy, setBusy] = useState(false);

  const commit = async () => {
    const n = Number(val);
    if (!Number.isFinite(n) || n < 0 || n > 100) {
      setVal(String(a.raw_score));
      return;
    }
    if (n === a.raw_score) return;
    setBusy(true);
    try {
      await onSave(a.assessments_id, n);
    } finally {
      setBusy(false);
    }
  };

  return (
    <tr>
      <td>{a.capability_name}</td>
      <td>{a.capability_tier}</td>
      <td className="num">{num(a.capability_weight, 1)}</td>
      <td className="num">
        <input
          className="score-input"
          type="number"
          min={0}
          max={100}
          value={val}
          disabled={busy}
          onChange={(e) => setVal(e.target.value)}
          onBlur={() => void commit()}
          onKeyDown={(e) => {
            if (e.key === 'Enter') (e.target as HTMLInputElement).blur();
          }}
        />
      </td>
      <td className="num">
        <DagCell table="Assessments" field="WeightedScore">
          {num(a.weighted_score, 1)}
        </DagCell>
      </td>
    </tr>
  );
}

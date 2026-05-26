import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useApi } from '../lib/useApi';
import { api } from '../lib/api';
import { DagCell } from '../explainer-dag';
import { num } from '../lib/fmt';
import type { Assessment, Capability, Intelligence } from '../types';

export default function AssessmentsMatrix() {
  const intelligences = useApi<Intelligence[]>('/intelligences');
  const capabilities = useApi<Capability[]>('/capabilities');
  const assessments = useApi<Assessment[]>('/assessments');

  const reloadAll = async () => {
    await Promise.all([intelligences.reload(), capabilities.reload(), assessments.reload()]);
  };

  const byPair = useMemo(() => {
    const m = new Map<string, Assessment>();
    for (const a of assessments.data ?? []) m.set(`${a.intelligence}|${a.capability}`, a);
    return m;
  }, [assessments.data]);

  if (intelligences.loading || capabilities.loading || assessments.loading)
    return <p>Loading…</p>;

  const intels = intelligences.data ?? [];
  const caps = capabilities.data ?? [];

  const saveScore = async (assessId: string, raw_score: number) => {
    await api(`/assessments/${assessId}`, {
      method: 'PATCH',
      body: JSON.stringify({ raw_score }),
    });
    await reloadAll();
  };

  return (
    <div className="page">
      <h1>Assessments</h1>
      <p className="lede">
        The full matrix. Rows are intelligences, columns are capabilities. Edit any cell to
        change its <code>RawScore</code>; the <code>WeightedScore</code> shows underneath,
        and the row totals on the right roll up immediately on save.
      </p>

      <div className="matrix-wrap">
        <table className="matrix">
          <thead>
            <tr>
              <th></th>
              {caps.map((c) => (
                <th key={c.capabilities_id}>
                  <div className="matrix-cap">
                    <div>{c.name}</div>
                    <div className="matrix-cap-sub">
                      <span className={`tier tier-${c.tier}`}>{c.tier}</span>
                      <span className="weight">×{num(c.weight, 1)}</span>
                    </div>
                  </div>
                </th>
              ))}
              <th className="num">Total</th>
              <th>Class</th>
            </tr>
          </thead>
          <tbody>
            {intels.map((i) => (
              <tr key={i.intelligences_id}>
                <th className="row-head">
                  <Link to={`/intelligences/${i.intelligences_id}`}>{i.name}</Link>
                  <div className="row-head-sub">{i.substrate}</div>
                </th>
                {caps.map((c) => {
                  const a = byPair.get(`${i.intelligences_id}|${c.capabilities_id}`);
                  return (
                    <td key={c.capabilities_id} className="matrix-cell">
                      {a ? (
                        <MatrixCell a={a} onSave={saveScore} />
                      ) : (
                        <span className="dim">—</span>
                      )}
                    </td>
                  );
                })}
                <td className="num">
                  <DagCell table="Intelligences" field="TotalWeightedScore">
                    {num(i.total_weighted_score, 1)}
                  </DagCell>
                </td>
                <td>
                  <DagCell table="Intelligences" field="TaxonomyClass">
                    <span className={`badge badge-${String(i.taxonomy_class).toLowerCase()}`}>
                      {i.taxonomy_class}
                    </span>
                  </DagCell>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

function MatrixCell({
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
    <div className="matrix-cell-inner">
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
      <div className="matrix-weighted">
        <DagCell table="Assessments" field="WeightedScore">
          = {num(a.weighted_score, 1)}
        </DagCell>
      </div>
    </div>
  );
}

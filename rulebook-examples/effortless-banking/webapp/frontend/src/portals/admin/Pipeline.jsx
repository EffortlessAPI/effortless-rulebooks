import React from 'react';
import { Page, Card } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { Field, formatValue } from '../../components/Field.jsx';
import { useNavigate } from 'react-router-dom';

const STAGES = ['Application', 'Pre-Qual', 'Underwriting', 'Committee', 'Approved', 'Docs', 'Funded'];

export default function Pipeline() {
  const { data: loans, loading } = useFetch('/api/loans');
  const nav = useNavigate();
  if (loading) return <div className="empty">Loading…</div>;

  const buckets = {};
  STAGES.forEach((s) => (buckets[s] = []));
  (loans || []).forEach((l) => {
    const s = l.underwriting_stage || 'Application';
    if (!buckets[s]) buckets[s] = [];
    buckets[s].push(l);
  });

  return (
    <Page title="Origination Pipeline" subtitle="Kanban-style view across underwriting stages.">
      <div style={{ display: 'grid', gridTemplateColumns: `repeat(${STAGES.length},1fr)`, gap: 12 }}>
        {STAGES.map((stage) => (
          <div key={stage} className="card" style={{ marginBottom: 0 }}>
            <div className="card-header" style={{ fontSize: 12, textTransform: 'uppercase', letterSpacing: '0.4px' }}>
              {stage} <span className="badge muted">{buckets[stage].length}</span>
            </div>
            <div className="card-body" style={{ padding: 8 }}>
              {buckets[stage].length === 0 && <div className="muted" style={{ fontSize: 12 }}>—</div>}
              {buckets[stage].map((l) => (
                <div
                  key={l.name}
                  className="pill"
                  style={{ marginBottom: 8, cursor: 'pointer' }}
                  onClick={() => nav(`/loan/${l.name}`)}
                >
                  <div className="mono" style={{ fontSize: 11, color: 'var(--muted)' }}>{l.loan_number}</div>
                  <Field as="div" table="Loans" col="business_label" value={l.business_label} className="value" />
                  <div style={{ marginTop: 6, fontSize: 12 }}>
                    <Field as="span" table="Loans" col="principal_usd" value={l.principal_usd} hint="money" />
                    {' · '}
                    <Field as="span" table="Loans" col="risk_rating_label" value={l.risk_rating_label} />
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </Page>
  );
}

import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Funded() {
  const { data, loading } = useFetch('/api/loans');
  if (loading) return <div className="empty">Loading…</div>;
  const rows = (data || []).filter((l) => l.is_funded);
  return (
    <Page title="Approved & Funded" subtitle="Loans that have closed and funded.">
      <DataTable
        table="Loans" rows={rows} rowHref={(r) => `/loan/${r.name}`}
        columns={[
          { key: 'loan_number' }, { key: 'business_label', label: 'Business' },
          { key: 'principal_usd', hint: 'money', num: true },
          { key: 'rate_pct', hint: 'pct', num: true },
          { key: 'term_months', num: true },
          { key: 'funded_at', hint: 'date' },
          { key: 'risk_rating_label', label: 'Risk' },
          { key: 'health_score', num: true },
        ]}
        empty="No funded loans."
      />
    </Page>
  );
}

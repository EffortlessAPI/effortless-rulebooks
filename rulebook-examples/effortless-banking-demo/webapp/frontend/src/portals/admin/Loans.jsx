import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Loans() {
  const { data, loading } = useFetch('/api/loans');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="All Loans" subtitle="Complete loan book across all stages.">
      <DataTable
        table="Loans" rows={data} rowHref={(r) => `/loan/${r.name}`}
        columns={[
          { key: 'loan_number' }, { key: 'business_label', label: 'Business' },
          { key: 'loan_purpose', label: 'Purpose' },
          { key: 'principal_usd', hint: 'money', num: true },
          { key: 'rate_pct', hint: 'pct', num: true },
          { key: 'underwriting_stage', label: 'Stage' },
          { key: 'risk_rating_label', label: 'Risk' },
          { key: 'dscr', hint: 'ratio', num: true },
          { key: 'ltv', hint: 'ratio', num: true },
          { key: 'on_watchlist', hint: 'bool', label: 'Watch' },
        ]}
      />
    </Page>
  );
}

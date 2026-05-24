import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Applications() {
  const { data, loading } = useFetch('/api/loans');
  if (loading) return <div className="empty">Loading…</div>;
  const rows = (data || []).filter((l) => ['Application', 'Pre-Qual'].includes(l.underwriting_stage));
  return (
    <Page title="Applications" subtitle="Loans in intake and pre-qualification.">
      <DataTable
        table="Loans" rows={rows} rowHref={(r) => `/loan/${r.name}`}
        columns={[
          { key: 'loan_number' }, { key: 'business_label', label: 'Business' },
          { key: 'loan_purpose' }, { key: 'principal_usd', hint: 'money', num: true },
          { key: 'rate_pct', hint: 'pct', num: true }, { key: 'term_months', num: true },
          { key: 'underwriting_stage' }, { key: 'originating_rm_label', label: 'RM' },
        ]}
        empty="No applications in intake."
      />
    </Page>
  );
}

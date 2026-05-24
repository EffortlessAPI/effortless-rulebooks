import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Underwriting() {
  const { data, loading } = useFetch('/api/loans');
  if (loading) return <div className="empty">Loading…</div>;
  const rows = (data || []).filter((l) => ['Underwriting', 'Committee'].includes(l.underwriting_stage));
  return (
    <Page title="Underwriting Queue" subtitle="Loans currently in underwriting review or committee.">
      <DataTable
        table="Loans" rows={rows} rowHref={(r) => `/loan/${r.name}`}
        columns={[
          { key: 'loan_number' }, { key: 'business_label', label: 'Business' },
          { key: 'business_naics_code', label: 'NAICS' },
          { key: 'principal_usd', hint: 'money', num: true },
          { key: 'dscr', hint: 'ratio', num: true },
          { key: 'ltv', hint: 'ratio', num: true },
          { key: 'underwriter_label', label: 'Underwriter' },
          { key: 'segregation_of_duties_ok', label: 'SoD', hint: 'bool' },
          { key: 'underwriting_stage' },
        ]}
        empty="Queue is empty."
      />
    </Page>
  );
}

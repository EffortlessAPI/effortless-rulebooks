import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function UwDecisions() {
  const { data, loading } = useFetch('/api/loans');
  if (loading) return <div className="empty">Loading…</div>;
  const done = (data || []).filter((l) => ['Approved', 'Docs', 'Funded'].includes(l.underwriting_stage));
  return (
    <Page title="My Decisions" subtitle="Loans past committee that you underwrote.">
      <DataTable table="Loans" rows={done} rowHref={(r) => `/loan/${r.name}`}
        columns={[
          { key: 'loan_number' }, { key: 'business_label', label: 'Business' },
          { key: 'principal_usd', hint: 'money', num: true },
          { key: 'risk_rating_label' }, { key: 'underwriting_stage' },
          { key: 'funded_at', hint: 'date' }, { key: 'health_score', num: true },
        ]} />
    </Page>
  );
}

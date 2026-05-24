import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';
import { useAuth } from '../../auth.jsx';

export default function UwQueue() {
  const { user } = useAuth();
  const { data, loading } = useFetch('/api/loans');
  if (loading) return <div className="empty">Loading…</div>;
  const queue = (data || []).filter((l) => ['Underwriting', 'Committee'].includes(l.underwriting_stage));
  return (
    <Page title={`${user.full_name} — Underwriting Queue`} subtitle="Loans assigned to you, still in review.">
      <DataTable table="Loans" rows={queue} rowHref={(r) => `/loan/${r.name}`}
        columns={[
          { key: 'loan_number' }, { key: 'business_label', label: 'Business' },
          { key: 'business_naics_code', label: 'NAICS' },
          { key: 'principal_usd', hint: 'money', num: true },
          { key: 'dscr', hint: 'ratio', num: true },
          { key: 'ltv', hint: 'ratio', num: true },
          { key: 'global_cash_flow_usd', label: 'GCF', hint: 'money', num: true },
          { key: 'underwriting_stage' },
          { key: 'segregation_of_duties_ok', label: 'SoD', hint: 'bool' },
        ]} />
    </Page>
  );
}

import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function RmLoans() {
  const { data, loading } = useFetch('/api/loans');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="My Loans" subtitle="Loans you originated.">
      <DataTable table="Loans" rows={data} rowHref={(r) => `/loan/${r.name}`}
        columns={[
          { key: 'loan_number' }, { key: 'business_label', label: 'Business' },
          { key: 'loan_purpose' }, { key: 'principal_usd', hint: 'money', num: true },
          { key: 'underwriting_stage' }, { key: 'risk_rating_label' },
          { key: 'on_watchlist', hint: 'bool' },
        ]} />
    </Page>
  );
}

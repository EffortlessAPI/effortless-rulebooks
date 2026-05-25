import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Businesses() {
  const { data, loading } = useFetch('/api/businesses');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Businesses" subtitle="All commercial clients and prospects.">
      <DataTable
        table="Businesses" rows={data} rowHref={(r) => `/business/${r.name}`}
        columns={[
          { key: 'legal_name' }, { key: 'business_structure' },
          { key: 'naics_description', label: 'Industry' },
          { key: 'annual_revenue_usd', hint: 'money', num: true },
          { key: 'status' }, { key: 'relationship_manager_label', label: 'RM' },
          { key: 'total_deposit_balance_usd', label: 'Deposits', hint: 'money', num: true },
          { key: 'total_loan_principal_usd', label: 'Loan Principal', hint: 'money', num: true },
          { key: 'portfolio_priority' },
        ]}
      />
    </Page>
  );
}

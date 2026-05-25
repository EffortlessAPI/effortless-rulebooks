import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function RmPortfolio() {
  const { data, loading } = useFetch('/api/businesses');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="My Portfolio" subtitle="Businesses you manage as RM.">
      <DataTable table="Businesses" rows={data} rowHref={(r) => `/business/${r.name}`}
        columns={[
          { key: 'legal_name' }, { key: 'business_structure' },
          { key: 'naics_description', label: 'Industry' },
          { key: 'annual_revenue_usd', hint: 'money', num: true },
          { key: 'status' }, { key: 'meets_cdd_rule', label: 'CDD', hint: 'bool' },
          { key: 'portfolio_priority' },
        ]} />
    </Page>
  );
}

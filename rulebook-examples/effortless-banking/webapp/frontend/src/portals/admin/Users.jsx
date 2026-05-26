import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Users() {
  const { data, loading } = useFetch('/api/users');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Users" subtitle="Bank staff: RMs, Underwriters, Branch Bankers, Admins.">
      <DataTable
        table="Users" rows={data}
        columns={[
          { key: 'full_name' }, { key: 'role' }, { key: 'email' },
          { key: 'count_of_portfolio_businesses', label: 'Portfolio', num: true },
          { key: 'count_of_originated_loans', label: 'Originated', num: true },
          { key: 'count_of_underwritten_loans', label: 'Underwritten', num: true },
        ]}
      />
    </Page>
  );
}

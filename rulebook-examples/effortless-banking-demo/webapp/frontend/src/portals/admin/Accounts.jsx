import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Accounts() {
  const { data, loading } = useFetch('/api/accounts');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Deposit Accounts" subtitle="Checking, savings, and treasury services.">
      <DataTable
        table="Accounts" rows={data}
        columns={[
          { key: 'name' }, { key: 'business_label', label: 'Business' },
          { key: 'account_type' }, { key: 'account_number_last4', label: 'Last 4' },
          { key: 'current_balance_usd', hint: 'money', num: true },
          { key: 'has_ach', hint: 'bool' }, { key: 'has_wire', hint: 'bool' },
          { key: 'has_card', hint: 'bool' },
          { key: 'opened_at', hint: 'date' },
        ]}
      />
    </Page>
  );
}

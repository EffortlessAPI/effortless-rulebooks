import React from 'react';
import { Page, Card } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';
import { useAuth } from '../../auth.jsx';

export default function BbDashboard() {
  const { user } = useAuth();
  const accounts = useFetch('/api/accounts');
  const businesses = useFetch('/api/businesses');
  if (accounts.loading || businesses.loading) return <div className="empty">Loading…</div>;
  const totalDeposits = (accounts.data || []).reduce((s, a) => s + Number(a.current_balance_usd || 0), 0);
  return (
    <Page title={`Branch — ${user.full_name}`} subtitle="Deposit accounts and walk-in business activity.">
      <div className="kpis">
        <div className="kpi"><div className="label">Accounts</div><div className="value">{accounts.data.length}</div></div>
        <div className="kpi"><div className="label">Total Deposits</div><div className="value">${totalDeposits.toLocaleString()}</div></div>
        <div className="kpi"><div className="label">Customers</div><div className="value">{businesses.data.filter((b) => b.is_customer).length}</div></div>
        <div className="kpi"><div className="label">Prospects</div><div className="value">{businesses.data.filter((b) => b.is_prospect).length}</div></div>
      </div>
      <Card title="Recent Account Activity">
        <DataTable table="Accounts" rows={accounts.data}
          columns={[
            { key: 'name' }, { key: 'business_label', label: 'Business' },
            { key: 'account_type' }, { key: 'current_balance_usd', hint: 'money', num: true },
            { key: 'opened_at', hint: 'date' },
          ]} />
      </Card>
    </Page>
  );
}

import React from 'react';
import { Page, Card } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';
import { useAuth } from '../../auth.jsx';

export default function RmDashboard() {
  const { user } = useAuth();
  const biz = useFetch('/api/businesses');
  const loans = useFetch('/api/loans');
  if (biz.loading || loans.loading) return <div className="empty">Loading…</div>;
  return (
    <Page title={`Welcome, ${user.full_name}`} subtitle="Your portfolio at a glance. RLS scopes results to businesses where you are the RM.">
      <div className="kpis">
        <Kpi label="My Businesses" value={biz.data.length} />
        <Kpi label="My Loans" value={loans.data.length} />
        <Kpi label="Funded" value={loans.data.filter((l) => l.is_funded).length} />
        <Kpi label="On Watchlist" value={loans.data.filter((l) => l.on_watchlist).length} warn />
      </div>
      <Card title="My Portfolio">
        <DataTable table="Businesses" rows={biz.data} rowHref={(r) => `/business/${r.name}`}
          columns={[
            { key: 'legal_name' }, { key: 'status' },
            { key: 'annual_revenue_usd', hint: 'money', num: true },
            { key: 'total_deposit_balance_usd', hint: 'money', num: true },
            { key: 'total_loan_principal_usd', hint: 'money', num: true },
            { key: 'portfolio_priority' },
          ]} />
      </Card>
    </Page>
  );
}

function Kpi({ label, value, sub, warn }) {
  return (
    <div className="kpi" style={warn ? { borderColor: 'var(--warn)' } : undefined}>
      <div className="label">{label}</div>
      <div className="value">{value}</div>
      {sub && <div className="sub">{sub}</div>}
    </div>
  );
}

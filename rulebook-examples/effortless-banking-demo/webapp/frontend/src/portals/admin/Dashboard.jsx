import React from 'react';
import { Page, Card } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';
import { Field, formatValue } from '../../components/Field.jsx';

export default function AdminDashboard() {
  const kpis = useFetch('/api/kpis');
  const loans = useFetch('/api/loans?limit=10');
  const covenants = useFetch('/api/covenants?limit=10');

  return (
    <Page
      title="Administrator Dashboard"
      subtitle="Bank-wide view of the loan origination and portfolio. Double-click any value to inspect its rulebook definition."
    >
      {kpis.loading ? <div className="empty">Loading KPIs…</div> : kpis.err ? <div className="empty">Error: {kpis.err}</div> : (
        <>
          <div className="kpis">
            <Kpi label="Total Loans" value={kpis.data.loans} sub={`${kpis.data.funded} funded`} />
            <Kpi label="Funded Principal" value={formatMoney(kpis.data.totalPrincipal)} sub="origination book" />
            <Kpi label="Deposit Balance" value={formatMoney(kpis.data.totalDeposits)} sub={`${kpis.data.customers} customers`} />
            <Kpi label="Watchlist" value={kpis.data.watchlist} sub={`${kpis.data.breachedCovenants} breached covenants`} warn />
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 18 }}>
            <Card title="Pipeline by Underwriting Stage">
              <DataTable
                table="Loans"
                rows={kpis.data.byStage.map((r) => ({ name: r.underwriting_stage, underwriting_stage: r.underwriting_stage, n: r.n }))}
                columns={[
                  { key: 'underwriting_stage', label: 'Stage' },
                  { key: 'n', label: 'Loans', num: true },
                ]}
              />
            </Card>
            <Card title="Distribution by Risk Rating">
              <DataTable
                table="Loans"
                rows={kpis.data.byRisk.map((r) => ({ name: r.risk_rating_label, risk_rating_label: r.risk_rating_label, n: r.n }))}
                columns={[
                  { key: 'risk_rating_label', label: 'Risk Rating' },
                  { key: 'n', label: 'Loans', num: true },
                ]}
              />
            </Card>
          </div>
        </>
      )}

      <Card title="Most Recent Loans">
        {loans.loading ? '…' : (
          <DataTable
            table="Loans"
            rows={loans.data}
            rowHref={(r) => `/loan/${r.name}`}
            columns={[
              { key: 'loan_number' },
              { key: 'business_label', label: 'Business' },
              { key: 'loan_purpose', label: 'Purpose' },
              { key: 'principal_usd', label: 'Principal', hint: 'money', num: true },
              { key: 'underwriting_stage', label: 'Stage' },
              { key: 'risk_rating_label', label: 'Risk' },
              { key: 'dscr', label: 'DSCR', hint: 'ratio', num: true },
              { key: 'on_watchlist', label: 'Watch', hint: 'bool' },
            ]}
          />
        )}
      </Card>

      <Card title="Covenants Due Soon">
        {covenants.loading ? '…' : (
          <DataTable
            table="Covenants"
            rows={covenants.data}
            columns={[
              { key: 'name' },
              { key: 'loan_label', label: 'Loan' },
              { key: 'covenant_type', label: 'Type' },
              { key: 'threshold_value', label: 'Threshold', num: true },
              { key: 'next_test_date', label: 'Next Test', hint: 'date' },
              { key: 'status' },
              { key: 'is_breached', label: 'Breached', hint: 'bool' },
            ]}
          />
        )}
      </Card>
    </Page>
  );
}

function Kpi({ label, value, sub, warn }) {
  return (
    <div className="kpi" style={warn ? { borderColor: 'var(--warn)' } : undefined}>
      <div className="label">{label}</div>
      <div className="value">{value}</div>
      <div className="sub">{sub}</div>
    </div>
  );
}

function formatMoney(n) {
  if (!n) return '$0';
  if (n >= 1e6) return `$${(n / 1e6).toFixed(1)}M`;
  if (n >= 1e3) return `$${(n / 1e3).toFixed(0)}K`;
  return `$${n.toFixed(0)}`;
}

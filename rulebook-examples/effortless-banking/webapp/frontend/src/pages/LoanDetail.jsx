import React from 'react';
import { useParams, Link } from 'react-router-dom';
import { Page, Card } from '../components/Page.jsx';
import { useFetch } from '../useFetch.js';
import { DataTable } from '../components/Table.jsx';
import { Field } from '../components/Field.jsx';

export default function LoanDetail() {
  const { name } = useParams();
  const { data, loading, err } = useFetch(`/api/loans/${name}/detail`, [name]);
  if (loading) return <div className="empty">Loading…</div>;
  if (err) return <div className="empty">Error: {err}</div>;
  if (!data) return <div className="empty">Not found.</div>;
  const l = data.loan;
  return (
    <Page
      title={`${l.loan_number} — ${l.business_label}`}
      subtitle={<>
        <Link to={`/business/${l.business}`}>{l.business_label}</Link>
        {' · '}{l.loan_purpose}{' · '}{l.underwriting_stage}
      </>}
    >
      <div className="pill-grid">
        {[
          ['Principal', 'principal_usd', 'money'],
          ['Rate', 'rate_pct', 'pct'],
          ['Term (mo)', 'term_months'],
          ['DSCR', 'dscr', 'ratio'],
          ['LTV', 'ltv', 'ratio'],
          ['Global Cash Flow', 'global_cash_flow_usd', 'money'],
          ['Risk Rating', 'risk_rating_label'],
          ['Health Score', 'health_score'],
          ['Originating RM', 'originating_rm_label'],
          ['Underwriter', 'underwriter_label'],
          ['Funded', 'funded_at', 'date'],
          ['Watchlist', 'on_watchlist', 'bool'],
        ].map(([label, key, hint]) => (
          <div className="pill" key={key}>
            <div className="label">{label}</div>
            <Field as="div" table="Loans" col={key} value={l[key]} hint={hint} className="value" />
          </div>
        ))}
      </div>

      <Card title={`Covenants (${data.covenants.length})`}>
        <DataTable table="Covenants" rows={data.covenants}
          columns={[
            { key: 'covenant_type' }, { key: 'threshold_value', num: true },
            { key: 'test_frequency' }, { key: 'next_test_date', hint: 'date' },
            { key: 'status' }, { key: 'is_breached', hint: 'bool' },
            { key: 'has_active_waiver', label: 'Waiver', hint: 'bool' },
          ]} />
      </Card>

      <Card title={`Risk Rating History (${data.history.length})`}>
        <DataTable table="RiskRatingHistory" rows={data.history}
          columns={[
            { key: 'effective_date', hint: 'date' },
            { key: 'prior_grade', num: true }, { key: 'new_grade', num: true },
            { key: 'changed_by_user_label', label: 'Changed By' },
            { key: 'reason' },
          ]} />
      </Card>

      <Card title={`Documents (${data.documents.length})`}>
        <DataTable table="Documents" rows={data.documents}
          columns={Object.keys(data.documents[0] || { name: 1 }).slice(0, 6).map((k) => ({ key: k }))} />
      </Card>
    </Page>
  );
}

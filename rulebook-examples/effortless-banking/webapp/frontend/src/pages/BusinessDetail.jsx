import React from 'react';
import { useParams } from 'react-router-dom';
import { Page, Card } from '../components/Page.jsx';
import { useFetch } from '../useFetch.js';
import { DataTable } from '../components/Table.jsx';
import { Field, formatValue } from '../components/Field.jsx';

export default function BusinessDetail() {
  const { name } = useParams();
  const { data, loading, err } = useFetch(`/api/businesses/${name}/detail`, [name]);
  if (loading) return <div className="empty">Loading…</div>;
  if (err) return <div className="empty">Error: {err}</div>;
  if (!data) return <div className="empty">Not found.</div>;
  const b = data.business;
  return (
    <Page title={b.legal_name} subtitle={`${b.business_structure} · ${b.naics_description} · ${b.status}`}>
      <div className="pill-grid">
        {[
          ['Deposits', 'total_deposit_balance_usd', 'money'],
          ['Loan Principal', 'total_loan_principal_usd', 'money'],
          ['Loans', 'count_of_loans'],
          ['Owners', 'count_of_beneficial_owners'],
          ['Accounts', 'count_of_accounts'],
          ['Interactions', 'count_of_interactions'],
          ['Annual Revenue', 'annual_revenue_usd', 'money'],
          ['Portfolio Priority', 'portfolio_priority'],
        ].map(([label, key, hint]) => (
          <div className="pill" key={key}>
            <div className="label">{label}</div>
            <Field as="div" table="Businesses" col={key} value={b[key]} hint={hint} className="value" />
          </div>
        ))}
      </div>

      <Card title={`Beneficial Owners (${data.owners.length})`}>
        <DataTable table="BeneficialOwners" rows={data.owners}
          columns={[
            { key: 'full_name' }, { key: 'ownership_percentage', hint: 'pct', num: true },
            { key: 'is_control_person', hint: 'bool' },
            { key: 'meets25_percent_threshold', label: '≥25%', hint: 'bool' },
          ]} />
      </Card>
      <Card title={`Contacts (${data.contacts.length})`}>
        <DataTable table="Contacts" rows={data.contacts}
          columns={[
            { key: 'full_name' }, { key: 'title' }, { key: 'contact_type' },
            { key: 'email' }, { key: 'phone' }, { key: 'is_authorized_signer', hint: 'bool' },
          ]} />
      </Card>
      <Card title={`Accounts (${data.accounts.length})`}>
        <DataTable table="Accounts" rows={data.accounts}
          columns={[
            { key: 'account_type' }, { key: 'account_number_last4', label: 'Last 4' },
            { key: 'current_balance_usd', hint: 'money', num: true },
            { key: 'has_ach', hint: 'bool' }, { key: 'has_wire', hint: 'bool' }, { key: 'has_card', hint: 'bool' },
            { key: 'opened_at', hint: 'date' },
          ]} />
      </Card>
      <Card title={`Loans (${data.loans.length})`}>
        <DataTable table="Loans" rows={data.loans} rowHref={(r) => `/loan/${r.name}`}
          columns={[
            { key: 'loan_number' }, { key: 'loan_purpose' },
            { key: 'principal_usd', hint: 'money', num: true },
            { key: 'underwriting_stage' }, { key: 'risk_rating_label' },
            { key: 'on_watchlist', hint: 'bool' },
          ]} />
      </Card>
      <Card title={`Interactions (${data.interactions.length})`}>
        <DataTable table="Interactions" rows={data.interactions}
          columns={Object.keys(data.interactions[0] || { name: 1 }).slice(0, 6).map((k) => ({ key: k }))} />
      </Card>
    </Page>
  );
}

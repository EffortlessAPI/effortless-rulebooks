import React from 'react';
import { Page, Card } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Watchlist() {
  const loans = useFetch('/api/loans');
  const covenants = useFetch('/api/covenants');
  const watch = (loans.data || []).filter((l) => l.on_watchlist || l.is_classified_asset);
  const breached = (covenants.data || []).filter((c) => c.is_breached);
  return (
    <Page title="Watchlist & Breaches" subtitle="Loans on the watchlist and covenants in breach.">
      <Card title={`Watchlist Loans (${watch.length})`}>
        <DataTable
          table="Loans" rows={watch} rowHref={(r) => `/loan/${r.name}`}
          columns={[
            { key: 'loan_number' }, { key: 'business_label', label: 'Business' },
            { key: 'principal_usd', hint: 'money', num: true },
            { key: 'risk_rating_label' }, { key: 'dscr', hint: 'ratio', num: true },
            { key: 'has_breached_covenant', hint: 'bool' },
            { key: 'is_classified_asset', hint: 'bool' },
          ]}
          empty="No loans on the watchlist."
        />
      </Card>
      <Card title={`Breached Covenants (${breached.length})`}>
        <DataTable
          table="Covenants" rows={breached}
          columns={[
            { key: 'name' }, { key: 'loan_label', label: 'Loan' },
            { key: 'covenant_type' }, { key: 'threshold_value', num: true },
            { key: 'next_test_date', hint: 'date' }, { key: 'status' },
            { key: 'has_active_waiver', hint: 'bool' },
          ]}
          empty="No breached covenants."
        />
      </Card>
    </Page>
  );
}

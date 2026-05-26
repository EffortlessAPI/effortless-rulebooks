import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Covenants() {
  const { data, loading } = useFetch('/api/covenants');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Covenants" subtitle="All loan covenants — type, threshold, breach status.">
      <DataTable
        table="Covenants" rows={data}
        columns={[
          { key: 'name' }, { key: 'loan_label', label: 'Loan' },
          { key: 'loan_business', label: 'Business' },
          { key: 'covenant_type' }, { key: 'threshold_value', num: true },
          { key: 'test_frequency' }, { key: 'next_test_date', hint: 'date' },
          { key: 'status' }, { key: 'is_breached', hint: 'bool' },
          { key: 'has_active_waiver', label: 'Waiver', hint: 'bool' },
        ]}
      />
    </Page>
  );
}

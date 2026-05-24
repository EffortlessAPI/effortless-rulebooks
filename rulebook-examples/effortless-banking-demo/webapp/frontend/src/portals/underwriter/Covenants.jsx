import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function UwCovenants() {
  const { data, loading } = useFetch('/api/covenants');
  if (loading) return <div className="empty">Loading…</div>;
  const sorted = [...(data || [])].sort((a, b) => (b.is_breached - a.is_breached));
  return (
    <Page title="Covenants" subtitle="Breaches surface first.">
      <DataTable table="Covenants" rows={sorted}
        columns={[
          { key: 'loan_label', label: 'Loan' }, { key: 'covenant_type' },
          { key: 'threshold_value', num: true }, { key: 'next_test_date', hint: 'date' },
          { key: 'status' }, { key: 'is_breached', hint: 'bool' },
          { key: 'has_active_waiver', label: 'Waiver', hint: 'bool' },
        ]} />
    </Page>
  );
}

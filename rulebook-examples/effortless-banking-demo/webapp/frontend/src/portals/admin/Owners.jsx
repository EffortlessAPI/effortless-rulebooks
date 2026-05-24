import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Owners() {
  const { data, loading } = useFetch('/api/beneficial_owners');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Beneficial Owners" subtitle="25%+ owners and control persons under CDD/BSA.">
      <DataTable
        table="BeneficialOwners" rows={data}
        columns={[
          { key: 'full_name' }, { key: 'business_label', label: 'Business' },
          { key: 'ownership_percentage', hint: 'pct', num: true },
          { key: 'is_control_person', hint: 'bool' },
          { key: 'meets25_percent_threshold', label: '≥25%', hint: 'bool' },
          { key: 'meets_cdd_threshold', label: 'CDD', hint: 'bool' },
        ]}
      />
    </Page>
  );
}

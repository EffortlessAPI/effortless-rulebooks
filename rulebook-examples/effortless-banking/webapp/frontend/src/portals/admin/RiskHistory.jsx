import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function RiskHistory() {
  const { data, loading } = useFetch('/api/risk_rating_history');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Risk Rating Changes" subtitle="Audit trail of grade changes across the portfolio.">
      <DataTable
        table="RiskRatingHistory" rows={data}
        columns={[
          { key: 'name' }, { key: 'loan_label', label: 'Loan' },
          { key: 'effective_date', hint: 'date' },
          { key: 'prior_grade', num: true }, { key: 'new_grade', num: true },
          { key: 'changed_by_user_label', label: 'Changed By' },
          { key: 'reason' },
        ]}
      />
    </Page>
  );
}

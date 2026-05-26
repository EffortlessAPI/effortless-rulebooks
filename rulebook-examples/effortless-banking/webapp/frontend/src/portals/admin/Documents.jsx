import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Documents() {
  const { data, loading } = useFetch('/api/documents');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Documents" subtitle="Loan packages, financials, KYC files.">
      <DataTable table="Documents" rows={data}
        columns={Object.keys(data?.[0] || { name: 1 }).slice(0, 8).map((k) => ({ key: k }))}
        empty="No documents." />
    </Page>
  );
}

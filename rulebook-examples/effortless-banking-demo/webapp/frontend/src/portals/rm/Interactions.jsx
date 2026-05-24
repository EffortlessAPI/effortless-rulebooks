import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function RmInteractions() {
  const { data, loading } = useFetch('/api/interactions');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Recent Interactions" subtitle="Calls, visits, emails across your book.">
      <DataTable table="Interactions" rows={data}
        columns={Object.keys(data?.[0] || { name: 1 }).slice(0, 8).map((k) => ({ key: k }))} />
    </Page>
  );
}

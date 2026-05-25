import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function BbContacts() {
  const { data, loading } = useFetch('/api/contacts');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Walk-In Contacts" subtitle="Business contacts you've serviced at the branch.">
      <DataTable table="Contacts" rows={data}
        columns={[
          { key: 'full_name' }, { key: 'title' },
          { key: 'business_label', label: 'Business' },
          { key: 'contact_type' }, { key: 'email' }, { key: 'phone' },
        ]} />
    </Page>
  );
}

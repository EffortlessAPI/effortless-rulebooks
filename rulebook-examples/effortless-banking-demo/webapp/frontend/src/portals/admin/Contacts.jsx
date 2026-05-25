import React from 'react';
import { Page } from '../../components/Page.jsx';
import { useFetch } from '../../useFetch.js';
import { DataTable } from '../../components/Table.jsx';

export default function Contacts() {
  const { data, loading } = useFetch('/api/contacts');
  if (loading) return <div className="empty">Loading…</div>;
  return (
    <Page title="Contacts" subtitle="Authorized signers, officers, AP clerks.">
      <DataTable
        table="Contacts" rows={data}
        columns={[
          { key: 'full_name' }, { key: 'title' },
          { key: 'business_label', label: 'Business' },
          { key: 'contact_type' }, { key: 'email' }, { key: 'phone' },
          { key: 'is_authorized_signer', hint: 'bool' },
        ]}
      />
    </Page>
  );
}

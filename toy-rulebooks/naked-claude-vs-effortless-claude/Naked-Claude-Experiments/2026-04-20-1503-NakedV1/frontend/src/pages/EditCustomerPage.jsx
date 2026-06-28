import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import CustomerForm from '../components/CustomerForm';

export default function EditCustomerPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [customer, setCustomer] = useState(null);
  const [loading, setLoading]   = useState(true);
  const [error, setError]       = useState('');

  useEffect(() => {
    fetch(`/api/customers/${id}`)
      .then(r => r.ok ? r.json() : Promise.reject('Not found'))
      .then(setCustomer)
      .catch(err => setError(typeof err === 'string' ? err : err.message))
      .finally(() => setLoading(false));
  }, [id]);

  async function handleSubmit(data) {
    const res = await fetch(`/api/customers/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.error || 'Failed to update customer');
    }
    navigate(`/customers/${id}`);
  }

  if (loading) return <div style={{ color: '#6b7280', padding: 40 }}>Loading…</div>;
  if (error)   return <div style={{ color: '#b91c1c', padding: 14 }}>{error}</div>;
  if (!customer) return null;

  return (
    <div>
      <div style={{ marginBottom: 20, fontSize: 14, color: '#6b7280' }}>
        <Link to={`/customers/${id}`} style={{ color: '#4f46e5' }}>← Back to {customer.name}</Link>
      </div>
      <div style={{
        background: '#fff', border: '1px solid #e5e7eb',
        borderRadius: 12, padding: 28,
        boxShadow: '0 2px 8px rgba(0,0,0,0.07)',
      }}>
        <h2 style={{ fontSize: 22, fontWeight: 800, marginBottom: 24, color: '#1a1a2e' }}>
          Edit: {customer.name}
        </h2>
        <CustomerForm
          initial={customer}
          onSubmit={handleSubmit}
          onCancel={() => navigate(`/customers/${id}`)}
          submitLabel="Save Changes"
        />
      </div>
    </div>
  );
}

import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import CustomerForm from '../components/CustomerForm';

export default function AddCustomerPage() {
  const navigate = useNavigate();

  async function handleSubmit(data) {
    const res = await fetch('/api/customers', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.error || 'Failed to create customer');
    }
    navigate('/');
  }

  return (
    <div>
      <div style={{ marginBottom: 20, fontSize: 14, color: '#6b7280' }}>
        <Link to="/" style={{ color: '#4f46e5' }}>← All Customers</Link>
      </div>
      <div style={{
        background: '#fff', border: '1px solid #e5e7eb',
        borderRadius: 12, padding: 28,
        boxShadow: '0 2px 8px rgba(0,0,0,0.07)',
      }}>
        <h2 style={{ fontSize: 22, fontWeight: 800, marginBottom: 24, color: '#1a1a2e' }}>Add Customer</h2>
        <CustomerForm
          onSubmit={handleSubmit}
          onCancel={() => navigate('/')}
          submitLabel="Add Customer"
        />
      </div>
    </div>
  );
}

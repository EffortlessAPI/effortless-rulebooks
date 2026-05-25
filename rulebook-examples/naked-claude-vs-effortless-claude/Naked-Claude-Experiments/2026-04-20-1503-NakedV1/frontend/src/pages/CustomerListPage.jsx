import React, { useEffect, useState, useCallback } from 'react';
import { Link } from 'react-router-dom';
import Summary from '../components/Summary';
import CustomerCard from '../components/CustomerCard';

export default function CustomerListPage() {
  const [customers, setCustomers] = useState([]);
  const [summary, setSummary]     = useState(null);
  const [loading, setLoading]     = useState(true);
  const [error, setError]         = useState('');

  const fetchAll = useCallback(async () => {
    try {
      const [cRes, sRes] = await Promise.all([
        fetch('/api/customers'),
        fetch('/api/summary'),
      ]);
      if (!cRes.ok || !sRes.ok) throw new Error('Failed to load data');
      const [cData, sData] = await Promise.all([cRes.json(), sRes.json()]);
      setCustomers(cData);
      setSummary(sData);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchAll(); }, [fetchAll]);

  async function handleDelete(id, name) {
    if (!window.confirm(`Delete "${name}"? This cannot be undone.`)) return;
    try {
      const res = await fetch(`/api/customers/${id}`, { method: 'DELETE' });
      if (!res.ok) throw new Error('Delete failed');
      await fetchAll();
    } catch (err) {
      alert(err.message);
    }
  }

  return (
    <div>
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
        <div>
          <h1 style={{ fontSize: 26, fontWeight: 800, color: '#1a1a2e' }}>Customer Tracker</h1>
          <p style={{ fontSize: 14, color: '#6b7280', marginTop: 2 }}>Keep track of your customers and their status.</p>
        </div>
        <Link to="/customers/new">
          <button style={{
            background: '#4f46e5', color: '#fff',
            border: 'none', borderRadius: 8,
            padding: '10px 20px', fontWeight: 600, fontSize: 14,
          }}>+ Add Customer</button>
        </Link>
      </div>

      {/* Summary */}
      {summary && <Summary summary={summary} />}

      {/* State */}
      {loading && <div style={{ color: '#6b7280', textAlign: 'center', padding: 40 }}>Loading…</div>}
      {error   && <div style={{ color: '#b91c1c', background: '#fef2f2', borderRadius: 8, padding: 14 }}>{error}</div>}

      {/* Customer list */}
      {!loading && !error && (
        customers.length === 0
          ? (
            <div style={{
              textAlign: 'center', padding: 60, color: '#9ca3af',
              background: '#fff', borderRadius: 12, border: '1px dashed #e5e7eb',
            }}>
              <div style={{ fontSize: 40, marginBottom: 12 }}>🙂</div>
              <div style={{ fontSize: 16, fontWeight: 600 }}>No customers yet.</div>
              <div style={{ fontSize: 14, marginTop: 4 }}>Add your first customer to get started.</div>
            </div>
          )
          : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
              {customers.map(c => (
                <CustomerCard key={c.id} customer={c} onDelete={handleDelete} />
              ))}
            </div>
          )
      )}
    </div>
  );
}

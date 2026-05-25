import React, { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchCustomers, fetchSummary, createCustomer } from './api';
import { colorToHex } from './colorUtils';
import SummaryBar from './SummaryBar';
import CustomerModal from './CustomerModal';

export default function CustomerList() {
  const [customers, setCustomers] = useState([]);
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);
  const [showAdd, setShowAdd] = useState(false);
  const navigate = useNavigate();

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [custs, summ] = await Promise.all([fetchCustomers(), fetchSummary()]);
      setCustomers(custs);
      setSummary(summ);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  async function handleAdd(data) {
    await createCustomer(data);
    setShowAdd(false);
    await load();
  }

  const existingIds = customers.map(c => c.customer_id);

  return (
    <div className="container">
      <SummaryBar summary={summary} />

      <div className="toolbar">
        <h2>Customers ({customers.length})</h2>
        <button className="btn btn-primary" onClick={() => setShowAdd(true)}>+ Add Customer</button>
      </div>

      {loading ? (
        <div className="loading">Loading…</div>
      ) : (
        <div className="customer-list">
          {customers.map(c => (
            <div
              key={c.customer_id}
              className={`customer-card${c.is_stopped ? ' stopped' : ''}`}
              onClick={() => navigate(`/customers/${c.customer_id}`)}
            >
              <div className="color-badge" style={{ background: colorToHex(c.current_color) }} />
              <div className="customer-info">
                <div className="customer-name">{c.name}</div>
                {c.notes && <div className="customer-notes">{c.notes}</div>}
              </div>
              <span className="color-label">{c.current_color}</span>
              {c.is_stopped && <span className="stopped-badge">Stopped</span>}
            </div>
          ))}
          {customers.length === 0 && (
            <p style={{ color: '#888', textAlign: 'center', padding: 32 }}>No customers yet. Add one!</p>
          )}
        </div>
      )}

      {showAdd && (
        <CustomerModal
          onSave={handleAdd}
          onClose={() => setShowAdd(false)}
          existingIds={existingIds}
        />
      )}
    </div>
  );
}

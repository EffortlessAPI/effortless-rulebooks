import React, { useState, useEffect, useCallback } from 'react';
import Summary from './components/Summary.jsx';
import CustomerList from './components/CustomerList.jsx';
import CustomerDetail from './components/CustomerDetail.jsx';
import CustomerForm from './components/CustomerForm.jsx';
import './App.css';

const API = '/api';

export default function App() {
  const [customers, setCustomers] = useState([]);
  const [summary, setSummary] = useState(null);
  const [view, setView] = useState('list'); // 'list' | 'detail' | 'add' | 'edit'
  const [selectedCustomer, setSelectedCustomer] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      const [custRes, sumRes] = await Promise.all([
        fetch(`${API}/customers`),
        fetch(`${API}/summary`)
      ]);
      if (!custRes.ok || !sumRes.ok) throw new Error('Failed to fetch');
      setCustomers(await custRes.json());
      setSummary(await sumRes.json());
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchData(); }, [fetchData]);

  const handleAdd = () => {
    setSelectedCustomer(null);
    setView('add');
  };

  const handleEdit = (customer) => {
    setSelectedCustomer(customer);
    setView('edit');
  };

  const handleView = (customer) => {
    setSelectedCustomer(customer);
    setView('detail');
  };

  const handleDelete = async (customerId) => {
    if (!window.confirm('Delete this customer?')) return;
    try {
      const res = await fetch(`${API}/customers/${customerId}`, { method: 'DELETE' });
      if (!res.ok) throw new Error('Delete failed');
      await fetchData();
      if (view === 'detail') setView('list');
    } catch (err) {
      alert('Error: ' + err.message);
    }
  };

  const handleSave = async (data) => {
    try {
      if (view === 'edit' && selectedCustomer) {
        const res = await fetch(`${API}/customers/${selectedCustomer.customer_id}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data)
        });
        if (!res.ok) throw new Error('Update failed');
      } else {
        const res = await fetch(`${API}/customers`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data)
        });
        if (!res.ok) throw new Error('Create failed');
      }
      await fetchData();
      setView('list');
    } catch (err) {
      alert('Error: ' + err.message);
    }
  };

  const handleCancel = () => {
    if (view === 'detail' && selectedCustomer) {
      setView('detail');
    }
    setView(view === 'add' ? 'list' : view === 'edit' ? 'detail' : 'list');
  };

  if (loading && !customers.length) {
    return <div className="loading">Loading…</div>;
  }

  if (error) {
    return <div className="error">Error: {error} <button onClick={fetchData}>Retry</button></div>;
  }

  return (
    <div className="app">
      <header className="app-header">
        <h1>Customer Tracker</h1>
        {view !== 'list' && (
          <button className="btn btn-secondary" onClick={() => setView('list')}>
            ← Back to List
          </button>
        )}
      </header>

      {summary && view === 'list' && <Summary summary={summary} />}

      {view === 'list' && (
        <CustomerList
          customers={customers}
          onView={handleView}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onAdd={handleAdd}
        />
      )}

      {view === 'detail' && selectedCustomer && (
        <CustomerDetail
          customer={selectedCustomer}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      )}

      {(view === 'add' || view === 'edit') && (
        <CustomerForm
          customer={view === 'edit' ? selectedCustomer : null}
          onSave={handleSave}
          onCancel={() => setView(view === 'edit' ? 'detail' : 'list')}
        />
      )}
    </div>
  );
}

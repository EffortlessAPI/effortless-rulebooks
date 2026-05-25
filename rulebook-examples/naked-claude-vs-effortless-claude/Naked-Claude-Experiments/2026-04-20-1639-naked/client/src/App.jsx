import { useState, useEffect, useCallback } from 'react';
import Summary from './components/Summary.jsx';
import CustomerList from './components/CustomerList.jsx';
import CustomerDetail from './components/CustomerDetail.jsx';
import CustomerForm from './components/CustomerForm.jsx';

const API = '/api';

export default function App() {
  const [customers, setCustomers] = useState([]);
  const [summary, setSummary]     = useState(null);
  const [loading, setLoading]     = useState(true);
  const [error, setError]         = useState(null);

  // view: 'list' | 'detail' | 'add' | 'edit'
  const [view, setView]             = useState('list');
  const [selectedId, setSelectedId] = useState(null);

  const fetchAll = useCallback(async () => {
    try {
      const [custRes, summRes] = await Promise.all([
        fetch(`${API}/customers`),
        fetch(`${API}/summary`),
      ]);
      if (!custRes.ok) throw new Error('Failed to load customers');
      if (!summRes.ok) throw new Error('Failed to load summary');
      setCustomers(await custRes.json());
      setSummary(await summRes.json());
      setError(null);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchAll(); }, [fetchAll]);

  // ── Navigation helpers ──────────────────────────────────────────────────
  const goList   = ()   => { setView('list');   setSelectedId(null); };
  const goDetail = (id) => { setView('detail'); setSelectedId(id);   };
  const goAdd    = ()   => { setView('add');    setSelectedId(null); };
  const goEdit   = (id) => { setView('edit');   setSelectedId(id);   };

  // ── CRUD handlers ───────────────────────────────────────────────────────
  const handleCreate = async (data) => {
    const res = await fetch(`${API}/customers`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.error || 'Failed to create customer');
    }
    await fetchAll();
    goList();
  };

  const handleUpdate = async (id, data) => {
    const res = await fetch(`${API}/customers/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.error || 'Failed to update customer');
    }
    await fetchAll();
    goDetail(id);
  };

  const handleDelete = async (id) => {
    const res = await fetch(`${API}/customers/${id}`, { method: 'DELETE' });
    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.error || 'Failed to delete customer');
    }
    await fetchAll();
    goList();
  };

  // ── Render ──────────────────────────────────────────────────────────────
  const selectedCustomer = customers.find((c) => c.id === selectedId) || null;

  return (
    <div style={styles.app}>
      <header style={styles.header}>
        <div style={styles.headerInner}>
          <h1 style={styles.title} onClick={goList} role="button" tabIndex={0}
              onKeyDown={(e) => e.key === 'Enter' && goList()}>
            Customer Tracker
          </h1>
          {view === 'list' && (
            <button style={styles.addBtn} onClick={goAdd}>
              + Add Customer
            </button>
          )}
          {view !== 'list' && (
            <button style={styles.backBtn} onClick={goList}>
              ← Back to List
            </button>
          )}
        </div>
      </header>

      <main style={styles.main}>
        {error && (
          <div style={styles.errorBanner}>{error}</div>
        )}

        {loading ? (
          <div style={styles.loading}>Loading…</div>
        ) : view === 'list' ? (
          <>
            <Summary summary={summary} />
            <CustomerList
              customers={customers}
              onSelect={goDetail}
            />
          </>
        ) : view === 'detail' ? (
          <CustomerDetail
            customer={selectedCustomer}
            onEdit={() => goEdit(selectedId)}
            onDelete={handleDelete}
            onBack={goList}
          />
        ) : view === 'add' ? (
          <CustomerForm
            title="New Customer"
            onSubmit={handleCreate}
            onCancel={goList}
          />
        ) : view === 'edit' ? (
          <CustomerForm
            title="Edit Customer"
            initial={selectedCustomer}
            onSubmit={(data) => handleUpdate(selectedId, data)}
            onCancel={() => goDetail(selectedId)}
          />
        ) : null}
      </main>
    </div>
  );
}

const styles = {
  app: {
    minHeight: '100vh',
    display: 'flex',
    flexDirection: 'column',
  },
  header: {
    background: '#1a1a2e',
    color: '#fff',
    padding: '0 24px',
    boxShadow: '0 2px 8px rgba(0,0,0,0.3)',
  },
  headerInner: {
    maxWidth: 900,
    margin: '0 auto',
    height: 64,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  title: {
    fontSize: 22,
    fontWeight: 700,
    letterSpacing: 0.5,
    cursor: 'pointer',
    userSelect: 'none',
  },
  addBtn: {
    background: '#4CAF50',
    color: '#fff',
    border: 'none',
    borderRadius: 6,
    padding: '8px 18px',
    fontSize: 14,
    fontWeight: 600,
    cursor: 'pointer',
  },
  backBtn: {
    background: 'transparent',
    color: '#ccc',
    border: '1px solid #555',
    borderRadius: 6,
    padding: '8px 18px',
    fontSize: 14,
    cursor: 'pointer',
  },
  main: {
    flex: 1,
    maxWidth: 900,
    width: '100%',
    margin: '0 auto',
    padding: '24px 16px',
  },
  errorBanner: {
    background: '#ffebee',
    color: '#c62828',
    border: '1px solid #ef9a9a',
    borderRadius: 6,
    padding: '12px 16px',
    marginBottom: 20,
    fontSize: 14,
  },
  loading: {
    textAlign: 'center',
    padding: 60,
    color: '#666',
    fontSize: 16,
  },
};

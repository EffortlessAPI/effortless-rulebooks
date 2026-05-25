import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { DagCell } from '../explainer-dag';

interface Transaction {
  transaction_id: string;
  product_name: string;
  transaction_type_name: string;
  quantity: number;
  amount: number;
  transaction_date: string;
  notes?: string;
}

interface Product {
  product_id: string;
  name: string;
}

interface TransactionType {
  transaction_type_id: string;
  name: string;
}

export default function Transactions() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [types, setTypes] = useState<TransactionType[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({
    transaction_id: '',
    product: '',
    transaction_type: '',
    quantity: 0,
    notes: '',
  });

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    try {
      const [txRes, prodRes, typeRes] = await Promise.all([
        api('/api/transactions'),
        api('/api/products'),
        api('/api/transaction-types'),
      ]);
      const txData = await txRes.json();
      const prodData = await prodRes.json();
      const typeData = await typeRes.json();

      setTransactions(txData.map((tx: any) => ({
        ...tx,
        quantity: Number(tx.quantity),
        amount: Number(tx.amount),
      })));
      setProducts(prodData.map((p: any) => ({
        ...p,
        unit_price: Number(p.unit_price),
        current_quantity: Number(p.current_quantity),
        reorder_level: Number(p.reorder_level),
      })));
      setTypes(typeData);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.transaction_id || !form.product || !form.transaction_type) {
      alert('Required fields missing');
      return;
    }

    try {
      const res = await api('/api/transactions', {
        method: 'POST',
        body: JSON.stringify({
          ...form,
          transaction_date: new Date().toISOString(),
        }),
      });
      if (res.ok) {
        await loadData();
        setForm({ transaction_id: '', product: '', transaction_type: '', quantity: 0, notes: '' });
        setShowForm(false);
      }
    } catch (err) {
      console.error(err);
    }
  }

  if (loading) return <div className="container"><p>Loading...</p></div>;

  return (
    <div className="container">
      <h1>Transactions</h1>
      <p>View and record inventory adjustments (purchases, sales, corrections)</p>

      <button onClick={() => setShowForm(!showForm)} style={{ marginBottom: '2rem' }}>
        {showForm ? '✕ Cancel' : '+ Record Transaction'}
      </button>

      {showForm && (
        <form onSubmit={handleSubmit} style={{ background: '#f9f9f9', padding: '1.5rem', borderRadius: '8px', marginBottom: '2rem', maxWidth: '400px' }}>
          <div style={{ marginBottom: '1rem' }}>
            <label>Transaction ID</label>
            <input
              type="text"
              required
              value={form.transaction_id}
              onChange={(e) => setForm({ ...form, transaction_id: e.target.value })}
              placeholder="e.g., TXN-011"
              style={{ width: '100%', padding: '0.5rem' }}
            />
          </div>
          <div style={{ marginBottom: '1rem' }}>
            <label>Product</label>
            <select
              required
              value={form.product}
              onChange={(e) => setForm({ ...form, product: e.target.value })}
              style={{ width: '100%', padding: '0.5rem' }}
            >
              <option value="">Select product...</option>
              {products.map((p) => (
                <option key={p.product_id} value={p.product_id}>
                  {p.name}
                </option>
              ))}
            </select>
          </div>
          <div style={{ marginBottom: '1rem' }}>
            <label>Type</label>
            <select
              required
              value={form.transaction_type}
              onChange={(e) => setForm({ ...form, transaction_type: e.target.value })}
              style={{ width: '100%', padding: '0.5rem' }}
            >
              <option value="">Select type...</option>
              {types.map((t) => (
                <option key={t.transaction_type_id} value={t.transaction_type_id}>
                  {t.name}
                </option>
              ))}
            </select>
          </div>
          <div style={{ marginBottom: '1rem' }}>
            <label>Quantity (negative for sales)</label>
            <input
              type="number"
              required
              value={form.quantity}
              onChange={(e) => setForm({ ...form, quantity: parseInt(e.target.value) })}
              style={{ width: '100%', padding: '0.5rem' }}
            />
          </div>
          <div style={{ marginBottom: '1rem' }}>
            <label>Notes</label>
            <input
              type="text"
              value={form.notes}
              onChange={(e) => setForm({ ...form, notes: e.target.value })}
              placeholder="Optional notes"
              style={{ width: '100%', padding: '0.5rem' }}
            />
          </div>
          <button type="submit">Record Transaction</button>
        </form>
      )}

      <h2>Recent Transactions</h2>
      {transactions.length === 0 ? (
        <p>No transactions yet</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Product</th>
              <th>Type</th>
              <th>Quantity</th>
              <th>Amount (Qty × Price)</th>
              <th>Date</th>
              <th>Notes</th>
            </tr>
          </thead>
          <tbody>
            {transactions.map((tx) => (
              <tr key={tx.transaction_id}>
                <td><strong>{tx.transaction_id}</strong></td>
                <td><DagCell table="Transactions" field="ProductName">{tx.product_name}</DagCell></td>
                <td><DagCell table="Transactions" field="TransactionTypeName">{tx.transaction_type_name}</DagCell></td>
                <td style={{ textAlign: 'right' }}>{tx.quantity}</td>
                <td style={{ textAlign: 'right' }}><DagCell table="Transactions" field="Amount">${tx.amount.toFixed(2)}</DagCell></td>
                <td>{new Date(tx.transaction_date).toLocaleDateString()}</td>
                <td>{tx.notes || '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getPayments } from './api';

function fmt(n) { return n != null ? '$' + parseFloat(n).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '—'; }

export default function PaymentsListPage() {
  const [payments, setPayments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    (async () => {
      try { setPayments(await getPayments()); }
      catch (e) { setError(e.message); }
      finally { setLoading(false); }
    })();
  }, []);

  const totalCompleted = payments.filter(p => p.is_completed).reduce((acc, p) => acc + parseFloat(p.completed_amount || 0), 0);

  return (
    <div style={{ maxWidth: '1100px', margin: '0 auto', padding: '28px 16px' }}>
      <div style={{ marginBottom: '22px' }}>
        <h1 style={{ fontSize: '1.55rem', fontWeight: 700, color: '#1a1a1a' }}>Payments</h1>
        <p style={{ color: '#6b7280', fontSize: '0.88rem', marginTop: '2px' }}>
          All recorded payments — {payments.length} total. Total completed: <strong>{fmt(totalCompleted)}</strong>
        </p>
      </div>

      {loading && <p style={{ color: '#6b7280' }}>Loading…</p>}
      {error && <p style={{ color: '#dc2626' }}>Error: {error}</p>}

      {!loading && !error && (
        <div style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden' }}>
          {payments.length === 0 ? (
            <p style={{ padding: '32px', textAlign: 'center', color: '#6b7280' }}>No payments recorded yet.</p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Date</th>
                  <th style={th}>Invoice</th>
                  <th style={th}>Amount</th>
                  <th style={th}>Method</th>
                  <th style={th}>Status</th>
                  <th style={th}>Completed Amt</th>
                  <th style={th}>Transaction ID</th>
                  <th style={th}>Invoice Total</th>
                  <th style={th}>Invoice Due</th>
                  <th style={th}>Notes</th>
                </tr>
              </thead>
              <tbody>
                {payments.map(p => (
                  <tr key={p.payment_id} style={{ borderBottom: '1px solid #e5e7eb', background: p.payment_status === 'Completed' ? '#fff' : '#fafafa' }}>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280' }}>{p.payment_date ? new Date(p.payment_date).toLocaleDateString() : '—'}</td>
                    <td style={td}>
                      <Link to={`/invoices/${encodeURIComponent(p.invoice)}`} style={{ fontWeight: 600, color: '#4f46e5', textDecoration: 'none' }}>
                        #{p.invoice_number}
                      </Link>
                      <div style={{ fontSize: '0.75rem', color: '#9ca3af' }}>{p.invoice}</div>
                    </td>
                    <td style={{ ...td, fontWeight: 600 }}>{fmt(p.amount)}</td>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280' }}>{p.payment_method || '—'}</td>
                    <td style={td}>
                      <span style={{ fontSize: '0.8rem', padding: '2px 8px', borderRadius: '8px', background: p.is_completed ? '#f0fdf4' : '#fef2f2', color: p.is_completed ? '#16a34a' : '#dc2626', border: `1px solid ${p.is_completed ? '#86efac' : '#fca5a5'}`, fontWeight: 600 }}>
                        {p.payment_status}
                      </span>
                    </td>
                    <td style={{ ...td, fontWeight: 600, color: parseFloat(p.completed_amount || 0) > 0 ? '#16a34a' : '#9ca3af' }}>{fmt(p.completed_amount)}</td>
                    <td style={{ ...td, fontSize: '0.78rem', fontFamily: 'monospace', color: '#6b7280', maxWidth: '130px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{p.transaction_id || '—'}</td>
                    <td style={td}>{fmt(p.invoice_total)}</td>
                    <td style={{ ...td, color: parseFloat(p.order_amount_due || 0) > 0 ? '#dc2626' : '#16a34a', fontWeight: 600 }}>{fmt(p.order_amount_due)}</td>
                    <td style={{ ...td, color: '#6b7280', fontSize: '0.82rem', maxWidth: '150px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{p.notes || '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      )}
    </div>
  );
}

const th = { padding: '11px 12px', textAlign: 'left', fontSize: '0.76rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '11px 12px', fontSize: '0.9rem', verticalAlign: 'middle' };

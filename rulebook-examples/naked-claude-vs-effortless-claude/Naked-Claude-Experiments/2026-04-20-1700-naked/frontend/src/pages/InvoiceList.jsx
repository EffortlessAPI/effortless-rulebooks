import { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { fetchInvoices, deleteInvoice } from '../api.js';

const fmt = (n) => Number(n ?? 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' });
const fmtDate = (d) => d ? new Date(d).toLocaleDateString() : '—';

function PayLabel({ label }) {
  const color = label === 'Paid' ? '#27ae60' : label === 'Unpaid' ? '#c0392b' : '#e67e22';
  return <span style={{ color, fontWeight: 700 }}>{label}</span>;
}

export default function InvoiceList() {
  const [invoices, setInvoices] = useState([]);
  const [loading, setLoading]   = useState(true);
  const navigate = useNavigate();

  const load = useCallback(async () => {
    setLoading(true);
    try { setInvoices(await fetchInvoices()); }
    catch (e) { console.error(e); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async (inv) => {
    if (!window.confirm(`Delete invoice #${inv.invoice_number} for ${inv.client_name}? This will also delete all its line items and payments.`)) return;
    try { await deleteInvoice(inv.id); load(); }
    catch (err) { alert(err.message); }
  };

  return (
    <>
      <div className="list-header">
        <h2>Invoices {!loading && `(${invoices.length})`}</h2>
        <button className="btn-primary" onClick={() => navigate('/invoices/new')}>+ Add Invoice</button>
      </div>
      {loading ? <p>Loading…</p> : invoices.length === 0 ? (
        <p style={{ color: '#888' }}>No invoices yet.</p>
      ) : (
        <table className="data-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Client</th>
              <th>Date</th>
              <th>Status</th>
              <th>Items</th>
              <th>Total</th>
              <th>Paid</th>
              <th>Due</th>
              <th>Pay Status</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {invoices.map((inv) => (
              <tr key={inv.id}>
                <td><Link to={`/invoices/${inv.id}`} className="link-plain">{inv.invoice_number}</Link></td>
                <td><Link to={`/clients/${inv.client_id}`} className="link-plain">{inv.client_name}</Link></td>
                <td>{fmtDate(inv.order_date)}</td>
                <td><span className="tag">{inv.order_status}</span></td>
                <td>{inv.item_count}</td>
                <td>{fmt(inv.invoice_total)}</td>
                <td>{fmt(inv.total_paid)}</td>
                <td>{fmt(inv.amount_due)}</td>
                <td><PayLabel label={inv.payment_status_label} /></td>
                <td>
                  <div className="row-actions">
                    <button className="btn-secondary btn-sm" onClick={() => navigate(`/invoices/${inv.id}`)}>View</button>
                    <button className="btn-secondary btn-sm" onClick={() => navigate(`/invoices/${inv.id}/edit`)}>Edit</button>
                    <button className="btn-danger btn-sm" onClick={() => handleDelete(inv)}>Delete</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  );
}

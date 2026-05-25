import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchClient, deleteClient, deleteInvoice, fetchClientApprovals, deleteClientApproval } from '../api.js';
import StatusBadge from '../components/StatusBadge.jsx';

const fmt = (n) => Number(n ?? 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' });
const fmtDate = (d) => d ? new Date(d).toLocaleDateString() : '—';

function Field({ label, value, mono }) {
  return (
    <div className="detail-field">
      <div className="field-label">{label}</div>
      <div className="field-value" style={mono ? { fontFamily: 'monospace', color: '#888' } : {}}>
        {value || <em style={{ color: '#aaa' }}>—</em>}
      </div>
    </div>
  );
}

function PayLabel({ label }) {
  const color = label === 'Paid' ? '#27ae60' : label === 'Unpaid' ? '#c0392b' : '#e67e22';
  return <span style={{ color, fontWeight: 700 }}>{label}</span>;
}

export default function ClientDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [client, setClient]      = useState(null);
  const [approvals, setApprovals] = useState([]);
  const [loading, setLoading]    = useState(true);
  const [error, setError]        = useState('');

  const load = () => {
    setLoading(true);
    Promise.all([fetchClient(id), fetchClientApprovals(id)])
      .then(([c, a]) => { setClient(c); setApprovals(a); })
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [id]);

  const handleDeleteApproval = async (a) => {
    if (!window.confirm(`Delete approval "${a.slug}"?`)) return;
    try { await deleteClientApproval(a.id); load(); }
    catch (err) { alert(err.message); }
  };

  const handleDelete = async () => {
    if (!window.confirm(`Delete "${client.name}"? This cannot be undone.`)) return;
    try {
      await deleteClient(id);
      navigate('/');
    } catch (err) {
      alert(err.message);
    }
  };

  const handleDeleteInvoice = async (invoice) => {
    if (!window.confirm(`Delete invoice #${invoice.invoice_number}? This will also delete all its line items and payments.`)) return;
    try {
      await deleteInvoice(invoice.id);
      load();
    } catch (err) {
      alert(err.message);
    }
  };

  if (loading) return <p>Loading…</p>;
  if (error)   return <p className="error-msg">{error}</p>;
  if (!client) return null;

  return (
    <div className="page-card">
      <Link className="back-link" to="/">← Back to clients</Link>

      {client.is_stopped && (
        <div className="stopped-banner">
          ⛔ This client is STOPPED — their status ({client.status_name}) is blocking
        </div>
      )}

      <h2>
        {client.name}
        {client.is_stopped && <span className="stopped-badge" style={{ fontSize: 14 }}>STOPPED</span>}
      </h2>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 0 }}>
        <Field label="Company"          value={client.company_name} />
        <Field label="Slug"             value={client.slug} mono />
        <Field label="Email"            value={client.email} />
        <Field label="Phone"            value={client.phone} />
        <Field label="Billing Address"  value={client.billing_address} />
        <Field label="Shipping Address" value={client.shipping_address} />
      </div>

      <div className="detail-field">
        <div className="field-label">Status</div>
        <div className="field-value">
          {client.status_name
            ? <StatusBadge name={client.status_name} isBlocking={client.status_is_blocking} />
            : <em style={{ color: '#aaa' }}>No status assigned</em>
          }
        </div>
      </div>

      <div className="detail-field">
        <div className="field-label">Category</div>
        <div className="field-value">
          {client.category_name ? (
            <>
              <Link to={`/client-categories/${client.category_id}`} className="link-plain">
                {client.category_name}
              </Link>
              <span className="tag" style={{ marginLeft: 8 }}>
                {(Number(client.category_discount) * 100).toFixed(0)}% discount
              </span>
            </>
          ) : (
            <em style={{ color: '#aaa' }}>No category assigned</em>
          )}
        </div>
      </div>
      {client.status_description && (
        <div className="detail-field">
          <div className="field-label">Status Description</div>
          <div className="field-value" style={{ fontSize: 14, color: '#555' }}>
            {client.status_description}
          </div>
        </div>
      )}

      <div className="detail-field">
        <div className="field-label">Stopped</div>
        <div className="field-value" style={{ fontWeight: 600, color: client.is_stopped ? '#c0392b' : '#27ae60' }}>
          {client.is_stopped ? 'Yes — currently stopped' : 'No — progressing normally'}
        </div>
      </div>

      <div className="detail-field">
        <div className="field-label">Client Since (days)</div>
        <div className="field-value">
          {fmtDate(client.created_at)}
          {client.created_at && (
            <span style={{ color: '#888', fontSize: 13, marginLeft: 8 }}>
              ({Math.floor((Date.now() - new Date(client.created_at)) / 86400000)} days)
            </span>
          )}
        </div>
      </div>

      <Field label="Notes" value={client.notes} />

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: 0 }}>
        <div className="detail-field">
          <div className="field-label">Avg Order Value</div>
          <div className="field-value" style={{ color: '#aaa', fontStyle: 'italic', fontSize: 13 }}>
            Unable to generate formula
          </div>
        </div>
        <div className="detail-field">
          <div className="field-label">Lifetime Margin %</div>
          <div className="field-value" style={{ color: '#aaa', fontStyle: 'italic', fontSize: 13 }}>
            Unable to generate formula
          </div>
        </div>
        <div className="detail-field">
          <div className="field-label">Days Since Last Order</div>
          <div className="field-value" style={{ color: '#aaa', fontStyle: 'italic', fontSize: 13 }}>
            Unable to generate formula
          </div>
        </div>
      </div>

      {/* Approvals inline */}
      <div style={{ marginTop: 24 }}>
        <div className="list-header">
          <h3 style={{ fontSize: 15 }}>
            Approvals ({approvals.length})
            {approvals.length > 0 && (
              <span style={{ marginLeft: 10, fontSize: 12, fontWeight: 400, color: '#666' }}>
                {approvals.filter((a) => a.is_approved).length} approved
                {' · '}
                {approvals.filter((a) => !a.is_approved).length} pending
              </span>
            )}
          </h3>
          <button className="btn-primary btn-sm" onClick={() => navigate('/client-approvals/new')}>
            + Add Approval
          </button>
        </div>
        {approvals.length === 0 ? (
          <p style={{ color: '#888', fontSize: 14 }}>No approvals yet.</p>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Status</th>
                <th>Approved By</th>
                <th>Notes</th>
                <th>Created</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {approvals.map((a) => (
                <tr key={a.id}>
                  <td>
                    <span className={`tag ${a.is_approved ? 'tag-green' : 'tag-grey'}`}>
                      {a.is_approved ? 'Approved' : 'Pending'}
                    </span>
                  </td>
                  <td>
                    {a.approver_name
                      ? <Link to={`/app-users/${a.approved_by_user_id}`} className="link-plain">
                          {a.approver_name} <span style={{ color: '#888', fontSize: 12 }}>({a.approver_role})</span>
                        </Link>
                      : <em style={{ color: '#aaa' }}>—</em>}
                  </td>
                  <td>{a.notes || <em style={{ color: '#aaa' }}>—</em>}</td>
                  <td>{fmtDate(a.created_at)}</td>
                  <td>
                    <div className="row-actions">
                      <button className="btn-secondary btn-sm"
                              onClick={() => navigate(`/client-approvals/${a.id}/edit`)}>Edit</button>
                      <button className="btn-danger btn-sm"
                              onClick={() => handleDeleteApproval(a)}>Delete</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Invoices inline */}
      <div style={{ marginTop: 24 }}>
        <div className="list-header">
          <h3 style={{ fontSize: 15 }}>Invoices ({(client.invoices || []).length})</h3>
          <button className="btn-primary btn-sm" onClick={() => navigate(`/invoices/new?client=${id}`)}>
            + Add Invoice
          </button>
        </div>
        {(client.invoices || []).length === 0 ? (
          <p style={{ color: '#888', fontSize: 14 }}>No invoices yet.</p>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>#</th>
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
              {client.invoices.map((inv) => (
                <tr key={inv.id}>
                  <td><Link to={`/invoices/${inv.id}`} className="link-plain">{inv.invoice_number}</Link></td>
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
                      <button className="btn-danger btn-sm" onClick={() => handleDeleteInvoice(inv)}>Delete</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <div className="form-actions">
        <button className="btn-primary" onClick={() => navigate(`/clients/${id}/edit`)}>Edit</button>
        <button className="btn-danger" onClick={handleDelete}>Delete</button>
        <button className="btn-secondary" onClick={() => navigate('/')}>Back</button>
      </div>
    </div>
  );
}

import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { getClient, updateClient, deleteClient, getAppUsers, createApproval, updateApproval, deleteApproval } from './api';
import ClientForm from './ClientForm';
import { ApprovalForm } from './ApprovalsPage';

function fmt(n) {
  if (n == null) return '—';
  return '$' + parseFloat(n).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

export default function ClientDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [client, setClient] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleteConfirm, setDeleteConfirm] = useState(false);
  const [deleteError, setDeleteError] = useState(null);
  const [appUsers, setAppUsers] = useState([]);
  const [showApprovalForm, setShowApprovalForm] = useState(false);
  const [editApproval, setEditApproval] = useState(null);
  const [approvalSaving, setApprovalSaving] = useState(false);

  async function load() {
    try {
      setLoading(true);
      const [data, users] = await Promise.all([getClient(id), getAppUsers()]);
      setClient(data);
      setAppUsers(users);
      setError(null);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { load(); }, [id]);

  async function handleSaveApproval(data) {
    setApprovalSaving(true);
    try {
      if (editApproval) await updateApproval(editApproval.client_approval_id, data);
      else await createApproval(data);
      setShowApprovalForm(false);
      setEditApproval(null);
      await load();
    } catch (e) { alert(e.message); } finally { setApprovalSaving(false); }
  }

  async function handleDeleteApproval(approvalId) {
    if (!confirm('Delete this approval?')) return;
    try { await deleteApproval(approvalId); await load(); }
    catch (e) { alert(e.message); }
  }

  async function handleSave(formData) {
    setSaving(true);
    try {
      await updateClient(id, formData);
      await load();
      setEditing(false);
    } catch (e) {
      alert(e.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    setDeleteError(null);
    try {
      await deleteClient(id);
      navigate('/');
    } catch (e) {
      setDeleteError(e.message);
    }
  }

  if (loading) return <div style={page}><p style={{ color: '#6b7280' }}>Loading…</p></div>;
  if (error)   return <div style={page}><p style={{ color: '#dc2626' }}>Error: {error}</p></div>;
  if (!client) return null;

  const invoices = client.invoice_list || [];
  const approvals = client.approval_list || [];

  return (
    <div style={page}>
      <div style={{ maxWidth: '820px', margin: '0 auto' }}>
        <Link to="/" style={{ color: '#4f46e5', fontWeight: 600, fontSize: '0.9rem', display: 'inline-flex', alignItems: 'center', gap: '4px', marginBottom: '20px', textDecoration: 'none' }}>
          ← Back to clients
        </Link>

        <div style={{
          background: '#fff', borderRadius: '14px', padding: '28px',
          boxShadow: '0 1px 6px rgba(0,0,0,0.08)',
          borderLeft: client.is_stopped ? '4px solid #dc2626' : '4px solid #16a34a',
          marginBottom: '20px'
        }}>
          {!editing ? (
            <>
              <div style={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', marginBottom: '20px' }}>
                <div>
                  <h1 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: '4px', display: 'flex', alignItems: 'center', gap: '10px' }}>
                    {client.name}
                    {client.is_vip && (
                      <span style={{ fontSize: '0.72rem', padding: '3px 10px', borderRadius: '10px', background: '#fef3c7', color: '#92400e', border: '1px solid #fcd34d', fontWeight: 800, letterSpacing: '0.04em' }}>
                        ⭐ VIP
                      </span>
                    )}
                  </h1>
                  {client.company_name && <div style={{ color: '#6b7280', fontSize: '0.92rem', marginBottom: '8px' }}>{client.company_name}</div>}
                  <div style={{ display: 'flex', gap: '8px', alignItems: 'center', flexWrap: 'wrap' }}>
                    {client.status_display_name ? (
                      <span style={{
                        padding: '3px 12px', borderRadius: '12px',
                        background: client.is_stopped ? '#fef2f2' : '#f3f4f6',
                        color: client.is_stopped ? '#991b1b' : '#374151',
                        fontWeight: 600, fontSize: '0.85rem',
                        border: `1px solid ${client.is_stopped ? '#fca5a5' : '#d1d5db'}`
                      }}>
                        {client.status_display_name}
                      </span>
                    ) : (
                      <span style={{ color: '#9ca3af', fontSize: '0.85rem' }}>No status</span>
                    )}
                    {client.category_name && (
                      <span style={{
                        padding: '3px 12px', borderRadius: '12px',
                        background: parseFloat(client.category_discount) > 0 ? '#f0fdf4' : '#eff6ff',
                        color: parseFloat(client.category_discount) > 0 ? '#166534' : '#1e40af',
                        fontWeight: 600, fontSize: '0.85rem',
                        border: `1px solid ${parseFloat(client.category_discount) > 0 ? '#86efac' : '#bfdbfe'}`
                      }}>
                        {client.category_name}
                        {parseFloat(client.category_discount) > 0 && (
                          <span style={{ marginLeft: '6px', fontWeight: 700 }}>
                            −{(parseFloat(client.category_discount) * 100).toFixed(0)}%
                          </span>
                        )}
                      </span>
                    )}
                    {client.is_stopped
                      ? <span style={stoppedBadge}>🚫 Stopped</span>
                      : <span style={okBadge}>✓ Active</span>}
                  </div>
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button onClick={() => setEditing(true)} style={editBtn}>Edit</button>
                  <button onClick={() => { setDeleteConfirm(true); setDeleteError(null); }} style={deleteBtn}>Delete</button>
                </div>
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '14px', borderTop: '1px solid #f3f4f6', paddingTop: '16px' }}>
                <Field label="Email" value={client.email} />
                <Field label="Phone" value={client.phone} />
                <Field label="Billing Address" value={client.billing_address} multiline />
                <Field label="Shipping Address" value={client.shipping_address} multiline />
              </div>

              {client.status_description && (
                <div style={{ borderTop: '1px solid #f3f4f6', paddingTop: '14px', marginTop: '14px' }}>
                  <label style={fieldLabel}>Status Description</label>
                  <p style={{ color: '#374151', fontSize: '0.92rem', lineHeight: '1.55', fontStyle: 'italic' }}>
                    {client.status_description}
                  </p>
                </div>
              )}

              <div style={{ borderTop: '1px solid #f3f4f6', paddingTop: '14px', marginTop: '14px' }}>
                <label style={fieldLabel}>Notes</label>
                <p style={{ color: client.notes ? '#374151' : '#d1d5db', fontSize: '0.93rem', lineHeight: '1.55' }}>
                  {client.notes || 'No notes.'}
                </p>
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4,1fr)', gap: '14px', borderTop: '1px solid #f3f4f6', paddingTop: '14px', marginTop: '14px' }}>
                <Field label="Client Since (days)" value={client.customer_since_days ?? '—'} />
                <Field label="Avg Invoice Value" value={client.average_order_value != null ? '$' + parseFloat(client.average_order_value).toFixed(2) : '—'} />
                <div>
                  <label style={fieldLabel}>Last Invoice</label>
                  <p style={{ color: client.last_invoice ? '#374151' : '#d1d5db', fontSize: '0.9rem' }}>
                    {client.last_invoice ? new Date(client.last_invoice).toLocaleDateString() : '—'}
                    {client.hast_recent_invoices && (
                      <span style={{ marginLeft: '6px', fontSize: '0.7rem', padding: '1px 7px', borderRadius: '8px', background: '#dcfce7', color: '#166534', fontWeight: 700 }}>
                        RECENT
                      </span>
                    )}
                  </p>
                </div>
                <Field label="Big Invoices" value={client.count_of_big_invoices ?? 0} />
              </div>

              <div style={{ borderTop: '1px solid #f3f4f6', paddingTop: '12px', marginTop: '12px' }}>
                <label style={fieldLabel}>ID</label>
                <p style={{ color: '#9ca3af', fontSize: '0.82rem', fontFamily: 'monospace' }}>{client.client_id}</p>
              </div>
            </>
          ) : (
            <>
              <h2 style={{ marginBottom: '18px', fontSize: '1.15rem', fontWeight: 700 }}>Edit Client</h2>
              <ClientForm initial={client} onSave={handleSave} onCancel={() => setEditing(false)} saving={saving} />
            </>
          )}
        </div>

        {/* Invoices list */}
        <div style={{ background: '#fff', borderRadius: '14px', padding: '22px 28px', boxShadow: '0 1px 6px rgba(0,0,0,0.06)' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '14px' }}>
            <h2 style={{ fontSize: '1rem', fontWeight: 700, color: '#374151' }}>
              Invoices
              <span style={{ marginLeft: '8px', background: '#dbeafe', color: '#1e40af', padding: '2px 9px', borderRadius: '10px', fontSize: '0.8rem', fontWeight: 700 }}>
                {invoices.length}
              </span>
            </h2>
            <Link
              to={`/invoices?client=${encodeURIComponent(client.client_id)}`}
              style={{ fontSize: '0.82rem', color: '#4f46e5', fontWeight: 600, textDecoration: 'none' }}
            >
              + New Invoice
            </Link>
          </div>
          {invoices.length === 0 ? (
            <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>No invoices yet.</p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Invoice #</th>
                  <th style={th}>Date</th>
                  <th style={th}>Status</th>
                  <th style={th}>Total</th>
                  <th style={th}>Due</th>
                  <th style={th}>Payment</th>
                </tr>
              </thead>
              <tbody>
                {invoices.map(o => (
                  <tr key={o.invoice_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>
                      <Link to={`/invoices/${encodeURIComponent(o.invoice_id)}`} style={{ fontWeight: 600, color: '#4f46e5', textDecoration: 'none' }}>
                        #{o.invoice_number}
                      </Link>
                    </td>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280' }}>
                      {o.order_date ? new Date(o.order_date).toLocaleDateString() : '—'}
                    </td>
                    <td style={td}>
                      <span style={{ fontSize: '0.82rem', padding: '2px 8px', borderRadius: '8px', background: '#f3f4f6', color: '#374151', border: '1px solid #d1d5db' }}>
                        {o.order_status || '—'}
                      </span>
                    </td>
                    <td style={{ ...td, fontWeight: 600 }}>{fmt(o.invoice_total)}</td>
                    <td style={{ ...td, color: parseFloat(o.amount_due) > 0 ? '#dc2626' : '#16a34a', fontWeight: 600 }}>{fmt(o.amount_due)}</td>
                    <td style={td}>
                      <PayBadge label={o.payment_status_label} />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {/* Approvals */}
        <div style={{ background: '#fff', borderRadius: '14px', padding: '22px 28px', boxShadow: '0 1px 6px rgba(0,0,0,0.06)', marginTop: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '14px' }}>
            <h2 style={{ fontSize: '1rem', fontWeight: 700, color: '#374151' }}>
              Approvals
              <span style={{ marginLeft: '8px', background: '#e9d5ff', color: '#6b21a8', padding: '2px 9px', borderRadius: '10px', fontSize: '0.8rem', fontWeight: 700 }}>
                {approvals.length}
              </span>
              {approvals.some(a => !a.is_approved) && (
                <span style={{ marginLeft: '6px', background: '#fffbeb', color: '#b45309', padding: '2px 9px', borderRadius: '10px', fontSize: '0.75rem', fontWeight: 700, border: '1px solid #fcd34d' }}>
                  {approvals.filter(a => !a.is_approved).length} PENDING
                </span>
              )}
            </h2>
            <button onClick={() => { setEditApproval(null); setShowApprovalForm(true); }} style={{ padding: '6px 14px', borderRadius: '7px', border: 'none', background: '#7c3aed', color: '#fff', fontWeight: 700, fontSize: '0.85rem', cursor: 'pointer' }}>
              + Request Approval
            </button>
          </div>
          {approvals.length === 0 ? (
            <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>No approval records yet.</p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Approved By</th>
                  <th style={th}>Role</th>
                  <th style={th}>Status</th>
                  <th style={th}>Notes</th>
                  <th style={{ ...th, width: '90px' }}></th>
                </tr>
              </thead>
              <tbody>
                {approvals.map(a => (
                  <tr key={a.client_approval_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>
                      {a.approved_by_contact_name ? (
                        <span style={{ fontWeight: 600, color: '#374151' }}>{a.approved_by_contact_name}</span>
                      ) : <span style={{ color: '#9ca3af', fontStyle: 'italic' }}>Unassigned</span>}
                    </td>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280' }}>{a.approved_by_role || '—'}</td>
                    <td style={td}>
                      {a.is_approved ? (
                        <span style={{ display: 'inline-block', fontSize: '0.78rem', padding: '3px 10px', borderRadius: '10px', background: '#f0fdf4', color: '#166534', border: '1px solid #86efac', fontWeight: 700 }}>✓ APPROVED</span>
                      ) : (
                        <span style={{ display: 'inline-block', fontSize: '0.78rem', padding: '3px 10px', borderRadius: '10px', background: '#fffbeb', color: '#b45309', border: '1px solid #fcd34d', fontWeight: 700 }}>⏳ PENDING</span>
                      )}
                    </td>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280', maxWidth: '200px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{a.notes || '—'}</td>
                    <td style={td}>
                      <div style={{ display: 'flex', gap: '4px' }}>
                        <button onClick={() => { setEditApproval(a); setShowApprovalForm(true); }} style={{ padding: '3px 9px', borderRadius: '5px', background: '#eff6ff', color: '#1e40af', fontSize: '0.78rem', fontWeight: 600, border: '1px solid #bfdbfe', cursor: 'pointer' }}>Edit</button>
                        <button onClick={() => handleDeleteApproval(a.client_approval_id)} style={{ padding: '3px 9px', borderRadius: '5px', background: '#fff', color: '#dc2626', fontSize: '0.78rem', fontWeight: 600, border: '1px solid #fca5a5', cursor: 'pointer' }}>Del</button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {showApprovalForm && (
          <div style={modalOverlay}>
            <div style={modalBox}>
              <h2 style={{ marginBottom: '18px', fontSize: '1.1rem', fontWeight: 700 }}>
                {editApproval ? 'Edit Approval' : 'Request Approval'} — {client.name}
              </h2>
              <ApprovalForm
                initial={editApproval}
                clients={[{ client_id: client.client_id, name: client.name, company_name: client.company_name }]}
                users={appUsers}
                onSave={handleSaveApproval}
                onCancel={() => { setShowApprovalForm(false); setEditApproval(null); }}
                saving={approvalSaving}
                lockedClient={client.client_id}
              />
            </div>
          </div>
        )}

        {deleteConfirm && (
          <div style={modalOverlay}>
            <div style={modalBox}>
              <h2 style={{ marginBottom: '12px', fontSize: '1.1rem', fontWeight: 700 }}>Delete Client?</h2>
              {deleteError ? (
                <div style={{ padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '7px', color: '#991b1b', marginBottom: '16px', fontSize: '0.88rem' }}>
                  ⚠️ {deleteError}
                </div>
              ) : (
                <p style={{ color: '#374151', marginBottom: '20px' }}>
                  Are you sure you want to delete <strong>{client.name}</strong>? This cannot be undone.
                </p>
              )}
              <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
                <button onClick={() => { setDeleteConfirm(false); setDeleteError(null); }} style={cancelBtn}>
                  {deleteError ? 'Close' : 'Cancel'}
                </button>
                {!deleteError && (
                  <button onClick={handleDelete} style={{ padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' }}>
                    Delete
                  </button>
                )}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

function Field({ label, value, multiline }) {
  return (
    <div>
      <label style={fieldLabel}>{label}</label>
      {multiline ? (
        <p style={{ color: value ? '#374151' : '#d1d5db', fontSize: '0.9rem', lineHeight: '1.5', whiteSpace: 'pre-wrap' }}>{value || '—'}</p>
      ) : (
        <p style={{ color: value ? '#374151' : '#d1d5db', fontSize: '0.9rem' }}>{value || '—'}</p>
      )}
    </div>
  );
}

function PayBadge({ label }) {
  const colors = { Paid: ['#f0fdf4','#16a34a','#86efac'], Partial: ['#fffbeb','#d97706','#fcd34d'], Unpaid: ['#fef2f2','#dc2626','#fca5a5'] };
  const [bg, color, border] = colors[label] || ['#f3f4f6','#374151','#d1d5db'];
  return <span style={{ fontSize: '0.8rem', padding: '2px 9px', borderRadius: '10px', background: bg, color, border: `1px solid ${border}`, fontWeight: 700 }}>{label || '—'}</span>;
}

const page = { maxWidth: '900px', margin: '0 auto', padding: '28px 16px' };
const fieldLabel = { display: 'block', fontSize: '0.75rem', fontWeight: 700, color: '#9ca3af', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: '5px' };
const th = { padding: '9px 14px', textAlign: 'left', fontSize: '0.75rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '10px 14px', fontSize: '0.9rem', verticalAlign: 'middle' };
const stoppedBadge = { display: 'inline-block', padding: '3px 10px', borderRadius: '12px', background: '#fef2f2', color: '#991b1b', fontWeight: 700, fontSize: '0.82rem', border: '1px solid #fca5a5' };
const okBadge = { display: 'inline-block', padding: '3px 10px', borderRadius: '12px', background: '#f0fdf4', color: '#166534', fontWeight: 600, fontSize: '0.82rem', border: '1px solid #86efac' };
const editBtn = { padding: '7px 16px', borderRadius: '7px', border: '1px solid #c7d2fe', background: '#eff6ff', color: '#1e40af', fontWeight: 600, fontSize: '0.88rem', cursor: 'pointer' };
const deleteBtn = { padding: '7px 14px', borderRadius: '7px', border: '1px solid #fca5a5', background: '#fff', color: '#dc2626', fontWeight: 600, fontSize: '0.88rem', cursor: 'pointer' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', maxWidth: '420px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };

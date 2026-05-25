import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import {
  fetchClientApproval, createClientApproval, updateClientApproval,
  fetchClients, fetchAppUsers,
} from '../api.js';

export default function ClientApprovalForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [clients, setClients] = useState([]);
  const [users, setUsers]     = useState([]);
  const [form, setForm] = useState({ client_id: '', approved_by_user_id: '', notes: '' });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving]   = useState(false);
  const [error, setError]     = useState('');

  useEffect(() => {
    (async () => {
      try {
        const [cs, us] = await Promise.all([fetchClients(), fetchAppUsers()]);
        setClients(cs);
        setUsers(us);
        if (isEdit) {
          const a = await fetchClientApproval(id);
          setForm({
            client_id: String(a.client_id),
            approved_by_user_id: a.approved_by_user_id ? String(a.approved_by_user_id) : '',
            notes: a.notes || '',
          });
        }
      } catch (e) { setError(e.message); }
      finally { setLoading(false); }
    })();
  }, [id, isEdit]);

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.client_id) { setError('Client is required.'); return; }
    setSaving(true); setError('');
    const payload = {
      client_id: Number(form.client_id),
      approved_by_user_id: form.approved_by_user_id ? Number(form.approved_by_user_id) : null,
      notes: form.notes,
    };
    try {
      if (isEdit) { await updateClientApproval(id, payload); navigate(`/client-approvals`); }
      else { await createClientApproval(payload); navigate('/client-approvals'); }
    } catch (err) { setError(err.message); }
    finally { setSaving(false); }
  };

  if (loading) return <p>Loading…</p>;

  return (
    <div className="page-card">
      <Link className="back-link" to="/client-approvals">← Back to approvals</Link>
      <h2>{isEdit ? 'Edit Approval' : 'Add Approval'}</h2>
      <form onSubmit={handleSubmit}>
        <div className="field">
          <label htmlFor="client_id">Client *</label>
          <select id="client_id" value={form.client_id} onChange={set('client_id')} required disabled={isEdit}>
            <option value="">— Select client —</option>
            {clients.map((c) => (
              <option key={c.id} value={c.id}>{c.name}{c.company_name ? ` (${c.company_name})` : ''}</option>
            ))}
          </select>
        </div>
        <div className="field">
          <label htmlFor="approved_by_user_id">Approved By (leave blank for Pending)</label>
          <select id="approved_by_user_id" value={form.approved_by_user_id} onChange={set('approved_by_user_id')}>
            <option value="">— Pending —</option>
            {users.filter((u) => u.role !== 'Customer').map((u) => (
              <option key={u.id} value={u.id}>{u.contact_name || u.email} ({u.role})</option>
            ))}
          </select>
        </div>
        <div className="field">
          <label htmlFor="notes">Notes</label>
          <textarea id="notes" value={form.notes} onChange={set('notes')} />
        </div>
        {error && <p className="error-msg">{error}</p>}
        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving}>
            {saving ? 'Saving…' : isEdit ? 'Save Changes' : 'Add Approval'}
          </button>
          <button type="button" className="btn-secondary" onClick={() => navigate('/client-approvals')}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

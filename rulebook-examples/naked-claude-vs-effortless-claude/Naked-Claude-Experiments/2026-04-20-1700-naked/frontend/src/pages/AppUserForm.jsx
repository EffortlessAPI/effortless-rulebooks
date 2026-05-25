import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchAppUser, createAppUser, updateAppUser } from '../api.js';

export default function AppUserForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [form, setForm] = useState({
    contact_name: '', email: '', phone: '', role: 'Customer', notes: '',
  });
  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving]   = useState(false);
  const [error, setError]     = useState('');

  useEffect(() => {
    if (!isEdit) return;
    fetchAppUser(id)
      .then((u) => setForm({
        contact_name: u.contact_name || '',
        email: u.email || '',
        phone: u.phone || '',
        role: u.role || 'Customer',
        notes: u.notes || '',
      }))
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.contact_name.trim() && !form.email.trim()) {
      setError('Contact name or email is required.'); return;
    }
    setSaving(true); setError('');
    try {
      if (isEdit) { await updateAppUser(id, form); navigate(`/app-users/${id}`); }
      else {
        const created = await createAppUser(form);
        navigate(`/app-users/${created.id}`);
      }
    } catch (err) { setError(err.message); }
    finally { setSaving(false); }
  };

  if (loading) return <p>Loading…</p>;

  return (
    <div className="page-card">
      <Link className="back-link" to={isEdit ? `/app-users/${id}` : '/app-users'}>
        ← {isEdit ? 'Back to user' : 'Back to users'}
      </Link>
      <h2>{isEdit ? 'Edit App User' : 'Add App User'}</h2>
      <form onSubmit={handleSubmit}>
        <div className="field">
          <label htmlFor="contact_name">Contact Name</label>
          <input id="contact_name" value={form.contact_name} onChange={set('contact_name')} placeholder="Jane Doe" />
        </div>
        <div className="field">
          <label htmlFor="email">Email</label>
          <input id="email" type="email" value={form.email} onChange={set('email')} placeholder="jane@example.com" />
        </div>
        <div className="field">
          <label htmlFor="phone">Phone</label>
          <input id="phone" value={form.phone} onChange={set('phone')} placeholder="555-0100" />
        </div>
        <div className="field">
          <label htmlFor="role">Role *</label>
          <select id="role" value={form.role} onChange={set('role')} required>
            <option value="Admin">Admin</option>
            <option value="Manager">Manager</option>
            <option value="Customer">Customer</option>
          </select>
        </div>
        <div className="field">
          <label htmlFor="notes">Notes</label>
          <textarea id="notes" value={form.notes} onChange={set('notes')} />
        </div>
        {error && <p className="error-msg">{error}</p>}
        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving}>
            {saving ? 'Saving…' : isEdit ? 'Save Changes' : 'Add User'}
          </button>
          <button type="button" className="btn-secondary"
            onClick={() => navigate(isEdit ? `/app-users/${id}` : '/app-users')}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

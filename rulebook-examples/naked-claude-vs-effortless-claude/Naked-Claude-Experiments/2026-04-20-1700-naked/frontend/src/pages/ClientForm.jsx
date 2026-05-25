import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchClient, fetchStatuses, fetchClientCategories, createClient, updateClient } from '../api.js';
import StatusBadge from '../components/StatusBadge.jsx';

const pct = (n) => `${(Number(n) * 100).toFixed(0)}%`;

export default function ClientForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [form, setForm] = useState({
    name: '', notes: '', status_id: '', category_id: '',
    company_name: '', email: '', phone: '',
    billing_address: '', shipping_address: '',
  });
  const [statuses, setStatuses]     = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading]   = useState(true);
  const [saving, setSaving]     = useState(false);
  const [error, setError]       = useState('');

  useEffect(() => {
    const loads = [fetchStatuses(), fetchClientCategories()];
    if (isEdit) loads.push(fetchClient(id));
    Promise.all(loads)
      .then(([sts, cats, client]) => {
        setStatuses(sts);
        setCategories(cats);
        if (client) {
          setForm({
            name: client.name || '',
            notes: client.notes || '',
            status_id: client.status_id ? String(client.status_id) : '',
            category_id: client.category_id ? String(client.category_id) : '',
            company_name: client.company_name || '',
            email: client.email || '',
            phone: client.phone || '',
            billing_address: client.billing_address || '',
            shipping_address: client.shipping_address || '',
          });
        }
      })
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const set = (k) => (e) => setForm((f) => ({ ...f, [k]: e.target.value }));

  const selectedStatus = statuses.find((s) => String(s.id) === form.status_id) || null;
  const willBeStopped  = selectedStatus?.is_blocking ?? false;

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.name.trim()) { setError('Name is required.'); return; }
    setSaving(true);
    setError('');
    try {
      const payload = {
        ...form,
        status_id: form.status_id ? parseInt(form.status_id, 10) : null,
        category_id: form.category_id ? parseInt(form.category_id, 10) : null,
      };
      if (isEdit) {
        await updateClient(id, payload);
      } else {
        await createClient(payload);
      }
      navigate(isEdit ? `/clients/${id}` : '/');
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <p>Loading…</p>;

  return (
    <div className="page-card">
      <Link className="back-link" to={isEdit ? `/clients/${id}` : '/'}>
        ← {isEdit ? 'Back to client' : 'Back to list'}
      </Link>
      <h2>{isEdit ? 'Edit Client' : 'Add Client'}</h2>
      {willBeStopped && (
        <div className="stopped-banner" style={{ marginBottom: 20 }}>
          ⛔ This client will be marked as STOPPED (status is blocking)
        </div>
      )}
      <form onSubmit={handleSubmit}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 20px' }}>
          <div className="field">
            <label>Name *</label>
            <input value={form.name} onChange={set('name')} placeholder="e.g. Alice Johnson" required />
          </div>
          <div className="field">
            <label>Company Name</label>
            <input value={form.company_name} onChange={set('company_name')} placeholder="e.g. AJ Consulting" />
          </div>
          <div className="field">
            <label>Email</label>
            <input type="email" value={form.email} onChange={set('email')} placeholder="alice@example.com" />
          </div>
          <div className="field">
            <label>Phone</label>
            <input value={form.phone} onChange={set('phone')} placeholder="555-000-0000" />
          </div>
          <div className="field">
            <label>Billing Address</label>
            <textarea value={form.billing_address} onChange={set('billing_address')} rows={2} placeholder="123 Main St…" />
          </div>
          <div className="field">
            <label>Shipping Address</label>
            <textarea value={form.shipping_address} onChange={set('shipping_address')} rows={2} placeholder="123 Main St…" />
          </div>
        </div>
        <div className="field">
          <label>Status</label>
          <select value={form.status_id} onChange={set('status_id')}>
            <option value="">— No status —</option>
            {statuses.map((s) => (
              <option key={s.id} value={String(s.id)}>
                {s.name}{s.is_blocking ? ' (blocking)' : ''}
              </option>
            ))}
          </select>
          {selectedStatus && (
            <div className="status-hint">
              <StatusBadge name={selectedStatus.name} isBlocking={selectedStatus.is_blocking} />
              <span className="status-desc">{selectedStatus.description}</span>
            </div>
          )}
        </div>
        <div className="field">
          <label>Category</label>
          <select value={form.category_id} onChange={set('category_id')}>
            <option value="">— No category —</option>
            {categories.map((cat) => (
              <option key={cat.id} value={String(cat.id)}>
                {cat.name} ({pct(cat.discount_percent)} discount)
              </option>
            ))}
          </select>
          <span style={{ fontSize: 13, color: '#666', marginTop: 4 }}>
            Category controls the per-line discount on every invoice for this client.
          </span>
        </div>
        <div className="field">
          <label>Notes</label>
          <textarea value={form.notes} onChange={set('notes')} placeholder="Any notes about this client…" />
        </div>
        {error && <p className="error-msg">{error}</p>}
        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving}>
            {saving ? 'Saving…' : isEdit ? 'Save Changes' : 'Add Client'}
          </button>
          <button type="button" className="btn-secondary"
            onClick={() => navigate(isEdit ? `/clients/${id}` : '/')}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

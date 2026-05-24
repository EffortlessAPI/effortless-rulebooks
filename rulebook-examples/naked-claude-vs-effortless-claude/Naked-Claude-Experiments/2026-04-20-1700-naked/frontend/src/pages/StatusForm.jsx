import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchStatus, createStatus, updateStatus } from '../api.js';

export default function StatusForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [name, setName]             = useState('');
  const [description, setDesc]      = useState('');
  const [isBlocking, setIsBlocking] = useState(false);
  const [sortOrder, setSortOrder]   = useState(0);
  const [loading, setLoading]       = useState(isEdit);
  const [saving, setSaving]         = useState(false);
  const [error, setError]           = useState('');

  useEffect(() => {
    if (!isEdit) return;
    fetchStatus(id)
      .then((s) => {
        setName(s.name);
        setDesc(s.description);
        setIsBlocking(s.is_blocking);
        setSortOrder(s.sort_order ?? 0);
      })
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const derivedSlug = name
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '');

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!name.trim()) { setError('Name is required.'); return; }
    setSaving(true);
    setError('');
    try {
      const payload = { name, description, is_blocking: isBlocking, sort_order: sortOrder };
      if (isEdit) {
        await updateStatus(id, payload);
        navigate(`/statuses/${id}`);
      } else {
        const created = await createStatus(payload);
        navigate(`/statuses/${created.id}`);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <p>Loading…</p>;

  return (
    <div className="page-card">
      <Link className="back-link" to={isEdit ? `/statuses/${id}` : '/statuses'}>
        ← {isEdit ? 'Back to status' : 'Back to statuses'}
      </Link>

      <h2>{isEdit ? 'Edit Status' : 'Add Status'}</h2>

      <form onSubmit={handleSubmit}>
        <div className="field">
          <label htmlFor="name">Display Name *</label>
          <input
            id="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="e.g. On-Hold"
            required
          />
          {derivedSlug && (
            <span style={{ fontSize: 12, color: '#888', marginTop: 4 }}>
              Internal handle: <code>{derivedSlug}</code>
            </span>
          )}
        </div>

        <div className="field">
          <label htmlFor="sort_order">Sort Order</label>
          <input
            id="sort_order"
            type="number"
            value={sortOrder}
            onChange={(e) => setSortOrder(parseInt(e.target.value, 10) || 0)}
            placeholder="0"
            style={{ width: 120 }}
          />
        </div>

        <div className="field">
          <label htmlFor="description">Description</label>
          <textarea
            id="description"
            value={description}
            onChange={(e) => setDesc(e.target.value)}
            placeholder="What does this status mean?"
          />
        </div>

        <div className="field">
          <label className="checkbox-label">
            <input
              type="checkbox"
              checked={isBlocking}
              onChange={(e) => setIsBlocking(e.target.checked)}
              style={{ width: 'auto', marginRight: 8 }}
            />
            Blocking status
          </label>
          <span style={{ fontSize: 13, color: '#666', marginTop: 4 }}>
            {isBlocking
              ? '⛔ Clients in this status will be marked as stopped.'
              : '✅ Clients in this status will NOT be marked as stopped.'}
          </span>
        </div>

        {error && <p className="error-msg">{error}</p>}

        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving}>
            {saving ? 'Saving…' : isEdit ? 'Save Changes' : 'Add Status'}
          </button>
          <button
            type="button"
            className="btn-secondary"
            onClick={() => navigate(isEdit ? `/statuses/${id}` : '/statuses')}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

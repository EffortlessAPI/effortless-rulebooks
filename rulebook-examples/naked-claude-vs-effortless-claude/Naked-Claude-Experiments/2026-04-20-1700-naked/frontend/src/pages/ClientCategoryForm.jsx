import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchClientCategory, createClientCategory, updateClientCategory } from '../api.js';

export default function ClientCategoryForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [name, setName]           = useState('');
  const [notes, setNotes]         = useState('');
  const [discountPct, setDiscount] = useState('0');  // stored as percent (e.g. "5" for 5%)
  const [loading, setLoading]     = useState(isEdit);
  const [saving, setSaving]       = useState(false);
  const [error, setError]         = useState('');

  useEffect(() => {
    if (!isEdit) return;
    fetchClientCategory(id)
      .then((c) => {
        setName(c.name);
        setNotes(c.notes || '');
        setDiscount(String(Number(c.discount_percent) * 100));
      })
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!name.trim()) { setError('Name is required.'); return; }
    setSaving(true);
    setError('');
    try {
      const payload = {
        name,
        notes,
        discount_percent: (parseFloat(discountPct) || 0) / 100,
      };
      if (isEdit) {
        await updateClientCategory(id, payload);
        navigate(`/client-categories/${id}`);
      } else {
        const created = await createClientCategory(payload);
        navigate(`/client-categories/${created.id}`);
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
      <Link className="back-link" to={isEdit ? `/client-categories/${id}` : '/client-categories'}>
        ← {isEdit ? 'Back to category' : 'Back to categories'}
      </Link>

      <h2>{isEdit ? 'Edit Client Category' : 'Add Client Category'}</h2>

      <form onSubmit={handleSubmit}>
        <div className="field">
          <label htmlFor="name">Name *</label>
          <input
            id="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="e.g. Gold"
            required
          />
        </div>

        <div className="field">
          <label htmlFor="discount">Discount %</label>
          <input
            id="discount"
            type="number"
            min="0"
            max="100"
            step="0.1"
            value={discountPct}
            onChange={(e) => setDiscount(e.target.value)}
            style={{ width: 120 }}
          />
          <span style={{ fontSize: 13, color: '#666', marginTop: 4 }}>
            Applied to every line item on invoices belonging to clients in this category.
          </span>
        </div>

        <div className="field">
          <label htmlFor="notes">Notes</label>
          <textarea
            id="notes"
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            placeholder="What distinguishes clients in this category?"
          />
        </div>

        {error && <p className="error-msg">{error}</p>}

        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving}>
            {saving ? 'Saving…' : isEdit ? 'Save Changes' : 'Add Category'}
          </button>
          <button
            type="button"
            className="btn-secondary"
            onClick={() => navigate(isEdit ? `/client-categories/${id}` : '/client-categories')}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

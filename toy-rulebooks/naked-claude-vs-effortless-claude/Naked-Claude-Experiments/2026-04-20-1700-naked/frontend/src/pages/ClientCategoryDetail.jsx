import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchClientCategory, deleteClientCategory } from '../api.js';

const pct = (n) => `${(Number(n) * 100).toFixed(0)}%`;

export default function ClientCategoryDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [cat, setCat] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState('');

  useEffect(() => {
    fetchClientCategory(id)
      .then(setCat)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id]);

  const handleDelete = async () => {
    if ((cat.clients || []).length > 0) {
      alert(
        `Cannot delete "${cat.name}" — ${cat.clients.length} client(s) are assigned to it. ` +
        `Reassign them first.`
      );
      return;
    }
    if (!window.confirm(`Delete category "${cat.name}"?`)) return;
    try {
      await deleteClientCategory(id);
      navigate('/client-categories');
    } catch (err) {
      alert(err.message);
    }
  };

  if (loading) return <p>Loading…</p>;
  if (error)   return <p className="error-msg">{error}</p>;
  if (!cat) return null;

  return (
    <div className="page-card">
      <Link className="back-link" to="/client-categories">← Back to categories</Link>

      <h2>
        {cat.name}
        <span className="tag" style={{ marginLeft: 10 }}>{pct(cat.discount_percent)} discount</span>
      </h2>

      <div className="detail-field">
        <div className="field-label">Discount</div>
        <div className="field-value" style={{ fontWeight: 600 }}>
          {pct(cat.discount_percent)}
          <span style={{ color: '#888', fontWeight: 400, marginLeft: 8, fontSize: 13 }}>
            applied to every line item on this category's clients' invoices
          </span>
        </div>
      </div>

      <div className="detail-field">
        <div className="field-label">Notes</div>
        <div className="field-value">{cat.notes || <em style={{ color: '#aaa' }}>—</em>}</div>
      </div>

      <div className="detail-field">
        <div className="field-label">Slug</div>
        <div className="field-value" style={{ fontFamily: 'monospace', color: '#888' }}>{cat.slug}</div>
      </div>

      <div className="detail-field">
        <div className="field-label">
          Clients in this category ({(cat.clients || []).length})
        </div>
        <div className="field-value">
          {(cat.clients || []).length === 0 ? (
            <em style={{ color: '#aaa' }}>No clients assigned</em>
          ) : (
            <ul className="status-customer-list">
              {cat.clients.map((c) => (
                <li key={c.id}>
                  <Link to={`/clients/${c.id}`} className="back-link" style={{ fontSize: 14 }}>
                    {c.name}
                  </Link>
                  {c.company_name && <span style={{ color: '#888', marginLeft: 8, fontSize: 13 }}>— {c.company_name}</span>}
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>

      <div className="form-actions">
        <button className="btn-primary" onClick={() => navigate(`/client-categories/${id}/edit`)}>
          Edit
        </button>
        <button className="btn-danger" onClick={handleDelete}>
          Delete
        </button>
        <button className="btn-secondary" onClick={() => navigate('/client-categories')}>
          Cancel
        </button>
      </div>
    </div>
  );
}

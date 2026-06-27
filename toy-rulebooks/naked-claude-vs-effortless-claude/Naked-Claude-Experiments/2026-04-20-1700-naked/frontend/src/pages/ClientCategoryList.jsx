import { useState, useEffect, useCallback } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { fetchClientCategories, deleteClientCategory } from '../api.js';

const pct = (n) => `${(Number(n) * 100).toFixed(0)}%`;

export default function ClientCategoryList() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading]       = useState(true);
  const navigate = useNavigate();

  const load = useCallback(async () => {
    setLoading(true);
    try {
      setCategories(await fetchClientCategories());
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async (cat) => {
    if (cat.client_count > 0) {
      alert(
        `Cannot delete "${cat.name}" — ${cat.client_count} client(s) are assigned to it. ` +
        `Reassign them first.`
      );
      return;
    }
    if (!window.confirm(`Delete category "${cat.name}"?`)) return;
    try {
      await deleteClientCategory(cat.id);
      load();
    } catch (err) {
      alert(err.message);
    }
  };

  return (
    <>
      <div className="list-header">
        <h2>Client Categories {!loading && `(${categories.length})`}</h2>
        <button className="btn-primary" onClick={() => navigate('/client-categories/new')}>
          + Add Category
        </button>
      </div>

      {loading ? (
        <p>Loading categories…</p>
      ) : categories.length === 0 ? (
        <p style={{ color: '#888' }}>No categories yet. Add one!</p>
      ) : (
        <div className="customer-list">
          {categories.map((cat) => (
            <div key={cat.id} className="customer-row status-row">
              <div className="status-main">
                <Link className="customer-name-link" to={`/client-categories/${cat.id}`}>
                  {cat.name}
                </Link>
                <span className="tag">{pct(cat.discount_percent)} discount</span>
              </div>
              {cat.notes && <span className="customer-notes-preview">{cat.notes}</span>}
              <span className="status-cust-count">
                {cat.client_count} {cat.client_count === 1 ? 'client' : 'clients'}
              </span>
              <div className="row-actions">
                <button className="btn-secondary btn-sm" onClick={() => navigate(`/client-categories/${cat.id}/edit`)}>
                  Edit
                </button>
                <button className="btn-danger btn-sm" onClick={() => handleDelete(cat)}>
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </>
  );
}

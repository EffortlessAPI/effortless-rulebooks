import { useEffect, useState } from 'react';
import { Link, useParams, useNavigate } from 'react-router-dom';
import { api } from '../api.js';
import { swatchColor } from '../colorStyle.js';

export default function CustomerDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [c, setC] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    api.get(id).then(setC).catch(e => setError(e.message));
  }, [id]);

  async function onDelete() {
    if (!confirm(`Delete ${c.name}?`)) return;
    await api.remove(id);
    navigate('/');
  }

  if (error) return <div className="error">Error: {error}</div>;
  if (!c) return <div>Loading…</div>;

  return (
    <div className={c.is_stopped ? 'detail stopped' : 'detail'}>
      <Link to="/">← Back</Link>
      <h2>
        <span className="swatch" style={{ background: swatchColor(c.color) }} />
        {c.name}
        {c.is_stopped && <span className="badge">STOPPED</span>}
      </h2>
      <dl>
        <dt>Slug</dt><dd>{c.slug}</dd>
        <dt>Color</dt><dd>{c.color || <em>(none)</em>}</dd>
        <dt>Stopped</dt><dd>{c.is_stopped ? 'Yes' : 'No'}</dd>
        <dt>Notes</dt><dd>{c.notes || <em>(none)</em>}</dd>
      </dl>
      <div className="actions">
        <Link to={`/customers/${c.id}/edit`} className="btn">Edit</Link>
        <button className="btn danger" onClick={onDelete}>Delete</button>
      </div>
    </div>
  );
}

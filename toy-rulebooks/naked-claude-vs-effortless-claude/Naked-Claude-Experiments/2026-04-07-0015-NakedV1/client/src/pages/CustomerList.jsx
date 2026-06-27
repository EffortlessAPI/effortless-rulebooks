import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../api.js';
import { swatchColor } from '../colorStyle.js';

export default function CustomerList() {
  const [customers, setCustomers] = useState(null);
  const [summary, setSummary] = useState(null);
  const [error, setError] = useState(null);

  async function load() {
    try {
      const [c, s] = await Promise.all([api.list(), api.summary()]);
      setCustomers(c);
      setSummary(s);
    } catch (e) {
      setError(e.message);
    }
  }

  useEffect(() => { load(); }, []);

  if (error) return <div className="error">Error: {error}</div>;
  if (!customers || !summary) return <div>Loading…</div>;

  return (
    <div>
      <section className="summary">
        <div className="summary-row">
          <div><strong>{summary.total}</strong> total</div>
          <div><strong>{summary.stopped}</strong> stopped</div>
        </div>
        <div className="summary-colors">
          {Object.entries(summary.colors).map(([color, count]) => (
            <span key={color} className="color-chip">
              <span className="swatch" style={{ background: swatchColor(color) }} />
              {color}: {count}
            </span>
          ))}
        </div>
      </section>

      <div className="list-header">
        <h2>Customers</h2>
        <Link to="/customers/new" className="btn">+ New</Link>
      </div>

      <ul className="customer-list">
        {customers.map(c => (
          <li key={c.id} className={c.is_stopped ? 'customer stopped' : 'customer'}>
            <Link to={`/customers/${c.id}`} className="customer-link">
              <span className="swatch" style={{ background: swatchColor(c.color) }} />
              <span className="name">{c.name}</span>
              {c.is_stopped && <span className="badge">STOPPED</span>}
              <span className="notes">{c.notes}</span>
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
}

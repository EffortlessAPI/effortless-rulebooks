import { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { fetchClients, fetchSummary, deleteClient } from '../api.js';
import StatusBadge from '../components/StatusBadge.jsx';

function fmt(n) {
  return Number(n ?? 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' });
}

function Dashboard({ summary }) {
  if (!summary) return <div className="summary-card">Loading…</div>;
  return (
    <div className="summary-card">
      <div className="summary-stat">
        <span className="value">{summary.total}</span>
        <span className="label">Clients</span>
      </div>
      <div className="summary-stat">
        <span className="value stopped-count">{summary.stopped}</span>
        <span className="label">Stopped</span>
      </div>
      <div className="summary-divider" />
      <div className="summary-stat">
        <span className="value">{summary.total_invoices}</span>
        <span className="label">Invoices</span>
      </div>
      <div className="summary-stat">
        <span className="value">{summary.total_products}</span>
        <span className="label">Products</span>
      </div>
      <div className="summary-stat">
        <span className="value">{summary.active_products}</span>
        <span className="label">Active</span>
      </div>
      <div className="summary-divider" />
      <div className="summary-stat">
        <span className="value money-val">{fmt(summary.total_revenue)}</span>
        <span className="label">Revenue</span>
      </div>
      <div className="summary-stat">
        <span className="value outstanding-val">{fmt(summary.outstanding)}</span>
        <span className="label">Outstanding</span>
      </div>
      <div className="summary-divider" />
      <div className="summary-stat">
        <span className="value">{summary.total_users ?? 0}</span>
        <span className="label">Users</span>
      </div>
      <div className="summary-stat">
        <span className="value" style={{ color: '#27ae60' }}>{summary.approved_count ?? 0}</span>
        <span className="label">Approved</span>
      </div>
      <div className="summary-stat">
        <span className="value" style={{ color: '#e67e22' }}>{summary.pending_count ?? 0}</span>
        <span className="label">Pending</span>
      </div>
      <div className="summary-divider" />
      <div className="summary-stat">
        <span className="value">{summary.total_on_hand ?? 0}</span>
        <span className="label">On Hand</span>
      </div>
      <div className="summary-stat">
        <span className="value">{summary.total_adjustments ?? 0}</span>
        <span className="label">Adjustments</span>
      </div>
      <div className="summary-divider" />
      <div className="dist-section">
        <div className="dist-label">Invoices by status</div>
        <div className="dist-chips">
          {(summary.invoices_by_status || []).map((s) => (
            <div className="dist-chip" key={s.order_status}>
              <span className="dist-name">{s.order_status}</span>
              <span className="dist-count">{s.cnt}</span>
            </div>
          ))}
        </div>
      </div>
      <div className="dist-section">
        <div className="dist-label">Payments by status</div>
        <div className="dist-chips">
          {(summary.payments_by_status || []).map((s) => (
            <div className="dist-chip" key={s.payment_status}>
              <span className="dist-name">{s.payment_status}</span>
              <span className="dist-count">{s.cnt}</span>
              {s.payment_status === 'Completed' && (
                <span className="dist-money">{fmt(s.total_amount)}</span>
              )}
            </div>
          ))}
        </div>
      </div>
      <div className="summary-divider" />
      <div className="dist-section">
        <div className="dist-label">Client status</div>
        <div className="dist-chips">
          {(summary.status_distribution || []).map((s) => (
            <div className="dist-chip" key={s.id} title={s.is_blocking ? 'Blocking' : 'Non-blocking'}>
              <span className={`dist-dot ${s.is_blocking ? 'blocking' : 'nonblocking'}`} />
              <span className="dist-name">{s.name}</span>
              <span className="dist-count">{s.client_count}</span>
            </div>
          ))}
        </div>
      </div>
      <div className="dist-section">
        <div className="dist-label">Client categories</div>
        <div className="dist-chips">
          {(summary.category_distribution || []).map((c) => (
            <div className="dist-chip" key={c.id}>
              <span className="dist-name">{c.name}</span>
              <span className="dist-count">{c.client_count}</span>
              <span className="dist-money">{(Number(c.discount_percent) * 100).toFixed(0)}%</span>
            </div>
          ))}
        </div>
      </div>
      <div className="dist-section">
        <div className="dist-label">Users by role</div>
        <div className="dist-chips">
          {(summary.users_by_role || []).map((u) => (
            <div className="dist-chip" key={u.role}>
              <span className="dist-name">{u.role}</span>
              <span className="dist-count">{u.cnt}</span>
            </div>
          ))}
        </div>
      </div>
      <div className="dist-section">
        <div className="dist-label">Inventory activity</div>
        <div className="dist-chips">
          <div className="dist-chip">
            <span className="dist-name">Added</span>
            <span className="dist-count" style={{ color: '#262' }}>+{summary.total_added ?? 0}</span>
          </div>
          <div className="dist-chip">
            <span className="dist-name">Removed</span>
            <span className="dist-count" style={{ color: '#c33' }}>−{summary.total_removed ?? 0}</span>
          </div>
          <div className="dist-chip">
            <span className="dist-name">Corrected</span>
            <span className="dist-count">{summary.total_corrected ?? 0}</span>
          </div>
        </div>
      </div>
    </div>
  );
}

export default function ClientList() {
  const [clients, setClients]  = useState([]);
  const [summary, setSummary]  = useState(null);
  const [loading, setLoading]  = useState(true);
  const navigate = useNavigate();

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [c, s] = await Promise.all([fetchClients(), fetchSummary()]);
      setClients(c);
      setSummary(s);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async (id, name) => {
    if (!window.confirm(`Delete "${name}"? This cannot be undone.`)) return;
    try {
      await deleteClient(id);
      load();
    } catch (err) {
      alert(err.message);
    }
  };

  return (
    <>
      <Dashboard summary={summary} />

      <div className="list-header">
        <h2>Clients {!loading && `(${clients.length})`}</h2>
        <button className="btn-primary" onClick={() => navigate('/clients/new')}>
          + Add Client
        </button>
      </div>

      {loading ? (
        <p>Loading clients…</p>
      ) : clients.length === 0 ? (
        <p style={{ color: '#888' }}>No clients yet. Add one!</p>
      ) : (
        <div className="customer-list">
          {clients.map((c) => (
            <div key={c.id} className={`customer-row ${c.is_stopped ? 'stopped' : 'not-stopped'}`}>
              <div className="customer-main">
                <Link className="customer-name-link" to={`/clients/${c.id}`}>
                  {c.name}
                </Link>
                {c.is_stopped && <span className="stopped-badge">⛔ STOPPED</span>}
                {c.status_name
                  ? <StatusBadge name={c.status_name} isBlocking={c.status_is_blocking} />
                  : <span className="no-status-badge">No status</span>
                }
                {c.category_name && (
                  <span className="tag">
                    {c.category_name} · {(Number(c.category_discount) * 100).toFixed(0)}%
                  </span>
                )}
                {c.company_name && <span className="customer-company">{c.company_name}</span>}
              </div>
              <div className="customer-contact">
                {c.email && <span>{c.email}</span>}
                {c.phone && <span>{c.phone}</span>}
              </div>
              {c.notes && <span className="customer-notes-preview">{c.notes}</span>}
              <div className="row-actions">
                <button className="btn-secondary btn-sm" onClick={() => navigate(`/clients/${c.id}/edit`)}>
                  Edit
                </button>
                <button className="btn-danger btn-sm" onClick={() => handleDelete(c.id, c.name)}>
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

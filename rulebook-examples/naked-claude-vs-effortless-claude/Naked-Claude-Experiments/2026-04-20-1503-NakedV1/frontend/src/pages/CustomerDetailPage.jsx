import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';

const COLOR_SWATCHES = {
  Green:  '#22c55e',
  Red:    '#ef4444',
  Yellow: '#eab308',
  Blue:   '#3b82f6',
};

export default function CustomerDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [customer, setCustomer] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetch(`/api/customers/${id}`)
      .then(r => r.ok ? r.json() : Promise.reject('Not found'))
      .then(setCustomer)
      .catch(err => setError(typeof err === 'string' ? err : err.message))
      .finally(() => setLoading(false));
  }, [id]);

  async function handleDelete() {
    if (!window.confirm(`Delete "${customer.name}"? This cannot be undone.`)) return;
    await fetch(`/api/customers/${id}`, { method: 'DELETE' });
    navigate('/');
  }

  if (loading) return <div style={{ color: '#6b7280', padding: 40 }}>Loading…</div>;
  if (error)   return <div style={{ color: '#b91c1c', padding: 14 }}>{error}</div>;
  if (!customer) return null;

  const { name, slug, color, notes, stopped } = customer;
  const dotColor = COLOR_SWATCHES[color] || '#9ca3af';

  return (
    <div>
      {/* Breadcrumb */}
      <div style={{ marginBottom: 20, fontSize: 14, color: '#6b7280' }}>
        <Link to="/" style={{ color: '#4f46e5' }}>← All Customers</Link>
      </div>

      <div style={{
        background: stopped ? '#f0fdf4' : '#fff',
        border: stopped ? '2px solid #86efac' : '1px solid #e5e7eb',
        borderRadius: 12,
        padding: 28,
        boxShadow: '0 2px 8px rgba(0,0,0,0.07)',
      }}>
        {/* Stopped badge */}
        {stopped && (
          <div style={{
            display: 'inline-block',
            background: '#22c55e', color: '#fff',
            fontSize: 12, fontWeight: 700,
            padding: '3px 12px', borderRadius: 6,
            letterSpacing: '0.06em', textTransform: 'uppercase',
            marginBottom: 14,
          }}>STOPPED</div>
        )}

        <div style={{ display: 'flex', alignItems: 'flex-start', gap: 16, flexWrap: 'wrap' }}>
          <div style={{
            width: 48, height: 48, borderRadius: '50%',
            background: dotColor, border: '3px solid rgba(0,0,0,0.1)',
            flexShrink: 0, marginTop: 4,
          }} />
          <div style={{ flex: 1 }}>
            <h2 style={{ fontSize: 28, fontWeight: 800, color: stopped ? '#15803d' : '#1a1a2e' }}>{name}</h2>
            <div style={{ fontSize: 13, color: '#9ca3af', marginTop: 2, marginBottom: 10 }}>slug: <code>{slug}</code></div>

            {/* Color */}
            <div style={{ marginBottom: 16 }}>
              <span style={{ fontSize: 13, color: '#6b7280', fontWeight: 600 }}>Color: </span>
              <span style={{
                background: dotColor + '22',
                color: dotColor,
                border: `1px solid ${dotColor}55`,
                borderRadius: 6, padding: '3px 10px',
                fontSize: 14, fontWeight: 600,
              }}>{color || <em>none</em>}</span>
            </div>

            {/* Notes */}
            {notes ? (
              <div style={{
                background: '#f9fafb', borderRadius: 8,
                padding: '12px 16px', fontSize: 15, color: '#374151',
                borderLeft: '4px solid #e5e7eb', marginBottom: 16,
              }}>{notes}</div>
            ) : (
              <div style={{ color: '#9ca3af', fontSize: 14, marginBottom: 16 }}>No notes.</div>
            )}

            {/* Stopped explanation */}
            <div style={{
              fontSize: 13, color: '#6b7280',
              background: '#f3f4f6', borderRadius: 6,
              padding: '6px 12px', display: 'inline-block', marginBottom: 20,
            }}>
              <strong>Stopped:</strong> {stopped ? 'Yes (color is Green)' : 'No'}
            </div>
          </div>
        </div>

        {/* Actions */}
        <div style={{ display: 'flex', gap: 10, marginTop: 8 }}>
          <Link to={`/customers/${id}/edit`}>
            <button style={{
              background: '#4f46e5', color: '#fff',
              border: 'none', borderRadius: 8,
              padding: '9px 20px', fontWeight: 600, fontSize: 14,
            }}>Edit</button>
          </Link>
          <button
            onClick={handleDelete}
            style={{
              background: '#fef2f2', color: '#b91c1c',
              border: '1px solid #fecaca', borderRadius: 8,
              padding: '9px 20px', fontWeight: 600, fontSize: 14,
            }}
          >Delete</button>
        </div>
      </div>
    </div>
  );
}

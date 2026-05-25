import React from 'react';
import { Link } from 'react-router-dom';

const COLOR_SWATCHES = {
  Green:  '#22c55e',
  Red:    '#ef4444',
  Yellow: '#eab308',
  Blue:   '#3b82f6',
};

function getColorBg(color) {
  return COLOR_SWATCHES[color] || '#9ca3af';
}

export default function CustomerCard({ customer, onDelete }) {
  const { id, name, color, notes, stopped } = customer;
  const dotColor = getColorBg(color);

  return (
    <div style={{
      background: stopped ? '#f0fdf4' : '#fff',
      border: stopped ? '2px solid #86efac' : '1px solid #e5e7eb',
      borderRadius: 10,
      padding: '16px 20px',
      display: 'flex',
      alignItems: 'center',
      gap: 16,
      boxShadow: stopped
        ? '0 0 0 1px #86efac, 0 2px 6px rgba(34,197,94,0.08)'
        : '0 1px 3px rgba(0,0,0,0.05)',
      position: 'relative',
      transition: 'box-shadow 0.15s',
    }}>
      {/* Stopped banner */}
      {stopped && (
        <span style={{
          position: 'absolute', top: -1, right: 12,
          background: '#22c55e', color: '#fff',
          fontSize: 11, fontWeight: 700,
          padding: '2px 10px',
          borderRadius: '0 0 6px 6px',
          letterSpacing: '0.06em',
          textTransform: 'uppercase',
        }}>
          STOPPED
        </span>
      )}

      {/* Color dot */}
      <div style={{
        width: 18, height: 18, borderRadius: '50%',
        background: dotColor, flexShrink: 0,
        border: '2px solid rgba(0,0,0,0.08)',
      }} title={color} />

      {/* Name + notes */}
      <div style={{ flex: 1, minWidth: 0 }}>
        <Link to={`/customers/${id}`} style={{
          fontWeight: 700, fontSize: 16,
          color: stopped ? '#15803d' : '#1a1a2e',
        }}>
          {name}
        </Link>
        {notes && (
          <div style={{
            fontSize: 13, color: '#6b7280', marginTop: 2,
            overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap',
          }}>{notes}</div>
        )}
      </div>

      {/* Color badge */}
      <span style={{
        background: dotColor + '22',
        color: dotColor === '#eab308' ? '#92400e' : dotColor,
        border: `1px solid ${dotColor}44`,
        borderRadius: 6, padding: '3px 10px',
        fontSize: 13, fontWeight: 600, flexShrink: 0,
      }}>{color || <em style={{ color: '#9ca3af' }}>no color</em>}</span>

      {/* Actions */}
      <div style={{ display: 'flex', gap: 8, flexShrink: 0 }}>
        <Link to={`/customers/${id}/edit`}>
          <button style={{
            background: '#f3f4f6', border: '1px solid #e5e7eb',
            borderRadius: 6, padding: '5px 12px',
            fontSize: 13, fontWeight: 500, color: '#374151',
          }}>Edit</button>
        </Link>
        <button
          onClick={() => onDelete(id, name)}
          style={{
            background: '#fef2f2', border: '1px solid #fecaca',
            borderRadius: 6, padding: '5px 12px',
            fontSize: 13, fontWeight: 500, color: '#b91c1c',
          }}>Delete</button>
      </div>
    </div>
  );
}

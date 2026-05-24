import { useState } from 'react';
import { colorStyle } from '../colorUtils.js';

export default function CustomerDetail({ customer, onEdit, onDelete, onBack }) {
  const [confirming, setConfirming] = useState(false);
  const [deleting,   setDeleting]   = useState(false);
  const [error,      setError]      = useState(null);

  if (!customer) {
    return <div style={styles.notFound}>Customer not found.</div>;
  }

  const { id, name, slug, notes, color, is_stopped } = customer;
  const cs = colorStyle(color);

  const handleDelete = async () => {
    setDeleting(true);
    try {
      await onDelete(id);
    } catch (e) {
      setError(e.message);
      setDeleting(false);
      setConfirming(false);
    }
  };

  return (
    <div style={styles.card}>
      {/* Color banner */}
      <div style={{ ...styles.banner, background: cs.bg }}>
        <div style={styles.bannerContent}>
          <span style={{ ...styles.colorLabel, color: cs.text }}>{color || 'No color'}</span>
          {is_stopped && (
            <span style={styles.stoppedBadge}>STOPPED</span>
          )}
        </div>
      </div>

      <div style={styles.body}>
        <h2 style={styles.name}>{name}</h2>
        <div style={styles.slug}>slug: {slug}</div>

        <div style={styles.field}>
          <span style={styles.fieldLabel}>Notes</span>
          <p style={styles.fieldValue}>{notes || <em style={{ color: '#aaa' }}>No notes yet.</em>}</p>
        </div>

        <div style={styles.field}>
          <span style={styles.fieldLabel}>Status</span>
          <p style={styles.fieldValue}>
            {is_stopped
              ? '🟢 Stopped — this customer is parked right now.'
              : '⚡ Active — this customer is not stopped.'}
          </p>
        </div>

        {error && <div style={styles.error}>{error}</div>}

        <div style={styles.actions}>
          <button style={styles.editBtn} onClick={onEdit}>Edit</button>

          {!confirming ? (
            <button style={styles.deleteBtn} onClick={() => setConfirming(true)}>
              Delete
            </button>
          ) : (
            <div style={styles.confirmRow}>
              <span style={styles.confirmText}>Are you sure?</span>
              <button
                style={styles.confirmYes}
                onClick={handleDelete}
                disabled={deleting}
              >
                {deleting ? 'Deleting…' : 'Yes, delete'}
              </button>
              <button style={styles.confirmNo} onClick={() => setConfirming(false)}>
                Cancel
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

const styles = {
  card: {
    background: '#fff',
    borderRadius: 12,
    overflow: 'hidden',
    boxShadow: '0 2px 8px rgba(0,0,0,0.12)',
    maxWidth: 600,
  },
  banner: {
    padding: '20px 24px',
  },
  bannerContent: {
    display: 'flex',
    alignItems: 'center',
    gap: 12,
  },
  colorLabel: {
    fontSize: 18,
    fontWeight: 700,
    letterSpacing: 0.5,
  },
  stoppedBadge: {
    background: 'rgba(255,255,255,0.35)',
    color: '#1a1a2e',
    fontSize: 11,
    fontWeight: 800,
    letterSpacing: 1.5,
    padding: '3px 10px',
    borderRadius: 12,
    textTransform: 'uppercase',
  },
  body: {
    padding: '24px',
  },
  name: {
    fontSize: 26,
    fontWeight: 800,
    color: '#1a1a2e',
    marginBottom: 4,
  },
  slug: {
    fontSize: 12,
    color: '#aaa',
    fontFamily: 'monospace',
    marginBottom: 20,
  },
  field: {
    marginBottom: 16,
  },
  fieldLabel: {
    display: 'block',
    fontSize: 11,
    fontWeight: 700,
    color: '#999',
    textTransform: 'uppercase',
    letterSpacing: 1,
    marginBottom: 4,
  },
  fieldValue: {
    fontSize: 15,
    color: '#333',
    lineHeight: 1.5,
  },
  error: {
    background: '#ffebee',
    color: '#c62828',
    borderRadius: 6,
    padding: '10px 14px',
    fontSize: 13,
    marginBottom: 16,
  },
  actions: {
    display: 'flex',
    gap: 10,
    alignItems: 'center',
    marginTop: 24,
    flexWrap: 'wrap',
  },
  editBtn: {
    background: '#2196F3',
    color: '#fff',
    border: 'none',
    borderRadius: 6,
    padding: '10px 22px',
    fontSize: 14,
    fontWeight: 600,
    cursor: 'pointer',
  },
  deleteBtn: {
    background: '#fff',
    color: '#f44336',
    border: '1px solid #f44336',
    borderRadius: 6,
    padding: '10px 22px',
    fontSize: 14,
    fontWeight: 600,
    cursor: 'pointer',
  },
  confirmRow: {
    display: 'flex',
    alignItems: 'center',
    gap: 8,
  },
  confirmText: {
    fontSize: 14,
    color: '#c62828',
    fontWeight: 600,
  },
  confirmYes: {
    background: '#f44336',
    color: '#fff',
    border: 'none',
    borderRadius: 6,
    padding: '8px 16px',
    fontSize: 13,
    fontWeight: 600,
    cursor: 'pointer',
  },
  confirmNo: {
    background: '#eee',
    color: '#333',
    border: 'none',
    borderRadius: 6,
    padding: '8px 16px',
    fontSize: 13,
    cursor: 'pointer',
  },
  notFound: {
    padding: 40,
    textAlign: 'center',
    color: '#888',
    fontSize: 16,
  },
};

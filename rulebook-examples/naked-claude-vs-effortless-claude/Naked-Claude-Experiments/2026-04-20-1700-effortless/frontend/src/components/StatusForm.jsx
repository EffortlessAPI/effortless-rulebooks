import { useState } from 'react';

const inputStyle = {
  width: '100%',
  padding: '8px 12px',
  borderRadius: '7px',
  border: '1px solid #d1d5db',
  fontSize: '0.95rem',
  outline: 'none',
  boxSizing: 'border-box'
};

const labelStyle = {
  display: 'block', fontSize: '0.85rem', fontWeight: 600, marginBottom: '5px', color: '#374151'
};

export default function StatusForm({ initial, onSave, onCancel, saving }) {
  const [displayName, setDisplayName] = useState(initial?.display_name || '');
  const [description, setDescription] = useState(initial?.description || '');
  const [isBlocking, setIsBlocking] = useState(
    initial?.is_blocking === true || initial?.is_blocking === 'true'
  );
  const [sortOrder, setSortOrder] = useState(initial?.sort_order ?? '');

  function handleSubmit(e) {
    e.preventDefault();
    if (!displayName.trim()) return;
    onSave({
      display_name: displayName.trim(),
      description: description.trim() || null,
      is_blocking: isBlocking,
      sort_order: sortOrder !== '' ? parseInt(sortOrder) : null
    });
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
      <div>
        <label style={labelStyle}>Display Name <span style={{ color: '#dc2626' }}>*</span></label>
        <input
          style={inputStyle}
          value={displayName}
          onChange={e => setDisplayName(e.target.value)}
          placeholder="e.g. On-Hold"
          required
          autoFocus
        />
        {displayName && (
          <p style={{ fontSize: '0.78rem', color: '#9ca3af', marginTop: '3px' }}>
            Internal ID: <code>{displayName.toLowerCase().trim().replace(/[^a-z0-9\s-]/g, '').replace(/\s+/g, '-').replace(/-+/g, '-')}</code>
          </p>
        )}
      </div>

      <div>
        <label style={labelStyle}>Description</label>
        <textarea
          style={{ ...inputStyle, minHeight: '80px', resize: 'vertical' }}
          value={description}
          onChange={e => setDescription(e.target.value)}
          placeholder="What does this status mean?"
        />
      </div>

      <div>
        <label style={{ display: 'flex', alignItems: 'center', gap: '10px', cursor: 'pointer' }}>
          <input
            type="checkbox"
            checked={isBlocking}
            onChange={e => setIsBlocking(e.target.checked)}
            style={{ width: '18px', height: '18px', cursor: 'pointer' }}
          />
          <span style={{ fontSize: '0.92rem', fontWeight: 600, color: '#374151' }}>
            Blocking status
          </span>
        </label>
        <p style={{ fontSize: '0.8rem', color: '#6b7280', marginTop: '4px', marginLeft: '28px' }}>
          {isBlocking
            ? '🔴 Clients in this status will be marked as Stopped'
            : '✓ Clients in this status will not be marked as Stopped'}
        </p>
      </div>

      <div>
        <label style={labelStyle}>Sort Order</label>
        <input
          style={inputStyle}
          type="number"
          value={sortOrder}
          onChange={e => setSortOrder(e.target.value)}
          placeholder="e.g. 1, 2, 3…"
          min="0"
        />
      </div>

      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end', marginTop: '4px' }}>
        {onCancel && (
          <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        )}
        <button type="submit" disabled={saving || !displayName.trim()} style={saveBtn(saving || !displayName.trim())}>
          {saving ? 'Saving…' : 'Save'}
        </button>
      </div>
    </form>
  );
}

const cancelBtn = {
  padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db',
  background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer'
};
function saveBtn(disabled) {
  return {
    padding: '8px 22px', borderRadius: '7px', border: 'none',
    background: disabled ? '#e5e7eb' : '#4f46e5',
    color: disabled ? '#9ca3af' : '#fff',
    fontSize: '0.9rem', fontWeight: 600, cursor: disabled ? 'not-allowed' : 'pointer'
  };
}

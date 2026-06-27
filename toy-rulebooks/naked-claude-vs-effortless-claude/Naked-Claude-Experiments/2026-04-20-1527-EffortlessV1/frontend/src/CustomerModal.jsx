import React, { useState } from 'react';
import { COLOR_OPTIONS } from './colorUtils';

function slugify(name) {
  return name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '');
}

export default function CustomerModal({ initial, onSave, onClose, existingIds = [] }) {
  const isEdit = !!initial;
  const [name, setName] = useState(initial?.name || '');
  const [notes, setNotes] = useState(initial?.notes || '');
  const [color, setColor] = useState(initial?.current_color || 'Red');
  const [customColor, setCustomColor] = useState(
    initial?.current_color && !COLOR_OPTIONS.includes(initial.current_color)
      ? initial.current_color
      : ''
  );
  const [useCustom, setUseCustom] = useState(
    initial?.current_color && !COLOR_OPTIONS.includes(initial.current_color)
  );
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  const effectiveColor = useCustom ? customColor : color;
  const derivedStopped = effectiveColor === 'Green';

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');
    if (!name.trim()) { setError('Name is required.'); return; }
    if (useCustom && !customColor.trim()) { setError('Custom color is required.'); return; }

    const customerId = isEdit ? initial.customer_id : slugify(name.trim()) || `cust-${Date.now()}`;
    if (!isEdit && existingIds.includes(customerId)) {
      setError(`Customer ID "${customerId}" already exists. Use a different name.`);
      return;
    }

    setSaving(true);
    try {
      await onSave({
        customer_id: customerId,
        name: name.trim(),
        notes: notes.trim(),
        current_color: effectiveColor
      });
    } catch (err) {
      setError(err.message);
      setSaving(false);
    }
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal" onClick={e => e.stopPropagation()}>
        <h2>{isEdit ? 'Edit Customer' : 'Add Customer'}</h2>
        {error && <div className="error-msg">{error}</div>}
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Name *</label>
            <input
              value={name}
              onChange={e => setName(e.target.value)}
              placeholder="e.g. Alice Johnson"
              autoFocus
            />
          </div>
          <div className="form-group">
            <label>Notes</label>
            <textarea
              value={notes}
              onChange={e => setNotes(e.target.value)}
              placeholder="Optional notes..."
            />
          </div>
          <div className="form-group">
            <label>Color *</label>
            {!useCustom ? (
              <select value={color} onChange={e => setColor(e.target.value)}>
                {COLOR_OPTIONS.map(c => (
                  <option key={c} value={c}>{c}</option>
                ))}
              </select>
            ) : (
              <input
                value={customColor}
                onChange={e => setCustomColor(e.target.value)}
                placeholder="Type a color..."
              />
            )}
            <div style={{ marginTop: 6 }}>
              <label style={{ display: 'inline-flex', alignItems: 'center', gap: 6, cursor: 'pointer', textTransform: 'none', letterSpacing: 0 }}>
                <input
                  type="checkbox"
                  checked={useCustom}
                  onChange={e => setUseCustom(e.target.checked)}
                />
                Use custom color
              </label>
            </div>
          </div>
          <div style={{ fontSize: '0.85rem', color: derivedStopped ? '#15803d' : '#6b7280', marginBottom: 16 }}>
            {derivedStopped
              ? '⏸ This customer will be marked as Stopped (color is Green).'
              : '▶ This customer is not stopped.'}
          </div>
          <div className="modal-actions">
            <button type="button" className="btn btn-secondary" onClick={onClose}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={saving}>
              {saving ? 'Saving…' : isEdit ? 'Save Changes' : 'Add Customer'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

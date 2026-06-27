import React, { useState } from 'react';

const PRESET_COLORS = ['Green', 'Red', 'Yellow', 'Blue'];

const COLOR_SWATCHES = {
  Green:  '#22c55e',
  Red:    '#ef4444',
  Yellow: '#eab308',
  Blue:   '#3b82f6',
};

export default function CustomerForm({ initial = {}, onSubmit, onCancel, submitLabel = 'Save' }) {
  const [name,  setName]  = useState(initial.name  || '');
  const [notes, setNotes] = useState(initial.notes || '');
  const [color, setColor] = useState(initial.color || '');
  const [customColor, setCustomColor] = useState(
    initial.color && !PRESET_COLORS.includes(initial.color) ? initial.color : ''
  );
  const [useCustom, setUseCustom] = useState(
    initial.color && !PRESET_COLORS.includes(initial.color)
  );
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const effectiveColor = useCustom ? customColor : color;
  const isStopped = effectiveColor === 'Green';

  function handlePresetClick(c) {
    setUseCustom(false);
    setColor(c);
  }

  function handleCustomToggle() {
    setUseCustom(true);
    setColor('');
  }

  async function handleSubmit(e) {
    e.preventDefault();
    if (!name.trim()) { setError('Name is required.'); return; }
    setSaving(true);
    setError('');
    try {
      await onSubmit({ name: name.trim(), notes, color: effectiveColor });
    } catch (err) {
      setError(err.message || 'Something went wrong.');
      setSaving(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
      {error && (
        <div style={{
          background: '#fef2f2', border: '1px solid #fecaca',
          borderRadius: 8, padding: '10px 14px',
          color: '#b91c1c', fontSize: 14,
        }}>{error}</div>
      )}

      {/* Name */}
      <div>
        <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, fontSize: 14 }}>
          Name <span style={{ color: '#ef4444' }}>*</span>
        </label>
        <input
          value={name}
          onChange={e => setName(e.target.value)}
          placeholder="e.g. Alice Johnson"
          style={{
            width: '100%', padding: '10px 12px',
            border: '1px solid #d1d5db', borderRadius: 8,
            fontSize: 15, outline: 'none',
          }}
          autoFocus
        />
      </div>

      {/* Notes */}
      <div>
        <label style={{ display: 'block', fontWeight: 600, marginBottom: 6, fontSize: 14 }}>
          Notes
        </label>
        <textarea
          value={notes}
          onChange={e => setNotes(e.target.value)}
          placeholder="Optional notes about this customer…"
          rows={3}
          style={{
            width: '100%', padding: '10px 12px',
            border: '1px solid #d1d5db', borderRadius: 8,
            fontSize: 15, outline: 'none', resize: 'vertical',
          }}
        />
      </div>

      {/* Color */}
      <div>
        <label style={{ display: 'block', fontWeight: 600, marginBottom: 8, fontSize: 14 }}>
          Color
          {isStopped && (
            <span style={{
              marginLeft: 10, fontSize: 12, fontWeight: 700,
              background: '#22c55e', color: '#fff',
              padding: '2px 8px', borderRadius: 4,
              textTransform: 'uppercase', letterSpacing: '0.05em',
            }}>→ Will be STOPPED</span>
          )}
        </label>
        <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap' }}>
          {PRESET_COLORS.map(c => (
            <button
              key={c}
              type="button"
              onClick={() => handlePresetClick(c)}
              style={{
                display: 'flex', alignItems: 'center', gap: 6,
                padding: '7px 14px',
                border: (!useCustom && color === c)
                  ? `2px solid ${COLOR_SWATCHES[c]}`
                  : '2px solid #e5e7eb',
                borderRadius: 8,
                background: (!useCustom && color === c) ? COLOR_SWATCHES[c] + '18' : '#fff',
                fontWeight: (!useCustom && color === c) ? 700 : 500,
                fontSize: 14,
                color: '#374151',
                transition: 'all 0.1s',
              }}
            >
              <span style={{
                width: 12, height: 12, borderRadius: '50%',
                background: COLOR_SWATCHES[c], display: 'inline-block',
              }} />
              {c}
            </button>
          ))}
          <button
            type="button"
            onClick={handleCustomToggle}
            style={{
              padding: '7px 14px',
              border: useCustom ? '2px solid #6366f1' : '2px solid #e5e7eb',
              borderRadius: 8,
              background: useCustom ? '#eef2ff' : '#fff',
              fontWeight: useCustom ? 700 : 500,
              fontSize: 14,
              color: useCustom ? '#4f46e5' : '#6b7280',
            }}
          >
            Other…
          </button>
        </div>
        {useCustom && (
          <input
            value={customColor}
            onChange={e => setCustomColor(e.target.value)}
            placeholder="Type a color name…"
            style={{
              marginTop: 10,
              width: '100%', padding: '10px 12px',
              border: '1px solid #d1d5db', borderRadius: 8,
              fontSize: 15, outline: 'none',
            }}
            autoFocus
          />
        )}
      </div>

      {/* Buttons */}
      <div style={{ display: 'flex', gap: 10, justifyContent: 'flex-end' }}>
        {onCancel && (
          <button
            type="button"
            onClick={onCancel}
            style={{
              padding: '9px 20px', borderRadius: 8,
              border: '1px solid #e5e7eb', background: '#f9fafb',
              fontSize: 14, fontWeight: 500, color: '#374151',
            }}
          >Cancel</button>
        )}
        <button
          type="submit"
          disabled={saving}
          style={{
            padding: '9px 24px', borderRadius: 8,
            border: 'none', background: '#4f46e5',
            fontSize: 14, fontWeight: 600, color: '#fff',
            opacity: saving ? 0.7 : 1,
          }}
        >{saving ? 'Saving…' : submitLabel}</button>
      </div>
    </form>
  );
}

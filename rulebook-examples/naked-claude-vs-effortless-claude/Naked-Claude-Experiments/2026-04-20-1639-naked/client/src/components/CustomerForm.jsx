import { useState } from 'react';
import { colorStyle, PRESET_COLORS } from '../colorUtils.js';

export default function CustomerForm({ title, initial = {}, onSubmit, onCancel }) {
  const [name,   setName]   = useState(initial.name   || '');
  const [notes,  setNotes]  = useState(initial.notes  || '');
  const [color,  setColor]  = useState(initial.color  || '');
  const [custom, setCustom] = useState(
    initial.color && !PRESET_COLORS.includes(initial.color) ? initial.color : ''
  );
  const [useCustom, setUseCustom] = useState(
    !!initial.color && !PRESET_COLORS.includes(initial.color)
  );

  const [saving, setSaving] = useState(false);
  const [error,  setError]  = useState(null);

  // Effective color value
  const effectiveColor = useCustom ? custom : color;

  // is_stopped preview
  const wouldBeStopped = effectiveColor === 'Green';

  const handleColorClick = (c) => {
    setUseCustom(false);
    setColor(c);
  };

  const handleCustomToggle = () => {
    setUseCustom(true);
    setColor('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!name.trim()) { setError('Name is required.'); return; }
    setSaving(true);
    setError(null);
    try {
      await onSubmit({ name: name.trim(), notes, color: effectiveColor });
    } catch (err) {
      setError(err.message);
      setSaving(false);
    }
  };

  const cs = colorStyle(effectiveColor);

  return (
    <div style={styles.card}>
      <h2 style={styles.heading}>{title}</h2>

      <form onSubmit={handleSubmit} style={styles.form}>
        {/* Name */}
        <div style={styles.field}>
          <label style={styles.label} htmlFor="name">Name *</label>
          <input
            id="name"
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            style={styles.input}
            placeholder="e.g. Alice Johnson"
            autoFocus
          />
        </div>

        {/* Notes */}
        <div style={styles.field}>
          <label style={styles.label} htmlFor="notes">Notes</label>
          <textarea
            id="notes"
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            style={styles.textarea}
            placeholder="A sentence or two about what's going on…"
            rows={3}
          />
        </div>

        {/* Color */}
        <div style={styles.field}>
          <label style={styles.label}>Color</label>
          <div style={styles.colorPicker}>
            {PRESET_COLORS.map((c) => {
              const s = colorStyle(c);
              const isSelected = !useCustom && color === c;
              return (
                <button
                  key={c}
                  type="button"
                  onClick={() => handleColorClick(c)}
                  style={{
                    ...styles.colorBtn,
                    background: s.bg,
                    color: s.text,
                    border: isSelected ? `3px solid ${s.border}` : '3px solid transparent',
                    boxShadow: isSelected ? `0 0 0 2px ${s.border}` : 'none',
                  }}
                >
                  {c}
                </button>
              );
            })}
            <button
              type="button"
              onClick={handleCustomToggle}
              style={{
                ...styles.colorBtn,
                background: useCustom ? '#555' : '#ddd',
                color: useCustom ? '#fff' : '#444',
                border: useCustom ? '3px solid #222' : '3px solid transparent',
              }}
            >
              Custom…
            </button>
          </div>

          {useCustom && (
            <input
              type="text"
              value={custom}
              onChange={(e) => setCustom(e.target.value)}
              style={{ ...styles.input, marginTop: 8 }}
              placeholder="Type a color name exactly as you want it stored"
              autoFocus
            />
          )}

          {/* Stopped preview */}
          <div style={styles.preview}>
            <div
              style={{
                ...styles.colorSwatch,
                background: effectiveColor ? cs.bg : '#eee',
              }}
            />
            <span style={styles.previewText}>
              {effectiveColor
                ? <>Color: <strong>{effectiveColor}</strong> — {wouldBeStopped ? '🟢 This customer will be STOPPED' : '⚡ Not stopped'}</>
                : 'No color selected'}
            </span>
          </div>
        </div>

        {error && <div style={styles.error}>{error}</div>}

        <div style={styles.actions}>
          <button type="submit" style={styles.saveBtn} disabled={saving}>
            {saving ? 'Saving…' : 'Save'}
          </button>
          <button type="button" style={styles.cancelBtn} onClick={onCancel} disabled={saving}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

const styles = {
  card: {
    background: '#fff',
    borderRadius: 12,
    padding: '28px 28px',
    boxShadow: '0 2px 8px rgba(0,0,0,0.12)',
    maxWidth: 560,
  },
  heading: {
    fontSize: 20,
    fontWeight: 800,
    color: '#1a1a2e',
    marginBottom: 24,
  },
  form: {
    display: 'flex',
    flexDirection: 'column',
    gap: 20,
  },
  field: {
    display: 'flex',
    flexDirection: 'column',
    gap: 6,
  },
  label: {
    fontSize: 12,
    fontWeight: 700,
    color: '#666',
    textTransform: 'uppercase',
    letterSpacing: 0.8,
  },
  input: {
    border: '1px solid #ddd',
    borderRadius: 6,
    padding: '10px 12px',
    fontSize: 15,
    color: '#1a1a2e',
    outline: 'none',
    width: '100%',
  },
  textarea: {
    border: '1px solid #ddd',
    borderRadius: 6,
    padding: '10px 12px',
    fontSize: 15,
    color: '#1a1a2e',
    outline: 'none',
    resize: 'vertical',
    fontFamily: 'inherit',
    width: '100%',
  },
  colorPicker: {
    display: 'flex',
    gap: 8,
    flexWrap: 'wrap',
  },
  colorBtn: {
    border: 'none',
    borderRadius: 6,
    padding: '8px 14px',
    fontSize: 13,
    fontWeight: 600,
    cursor: 'pointer',
    transition: 'all 0.1s',
  },
  preview: {
    display: 'flex',
    alignItems: 'center',
    gap: 10,
    marginTop: 8,
    padding: '8px 12px',
    background: '#f5f5f5',
    borderRadius: 6,
  },
  colorSwatch: {
    width: 20,
    height: 20,
    borderRadius: '50%',
    border: '1px solid #ccc',
    flexShrink: 0,
  },
  previewText: {
    fontSize: 13,
    color: '#555',
  },
  error: {
    background: '#ffebee',
    color: '#c62828',
    borderRadius: 6,
    padding: '10px 14px',
    fontSize: 13,
  },
  actions: {
    display: 'flex',
    gap: 10,
  },
  saveBtn: {
    background: '#1a1a2e',
    color: '#fff',
    border: 'none',
    borderRadius: 6,
    padding: '11px 28px',
    fontSize: 14,
    fontWeight: 700,
    cursor: 'pointer',
  },
  cancelBtn: {
    background: '#eee',
    color: '#444',
    border: 'none',
    borderRadius: 6,
    padding: '11px 20px',
    fontSize: 14,
    cursor: 'pointer',
  },
};

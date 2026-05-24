import { useState } from 'react';

const PRESETS = ['Red', 'Green', 'Yellow', 'Blue'];

export default function CustomerForm({ initial = {}, onSubmit, submitLabel = 'Save' }) {
  const [name, setName] = useState(initial.name || '');
  const [notes, setNotes] = useState(initial.notes || '');
  const [color, setColor] = useState(initial.color || '');
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState(null);

  async function handle(e) {
    e.preventDefault();
    setBusy(true);
    setError(null);
    try {
      await onSubmit({ name, notes, color });
    } catch (err) {
      setError(err.message);
      setBusy(false);
    }
  }

  return (
    <form onSubmit={handle} className="form">
      <label>
        Name
        <input value={name} onChange={e => setName(e.target.value)} required />
      </label>

      <label>
        Notes
        <textarea value={notes} onChange={e => setNotes(e.target.value)} rows={4} />
      </label>

      <label>
        Color
        <div className="color-row">
          {PRESETS.map(p => (
            <button
              type="button"
              key={p}
              className={color === p ? 'chip selected' : 'chip'}
              onClick={() => setColor(p)}
            >
              {p}
            </button>
          ))}
          <input
            value={color}
            onChange={e => setColor(e.target.value)}
            placeholder="or type any color"
          />
        </div>
      </label>

      {color === 'Green' && (
        <p className="hint">Heads up: Green means this customer will be marked as <strong>STOPPED</strong>.</p>
      )}

      {error && <div className="error">{error}</div>}

      <button type="submit" className="btn primary" disabled={busy}>
        {busy ? 'Saving…' : submitLabel}
      </button>
    </form>
  );
}

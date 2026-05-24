import React, { useState, useEffect } from 'react';
import ColorDot from './ColorDot.jsx';
import './CustomerForm.css';

const PRESET_COLORS = ['Green', 'Red', 'Yellow', 'Blue'];

export default function CustomerForm({ customer, onSave, onCancel }) {
  const isEdit = !!customer;

  const [name, setName] = useState(customer?.name || '');
  const [notes, setNotes] = useState(customer?.notes || '');
  const [colorInput, setColorInput] = useState(customer?.current_color || '');
  const [customColor, setCustomColor] = useState('');
  const [useCustom, setUseCustom] = useState(
    customer?.current_color && !PRESET_COLORS.includes(customer.current_color)
  );

  const currentColor = useCustom ? customColor : colorInput;
  const isStopped = currentColor === 'Green';

  useEffect(() => {
    if (customer?.current_color && !PRESET_COLORS.includes(customer.current_color)) {
      setUseCustom(true);
      setCustomColor(customer.current_color);
    }
  }, []);

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!name.trim()) return alert('Name is required');
    onSave({
      name: name.trim(),
      notes: notes.trim() || null,
      current_color: currentColor || null
    });
  };

  return (
    <div className="form-card">
      <h2>{isEdit ? 'Edit Customer' : 'Add Customer'}</h2>
      <form onSubmit={handleSubmit} className="form-body">
        <div className="form-field">
          <label>Name *</label>
          <input
            type="text"
            value={name}
            onChange={e => setName(e.target.value)}
            placeholder="Customer name"
            required
          />
        </div>

        <div className="form-field">
          <label>Notes</label>
          <textarea
            value={notes}
            onChange={e => setNotes(e.target.value)}
            placeholder="Optional notes…"
            rows={3}
          />
        </div>

        <div className="form-field">
          <label>Color</label>
          <div className="color-options">
            {PRESET_COLORS.map(c => (
              <button
                key={c}
                type="button"
                className={`color-option ${!useCustom && colorInput === c ? 'selected' : ''}`}
                onClick={() => {
                  setColorInput(c);
                  setUseCustom(false);
                }}
              >
                <ColorDot color={c} />
                {c}
              </button>
            ))}
            <button
              type="button"
              className={`color-option ${useCustom ? 'selected' : ''}`}
              onClick={() => setUseCustom(true)}
            >
              Custom…
            </button>
          </div>
          {useCustom && (
            <input
              type="text"
              className="custom-color-input"
              value={customColor}
              onChange={e => setCustomColor(e.target.value)}
              placeholder="Type a color (e.g. Purple)"
              autoFocus
            />
          )}
          {currentColor && (
            <div className="color-preview">
              <ColorDot color={currentColor} />
              <span>{currentColor}</span>
              {isStopped && <span className="will-be-stopped">→ will be marked Stopped</span>}
            </div>
          )}
        </div>

        <div className="form-actions">
          <button type="button" className="btn btn-secondary" onClick={onCancel}>Cancel</button>
          <button type="submit" className="btn btn-primary">{isEdit ? 'Save Changes' : 'Add Customer'}</button>
        </div>
      </form>
    </div>
  );
}

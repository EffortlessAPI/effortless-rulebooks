import React from 'react';
import ColorDot from './ColorDot.jsx';
import './CustomerDetail.css';

export default function CustomerDetail({ customer, onEdit, onDelete }) {
  const { customer_id, name, notes, current_color, is_stopped } = customer;

  return (
    <div className="detail-card">
      <div className="detail-header">
        <div className="detail-title">
          <ColorDot color={current_color} />
          <h2>{name}</h2>
          {is_stopped && <span className="stopped-badge-lg">STOPPED</span>}
        </div>
        <div className="detail-actions">
          <button className="btn btn-edit" onClick={() => onEdit(customer)}>Edit</button>
          <button className="btn btn-danger" onClick={() => onDelete(customer_id)}>Delete</button>
        </div>
      </div>

      <div className="detail-body">
        <div className="detail-field">
          <span className="field-label">Customer ID</span>
          <span className="field-value mono">{customer_id}</span>
        </div>
        <div className="detail-field">
          <span className="field-label">Color</span>
          <span className="field-value">
            <ColorDot color={current_color} /> {current_color || '—'}
          </span>
        </div>
        <div className="detail-field">
          <span className="field-label">Status</span>
          <span className={`field-value ${is_stopped ? 'status-stopped' : 'status-active'}`}>
            {is_stopped ? 'Stopped' : 'Active'}
          </span>
        </div>
        <div className="detail-field">
          <span className="field-label">Notes</span>
          <span className="field-value">{notes || <em className="empty">No notes</em>}</span>
        </div>
      </div>
    </div>
  );
}

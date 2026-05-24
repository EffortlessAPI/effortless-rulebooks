import React from 'react';
import ColorDot from './ColorDot.jsx';
import './CustomerRow.css';

export default function CustomerRow({ customer, onView, onEdit, onDelete }) {
  const { customer_id, name, notes, current_color, is_stopped } = customer;

  return (
    <div className={`customer-row ${is_stopped ? 'stopped' : ''}`}>
      <div className="row-left">
        <ColorDot color={current_color} />
        <div className="row-info">
          <div className="row-name">
            {name}
            {is_stopped && <span className="stopped-badge">STOPPED</span>}
          </div>
          {notes && <div className="row-notes">{notes}</div>}
        </div>
      </div>
      <div className="row-actions">
        <button className="btn btn-secondary btn-sm" onClick={() => onView(customer)}>View</button>
        <button className="btn btn-edit btn-sm" onClick={() => onEdit(customer)}>Edit</button>
        <button className="btn btn-danger btn-sm" onClick={() => onDelete(customer_id)}>Delete</button>
      </div>
    </div>
  );
}

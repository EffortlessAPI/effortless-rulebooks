import React from 'react';
import CustomerRow from './CustomerRow.jsx';
import './CustomerList.css';

export default function CustomerList({ customers, onView, onEdit, onDelete, onAdd }) {
  return (
    <div className="customer-list-section">
      <div className="list-header">
        <h2>Customers</h2>
        <button className="btn btn-primary" onClick={onAdd}>+ Add Customer</button>
      </div>
      {customers.length === 0 ? (
        <div className="empty-state">No customers yet. Add one above!</div>
      ) : (
        <div className="customer-list">
          {customers.map(c => (
            <CustomerRow
              key={c.customer_id}
              customer={c}
              onView={onView}
              onEdit={onEdit}
              onDelete={onDelete}
            />
          ))}
        </div>
      )}
    </div>
  );
}

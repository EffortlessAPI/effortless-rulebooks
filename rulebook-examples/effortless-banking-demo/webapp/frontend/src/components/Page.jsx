import React from 'react';

export function Page({ title, subtitle, actions, children }) {
  return (
    <>
      <div className="page-header">
        <div>
          <h1>{title}</h1>
          {subtitle && <div className="subtitle">{subtitle}</div>}
        </div>
        <div className="row">{actions}</div>
      </div>
      {children}
    </>
  );
}

export function Card({ title, right, children }) {
  return (
    <div className="card">
      {title && (
        <div className="card-header">
          <span>{title}</span>
          {right}
        </div>
      )}
      <div className="card-body">{children}</div>
    </div>
  );
}

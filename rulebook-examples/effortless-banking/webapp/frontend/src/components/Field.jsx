import React from 'react';
import { useFieldDrawer } from '../FieldDrawer.jsx';

// snake_case (DB) -> PascalCase (rulebook field name).
export function dbToRulebook(col) {
  return col.split('_').map((s) => s.charAt(0).toUpperCase() + s.slice(1)).join('');
}

const moneyFmt = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 });
const numFmt = new Intl.NumberFormat('en-US');

export function formatValue(v, hint) {
  if (v === null || v === undefined || v === '') return <span className="muted">—</span>;
  if (hint === 'money') return moneyFmt.format(Number(v));
  if (hint === 'pct') return `${Number(v).toFixed(2)}%`;
  if (hint === 'ratio') return Number(v).toFixed(2);
  if (hint === 'date' && typeof v === 'string') return v.slice(0, 10);
  if (hint === 'bool') return v ? <span className="badge ok">yes</span> : <span className="badge muted">no</span>;
  if (typeof v === 'number') return numFmt.format(v);
  return String(v);
}

// A clickable cell: double-click opens the FieldDrawer for the rulebook field.
export function Field({ table, col, value, hint, onClick, as = 'td', className }) {
  const drawer = useFieldDrawer();
  const Tag = as;
  const handleDoubleClick = (e) => {
    e.stopPropagation();
    drawer.open(table, dbToRulebook(col), value);
  };
  return (
    <Tag
      className={className}
      onClick={onClick}
      onDoubleClick={handleDoubleClick}
      title={`Double-click for ${table}.${dbToRulebook(col)}`}
    >
      {formatValue(value, hint)}
    </Tag>
  );
}

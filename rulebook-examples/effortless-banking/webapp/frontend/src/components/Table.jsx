import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Field, dbToRulebook } from './Field.jsx';

// Generic data table. `columns` is an array of:
//   { key: 'principal_usd', label: 'Principal', hint: 'money', num: true }
// Row click navigates via rowHref(row); double-clicking any cell opens the
// field drawer for that field.
export function DataTable({ table, rows, columns, rowHref, empty = 'No rows.' }) {
  const nav = useNavigate();
  if (!rows?.length) return <div className="empty">{empty}</div>;
  return (
    <table className="data">
      <thead>
        <tr>
          {columns.map((c) => (
            <th key={c.key} className={c.num ? 'num' : ''}>
              {c.label || dbToRulebook(c.key)}
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {rows.map((r, i) => (
          <tr key={r.name || r.id || i}>
            {columns.map((c) => (
              <Field
                key={c.key}
                table={table}
                col={c.key}
                value={r[c.key]}
                hint={c.hint}
                className={c.num ? 'num' : ''}
                onClick={rowHref ? () => nav(rowHref(r)) : undefined}
              />
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
}

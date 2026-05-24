import React from 'react';
import { colorToHex } from './colorUtils';

const TRACKED_COLORS = ['Green', 'Red', 'Yellow', 'Blue'];

export default function SummaryBar({ summary }) {
  if (!summary) return null;

  return (
    <div className="summary-bar">
      <div className="summary-stat">
        <span className="big">{summary.total}</span>
        <span className="label">Total</span>
      </div>
      <div className="summary-stat">
        <span className="big" style={{ color: '#22c55e' }}>{summary.stopped}</span>
        <span className="label">Stopped</span>
      </div>
      <div className="summary-divider" />
      <div className="color-pills">
        {TRACKED_COLORS.map(c => {
          const count = summary[c.toLowerCase()] ?? 0;
          return (
            <div key={c} className="color-pill">
              <div className="dot" style={{ background: colorToHex(c) }} />
              {c}: {count}
            </div>
          );
        })}
        {summary.other > 0 && (
          <div className="color-pill">
            <div className="dot" style={{ background: '#9ca3af' }} />
            Other: {summary.other}
          </div>
        )}
      </div>
    </div>
  );
}

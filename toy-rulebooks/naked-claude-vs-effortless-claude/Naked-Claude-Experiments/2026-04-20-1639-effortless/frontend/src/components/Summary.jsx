import React from 'react';
import './Summary.css';

function ColorBadge({ color, count }) {
  if (!count || Number(count) === 0) return null;
  return (
    <span className={`color-badge color-badge-${color.toLowerCase()}`}>
      {color}: {count}
    </span>
  );
}

export default function Summary({ summary }) {
  return (
    <div className="summary-card">
      <div className="summary-stats">
        <div className="stat">
          <span className="stat-value">{summary.total}</span>
          <span className="stat-label">Total Customers</span>
        </div>
        <div className="stat stat-stopped">
          <span className="stat-value">{summary.stopped}</span>
          <span className="stat-label">Stopped</span>
        </div>
      </div>
      <div className="color-breakdown">
        <span className="breakdown-label">Color mix:</span>
        <ColorBadge color="Green" count={summary.green} />
        <ColorBadge color="Red" count={summary.red} />
        <ColorBadge color="Yellow" count={summary.yellow} />
        <ColorBadge color="Blue" count={summary.blue} />
        {Number(summary.other) > 0 && (
          <span className="color-badge color-badge-other">Other: {summary.other}</span>
        )}
      </div>
    </div>
  );
}

import React from 'react';
import './ColorDot.css';

const COLOR_MAP = {
  Green: '#22c55e',
  Red: '#ef4444',
  Yellow: '#eab308',
  Blue: '#3b82f6',
};

export default function ColorDot({ color }) {
  const bg = COLOR_MAP[color] || '#9ca3af';
  return (
    <span
      className="color-dot"
      style={{ background: bg }}
      title={color || 'No color'}
    />
  );
}

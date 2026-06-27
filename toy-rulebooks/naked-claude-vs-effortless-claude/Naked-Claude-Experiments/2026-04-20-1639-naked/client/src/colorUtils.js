const COLOR_MAP = {
  Green:  { bg: '#4CAF50', text: '#fff', border: '#388E3C' },
  Red:    { bg: '#f44336', text: '#fff', border: '#c62828' },
  Yellow: { bg: '#FFC107', text: '#333', border: '#F9A825' },
  Blue:   { bg: '#2196F3', text: '#fff', border: '#1565C0' },
};

const FALLBACK = { bg: '#9E9E9E', text: '#fff', border: '#616161' };

export function colorStyle(color) {
  return COLOR_MAP[color] || FALLBACK;
}

export const PRESET_COLORS = ['Green', 'Red', 'Yellow', 'Blue'];

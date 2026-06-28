export const COLOR_OPTIONS = ['Green', 'Red', 'Yellow', 'Blue'];

export function colorToHex(color) {
  const map = {
    Green: '#22c55e',
    Red: '#ef4444',
    Yellow: '#eab308',
    Blue: '#3b82f6',
  };
  return map[color] || '#9ca3af';
}

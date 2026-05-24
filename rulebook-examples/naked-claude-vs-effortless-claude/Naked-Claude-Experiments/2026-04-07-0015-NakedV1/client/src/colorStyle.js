// Map a color string to a CSS color for the swatch.
// Anything we don't know about renders as a neutral gray.
const KNOWN = {
  Red: '#e53935',
  Green: '#43a047',
  Yellow: '#fdd835',
  Blue: '#1e88e5'
};

export function swatchColor(color) {
  if (!color) return '#cccccc';
  if (KNOWN[color]) return KNOWN[color];
  // Allow free-form colors — try the literal CSS value, fallback gray.
  return color;
}

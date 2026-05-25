// Dev login — identity is stored in localStorage so refresh survives.

const KEY = 'vss.identity';

export function getIdentity() {
  try {
    const raw = localStorage.getItem(KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

export function setIdentity(ident) {
  localStorage.setItem(KEY, JSON.stringify(ident));
}

export function clearIdentity() {
  localStorage.removeItem(KEY);
}

export const ROLE = {
  COORDINATOR: 'coordinator',
  VOLUNTEER: 'volunteer',
  VIEWER: 'viewer',
};

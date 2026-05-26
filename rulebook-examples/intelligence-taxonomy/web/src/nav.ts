import type { Role } from './types';

export type NavItem = { to: string; label: string };

export function navFor(role: Role): NavItem[] {
  if (role === 'researcher') {
    return [
      { to: '/', label: 'Dashboard' },
      { to: '/intelligences', label: 'Intelligences' },
      { to: '/capabilities', label: 'Capabilities' },
      { to: '/assessments', label: 'Assessments' },
    ];
  }
  return [{ to: '/', label: 'Home' }];
}

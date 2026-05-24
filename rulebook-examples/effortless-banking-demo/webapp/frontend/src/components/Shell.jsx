import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth.jsx';

const NAV = {
  Admin: [
    ['Overview', [
      ['Dashboard', '/admin'],
      ['Pipeline', '/admin/pipeline'],
      ['Watchlist', '/admin/watchlist'],
    ]],
    ['Origination', [
      ['Applications', '/admin/applications'],
      ['Underwriting Queue', '/admin/underwriting'],
      ['Approved & Funded', '/admin/funded'],
    ]],
    ['Portfolio', [
      ['Loans', '/admin/loans'],
      ['Covenants', '/admin/covenants'],
      ['Risk Rating Changes', '/admin/risk-history'],
    ]],
    ['Clients', [
      ['Businesses', '/admin/businesses'],
      ['Beneficial Owners', '/admin/owners'],
      ['Contacts', '/admin/contacts'],
      ['Accounts', '/admin/accounts'],
    ]],
    ['Operations', [
      ['Documents', '/admin/documents'],
      ['Interactions', '/admin/interactions'],
      ['Users', '/admin/users'],
    ]],
  ],
  RM: [
    ['Relationship Manager', [
      ['My Dashboard', '/rm'],
      ['My Portfolio', '/rm/portfolio'],
      ['My Loans', '/rm/loans'],
      ['Recent Interactions', '/rm/interactions'],
    ]],
  ],
  Underwriter: [
    ['Underwriting', [
      ['My Queue', '/underwriter'],
      ['My Decisions', '/underwriter/decisions'],
      ['Covenants', '/underwriter/covenants'],
    ]],
  ],
  BranchBanker: [
    ['Branch', [
      ['Dashboard', '/branch'],
      ['New Account Openings', '/branch/accounts'],
      ['Walk-In Contacts', '/branch/contacts'],
    ]],
  ],
};

export function Shell({ children }) {
  const { user, logout } = useAuth();
  const nav = useNavigate();
  const groups = NAV[user.role] || [];
  return (
    <div className="shell">
      <aside className="sidebar" style={{ display: 'flex', flexDirection: 'column' }}>
        <div>
          <div className="brand">
            First Valley Bank
            <small>{user.role} Portal</small>
          </div>
          {groups.map(([title, links]) => (
            <div className="nav-group" key={title}>
              <div className="nav-group-title">{title}</div>
              {links.map(([label, to]) => (
                <NavLink
                  key={to}
                  to={to}
                  end
                  className={({ isActive }) => 'nav-item' + (isActive ? ' active' : '')}
                >
                  {label}
                </NavLink>
              ))}
            </div>
          ))}
        </div>
        <div className="user-card">
          <div>{user.full_name}</div>
          <div className="role">{user.role} · {user.email}</div>
          <button
            className="ghost"
            style={{ marginTop: 10, width: '100%' }}
            onClick={async () => { await logout(); nav('/login'); }}
          >
            Sign out
          </button>
        </div>
      </aside>
      <main className="main">{children}</main>
    </div>
  );
}

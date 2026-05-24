import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './auth.jsx';
import { Shell } from './components/Shell.jsx';
import Login from './pages/Login.jsx';

import AdminDashboard from './portals/admin/Dashboard.jsx';
import AdminPipeline from './portals/admin/Pipeline.jsx';
import AdminWatchlist from './portals/admin/Watchlist.jsx';
import AdminApplications from './portals/admin/Applications.jsx';
import AdminUnderwriting from './portals/admin/Underwriting.jsx';
import AdminFunded from './portals/admin/Funded.jsx';
import AdminLoans from './portals/admin/Loans.jsx';
import AdminCovenants from './portals/admin/Covenants.jsx';
import AdminRiskHistory from './portals/admin/RiskHistory.jsx';
import AdminBusinesses from './portals/admin/Businesses.jsx';
import AdminOwners from './portals/admin/Owners.jsx';
import AdminContacts from './portals/admin/Contacts.jsx';
import AdminAccounts from './portals/admin/Accounts.jsx';
import AdminDocuments from './portals/admin/Documents.jsx';
import AdminInteractions from './portals/admin/Interactions.jsx';
import AdminUsers from './portals/admin/Users.jsx';

import BusinessDetail from './pages/BusinessDetail.jsx';
import LoanDetail from './pages/LoanDetail.jsx';

import RmDashboard from './portals/rm/Dashboard.jsx';
import RmPortfolio from './portals/rm/Portfolio.jsx';
import RmLoans from './portals/rm/Loans.jsx';
import RmInteractions from './portals/rm/Interactions.jsx';

import UwQueue from './portals/underwriter/Queue.jsx';
import UwDecisions from './portals/underwriter/Decisions.jsx';
import UwCovenants from './portals/underwriter/Covenants.jsx';

import BbDashboard from './portals/branchbanker/Dashboard.jsx';
import BbAccounts from './portals/branchbanker/Accounts.jsx';
import BbContacts from './portals/branchbanker/Contacts.jsx';

export default function App() {
  const { user, loading } = useAuth();
  if (loading) return <div className="login"><div>Loading…</div></div>;
  if (!user) {
    return (
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    );
  }

  const home = {
    Admin: '/admin',
    RM: '/rm',
    Underwriter: '/underwriter',
    BranchBanker: '/branch',
  }[user.role] || '/admin';

  return (
    <Shell>
      <Routes>
        <Route path="/login" element={<Navigate to={home} replace />} />
        <Route path="/" element={<Navigate to={home} replace />} />

        {/* Admin */}
        <Route path="/admin" element={<AdminDashboard />} />
        <Route path="/admin/pipeline" element={<AdminPipeline />} />
        <Route path="/admin/watchlist" element={<AdminWatchlist />} />
        <Route path="/admin/applications" element={<AdminApplications />} />
        <Route path="/admin/underwriting" element={<AdminUnderwriting />} />
        <Route path="/admin/funded" element={<AdminFunded />} />
        <Route path="/admin/loans" element={<AdminLoans />} />
        <Route path="/admin/covenants" element={<AdminCovenants />} />
        <Route path="/admin/risk-history" element={<AdminRiskHistory />} />
        <Route path="/admin/businesses" element={<AdminBusinesses />} />
        <Route path="/admin/owners" element={<AdminOwners />} />
        <Route path="/admin/contacts" element={<AdminContacts />} />
        <Route path="/admin/accounts" element={<AdminAccounts />} />
        <Route path="/admin/documents" element={<AdminDocuments />} />
        <Route path="/admin/interactions" element={<AdminInteractions />} />
        <Route path="/admin/users" element={<AdminUsers />} />

        {/* Shared detail routes */}
        <Route path="/business/:name" element={<BusinessDetail />} />
        <Route path="/loan/:name" element={<LoanDetail />} />

        {/* RM */}
        <Route path="/rm" element={<RmDashboard />} />
        <Route path="/rm/portfolio" element={<RmPortfolio />} />
        <Route path="/rm/loans" element={<RmLoans />} />
        <Route path="/rm/interactions" element={<RmInteractions />} />

        {/* Underwriter */}
        <Route path="/underwriter" element={<UwQueue />} />
        <Route path="/underwriter/decisions" element={<UwDecisions />} />
        <Route path="/underwriter/covenants" element={<UwCovenants />} />

        {/* BranchBanker */}
        <Route path="/branch" element={<BbDashboard />} />
        <Route path="/branch/accounts" element={<BbAccounts />} />
        <Route path="/branch/contacts" element={<BbContacts />} />

        <Route path="*" element={<Navigate to={home} replace />} />
      </Routes>
    </Shell>
  );
}

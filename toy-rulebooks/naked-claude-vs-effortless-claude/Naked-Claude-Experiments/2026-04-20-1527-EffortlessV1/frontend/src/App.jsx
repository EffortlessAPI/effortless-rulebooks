import React from 'react';
import { Routes, Route } from 'react-router-dom';
import CustomerList from './CustomerList';
import CustomerDetail from './CustomerDetail';

export default function App() {
  return (
    <>
      <header className="app-header">
        <span style={{ fontSize: '1.5rem' }}>🎨</span>
        <h1>Customer Tracker</h1>
      </header>
      <Routes>
        <Route path="/" element={<CustomerList />} />
        <Route path="/customers/:id" element={<CustomerDetail />} />
      </Routes>
    </>
  );
}

import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import App from './App.jsx';
import CustomerList from './pages/CustomerList.jsx';
import CustomerDetail from './pages/CustomerDetail.jsx';
import CustomerEdit from './pages/CustomerEdit.jsx';
import CustomerNew from './pages/CustomerNew.jsx';
import './styles.css';

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<App />}>
          <Route index element={<CustomerList />} />
          <Route path="customers/new" element={<CustomerNew />} />
          <Route path="customers/:id" element={<CustomerDetail />} />
          <Route path="customers/:id/edit" element={<CustomerEdit />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>
);

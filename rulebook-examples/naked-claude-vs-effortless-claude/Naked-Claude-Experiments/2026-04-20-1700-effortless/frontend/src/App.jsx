import { Routes, Route } from 'react-router-dom';
import ClientListPage from './components/ClientListPage';
import ClientDetailPage from './components/ClientDetailPage';
import ClientCategoryListPage from './components/ClientCategoryListPage';
import StatusListPage from './components/StatusListPage';
import StatusDetailPage from './components/StatusDetailPage';
import ProductListPage from './components/ProductListPage';
import ProductDetailPage from './components/ProductDetailPage';
import InvoiceListPage from './components/InvoiceListPage';
import InvoiceDetailPage from './components/InvoiceDetailPage';
import PaymentsListPage from './components/PaymentsListPage';
import InventoryPage from './components/InventoryPage';
import AppUsersPage from './components/AppUsersPage';
import ApprovalsPage from './components/ApprovalsPage';
import Nav from './components/Nav';

export default function App() {
  return (
    <>
      <Nav />
      <Routes>
        <Route path="/" element={<ClientListPage />} />
        <Route path="/clients/:id" element={<ClientDetailPage />} />
        <Route path="/client-categories" element={<ClientCategoryListPage />} />
        <Route path="/statuses" element={<StatusListPage />} />
        <Route path="/statuses/:id" element={<StatusDetailPage />} />
        <Route path="/products" element={<ProductListPage />} />
        <Route path="/products/:id" element={<ProductDetailPage />} />
        <Route path="/invoices" element={<InvoiceListPage />} />
        <Route path="/invoices/:id" element={<InvoiceDetailPage />} />
        <Route path="/payments" element={<PaymentsListPage />} />
        <Route path="/inventory" element={<InventoryPage />} />
        <Route path="/app-users" element={<AppUsersPage />} />
        <Route path="/approvals" element={<ApprovalsPage />} />
      </Routes>
    </>
  );
}

import { Routes, Route, NavLink } from 'react-router-dom';
import ClientList     from './pages/ClientList.jsx';
import ClientDetail   from './pages/ClientDetail.jsx';
import ClientForm     from './pages/ClientForm.jsx';
import StatusList     from './pages/StatusList.jsx';
import StatusDetail   from './pages/StatusDetail.jsx';
import StatusForm     from './pages/StatusForm.jsx';
import ProductList    from './pages/ProductList.jsx';
import ProductDetail  from './pages/ProductDetail.jsx';
import ProductForm    from './pages/ProductForm.jsx';
import InvoiceList    from './pages/InvoiceList.jsx';
import InvoiceDetail  from './pages/InvoiceDetail.jsx';
import InvoiceForm    from './pages/InvoiceForm.jsx';
import ClientCategoryList   from './pages/ClientCategoryList.jsx';
import ClientCategoryDetail from './pages/ClientCategoryDetail.jsx';
import ClientCategoryForm   from './pages/ClientCategoryForm.jsx';
import AppUserList     from './pages/AppUserList.jsx';
import AppUserDetail   from './pages/AppUserDetail.jsx';
import AppUserForm     from './pages/AppUserForm.jsx';
import ClientApprovalList from './pages/ClientApprovalList.jsx';
import ClientApprovalForm from './pages/ClientApprovalForm.jsx';
import InventoryList   from './pages/InventoryList.jsx';
import InventoryForm   from './pages/InventoryForm.jsx';

function Header() {
  return (
    <header className="app-header">
      <h1>Clients &amp; Invoices</h1>
      <nav className="app-nav">
        <NavLink to="/"          className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'} end>
          Clients
        </NavLink>
        <NavLink to="/statuses"  className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
          Statuses
        </NavLink>
        <NavLink to="/client-categories" className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
          Categories
        </NavLink>
        <NavLink to="/client-approvals" className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
          Approvals
        </NavLink>
        <NavLink to="/products"  className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
          Products
        </NavLink>
        <NavLink to="/inventory" className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
          Inventory
        </NavLink>
        <NavLink to="/invoices"  className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
          Invoices
        </NavLink>
        <NavLink to="/app-users" className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
          Users
        </NavLink>
      </nav>
    </header>
  );
}

export default function App() {
  return (
    <>
      <Header />
      <div className="container">
        <Routes>
          <Route path="/"                    element={<ClientList />} />
          <Route path="/clients/new"         element={<ClientForm />} />
          <Route path="/clients/:id"         element={<ClientDetail />} />
          <Route path="/clients/:id/edit"    element={<ClientForm />} />
          <Route path="/statuses"            element={<StatusList />} />
          <Route path="/statuses/new"        element={<StatusForm />} />
          <Route path="/statuses/:id"        element={<StatusDetail />} />
          <Route path="/statuses/:id/edit"   element={<StatusForm />} />
          <Route path="/client-categories"          element={<ClientCategoryList />} />
          <Route path="/client-categories/new"      element={<ClientCategoryForm />} />
          <Route path="/client-categories/:id"      element={<ClientCategoryDetail />} />
          <Route path="/client-categories/:id/edit" element={<ClientCategoryForm />} />
          <Route path="/client-approvals"           element={<ClientApprovalList />} />
          <Route path="/client-approvals/new"       element={<ClientApprovalForm />} />
          <Route path="/client-approvals/:id/edit"  element={<ClientApprovalForm />} />
          <Route path="/products"            element={<ProductList />} />
          <Route path="/products/new"        element={<ProductForm />} />
          <Route path="/products/:id"        element={<ProductDetail />} />
          <Route path="/products/:id/edit"   element={<ProductForm />} />
          <Route path="/inventory"           element={<InventoryList />} />
          <Route path="/inventory/new"       element={<InventoryForm />} />
          <Route path="/inventory/:id/edit"  element={<InventoryForm />} />
          <Route path="/invoices"            element={<InvoiceList />} />
          <Route path="/invoices/new"        element={<InvoiceForm />} />
          <Route path="/invoices/:id"        element={<InvoiceDetail />} />
          <Route path="/invoices/:id/edit"   element={<InvoiceForm />} />
          <Route path="/app-users"           element={<AppUserList />} />
          <Route path="/app-users/new"       element={<AppUserForm />} />
          <Route path="/app-users/:id"       element={<AppUserDetail />} />
          <Route path="/app-users/:id/edit"  element={<AppUserForm />} />
        </Routes>
      </div>
    </>
  );
}

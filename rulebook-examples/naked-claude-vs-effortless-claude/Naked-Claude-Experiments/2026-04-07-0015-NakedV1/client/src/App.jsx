import { Outlet, Link } from 'react-router-dom';

export default function App() {
  return (
    <div className="app">
      <header>
        <Link to="/" className="brand">Customers — v1</Link>
      </header>
      <main>
        <Outlet />
      </main>
    </div>
  );
}

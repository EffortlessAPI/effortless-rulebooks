import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { DagCell } from '../explainer-dag';

interface Product {
  name: string;
  unit_price: number;
  current_quantity: number;
  is_low_stock: boolean;
  reorder_level: number;
}

export default function Dashboard() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [exporting, setExporting] = useState(false);

  async function downloadExcel() {
    setExporting(true);
    try {
      const res = await api('/api/export/xlsx');
      if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        alert(`Export failed: ${err.details || res.statusText}`);
        return;
      }
      const blob = await res.blob();
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'rulebook.xlsx';
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
    } finally {
      setExporting(false);
    }
  }

  useEffect(() => {
    loadProducts();
  }, []);

  async function loadProducts() {
    try {
      const res = await api('/api/products');
      const data = await res.json();
      setProducts(data.map((p: any) => ({
        ...p,
        unit_price: Number(p.unit_price),
        current_quantity: Number(p.current_quantity),
        reorder_level: Number(p.reorder_level),
      })));
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  const totalValue = products.reduce((sum, p) => sum + p.current_quantity * p.unit_price, 0);
  const lowStockCount = products.filter((p) => p.is_low_stock).length;

  if (loading) return <div className="container"><p>Loading...</p></div>;

  return (
    <div className="container">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', gap: '1rem' }}>
        <div>
          <h1>Inventory Dashboard</h1>
          <p>Real-time overview of product inventory and low-stock alerts</p>
        </div>
        <button
          onClick={downloadExcel}
          disabled={exporting}
          style={{
            padding: '0.6rem 1rem',
            background: exporting ? '#95a5a6' : '#1d6f42',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: exporting ? 'wait' : 'pointer',
            fontSize: '0.9rem',
            whiteSpace: 'nowrap',
          }}
          title="Snapshot current DB into effortless-export.json, run rulebook-to-xlsx, download rulebook.xlsx"
        >
          {exporting ? 'Generating…' : '📥 Download as Excel'}
        </button>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1.5rem', marginBottom: '2rem' }}>
        <div style={{ background: 'white', padding: '1.5rem', borderRadius: '8px', boxShadow: '0 1px 3px rgba(0,0,0,0.1)' }}>
          <div style={{ fontSize: '0.85rem', color: '#666', marginBottom: '0.5rem' }}>Total Inventory Value</div>
          <div style={{ fontSize: '2rem', fontWeight: 'bold' }}>${totalValue.toFixed(2)}</div>
        </div>
        <div style={{ background: 'white', padding: '1.5rem', borderRadius: '8px', boxShadow: '0 1px 3px rgba(0,0,0,0.1)' }}>
          <div style={{ fontSize: '0.85rem', color: '#666', marginBottom: '0.5rem' }}>Total SKUs</div>
          <div style={{ fontSize: '2rem', fontWeight: 'bold' }}>{products.length}</div>
        </div>
        <div style={{ background: 'white', padding: '1.5rem', borderRadius: '8px', boxShadow: '0 1px 3px rgba(0,0,0,0.1)' }}>
          <div style={{ fontSize: '0.85rem', color: '#666', marginBottom: '0.5rem' }}>Low Stock Items</div>
          <div style={{ fontSize: '2rem', fontWeight: 'bold', color: lowStockCount > 0 ? '#e74c3c' : '#27ae60' }}>
            {lowStockCount}
          </div>
        </div>
      </div>

      <h2>Product Status</h2>
      <table>
        <thead>
          <tr>
            <th>Product</th>
            <th>Current Qty</th>
            <th>Reorder Level</th>
            <th>Unit Price</th>
            <th>Total Value</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {products.map((p) => (
            <tr key={p.name}>
              <td><DagCell table="Products" field="Name"><strong>{p.name}</strong></DagCell></td>
              <td><DagCell table="Products" field="CurrentQuantity">{p.current_quantity}</DagCell></td>
              <td>{p.reorder_level}</td>
              <td>${p.unit_price.toFixed(2)}</td>
              <td>${(p.current_quantity * p.unit_price).toFixed(2)}</td>
              <td>
                <DagCell table="Products" field="IsLowStock">
                  <span className={`badge ${p.is_low_stock ? 'danger' : 'success'}`}>
                    {p.is_low_stock ? '⚠️ Low Stock' : '✓ OK'}
                  </span>
                </DagCell>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

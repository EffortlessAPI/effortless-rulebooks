import { useEffect, useState } from 'react';
import { api } from '../lib/api';
import { DagCell } from '../explainer-dag';

interface Product {
  product_id: string;
  name: string;
  unit_price: number;
  current_quantity: number;
  is_low_stock: boolean;
  reorder_level: number;
  reorder_status: string;
}

export default function Products() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({ unit_price: 0, reorder_level: 0 });

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

  function startEdit(product: Product) {
    setEditingId(product.product_id);
    setFormData({ unit_price: product.unit_price, reorder_level: product.reorder_level });
  }

  async function saveEdit() {
    if (!editingId) return;
    try {
      const res = await api(`/api/products/${editingId}`, {
        method: 'PATCH',
        body: JSON.stringify(formData),
      });
      if (res.ok) {
        await loadProducts();
        setEditingId(null);
      }
    } catch (err) {
      console.error(err);
    }
  }

  if (loading) return <div className="container"><p>Loading...</p></div>;

  return (
    <div className="container">
      <h1>Products</h1>
      <p>View and manage product master data. Editing unit price or reorder level will affect calculated fields immediately.</p>

      <table>
        <thead>
          <tr>
            <th>Product ID</th>
            <th>Unit Price</th>
            <th>Current Qty</th>
            <th>Reorder Level</th>
            <th>Total Value</th>
            <th>Status</th>
            <th>Reorder Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {products.map((p) => (
            <tr key={p.product_id}>
              <td><DagCell table="Products" field="Name"><strong>{p.name}</strong></DagCell></td>
              <td>
                {editingId === p.product_id ? (
                  <input
                    type="number"
                    step="0.01"
                    value={formData.unit_price}
                    onChange={(e) => setFormData({ ...formData, unit_price: parseFloat(e.target.value) })}
                    style={{ width: '100px' }}
                  />
                ) : (
                  `$${p.unit_price.toFixed(2)}`
                )}
              </td>
              <td><DagCell table="Products" field="CurrentQuantity">{p.current_quantity}</DagCell></td>
              <td>
                {editingId === p.product_id ? (
                  <input
                    type="number"
                    value={formData.reorder_level}
                    onChange={(e) => setFormData({ ...formData, reorder_level: parseInt(e.target.value) })}
                    style={{ width: '100px' }}
                  />
                ) : (
                  p.reorder_level
                )}
              </td>
              <td>${(p.current_quantity * p.unit_price).toFixed(2)}</td>
              <td>
                <DagCell table="Products" field="IsLowStock">
                  <span className={`badge ${p.is_low_stock ? 'danger' : 'success'}`}>
                    {p.is_low_stock ? '⚠️ Low' : '✓ OK'}
                  </span>
                </DagCell>
              </td>
              <td>
                <DagCell table="Products" field="ReorderStatus">
                  <span className={p.is_low_stock ? 'badge danger' : 'badge success'}>
                    {p.reorder_status}
                  </span>
                </DagCell>
              </td>
              <td>
                {editingId === p.product_id ? (
                  <>
                    <button onClick={saveEdit} style={{ marginRight: '0.5rem', background: '#27ae60' }}>Save</button>
                    <button onClick={() => setEditingId(null)} style={{ background: '#95a5a6' }}>Cancel</button>
                  </>
                ) : (
                  <button onClick={() => startEdit(p)}>Edit</button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

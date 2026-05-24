import { useState, useEffect, useCallback } from 'react'
import Dashboard from './components/Dashboard.jsx'
import CustomerList from './components/CustomerList.jsx'
import CustomerDetail from './components/CustomerDetail.jsx'
import CustomerForm from './components/CustomerForm.jsx'
import StatusList from './components/StatusList.jsx'
import StatusDetail from './components/StatusDetail.jsx'
import StatusForm from './components/StatusForm.jsx'
import ProductList from './components/ProductList.jsx'
import ProductDetail from './components/ProductDetail.jsx'
import ProductForm from './components/ProductForm.jsx'
import OrderList from './components/OrderList.jsx'
import OrderDetail from './components/OrderDetail.jsx'
import OrderForm from './components/OrderForm.jsx'
import PaymentList from './components/PaymentList.jsx'

const API = '/api'

const SECTIONS = [
  { key: 'dashboard', label: '📊 Dashboard' },
  { key: 'customers', label: '👥 Customers' },
  { key: 'statuses', label: '🏷️ Statuses' },
  { key: 'products', label: '📦 Products' },
  { key: 'orders', label: '🛒 Orders' },
  { key: 'payments', label: '💳 Payments' },
]

export default function App() {
  const [section, setSection] = useState('dashboard')
  const [view, setView] = useState('list')   // 'list' | 'detail' | 'add' | 'edit'
  const [selected, setSelected] = useState(null)
  const [customers, setCustomers] = useState([])
  const [statuses, setStatuses] = useState([])
  const [products, setProducts] = useState([])
  const [orders, setOrders] = useState([])
  const [payments, setPayments] = useState([])
  const [summary, setSummary] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  const fetchAll = useCallback(async () => {
    try {
      setLoading(true)
      const [cR, sR, pR, oR, pmR, sumR] = await Promise.all([
        fetch(`${API}/customers`), fetch(`${API}/statuses`),
        fetch(`${API}/products`), fetch(`${API}/orders`),
        fetch(`${API}/payments`), fetch(`${API}/summary`)
      ])
      if (!cR.ok || !sR.ok || !pR.ok || !oR.ok || !pmR.ok || !sumR.ok) throw new Error('Failed to load data')
      const [cD, sD, pD, oD, pmD, sumD] = await Promise.all([
        cR.json(), sR.json(), pR.json(), oR.json(), pmR.json(), sumR.json()
      ])
      setCustomers(cD); setStatuses(sD); setProducts(pD)
      setOrders(oD); setPayments(pmD); setSummary(sumD)
      setError(null)
    } catch (err) { setError(err.message) }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { fetchAll() }, [fetchAll])

  const navigate = (sec, v='list', sel=null) => {
    setSection(sec); setView(v); setSelected(sel)
  }

  // Generic save/delete
  const handleSave = async (entity, formData, idField, apiPath, viewSection) => {
    try {
      let res
      if (view === 'add') {
        res = await fetch(`${API}/${apiPath}`, { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify(formData) })
      } else {
        res = await fetch(`${API}/${apiPath}/${selected[idField]}`, { method: 'PUT', headers: {'Content-Type':'application/json'}, body: JSON.stringify(formData) })
      }
      if (!res.ok) { const e = await res.json(); throw new Error(e.error || 'Save failed') }
      await fetchAll()
      navigate(viewSection)
    } catch (err) { alert('Error: ' + err.message) }
  }

  const handleDelete = async (entity, id, apiPath, viewSection, label) => {
    if (!confirm(`Delete ${label}?`)) return
    try {
      const res = await fetch(`${API}/${apiPath}/${id}`, { method: 'DELETE' })
      if (!res.ok) { const e = await res.json(); throw new Error(e.error || 'Delete failed') }
      await fetchAll()
      navigate(viewSection)
    } catch (err) { alert('Error: ' + err.message) }
  }

  const back = () => navigate(section, 'list')

  if (loading && !summary) {
    return <div style={{display:'flex',justifyContent:'center',alignItems:'center',height:'100vh',fontSize:18,color:'#666'}}>Loading...</div>
  }

  return (
    <div style={{maxWidth:1000,margin:'0 auto',padding:'20px 16px'}}>
      <header style={{display:'flex',alignItems:'center',justifyContent:'space-between',marginBottom:20}}>
        <h1 style={{margin:0,fontSize:26,fontWeight:700,color:'#1a1a2e',cursor:'pointer'}} onClick={()=>navigate('dashboard')}>
          🏢 Order Tracker v3
        </h1>
        {view !== 'list' && <button onClick={back} style={styles.secondaryBtn}>← Back</button>}
      </header>

      <nav style={{display:'flex',gap:0,marginBottom:24,borderBottom:'2px solid #e5e7eb',flexWrap:'wrap'}}>
        {SECTIONS.map(s=>(
          <button key={s.key} onClick={()=>navigate(s.key)} style={{
            padding:'10px 18px',fontSize:14,fontWeight:600,border:'none',
            borderBottom:section===s.key?'3px solid #4f46e5':'3px solid transparent',
            background:'transparent',color:section===s.key?'#4f46e5':'#6b7280',
            cursor:'pointer',marginBottom:-2
          }}>{s.label}</button>
        ))}
      </nav>

      {error && <div style={{background:'#fee2e2',border:'1px solid #fca5a5',borderRadius:8,padding:12,marginBottom:16,color:'#b91c1c'}}>{error}</div>}

      {/* ===== DASHBOARD ===== */}
      {section==='dashboard' && <Dashboard summary={summary} onNavigate={navigate} />}

      {/* ===== CUSTOMERS ===== */}
      {section==='customers' && view==='list' && (
        <>
          <div style={{display:'flex',justifyContent:'flex-end',marginBottom:14}}>
            <button onClick={()=>navigate('customers','add')} style={styles.primaryBtn}>+ Add Customer</button>
          </div>
          <CustomerList customers={customers}
            onSelect={c=>navigate('customers','detail',c)}
            onEdit={c=>navigate('customers','edit',c)}
            onDelete={c=>handleDelete('customer',c.customer_id,'customers','customers',`customer "${c.name}"`)} />
        </>
      )}
      {section==='customers' && view==='detail' && selected && (
        <CustomerDetail customer={selected}
          onEdit={()=>navigate('customers','edit',selected)}
          onDelete={()=>handleDelete('customer',selected.customer_id,'customers','customers',`customer "${selected.name}"`)}
          onSelectOrder={o=>navigate('orders','detail',o)} />
      )}
      {section==='customers' && (view==='add'||view==='edit') && (
        <CustomerForm customer={selected} statuses={statuses}
          onSave={f=>handleSave('customer',f,'customer_id','customers','customers')}
          onCancel={back} />
      )}

      {/* ===== STATUSES ===== */}
      {section==='statuses' && view==='list' && (
        <>
          <div style={{display:'flex',justifyContent:'flex-end',marginBottom:14}}>
            <button onClick={()=>navigate('statuses','add')} style={styles.primaryBtn}>+ Add Status</button>
          </div>
          <StatusList statuses={statuses}
            onSelect={s=>navigate('statuses','detail',s)}
            onEdit={s=>navigate('statuses','edit',s)}
            onDelete={s=>handleDelete('status',s.statuse_id,'statuses','statuses',`status "${s.display_name}"`)} />
        </>
      )}
      {section==='statuses' && view==='detail' && selected && (
        <StatusDetail status={selected}
          customers={customers.filter(c=>c.status===selected.statuse_id)}
          onEdit={()=>navigate('statuses','edit',selected)}
          onDelete={()=>handleDelete('status',selected.statuse_id,'statuses','statuses',`status "${selected.display_name}"`)}
          onSelectCustomer={c=>navigate('customers','detail',c)} />
      )}
      {section==='statuses' && (view==='add'||view==='edit') && (
        <StatusForm status={selected}
          onSave={f=>handleSave('status',f,'statuse_id','statuses','statuses')}
          onCancel={back} />
      )}

      {/* ===== PRODUCTS ===== */}
      {section==='products' && view==='list' && (
        <>
          <div style={{display:'flex',justifyContent:'flex-end',marginBottom:14}}>
            <button onClick={()=>navigate('products','add')} style={styles.primaryBtn}>+ Add Product</button>
          </div>
          <ProductList products={products}
            onSelect={p=>navigate('products','detail',p)}
            onEdit={p=>navigate('products','edit',p)}
            onDelete={p=>handleDelete('product',p.product_id,'products','products',`product "${p.display_name||p.sku}"`)} />
        </>
      )}
      {section==='products' && view==='detail' && selected && (
        <ProductDetail product={selected}
          onEdit={()=>navigate('products','edit',selected)}
          onDelete={()=>handleDelete('product',selected.product_id,'products','products',`product "${selected.display_name||selected.sku}"`)} />
      )}
      {section==='products' && (view==='add'||view==='edit') && (
        <ProductForm product={selected}
          onSave={f=>handleSave('product',f,'product_id','products','products')}
          onCancel={back} />
      )}

      {/* ===== ORDERS ===== */}
      {section==='orders' && view==='list' && (
        <>
          <div style={{display:'flex',justifyContent:'flex-end',marginBottom:14}}>
            <button onClick={()=>navigate('orders','add')} style={styles.primaryBtn}>+ Add Order</button>
          </div>
          <OrderList orders={orders} customers={customers}
            onSelect={o=>navigate('orders','detail',o)}
            onEdit={o=>navigate('orders','edit',o)}
            onDelete={o=>handleDelete('order',o.order_id,'orders','orders',`order "${o.order_id}"`)} />
        </>
      )}
      {section==='orders' && view==='detail' && selected && (
        <OrderDetail order={selected} products={products}
          onEdit={()=>navigate('orders','edit',selected)}
          onDelete={()=>handleDelete('order',selected.order_id,'orders','orders',`order "${selected.order_id}"`)}
          onRefresh={fetchAll} />
      )}
      {section==='orders' && (view==='add'||view==='edit') && (
        <OrderForm order={selected} customers={customers}
          onSave={f=>handleSave('order',f,'order_id','orders','orders')}
          onCancel={back} />
      )}

      {/* ===== PAYMENTS ===== */}
      {section==='payments' && view==='list' && (
        <PaymentList payments={payments} orders={orders}
          onEdit={p=>navigate('payments','edit',p)}
          onDelete={p=>handleDelete('payment',p.payment_id,'payments','payments',`payment "${p.payment_id}"`)}
          onRefresh={fetchAll} />
      )}
    </div>
  )
}

const styles = {
  primaryBtn: { background:'#4f46e5',color:'#fff',border:'none',borderRadius:8,padding:'10px 20px',fontSize:15,fontWeight:600,cursor:'pointer' },
  secondaryBtn: { background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:8,padding:'10px 20px',fontSize:15,fontWeight:600,cursor:'pointer' }
}

import { useState } from 'react'

export default function ProductForm({ product, onSave, onCancel }) {
  const [form, setForm] = useState({
    sku: product?.sku || '',
    display_name: product?.display_name || '',
    description: product?.description || '',
    unit_price: product?.unit_price || '',
    cost: product?.cost || '',
    stock_quantity: product?.stock_quantity ?? '',
    is_active: product?.is_active !== false,
  })

  const set = (k) => (e) => setForm(f=>({...f,[k]:e.target.value}))
  const setCheck = (k) => (e) => setForm(f=>({...f,[k]:e.target.checked}))

  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:24,boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <h2 style={{margin:'0 0 20px',fontSize:20,fontWeight:700}}>{product ? 'Edit Product' : 'Add Product'}</h2>
      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:16}}>
        <Field label="SKU *"><input value={form.sku} onChange={set('sku')} style={inp} placeholder="WIDGET-001" /></Field>
        <Field label="Display Name"><input value={form.display_name} onChange={set('display_name')} style={inp} placeholder="Standard Widget" /></Field>
        <Field label="Unit Price"><input type="number" step="0.01" value={form.unit_price} onChange={set('unit_price')} style={inp} placeholder="19.99" /></Field>
        <Field label="Cost"><input type="number" step="0.01" value={form.cost} onChange={set('cost')} style={inp} placeholder="7.50" /></Field>
        <Field label="Stock Quantity"><input type="number" value={form.stock_quantity} onChange={set('stock_quantity')} style={inp} placeholder="100" /></Field>
        <div style={{display:'flex',alignItems:'center',gap:10,alignSelf:'end',paddingBottom:8}}>
          <input type="checkbox" id="active" checked={form.is_active} onChange={setCheck('is_active')} style={{width:18,height:18}} />
          <label htmlFor="active" style={{fontSize:14,fontWeight:600,color:'#374151',cursor:'pointer'}}>Active (available for new orders)</label>
        </div>
        <div style={{gridColumn:'1/-1'}}>
          <Field label="Description">
            <textarea value={form.description} onChange={set('description')} style={{...inp,height:80,resize:'vertical'}} placeholder="Detailed product description..." />
          </Field>
        </div>
      </div>
      <div style={{display:'flex',gap:10,marginTop:20}}>
        <button onClick={()=>onSave(form)} style={styles.primary}>{product ? 'Save Changes' : 'Add Product'}</button>
        <button onClick={onCancel} style={styles.secondary}>Cancel</button>
      </div>
    </div>
  )
}

function Field({ label, children }) {
  return <div><label style={{display:'block',fontSize:13,fontWeight:600,color:'#374151',marginBottom:4}}>{label}</label>{children}</div>
}

const inp = {width:'100%',padding:'8px 12px',fontSize:14,border:'1px solid #d1d5db',borderRadius:8,boxSizing:'border-box'}
const styles = {
  primary: {background:'#4f46e5',color:'#fff',border:'none',borderRadius:8,padding:'10px 20px',fontSize:15,fontWeight:600,cursor:'pointer'},
  secondary: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:8,padding:'10px 20px',fontSize:15,fontWeight:600,cursor:'pointer'}
}

import { useState } from 'react'

const fmtDateInput = (d) => d ? new Date(d).toISOString().split('T')[0] : ''

export default function OrderForm({ order, customers, onSave, onCancel }) {
  const [form, setForm] = useState({
    customer: order?.customer || '',
    order_number: order?.order_number || '',
    order_date: order?.order_date ? fmtDateInput(order.order_date) : new Date().toISOString().split('T')[0],
    order_status: order?.order_status || 'New',
    tax_rate: order?.tax_rate ?? '',
    shipping_address: order?.shipping_address || '',
    billing_address: order?.billing_address || '',
    notes: order?.notes || '',
  })
  const set = k => e => setForm(f=>({...f,[k]:e.target.value}))

  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:24,boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <h2 style={{margin:'0 0 20px',fontSize:20,fontWeight:700}}>{order ? 'Edit Order' : 'Add Order'}</h2>
      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:16}}>
        <Field label="Customer *">
          <select value={form.customer} onChange={set('customer')} style={inp}>
            <option value="">— Select customer —</option>
            {customers.map(c=><option key={c.customer_id} value={c.customer_id}>{c.name}</option>)}
          </select>
        </Field>
        <Field label="Order Number *"><input value={form.order_number} onChange={set('order_number')} style={inp} placeholder="1001" /></Field>
        <Field label="Order Date"><input type="date" value={form.order_date} onChange={set('order_date')} style={inp} /></Field>
        <Field label="Order Status">
          <select value={form.order_status} onChange={set('order_status')} style={inp}>
            {['New','Pending','Processing','Shipped','Delivered','Cancelled','Returned'].map(s=><option key={s}>{s}</option>)}
          </select>
        </Field>
        <Field label="Tax Rate (decimal, e.g. 0.085)">
          <input type="number" step="0.001" value={form.tax_rate} onChange={set('tax_rate')} style={inp} placeholder="0.085" />
        </Field>
        <Field label="Notes"><input value={form.notes} onChange={set('notes')} style={inp} placeholder="Notes..." /></Field>
        <Field label="Shipping Address">
          <textarea value={form.shipping_address} onChange={set('shipping_address')} style={{...inp,height:72,resize:'vertical'}} placeholder="Ship to address" />
        </Field>
        <Field label="Billing Address">
          <textarea value={form.billing_address} onChange={set('billing_address')} style={{...inp,height:72,resize:'vertical'}} placeholder="Bill to address" />
        </Field>
      </div>
      <div style={{display:'flex',gap:10,marginTop:20}}>
        <button onClick={()=>onSave(form)} style={styles.primary}>{order ? 'Save Changes' : 'Add Order'}</button>
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

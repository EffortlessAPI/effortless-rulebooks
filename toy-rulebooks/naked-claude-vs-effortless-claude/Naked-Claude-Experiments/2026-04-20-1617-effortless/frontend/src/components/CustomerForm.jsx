import { useState } from 'react'

export default function CustomerForm({ customer, statuses, onSave, onCancel }) {
  const [form, setForm] = useState({
    name: customer?.name || '',
    company_name: customer?.company_name || '',
    email: customer?.email || '',
    phone: customer?.phone || '',
    billing_address: customer?.billing_address || '',
    shipping_address: customer?.shipping_address || '',
    status: customer?.status || '',
    notes: customer?.notes || '',
  })

  const set = (k) => (e) => setForm(f=>({...f,[k]:e.target.value}))

  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:24,boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <h2 style={{margin:'0 0 20px',fontSize:20,fontWeight:700}}>{customer ? 'Edit Customer' : 'Add Customer'}</h2>
      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:16}}>
        <Field label="Name *"><input value={form.name} onChange={set('name')} style={inp} placeholder="Customer name" /></Field>
        <Field label="Company Name"><input value={form.company_name} onChange={set('company_name')} style={inp} placeholder="Company (optional)" /></Field>
        <Field label="Email"><input value={form.email} onChange={set('email')} style={inp} placeholder="email@example.com" /></Field>
        <Field label="Phone"><input value={form.phone} onChange={set('phone')} style={inp} placeholder="+1 555-000-0000" /></Field>
        <Field label="Status">
          <select value={form.status} onChange={set('status')} style={inp}>
            <option value="">— No status —</option>
            {statuses.map(s=><option key={s.statuse_id} value={s.statuse_id}>{s.display_name}</option>)}
          </select>
        </Field>
        <Field label="Notes"><input value={form.notes} onChange={set('notes')} style={inp} placeholder="Any notes..." /></Field>
        <Field label="Billing Address"><textarea value={form.billing_address} onChange={set('billing_address')} style={{...inp,height:80,resize:'vertical'}} placeholder="Billing address" /></Field>
        <Field label="Shipping Address"><textarea value={form.shipping_address} onChange={set('shipping_address')} style={{...inp,height:80,resize:'vertical'}} placeholder="Shipping address" /></Field>
      </div>
      <div style={{display:'flex',gap:10,marginTop:20}}>
        <button onClick={()=>onSave(form)} style={styles.primary}>{customer ? 'Save Changes' : 'Add Customer'}</button>
        <button onClick={onCancel} style={styles.secondary}>Cancel</button>
      </div>
    </div>
  )
}

function Field({ label, children }) {
  return (
    <div>
      <label style={{display:'block',fontSize:13,fontWeight:600,color:'#374151',marginBottom:4}}>{label}</label>
      {children}
    </div>
  )
}

const inp = {width:'100%',padding:'8px 12px',fontSize:14,border:'1px solid #d1d5db',borderRadius:8,boxSizing:'border-box'}
const styles = {
  primary: {background:'#4f46e5',color:'#fff',border:'none',borderRadius:8,padding:'10px 20px',fontSize:15,fontWeight:600,cursor:'pointer'},
  secondary: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:8,padding:'10px 20px',fontSize:15,fontWeight:600,cursor:'pointer'}
}

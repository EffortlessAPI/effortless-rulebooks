import { useState } from 'react'

export default function StatusForm({ status, onSave, onCancel }) {
  const [form, setForm] = useState({
    display_name: status?.display_name || '',
    description: status?.description || '',
    is_blocking: status?.is_blocking || false,
    sort_order: status?.sort_order || '',
  })

  const set = (k) => (e) => setForm(f=>({...f,[k]:e.target.value}))
  const setCheck = (k) => (e) => setForm(f=>({...f,[k]:e.target.checked}))

  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:24,boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <h2 style={{margin:'0 0 20px',fontSize:20,fontWeight:700}}>{status ? 'Edit Status' : 'Add Status'}</h2>
      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:16}}>
        <Field label="Display Name *"><input value={form.display_name} onChange={set('display_name')} style={inp} placeholder="e.g. On-Hold" /></Field>
        <Field label="Sort Order"><input type="number" value={form.sort_order} onChange={set('sort_order')} style={inp} placeholder="1, 2, 3..." /></Field>
        <div style={{gridColumn:'1/-1'}}>
          <Field label="Description"><textarea value={form.description} onChange={set('description')} style={{...inp,height:80,resize:'vertical'}} placeholder="What does this status mean?" /></Field>
        </div>
        <div style={{display:'flex',alignItems:'center',gap:10}}>
          <input type="checkbox" id="blocking" checked={form.is_blocking} onChange={setCheck('is_blocking')} style={{width:18,height:18,cursor:'pointer'}} />
          <label htmlFor="blocking" style={{fontSize:14,fontWeight:600,color:'#374151',cursor:'pointer'}}>Blocking (customers here are stopped)</label>
        </div>
      </div>
      <div style={{display:'flex',gap:10,marginTop:20}}>
        <button onClick={()=>onSave(form)} style={styles.primary}>{status ? 'Save Changes' : 'Add Status'}</button>
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

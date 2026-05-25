import { StatusBadge } from './CustomerList.jsx'

export default function StatusDetail({ status, customers, onEdit, onDelete, onSelectCustomer }) {
  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,padding:24,boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <div style={{display:'flex',justifyContent:'space-between',alignItems:'flex-start',marginBottom:20}}>
        <div>
          <h2 style={{margin:'0 0 6px',fontSize:22,fontWeight:700}}>{status.display_name}</h2>
          <div style={{color:'#6b7280',fontSize:14}}>{status.description}</div>
        </div>
        <div style={{display:'flex',gap:8}}>
          <button onClick={onEdit} style={styles.btn}>✏️ Edit</button>
          <button onClick={onDelete} style={{...styles.btn,...styles.danger}}>🗑️ Delete</button>
        </div>
      </div>
      <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:12,marginBottom:20}}>
        <Field label="Sort Order" value={status.sort_order} />
        <Field label="Blocking" value={status.is_blocking ? '🔴 Yes — customers here are stopped' : '✅ No — not blocking'} />
        <Field label="Internal ID" value={<code style={{background:'#f3f4f6',padding:'2px 6px',borderRadius:4,fontSize:13}}>{status.statuse_id}</code>} />
      </div>
      <h3 style={{margin:'20px 0 12px',fontSize:16,fontWeight:700}}>Customers in this status ({customers.length})</h3>
      {customers.length===0 ? <div style={{color:'#9ca3af',fontSize:14}}>No customers in this status</div> : (
        <div style={{display:'flex',flexDirection:'column',gap:8}}>
          {customers.map(c=>(
            <div key={c.customer_id} onClick={()=>onSelectCustomer(c)} style={{display:'flex',alignItems:'center',gap:12,padding:'10px 14px',background:'#f9fafb',borderRadius:8,cursor:'pointer',border:'1px solid #e5e7eb'}}
              onMouseEnter={e=>e.currentTarget.style.background='#f3f4f6'}
              onMouseLeave={e=>e.currentTarget.style.background='#f9fafb'}>
              <span style={{fontWeight:600,color:'#4f46e5'}}>{c.name}</span>
              {c.company_name && <span style={{color:'#6b7280',fontSize:13}}>· {c.company_name}</span>}
              {c.is_stopped && <span style={{color:'#dc2626',fontSize:12,fontWeight:700}}>⛔ STOPPED</span>}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

function Field({ label, value }) {
  return <div>
    <div style={{fontSize:12,fontWeight:600,color:'#9ca3af',textTransform:'uppercase',letterSpacing:'0.05em',marginBottom:3}}>{label}</div>
    <div style={{color:'#1a1a2e',fontSize:14}}>{value ?? <span style={{color:'#9ca3af'}}>—</span>}</div>
  </div>
}

const styles = {
  btn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:8,padding:'8px 16px',fontSize:14,fontWeight:600,cursor:'pointer'},
  danger: {background:'#fef2f2',color:'#b91c1c',borderColor:'#fca5a5'}
}

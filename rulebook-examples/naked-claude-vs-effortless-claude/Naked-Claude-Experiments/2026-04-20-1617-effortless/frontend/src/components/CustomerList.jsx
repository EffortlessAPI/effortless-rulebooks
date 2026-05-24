export default function CustomerList({ customers, onSelect, onEdit, onDelete }) {
  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,overflow:'hidden',boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <table style={{width:'100%',borderCollapse:'collapse',fontSize:14}}>
        <thead>
          <tr style={{background:'#f9fafb'}}>
            {['Name','Company','Email/Phone','Status','Stopped','Notes','Actions'].map(h=>(
              <th key={h} style={{padding:'10px 14px',textAlign:'left',fontWeight:600,color:'#6b7280',fontSize:13,borderBottom:'1px solid #e5e7eb'}}>{h}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {customers.length===0 && <tr><td colSpan={7} style={{padding:24,textAlign:'center',color:'#9ca3af'}}>No customers yet</td></tr>}
          {customers.map(c=>(
            <tr key={c.customer_id} style={{borderBottom:'1px solid #f3f4f6'}}
              onMouseEnter={e=>e.currentTarget.style.background='#f9fafb'}
              onMouseLeave={e=>e.currentTarget.style.background=''}>
              <td style={{padding:'10px 14px'}}>
                <span style={{fontWeight:600,color:'#4f46e5',cursor:'pointer'}} onClick={()=>onSelect(c)}>{c.name}</span>
              </td>
              <td style={{padding:'10px 14px',color:'#374151'}}>{c.company_name||''}</td>
              <td style={{padding:'10px 14px',color:'#374151',fontSize:13}}>
                {c.email && <div>{c.email}</div>}
                {c.phone && <div>{c.phone}</div>}
              </td>
              <td style={{padding:'10px 14px'}}>
                {c.status_display_name && <StatusBadge label={c.status_display_name} blocking={c.is_stopped} />}
              </td>
              <td style={{padding:'10px 14px',textAlign:'center'}}>
                {c.is_stopped ? <span style={{color:'#dc2626',fontWeight:700}}>⛔ YES</span> : <span style={{color:'#059669'}}>✓</span>}
              </td>
              <td style={{padding:'10px 14px',color:'#6b7280',fontSize:13,maxWidth:200}}>{c.notes||''}</td>
              <td style={{padding:'10px 14px'}}>
                <button onClick={()=>onEdit(c)} style={styles.btn}>Edit</button>
                <button onClick={()=>onDelete(c)} style={{...styles.btn,...styles.danger}}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export function StatusBadge({ label, blocking }) {
  return (
    <span style={{
      display:'inline-block',padding:'2px 10px',borderRadius:999,fontSize:12,fontWeight:600,
      background: blocking ? '#fee2e2' : '#f0fdf4',
      color: blocking ? '#b91c1c' : '#166534',
      border: blocking ? '1px solid #fca5a5' : '1px solid #86efac'
    }}>{label}</span>
  )
}

const styles = {
  btn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:6,padding:'4px 10px',fontSize:12,fontWeight:600,cursor:'pointer',marginRight:4},
  danger: {background:'#fef2f2',color:'#b91c1c',borderColor:'#fca5a5'}
}

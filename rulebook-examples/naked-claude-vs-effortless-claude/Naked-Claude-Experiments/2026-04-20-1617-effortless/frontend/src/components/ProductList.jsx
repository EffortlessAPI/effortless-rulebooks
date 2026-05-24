const fmt = (n) => n == null ? '-' : Number(n).toLocaleString('en-US', {minimumFractionDigits:2,maximumFractionDigits:2})

export default function ProductList({ products, onSelect, onEdit, onDelete }) {
  return (
    <div style={{background:'#fff',border:'1px solid #e5e7eb',borderRadius:12,overflow:'hidden',boxShadow:'0 1px 3px rgba(0,0,0,0.07)'}}>
      <table style={{width:'100%',borderCollapse:'collapse',fontSize:14}}>
        <thead>
          <tr style={{background:'#f9fafb'}}>
            {['SKU','Display Name','Unit Price','Cost','Stock','Active','Actions'].map(h=>(
              <th key={h} style={{padding:'10px 14px',textAlign:'left',fontWeight:600,color:'#6b7280',fontSize:13,borderBottom:'1px solid #e5e7eb'}}>{h}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {products.length===0 && <tr><td colSpan={7} style={{padding:24,textAlign:'center',color:'#9ca3af'}}>No products yet</td></tr>}
          {products.map(p=>(
            <tr key={p.product_id} style={{borderBottom:'1px solid #f3f4f6'}}
              onMouseEnter={e=>e.currentTarget.style.background='#f9fafb'}
              onMouseLeave={e=>e.currentTarget.style.background=''}>
              <td style={{padding:'10px 14px'}}>
                <span style={{fontFamily:'monospace',fontWeight:700,fontSize:13,color:'#374151',cursor:'pointer'}} onClick={()=>onSelect(p)}>{p.sku}</span>
              </td>
              <td style={{padding:'10px 14px'}}>
                <span style={{fontWeight:600,color:'#4f46e5',cursor:'pointer'}} onClick={()=>onSelect(p)}>{p.display_name}</span>
              </td>
              <td style={{padding:'10px 14px',fontWeight:600}}>${fmt(p.unit_price)}</td>
              <td style={{padding:'10px 14px',color:'#6b7280'}}>${fmt(p.cost)}</td>
              <td style={{padding:'10px 14px',color: p.stock_quantity===0?'#dc2626':'#374151'}}>{p.stock_quantity ?? '—'}</td>
              <td style={{padding:'10px 14px'}}>
                {p.is_active
                  ? <span style={{color:'#059669',fontWeight:600,background:'#f0fdf4',padding:'2px 10px',borderRadius:999,fontSize:12}}>✅ Active</span>
                  : <span style={{color:'#dc2626',fontWeight:600,background:'#fee2e2',padding:'2px 10px',borderRadius:999,fontSize:12}}>❌ Inactive</span>}
              </td>
              <td style={{padding:'10px 14px'}}>
                <button onClick={()=>onEdit(p)} style={styles.btn}>Edit</button>
                <button onClick={()=>onDelete(p)} style={{...styles.btn,...styles.danger}}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

const styles = {
  btn: {background:'#f3f4f6',color:'#374151',border:'1px solid #d1d5db',borderRadius:6,padding:'4px 10px',fontSize:12,fontWeight:600,cursor:'pointer',marginRight:4},
  danger: {background:'#fef2f2',color:'#b91c1c',borderColor:'#fca5a5'}
}

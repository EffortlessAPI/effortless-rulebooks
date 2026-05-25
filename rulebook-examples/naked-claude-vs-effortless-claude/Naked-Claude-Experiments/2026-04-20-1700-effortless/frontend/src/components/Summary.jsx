import { useState, useEffect } from 'react';
import { getSummary } from './api';

function fmt(n) {
  return typeof n === 'number' ? '$' + n.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '$0.00';
}

export default function Summary() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  async function load() {
    try {
      const d = await getSummary();
      setData(d);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { load(); }, []);

  // Expose refresh for parent
  Summary.refresh = load;

  if (loading || !data) return null;

  return (
    <div style={{ marginBottom: '28px' }}>
      {/* Top stat cards — row 1 */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '14px', marginBottom: '14px' }}>
        <StatCard label="Total Clients" value={data.total_clients} accent="#4f46e5" bg="#f5f3ff" />
        <StatCard
          label="Stopped Clients"
          value={data.stopped_clients}
          accent={data.stopped_clients > 0 ? '#dc2626' : '#16a34a'}
          bg={data.stopped_clients > 0 ? '#fef2f2' : '#f0fdf4'}
          highlight={data.stopped_clients > 0}
        />
        <StatCard label="Total Invoices" value={data.total_invoices} accent="#0891b2" bg="#ecfeff" />
        <StatCard label="Active Products" value={`${data.active_products} / ${data.total_products}`} accent="#059669" bg="#f0fdf4" />
      </div>

      {/* Row 2: money metrics */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '14px', marginBottom: '14px' }}>
        <StatCard label="Total Revenue" value={fmt(data.total_revenue)} accent="#7c3aed" bg="#faf5ff" />
        <StatCard
          label="Outstanding"
          value={fmt(data.total_outstanding)}
          accent={data.total_outstanding > 0 ? '#dc2626' : '#16a34a'}
          bg={data.total_outstanding > 0 ? '#fef2f2' : '#f0fdf4'}
          highlight={data.total_outstanding > 0}
        />
        <StatCard label="Total Payments" value={data.total_payments} accent="#d97706" bg="#fffbeb" />
      </div>

      {/* Row 3: v6 business signals */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '14px', marginBottom: '14px' }}>
        <StatCard label="⭐ VIP Clients" value={data.vip_clients ?? 0} accent="#b45309" bg="#fef3c7" />
        <StatCard label="💰 Big Orders" value={data.big_orders ?? 0} accent="#b45309" bg="#fef3c7" />
        <StatCard label="⭐ High-Margin Products" value={data.high_margin_products ?? 0} accent="#b45309" bg="#fef3c7" />
        <StatCard
          label="Pending Approvals"
          value={`${data.pending_approvals ?? 0} / ${data.total_approvals ?? 0}`}
          accent={data.pending_approvals > 0 ? '#b45309' : '#16a34a'}
          bg={data.pending_approvals > 0 ? '#fffbeb' : '#f0fdf4'}
          highlight={data.pending_approvals > 0}
        />
      </div>

      {/* Distributions row */}
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '14px' }}>
        {/* Client Status Distribution */}
        {data.distribution && data.distribution.length > 0 && (
          <DistCard title="Client Statuses">
            {data.distribution.map(s => (
              <DistRow key={s.statuse_id} label={s.display_name} count={s.count} blocking={s.is_blocking} />
            ))}
          </DistCard>
        )}

        {/* Invoice Status Distribution */}
        {data.invoice_status_distribution && data.invoice_status_distribution.length > 0 && (
          <DistCard title="Invoice Statuses">
            {data.invoice_status_distribution.map(s => (
              <DistRow key={s.order_status} label={s.order_status || 'Unknown'} count={s.count} />
            ))}
          </DistCard>
        )}

        {/* Payment Status Distribution */}
        {data.payment_status_distribution && data.payment_status_distribution.length > 0 && (
          <DistCard title="Payment Statuses">
            {data.payment_status_distribution.map(s => (
              <div key={s.payment_status} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '3px 0', fontSize: '0.85rem' }}>
                <span style={{ fontWeight: 600, color: '#374151' }}>{s.payment_status || 'Unknown'}</span>
                <div style={{ display: 'flex', gap: '6px', alignItems: 'center' }}>
                  {s.payment_status === 'Completed' && (
                    <span style={{ color: '#059669', fontSize: '0.78rem', fontWeight: 700 }}>
                      {fmt(parseFloat(s.completed_amount || 0))}
                    </span>
                  )}
                  <span style={{
                    background: s.count > 0 ? '#dbeafe' : '#f3f4f6',
                    color: s.count > 0 ? '#1e40af' : '#9ca3af',
                    padding: '1px 7px', borderRadius: '10px', fontWeight: 700, fontSize: '0.78rem'
                  }}>{s.count}</span>
                </div>
              </div>
            ))}
          </DistCard>
        )}
      </div>
    </div>
  );
}

function StatCard({ label, value, accent, bg, highlight }) {
  return (
    <div style={{
      background: bg,
      border: `1px solid ${accent}33`,
      borderRadius: '10px',
      padding: '16px 20px',
      boxShadow: highlight ? `0 0 0 2px ${accent}44` : 'none'
    }}>
      <div style={{ fontSize: '0.72rem', fontWeight: 700, color: accent, textTransform: 'uppercase', letterSpacing: '0.07em', marginBottom: '6px' }}>
        {label}
      </div>
      <div style={{ fontSize: '1.8rem', fontWeight: 800, color: accent, lineHeight: 1 }}>
        {value}
      </div>
    </div>
  );
}

function DistCard({ title, children }) {
  return (
    <div style={{ background: '#fff', border: '1px solid #e5e7eb', borderRadius: '10px', padding: '14px 18px' }}>
      <div style={{ fontSize: '0.72rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: '10px' }}>
        {title}
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
        {children}
      </div>
    </div>
  );
}

function DistRow({ label, count, blocking }) {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '3px 0', fontSize: '0.85rem' }}>
      <span style={{ display: 'flex', alignItems: 'center', gap: '4px', fontWeight: 600, color: '#374151' }}>
        {blocking && <span style={{ color: '#dc2626', fontSize: '0.75rem' }}>🔴</span>}
        {label}
      </span>
      <span style={{
        background: count > 0 ? '#dbeafe' : '#f3f4f6',
        color: count > 0 ? '#1e40af' : '#9ca3af',
        padding: '1px 7px', borderRadius: '10px', fontWeight: 700, fontSize: '0.78rem'
      }}>{count}</span>
    </div>
  );
}

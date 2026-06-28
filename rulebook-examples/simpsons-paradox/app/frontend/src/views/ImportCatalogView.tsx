import { useEffect, useMemo, useState } from 'react';
import { api } from '../api';
import type {
  CandidateStudyRow,
  CorpusCatalogSummary,
  DomainExpansionTarget,
  StudyImportTemplateStep,
} from '../types';

const STATUS_COLORS: Record<string, string> = {
  imported: '#34a853',
  candidate: '#1a73e8',
  blocked: '#9aa0a6',
  rejected: '#ea4335',
};

export function ImportCatalogView() {
  const [summary, setSummary] = useState<CorpusCatalogSummary | null>(null);
  const [candidates, setCandidates] = useState<CandidateStudyRow[]>([]);
  const [domains, setDomains] = useState<DomainExpansionTarget[]>([]);
  const [template, setTemplate] = useState<StudyImportTemplateStep[]>([]);
  const [statusFilter, setStatusFilter] = useState<string>('candidate');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      api.corpusCatalogSummary(),
      api.candidateStudyCatalog(),
      api.domainExpansionTargets(),
      api.studyImportTemplate(),
    ])
      .then(([s, c, d, t]) => {
        setSummary(s);
        setCandidates(c);
        setDomains(d);
        setTemplate(t);
      })
      .finally(() => setLoading(false));
  }, []);

  const filtered = useMemo(() => {
    const rows =
      statusFilter === 'all'
        ? candidates
        : candidates.filter(r => r.ingestion_status === statusFilter);
    return [...rows].sort((a, b) => a.priority - b.priority || a.title.localeCompare(b.title));
  }, [candidates, statusFilter]);

  if (loading) return <div className="loading">Loading…</div>;
  if (!summary) return <div className="error">No catalog summary found.</div>;

  return (
    <div style={{ padding: '1.5rem', maxWidth: 1100 }}>
      <h2 style={{ margin: '0 0 .25rem' }}>Import Backlog</h2>
      <p style={{ color: '#888', margin: '0 0 1.25rem', fontSize: 14 }}>
        Curated catalog of published Simpson&apos;s-paradox-eligible studies. Loop 51–54 prep —
        encode candidates using the six-step template, then flip catalog status to imported.
      </p>

      <div
        style={{
          padding: '12px 16px',
          borderRadius: 8,
          marginBottom: '1.25rem',
          background: summary.import_session_ready ? '#e6f4ea' : '#fef7e0',
          border: `1px solid ${summary.import_session_ready ? '#34a853' : '#fbbc04'}`,
          fontSize: 14,
        }}
      >
        <strong>{summary.catalog_witness_note}</strong>
        <div style={{ marginTop: 4, color: '#555' }}>
          {summary.imported_count} imported · {summary.candidate_count} candidates ·{' '}
          {summary.ready_to_encode_count} encode-ready · {summary.blocked_count} blocked
        </div>
      </div>

      <h3 style={{ fontSize: 16, margin: '0 0 .75rem' }}>Domain gaps</h3>
      <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13, marginBottom: '1.5rem' }}>
        <thead>
          <tr style={{ borderBottom: '2px solid #ddd', textAlign: 'left' }}>
            <th style={{ padding: '6px 8px' }}>Domain</th>
            <th style={{ padding: '6px 8px' }}>In corpus</th>
            <th style={{ padding: '6px 8px' }}>Queued</th>
            <th style={{ padding: '6px 8px' }}>Target</th>
            <th style={{ padding: '6px 8px' }}>Gap</th>
          </tr>
        </thead>
        <tbody>
          {domains.map(d => (
            <tr key={d.domain_target_id} style={{ borderBottom: '1px solid #eee' }}>
              <td style={{ padding: '6px 8px' }}>{d.domain}</td>
              <td style={{ padding: '6px 8px' }}>{d.current_imported_count}</td>
              <td style={{ padding: '6px 8px' }}>{d.candidate_queued_count}</td>
              <td style={{ padding: '6px 8px' }}>{d.target_min_count}</td>
              <td style={{ padding: '6px 8px', color: d.is_under_represented ? '#c5221f' : '#137333' }}>
                {d.gap_count}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <h3 style={{ fontSize: 16, margin: '0 0 .75rem' }}>Encoding checklist</h3>
      <ol style={{ fontSize: 13, margin: '0 0 1.5rem', paddingLeft: '1.25rem', color: '#444' }}>
        {template.map(step => (
          <li key={step.template_step_id} style={{ marginBottom: 6 }}>
            <strong>{step.target_table}</strong> — {step.row_description}
            <div style={{ color: '#888', fontSize: 12 }}>{step.mechanical_check}</div>
          </li>
        ))}
      </ol>

      <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap', marginBottom: '1rem' }}>
        {['candidate', 'imported', 'blocked', 'all'].map(s => (
          <button
            key={s}
            type="button"
            onClick={() => setStatusFilter(s)}
            style={{
              padding: '4px 12px',
              borderRadius: 16,
              border: statusFilter === s ? '2px solid #1a73e8' : '1px solid #ccc',
              background: statusFilter === s ? '#e8f0fe' : '#fff',
              cursor: 'pointer',
              fontSize: 13,
            }}
          >
            {s}
          </button>
        ))}
      </div>

      <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
        <thead>
          <tr style={{ borderBottom: '2px solid #ddd', textAlign: 'left' }}>
            <th style={{ padding: '6px 8px' }}>Pri</th>
            <th style={{ padding: '6px 8px' }}>Status</th>
            <th style={{ padding: '6px 8px' }}>Title</th>
            <th style={{ padding: '6px 8px' }}>Domain</th>
            <th style={{ padding: '6px 8px' }}>Stratum var</th>
            <th style={{ padding: '6px 8px' }}>Exp. type</th>
            <th style={{ padding: '6px 8px' }}>Proposed ID</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map(row => (
            <tr key={row.candidate_id} style={{ borderBottom: '1px solid #eee' }}>
              <td style={{ padding: '6px 8px' }}>{row.priority}</td>
              <td style={{ padding: '6px 8px', color: STATUS_COLORS[row.ingestion_status] ?? '#333' }}>
                {row.ingestion_status}
              </td>
              <td style={{ padding: '6px 8px', maxWidth: 280 }}>
                <div>{row.title}</div>
                <div style={{ color: '#888', fontSize: 11 }}>{row.data_source_note}</div>
              </td>
              <td style={{ padding: '6px 8px' }}>{row.domain}</td>
              <td style={{ padding: '6px 8px', fontFamily: 'monospace', fontSize: 12 }}>
                {row.stratum_variable_name}
              </td>
              <td style={{ padding: '6px 8px', fontWeight: 600 }}>{row.expected_distortion_type}</td>
              <td style={{ padding: '6px 8px', fontFamily: 'monospace', fontSize: 11 }}>
                {row.proposed_study_id}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

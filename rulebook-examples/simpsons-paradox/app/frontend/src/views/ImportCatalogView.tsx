import { useEffect, useMemo, useState } from 'react';
import { api } from '../api';
import { ViewDagScan } from '../components/DagValue';
import { Cell } from '../components/dag-display';
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
        <strong><Cell table="CorpusCatalogSummary" col="catalog_witness_note">{summary.catalog_witness_note}</Cell></strong>
        <div style={{ marginTop: 4, color: '#555' }}>
          <Cell table="CorpusCatalogSummary" col="imported_count">{summary.imported_count}</Cell> imported ·{' '}
          <Cell table="CorpusCatalogSummary" col="candidate_count">{summary.candidate_count}</Cell> candidates ·{' '}
          <Cell table="CorpusCatalogSummary" col="ready_to_encode_count">{summary.ready_to_encode_count}</Cell> encode-ready ·{' '}
          <Cell table="CorpusCatalogSummary" col="blocked_count">{summary.blocked_count}</Cell> blocked
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
              <td style={{ padding: '6px 8px' }}>
                <Cell table="DomainExpansionTargets" col="current_imported_count">{d.current_imported_count}</Cell>
              </td>
              <td style={{ padding: '6px 8px' }}>
                <Cell table="DomainExpansionTargets" col="candidate_queued_count">{d.candidate_queued_count}</Cell>
              </td>
              <td style={{ padding: '6px 8px' }}>
                <Cell table="DomainExpansionTargets" col="target_min_count">{d.target_min_count}</Cell>
              </td>
              <td style={{ padding: '6px 8px', color: d.is_under_represented ? '#c5221f' : '#137333' }}>
                <Cell table="DomainExpansionTargets" col="gap_count">{d.gap_count}</Cell>
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
            <div style={{ color: '#888', fontSize: 12 }}>
              <Cell table="StudyImportTemplate" col="mechanical_check">{step.mechanical_check}</Cell>
            </div>
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
              <td style={{ padding: '6px 8px' }}>
                <Cell table="CandidateStudyCatalog" col="priority">{row.priority}</Cell>
              </td>
              <td style={{ padding: '6px 8px', color: STATUS_COLORS[row.ingestion_status] ?? '#333' }}>
                <Cell table="CandidateStudyCatalog" col="ingestion_status">{row.ingestion_status}</Cell>
              </td>
              <td style={{ padding: '6px 8px', maxWidth: 280 }}>
                <div>{row.title}</div>
                <div style={{ color: '#888', fontSize: 11 }}>{row.data_source_note}</div>
              </td>
              <td style={{ padding: '6px 8px' }}>{row.domain}</td>
              <td style={{ padding: '6px 8px', fontFamily: 'monospace', fontSize: 12 }}>
                {row.stratum_variable_name}
              </td>
              <td style={{ padding: '6px 8px', fontWeight: 600 }}>
                <Cell table="CandidateStudyCatalog" col="expected_distortion_type">{row.expected_distortion_type}</Cell>
              </td>
              <td style={{ padding: '6px 8px', fontFamily: 'monospace', fontSize: 11 }}>
                {row.proposed_study_id}
                {row.is_imported && (
                  <> · linked <Cell table="CandidateStudyCatalog" col="linked_study_id">{row.linked_study_id}</Cell></>
                )}
                {row.observed_distortion_type && (
                  <> · observed <Cell table="CandidateStudyCatalog" col="observed_distortion_type">{row.observed_distortion_type}</Cell></>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <ViewDagScan ready={!loading} deps={[filtered, candidates, domains, summary, statusFilter]} />
    </div>
  );
}

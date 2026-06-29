import fs from 'fs';
import path from 'path';
import PDFDocument from 'pdfkit';
import { fileURLToPath } from 'url';
import { query } from './db.js';
import {
  commitUrl,
  projectFileUrl,
  projectTreeUrl,
  PROJECT_LINKS,
  tagUrl,
} from './github-links.js';
import {
  CATEGORY_ORDER,
  SCOPE_BOUNDARY_TEXT,
  SCOPE_BOUNDARY_TITLE,
  SWEEP_CONTRACT,
  LIMITS_TEXT,
  conclusionPdfLabel,
  domainCaveat,
  formatObservedMetric,
  isConsistencyCheck,
  tierConclusionCounts,
} from '../shared/epistemic-framing.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const projectRoot = path.resolve(__dirname, '../..');
const rulebookPath = path.join(
  projectRoot,
  'effortless-rulebook/simpsons-paradox-rulebook.json',
);

const PAGE_MARGIN = 54;
const BODY_SIZE = 10;
const SMALL_SIZE = 8.5;
const TITLE_SIZE = 20;
const SECTION_SIZE = 13;

interface SummaryRow {
  model_summary_id: string;
  study_count: number;
  reversal_count: number;
  explained_count: number;
  type_a_count: number;
  type_b_count: number;
  type_d_count: number;
  type_c_plus_count: number;
  type_c_minus_count: number;
  danger_tier_count: number;
  caution_tier_count: number;
  safe_tier_count: number;
  avg_signal_purity: number | null;
  avg_signal_purity_reversal: number | null;
  avg_signal_purity_non_reversal: number | null;
  sign_flip_signal_purity_max: number | null;
  real_study_count: number | null;
  economics_sign_flip_count: number | null;
  domain_diversity_note: string | null;
  latent_type_d_count: number | null;
  latent_type_d_fraction: number | null;
  discovery_witness_note: string | null;
}

interface ConclusionRow {
  conclusion_id: string;
  category: string;
  status: string;
  title: string;
  evidence: string | null;
  witnessed_in_loop: string | null;
  witnessed_in_loop_commit_hash: string | null;
  witnessed_in_loop_commit_short: string | null;
  witnessed_in_loop_commit_date: string | null;
  witnessed_in_loop_git_tag: string | null;
  invariant_protecting_count: number | null;
}

interface DiscoveryHypothesisRow {
  hypothesis_id: string;
  statement: string;
  expected_outcome: string;
  epistemic_tier: string | null;
}

interface DiscoveryFindingRow {
  hypothesis_id: string;
  observed_metric: string | null;
  is_confirmed: boolean | null;
}

interface InvariantRow {
  invariant_check_id: string;
  natural_language: string;
  pass_count: number;
  fail_count: number;
  is_green: boolean;
  severity: string;
  protects_conclusion: string | null;
}

interface SpotlightRow {
  study: string;
  title: string | null;
  domain: string | null;
  distortion_type: string;
  paradox_strength: number;
  allocation_distortion: number;
  signal_purity: number | null;
  screening_tier: string;
}

function readTagline(): string {
  const rb = JSON.parse(fs.readFileSync(rulebookPath, 'utf8')) as {
    __meta__?: { data?: Array<{ MetaKey: string; StringValue?: string }> };
  };
  const row = rb.__meta__?.data?.find(m => m.MetaKey === 'tagline');
  if (!row?.StringValue) {
    throw new Error('__meta__ tagline missing in simpsons-paradox-rulebook.json');
  }
  return row.StringValue;
}

function fmtNum(v: number | string | null | undefined, dp = 3): string {
  if (v == null || v === '') return '—';
  return Number(v).toFixed(dp);
}

function fmtPct(v: number | null | undefined): string {
  if (v == null) return '—';
  return `${(Number(v) * 100).toFixed(1)}%`;
}

type PdfDoc = InstanceType<typeof PDFDocument>;

function ensureSpace(doc: PdfDoc, y: number, needed = 40): number {
  const bottom = doc.page.height - PAGE_MARGIN;
  if (y + needed > bottom) {
    doc.addPage();
    return PAGE_MARGIN;
  }
  return y;
}

function sectionHeading(doc: PdfDoc, y: number, text: string): number {
  y = ensureSpace(doc, y, 28);
  doc.font('Helvetica-Bold').fontSize(SECTION_SIZE).fillColor('#111111');
  doc.text(text, PAGE_MARGIN, y);
  return doc.y + 10;
}

function bodyText(
  doc: PdfDoc,
  y: number,
  text: string,
  opts?: { size?: number; color?: string; link?: string },
): number {
  const size = opts?.size ?? BODY_SIZE;
  const color = opts?.color ?? '#333333';
  doc.font('Helvetica').fontSize(size).fillColor(color);
  y = ensureSpace(doc, y, 24);
  doc.text(text, PAGE_MARGIN, y, {
    width: doc.page.width - PAGE_MARGIN * 2,
    lineGap: 2,
    link: opts?.link,
  });
  return doc.y + 6;
}

function bulletLine(
  doc: PdfDoc,
  y: number,
  label: string,
  value: string,
  link?: string,
): number {
  y = ensureSpace(doc, y, 16);
  doc.font('Helvetica').fontSize(BODY_SIZE).fillColor('#333333');
  doc.text(`${label}: `, PAGE_MARGIN, y, { continued: true });
  if (link) {
    doc.fillColor('#1155cc').text(value, { link, underline: true });
  } else {
    doc.fillColor('#333333').text(value);
  }
  return doc.y + 2;
}

function conclusionEvidenceLink(c: ConclusionRow): string | undefined {
  if (c.witnessed_in_loop_commit_hash) {
    return commitUrl(c.witnessed_in_loop_commit_hash);
  }
  if (c.witnessed_in_loop_git_tag) {
    return tagUrl(c.witnessed_in_loop_git_tag);
  }
  if (c.witnessed_in_loop) {
    return projectFileUrl(
      `effortless-rulebook/simpsons-paradox-rulebook.json`,
    );
  }
  return undefined;
}

function renderDiscoveryHypothesis(
  doc: PdfDoc,
  y: number,
  h: DiscoveryHypothesisRow,
  f: DiscoveryFindingRow | undefined,
  subtitle: string,
): number {
  const pass = f?.is_confirmed === true;
  y = ensureSpace(doc, y, 40);
  doc.font('Helvetica-Bold').fontSize(BODY_SIZE).fillColor('#111111');
  doc.text(`${h.hypothesis_id} — ${pass ? 'PASS' : 'FAIL'} (${subtitle})`, PAGE_MARGIN, y);
  y = doc.y + 2;
  y = bodyText(doc, y, h.statement, { size: SMALL_SIZE });
  y = bulletLine(doc, y, 'Expected', h.expected_outcome);
  if (f?.observed_metric) {
    y = bulletLine(doc, y, 'Observed', formatObservedMetric(f.observed_metric));
  }
  return y + 4;
}

function renderConclusionBlock(doc: PdfDoc, y: number, c: ConclusionRow): number {
  y = ensureSpace(doc, y, 48);
  doc.font('Helvetica-Bold').fontSize(BODY_SIZE).fillColor('#111111');
  doc.text(`${c.conclusion_id} · ${conclusionPdfLabel(c.category)}`, PAGE_MARGIN, y);
  y = doc.y + 2;
  y = bodyText(doc, y, c.title);
  const caveat = domainCaveat(c.conclusion_id);
  if (caveat) {
    y = bodyText(doc, y, caveat, { size: SMALL_SIZE, color: '#884400' });
  }
  if (c.evidence) {
    const evidence =
      c.evidence.length > 520 ? `${c.evidence.slice(0, 517)}…` : c.evidence;
    y = bodyText(doc, y, evidence, { size: SMALL_SIZE, color: '#444444' });
  }
  if (c.witnessed_in_loop) {
    const replay = c.witnessed_in_loop_commit_short
      ? `${c.witnessed_in_loop} (${c.witnessed_in_loop_commit_short}${c.witnessed_in_loop_commit_date ? `, ${c.witnessed_in_loop_commit_date}` : ''})`
      : c.witnessed_in_loop;
    y = bulletLine(doc, y, 'Loop replay', replay, conclusionEvidenceLink(c));
  }
  if (c.invariant_protecting_count && c.invariant_protecting_count > 0) {
    y = bulletLine(
      doc,
      y,
      'Protecting invariants',
      String(c.invariant_protecting_count),
    );
  }
  return y + 6;
}

export async function buildSummaryPdf(): Promise<Buffer> {
  const [
    summaryRows,
    conclusionRows,
    hypothesisRows,
    findingRows,
    invariantRows,
    spotlightRows,
  ] = await Promise.all([
    query<SummaryRow>('SELECT * FROM vw_model_summary LIMIT 1'),
    query<ConclusionRow>(
      'SELECT * FROM vw_conclusions ORDER BY conclusion_id',
    ),
    query<DiscoveryHypothesisRow>(
      `SELECT hypothesis_id, statement, expected_outcome,
              epistemic_tier
       FROM vw_discovery_hypotheses ORDER BY hypothesis_id`,
    ),
    query<DiscoveryFindingRow>(
      'SELECT hypothesis_id, observed_metric, is_confirmed FROM vw_discovery_findings ORDER BY hypothesis_id',
    ),
    query<InvariantRow>(
      `SELECT invariant_check_id, natural_language, pass_count, fail_count, is_green, severity, protects_conclusion
       FROM vw_invariant_checks ORDER BY invariant_check_id`,
    ),
    query<SpotlightRow>(
      `SELECT tr.study, s.title, s.domain, tr.distortion_type, tr.paradox_strength,
              tr.allocation_distortion, tr.signal_purity, tr.screening_tier
       FROM vw_treatment_rankings tr
       JOIN vw_studies s ON s.study_id = tr.study
       ORDER BY tr.paradox_strength DESC
       LIMIT 8`,
    ),
  ]);

  const summary = summaryRows[0];
  if (!summary) {
    throw new Error('vw_model_summary returned no rows — cannot build summary PDF');
  }

  const tagline = readTagline();
  const witnessed = conclusionRows.filter(c => c.status === 'witnessed');
  const tiers = tierConclusionCounts(conclusionRows);
  const findingsByHypothesis = new Map(
    findingRows.map(f => [f.hypothesis_id, f]),
  );
  const criticalInvariants = invariantRows.filter(i => i.severity === 'critical');
  const criticalFails = criticalInvariants.reduce(
    (n, i) => n + Number(i.fail_count),
    0,
  );
  const generatedAt = new Date().toISOString().slice(0, 10);

  const consistencyHypotheses = hypothesisRows.filter(h =>
    isConsistencyCheck(h.hypothesis_id) ||
    h.epistemic_tier === 'consistency-check',
  );
  const corpusHypotheses = hypothesisRows.filter(
    h =>
      !isConsistencyCheck(h.hypothesis_id) &&
      h.epistemic_tier !== 'consistency-check',
  );
  const corpusConfirmed = corpusHypotheses.filter(
    h => findingsByHypothesis.get(h.hypothesis_id)?.is_confirmed === true,
  ).length;
  const consistencyConfirmed = consistencyHypotheses.filter(
    h => findingsByHypothesis.get(h.hypothesis_id)?.is_confirmed === true,
  ).length;

  const chunks: Buffer[] = [];
  const doc = new PDFDocument({
    size: 'LETTER',
    margins: { top: PAGE_MARGIN, bottom: PAGE_MARGIN, left: PAGE_MARGIN, right: PAGE_MARGIN },
    info: {
      Title: "Simpson's Paradox — Corpus Summary",
      Author: 'Effortless Rulebook',
      Subject: 'Witnessed corpus snapshot with deductive/empirical framing',
    },
  });

  doc.on('data', (chunk: Buffer) => chunks.push(chunk));

  const finished = new Promise<Buffer>((resolve, reject) => {
    doc.on('end', () => resolve(Buffer.concat(chunks)));
    doc.on('error', reject);
  });

  let y = PAGE_MARGIN;

  doc.font('Helvetica-Bold').fontSize(TITLE_SIZE).fillColor('#111111');
  doc.text("Simpson's Paradox — Corpus Summary", PAGE_MARGIN, y);
  y = doc.y + 4;

  y = bodyText(doc, y, tagline, { size: 11, color: '#555555' });
  y = bodyText(
    doc,
    y,
    `Generated ${generatedAt} from live vw_* views — same data as /conclusions. SSoT is the rulebook JSON.`,
    { size: SMALL_SIZE, color: '#666666' },
  );

  y = sectionHeading(doc, y, SCOPE_BOUNDARY_TITLE);
  y = bodyText(doc, y, SCOPE_BOUNDARY_TEXT, { size: BODY_SIZE, color: '#333333' });

  y = sectionHeading(doc, y, 'Scope & limits');
  y = bodyText(doc, y, LIMITS_TEXT, { size: SMALL_SIZE, color: '#444444' });
  y = bodyText(doc, y, SWEEP_CONTRACT, { size: SMALL_SIZE, color: '#444444' });

  y = sectionHeading(doc, y, 'Epistemic summary (tiered)');
  y = bulletLine(doc, y, 'Proved by construction (theorem)', String(tiers.proved));
  y = bulletLine(
    doc,
    y,
    'Established in instrument (instrument · taxonomy · methodology · scope)',
    String(tiers.instrument),
  );
  y = bulletLine(
    doc,
    y,
    'Observed in this corpus (domain — provisional)',
    String(tiers.corpus),
  );
  y = bulletLine(doc, y, 'Total conclusions', String(tiers.total));
  y = bulletLine(
    doc,
    y,
    'Corpus hypotheses (loop-61)',
    `${corpusConfirmed} / ${corpusHypotheses.length} confirmed`,
  );
  y = bulletLine(
    doc,
    y,
    'Consistency checks (definition-linked)',
    `${consistencyConfirmed} / ${consistencyHypotheses.length} pass`,
  );
  if (summary.latent_type_d_count != null && summary.type_d_count != null) {
    y = bulletLine(
      doc,
      y,
      'Latent Type-D (under sweep contract above)',
      `${summary.latent_type_d_count} / ${summary.type_d_count} (${fmtPct(summary.latent_type_d_fraction)})`,
    );
  }
  y = bulletLine(doc, y, 'Studies in corpus', String(summary.study_count));
  y = bulletLine(
    doc,
    y,
    'Distortion taxonomy',
    `A ${summary.type_a_count} · B ${summary.type_b_count} · C+ ${summary.type_c_plus_count} · C− ${summary.type_c_minus_count} · D ${summary.type_d_count}`,
  );

  y = sectionHeading(doc, y, 'Pre-registered discovery (loop-61)');

  y = bodyText(
    doc,
    y,
    'Consistency checks — confirm implementation matches stated algebra (same tier as conc-12 / inv-signal-purity-sign-flip). Not independent discoveries about nature.',
    { size: SMALL_SIZE, color: '#666666' },
  );
  if (consistencyHypotheses.length === 0) {
    y = bodyText(doc, y, '(none)');
  } else {
    for (const h of consistencyHypotheses) {
      y = renderDiscoveryHypothesis(
        doc,
        y,
        h,
        findingsByHypothesis.get(h.hypothesis_id),
        'consistency check',
      );
    }
  }

  y += 4;
  y = bodyText(
    doc,
    y,
    'Corpus hypotheses — contingent patterns on this convenience sample. Directional inequalities only; not inferential. See conc-14.',
    { size: SMALL_SIZE, color: '#666666' },
  );
  if (corpusHypotheses.length === 0) {
    y = bodyText(doc, y, '(none)');
  } else {
    for (const h of corpusHypotheses) {
      y = renderDiscoveryHypothesis(
        doc,
        y,
        h,
        findingsByHypothesis.get(h.hypothesis_id),
        'corpus hypothesis',
      );
    }
  }

  for (const cat of CATEGORY_ORDER) {
    const group = witnessed.filter(c => c.category === cat);
    if (group.length === 0) continue;
    y = sectionHeading(
      doc,
      y,
      `${conclusionPdfLabel(cat)} (${group.length})`,
    );
    for (const c of group) {
      y = renderConclusionBlock(doc, y, c);
    }
  }

  y = sectionHeading(doc, y, 'Invariant health');
  y = bulletLine(
    doc,
    y,
    'Critical invariant failures',
    String(criticalFails),
    criticalFails === 0 ? PROJECT_LINKS.invariantChecks() : undefined,
  );
  const failed = invariantRows.filter(i => Number(i.fail_count) > 0);
  if (failed.length > 0) {
    for (const inv of failed.slice(0, 12)) {
      y = bodyText(
        doc,
        y,
        `${inv.invariant_check_id}: ${inv.natural_language} (fail=${inv.fail_count})`,
        { size: SMALL_SIZE },
      );
    }
  } else {
    y = bodyText(
      doc,
      y,
      `All ${criticalInvariants.length} critical invariants pass — transpiler regression checks against stated algebra.`,
      { size: SMALL_SIZE },
    );
  }

  y = sectionHeading(doc, y, 'Most misleading studies (by paradox strength)');
  for (const row of spotlightRows) {
    y = ensureSpace(doc, y, 20);
    doc.font('Helvetica').fontSize(BODY_SIZE).fillColor('#333333');
    doc.text(
      `${row.title ?? row.study} — type ${row.distortion_type}, strength ${fmtNum(row.paradox_strength, 4)}, distortion ${fmtNum(row.allocation_distortion)}, purity ${fmtPct(row.signal_purity)} (${row.screening_tier})`,
      PAGE_MARGIN,
      y,
      { width: doc.page.width - PAGE_MARGIN * 2 },
    );
    y = doc.y + 2;
  }

  y = sectionHeading(doc, y, 'Rulebook & repo (full derivation)');
  y = bodyText(
    doc,
    y,
    'Formulas, Loops build history, InvariantChecks, and Conclusions rows live in the rulebook JSON. README.md carries the full framing.',
    { size: SMALL_SIZE, color: '#666666' },
  );
  y = bulletLine(doc, y, 'Project folder', projectTreeUrl(), projectTreeUrl());
  y = bulletLine(doc, y, 'README (full repo guide)', 'README.md', PROJECT_LINKS.readme());
  y = bulletLine(
    doc,
    y,
    'Rulebook SSoT (formulas, Loops, InvariantChecks, Conclusions)',
    'effortless-rulebook/simpsons-paradox-rulebook.json',
    PROJECT_LINKS.rulebook(),
  );
  y = bulletLine(
    doc,
    y,
    'Rulespeak (plain-English business rules)',
    'rulespeak/rulespeak.md',
    PROJECT_LINKS.rulespeak(),
  );
  y = bulletLine(
    doc,
    y,
    'Postgres substrate (views, init-db.sh)',
    'effortless-postgres/README.md',
    PROJECT_LINKS.postgres(),
  );
  y = bulletLine(
    doc,
    y,
    'Standalone HTML explorer',
    'simpsons-paradox-explorer.html',
    PROJECT_LINKS.explorerHtml(),
  );
  y = bulletLine(
    doc,
    y,
    'Live API (with ./start.sh)',
    'GET localhost:3001/api/invariant-checks · /api/treatment-rankings · /api/model-summary',
  );

  const taggedConclusions = witnessed.filter(c => c.witnessed_in_loop_git_tag);
  if (taggedConclusions.length > 0) {
    y += 4;
    y = bodyText(doc, y, 'Loop landing tags (replay checkpoints):', {
      size: SMALL_SIZE,
      color: '#555555',
    });
    for (const c of taggedConclusions.slice(0, 10)) {
      const tag = c.witnessed_in_loop_git_tag!;
      y = bulletLine(doc, y, tag, c.conclusion_id, tagUrl(tag));
    }
  }

  doc.end();
  return finished;
}

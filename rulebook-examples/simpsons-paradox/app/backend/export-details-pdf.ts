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
  SWEEP_CONTRACT,
  LIMITS_TEXT,
  classifyInvariant,
  invariantKindCounts,
  conclusionPdfLabel,
  domainCaveat,
  formatObservedMetric,
  isConsistencyCheck,
  tierConclusionCounts,
} from '../shared/epistemic-framing.js';
import {
  renderPlaneChart,
  renderRecoveryChart,
  renderScreeningChart,
  renderPurityChart,
  type RankingRow,
} from './pdf-charts.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const projectRoot = path.resolve(__dirname, '../..');
const rulebookPath = path.join(
  projectRoot,
  'effortless-rulebook/simpsons-paradox-rulebook.json',
);

/** PDFKit's base-14 Helvetica lacks glyphs the rulebook text actually uses (U+2212 minus
 *  sign, ⟺ biconditional, etc.), which render as blank/corrupted boxes. DejaVu Sans covers
 *  them — register it once per document instead of relying on the base-14 font. */
const FONT_REGULAR = 'DejaVuSans';
const FONT_BOLD = 'DejaVuSans-Bold';
const FONT_PATH_REGULAR = path.join(
  __dirname,
  'node_modules/dejavu-fonts-ttf/ttf/DejaVuSans.ttf',
);
const FONT_PATH_BOLD = path.join(
  __dirname,
  'node_modules/dejavu-fonts-ttf/ttf/DejaVuSans-Bold.ttf',
);

function registerFonts(doc: InstanceType<typeof PDFDocument>): void {
  doc.registerFont(FONT_REGULAR, FONT_PATH_REGULAR);
  doc.registerFont(FONT_BOLD, FONT_PATH_BOLD);
}

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
  c_amplification_count: number;
  c_compression_count: number;
  avg_signal_purity_reversal: number | null;
  avg_signal_purity_non_reversal: number | null;
  sign_flip_signal_purity_max: number | null;
  real_study_count: number | null;
  economics_sign_flip_count: number | null;
  domain_diversity_note: string | null;
  latent_type_d_count: number | null;
  latent_type_d_fraction: number | null;
}

interface ConclusionRow {
  conclusion_id: string;
  category: string;
  status: string;
  report_tier: 'critical' | 'supporting' | 'transient';
  validating_hypothesis: string | null;
  live_status_confirmed: boolean | null;
  live_status_observed_metric: string | null;
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

interface MetaRow {
  MetaKey: string;
  StringValue?: string;
  JsonValue?: string;
}

function readMeta(): { data: MetaRow[]; rootDescription: string } {
  const rb = JSON.parse(fs.readFileSync(rulebookPath, 'utf8')) as {
    __meta__?: { data?: MetaRow[] };
    Description?: string;
  };
  return {
    data: rb.__meta__?.data ?? [],
    rootDescription: rb.Description ?? '',
  };
}

function metaString(rows: MetaRow[], key: string): string {
  const row = rows.find(m => m.MetaKey === key);
  if (!row?.StringValue) {
    throw new Error(`__meta__ ${key} missing in simpsons-paradox-rulebook.json`);
  }
  return row.StringValue;
}

function metaJsonArray(rows: MetaRow[], key: string): string[] {
  const row = rows.find(m => m.MetaKey === key);
  if (!row?.JsonValue) return [];
  return JSON.parse(row.JsonValue) as string[];
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
  doc.font(FONT_BOLD).fontSize(SECTION_SIZE).fillColor('#111111');
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
  doc.font(FONT_REGULAR).fontSize(size).fillColor(color);
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
  doc.font(FONT_REGULAR).fontSize(BODY_SIZE).fillColor('#333333');
  doc.text(`${label}: `, PAGE_MARGIN, y, { continued: true });
  if (link) {
    doc.fillColor('#1155cc').text(value, { link, underline: true });
  } else {
    doc.fillColor('#333333').text(value);
  }
  return doc.y + 2;
}

/** A bordered callout box for the rulebook link — front-and-center so a reader who has
 *  never heard of this repo can jump straight to the source of truth at any point. */
function linkBox(doc: PdfDoc, y: number, label: string, value: string, link: string): number {
  y = ensureSpace(doc, y, 40);
  const width = doc.page.width - PAGE_MARGIN * 2;
  const boxTop = y;
  doc.font(FONT_REGULAR).fontSize(SMALL_SIZE).fillColor('#666666');
  doc.text(label, PAGE_MARGIN + 12, y + 8);
  doc.font(FONT_BOLD).fontSize(BODY_SIZE).fillColor('#1155cc');
  doc.text(value, PAGE_MARGIN + 12, doc.y + 1, { link, underline: true, width: width - 24 });
  const boxHeight = doc.y - boxTop + 10;
  doc.roundedRect(PAGE_MARGIN, boxTop, width, boxHeight, 4)
    .lineWidth(1)
    .strokeColor('#c9d6f0')
    .stroke();
  return boxTop + boxHeight + 10;
}

function chartImage(doc: PdfDoc, y: number, png: Buffer, aspectRatio = 480 / 900): number {
  const width = doc.page.width - PAGE_MARGIN * 2;
  const height = width * aspectRatio;
  y = ensureSpace(doc, y, height + 8);
  doc.image(png, PAGE_MARGIN, y, { width, height });
  return y + height + 10;
}

function bulletDot(
  doc: PdfDoc,
  y: number,
  text: string,
  opts?: { size?: number; color?: string },
): number {
  const size = opts?.size ?? SMALL_SIZE;
  y = ensureSpace(doc, y, 14);
  doc.font(FONT_REGULAR).fontSize(size).fillColor(opts?.color ?? '#333333');
  doc.text(`•  ${text}`, PAGE_MARGIN, y, {
    width: doc.page.width - PAGE_MARGIN * 2,
    lineGap: 1,
  });
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

/** Red banner for a theorem-tier conclusion whose ValidatingHypothesis currently FAILs live —
 *  the archived Evidence text below may still say "Proved"/"PASS" from when it was witnessed,
 *  but the corpus has since grown past it. This must be impossible to miss or skim past. */
function liveStatusBanner(doc: PdfDoc, y: number, observedMetric: string | null): number {
  y = ensureSpace(doc, y, 30);
  const width = doc.page.width - PAGE_MARGIN * 2;
  const boxTop = y;
  doc.font(FONT_BOLD).fontSize(BODY_SIZE).fillColor('#a4001d');
  doc.text('⚠ LIVE STATUS: CONTRADICTED BY CURRENT CORPUS', PAGE_MARGIN + 10, y + 6, { width: width - 20 });
  if (observedMetric) {
    doc.font(FONT_REGULAR).fontSize(SMALL_SIZE).fillColor('#a4001d');
    doc.text(`Current observed: ${formatObservedMetric(observedMetric, 6)}`, PAGE_MARGIN + 10, doc.y + 2, { width: width - 20 });
  }
  const boxHeight = doc.y - boxTop + 8;
  doc.rect(PAGE_MARGIN, boxTop, width, boxHeight).fillOpacity(0.08).fillAndStroke('#a4001d', '#a4001d');
  doc.fillOpacity(1);
  return boxTop + boxHeight + 8;
}

/** Four-state status for a discovery finding — a blank ObservedMetric means the check never
 *  ran (missing join, empty population, unencoded data), which is categorically different from
 *  a check that ran and failed. Collapsing both into "FAIL" hides instrumentation bugs as
 *  scientific negatives. */
function findingStatus(f: DiscoveryFindingRow | undefined): 'PASS' | 'FAIL' | 'NOT EVALUABLE' {
  if (!f || f.observed_metric == null || f.observed_metric === '') return 'NOT EVALUABLE';
  if (f.is_confirmed === null || f.is_confirmed === undefined) return 'NOT EVALUABLE';
  return f.is_confirmed ? 'PASS' : 'FAIL';
}

function renderDiscoveryHypothesis(
  doc: PdfDoc,
  y: number,
  h: DiscoveryHypothesisRow,
  f: DiscoveryFindingRow | undefined,
  subtitle: string,
): number {
  const status = findingStatus(f);
  const statusColor = status === 'PASS' ? '#0a7a2f' : status === 'FAIL' ? '#a4001d' : '#8a6d00';
  y = ensureSpace(doc, y, 40);
  doc.font(FONT_BOLD).fontSize(BODY_SIZE).fillColor('#111111');
  doc.text(`${h.hypothesis_id} — `, PAGE_MARGIN, y, { continued: true });
  doc.fillColor(statusColor).text(`${status} (${subtitle})`);
  y = doc.y + 2;
  y = bodyText(doc, y, h.statement, { size: SMALL_SIZE });
  y = bulletLine(doc, y, 'Expected', h.expected_outcome);
  if (status === 'NOT EVALUABLE') {
    y = bulletLine(doc, y, 'Observed', 'no observed metric recorded — check did not run (missing join, empty population, or unencoded data), not a scientific negative');
  } else if (f?.observed_metric) {
    y = bulletLine(doc, y, 'Observed', formatObservedMetric(f.observed_metric, 6));
  }
  return y + 4;
}

function renderConclusionBlock(doc: PdfDoc, y: number, c: ConclusionRow): number {
  y = ensureSpace(doc, y, 48);
  doc.font(FONT_BOLD).fontSize(BODY_SIZE).fillColor('#111111');
  doc.text(`${c.conclusion_id} · ${conclusionPdfLabel(c.category, c.conclusion_id)}`, PAGE_MARGIN, y);
  y = doc.y + 2;
  y = bodyText(doc, y, c.title);
  if (c.validating_hypothesis && c.live_status_confirmed === false) {
    y = liveStatusBanner(doc, y, c.live_status_observed_metric);
  }
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

/** Supporting tier: 1-2 line bullet — title + optional one-line evidence excerpt. */
function renderSupportingConclusion(doc: PdfDoc, y: number, c: ConclusionRow): number {
  y = ensureSpace(doc, y, 22);
  doc.font(FONT_BOLD).fontSize(SMALL_SIZE).fillColor('#111111');
  doc.text(`${c.conclusion_id}  ·  `, PAGE_MARGIN, y, { continued: true });
  doc.font(FONT_REGULAR).fillColor('#333333').text(c.title, { width: doc.page.width - PAGE_MARGIN * 2 - 10 });
  y = doc.y + 1;
  const caveat = domainCaveat(c.conclusion_id);
  if (caveat) {
    y = bodyText(doc, y, caveat, { size: 7.5, color: '#884400' });
  }
  return y + 3;
}

/** Transient tier: folded into a single compact list, one line per row. */
function renderTransientLine(doc: PdfDoc, y: number, c: ConclusionRow): number {
  return bulletDot(doc, y, `${c.conclusion_id} — ${c.title}`, { size: 7.5, color: '#666666' });
}

export async function buildDetailsPdf(): Promise<Buffer> {
  const [
    summaryRows,
    conclusionRows,
    hypothesisRows,
    findingRows,
    invariantRows,
    spotlightRows,
    rankingRows,
  ] = await Promise.all([
    query<SummaryRow>(
      `SELECT model_summary_id, study_count, reversal_count, explained_count,
              type_a_count, type_b_count, type_d_count,
              c_amplification_count, c_compression_count,
              avg_signal_purity_reversal, avg_signal_purity_non_reversal,
              sign_flip_signal_purity_max, real_study_count,
              economics_sign_flip_count, domain_diversity_note,
              latent_type_d_count, latent_type_d_fraction
       FROM vw_model_summary LIMIT 1`,
    ),
    query<ConclusionRow>(
      'SELECT * FROM vw_conclusions ORDER BY conclusion_id',
    ),
    query<DiscoveryHypothesisRow>(
      `SELECT hypothesis_id, statement, expected_outcome,
              epistemic_tier
       FROM vw_discovery_hypotheses
       ORDER BY hypothesis_id`,
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
    query<RankingRow>(
      `SELECT study, distortion_type, screening_tier, is_sign_flip, allocation_distortion,
              paradox_strength, signed_pooled_gap, weighted_stratum_gap_sum, signal_purity
       FROM vw_treatment_rankings`,
    ),
  ]);

  const summary = summaryRows[0];
  if (!summary) {
    throw new Error('vw_model_summary returned no rows — cannot build summary PDF');
  }

  const meta = readMeta();
  const tagline = metaString(meta.data, 'tagline');
  const cmccSummary = metaString(meta.data, 'cmcc_summary');
  const journalSeed = metaString(meta.data, 'journal_seed');
  const useCases = metaJsonArray(meta.data, 'use_cases');
  const witnessed = conclusionRows.filter(c => c.status === 'witnessed');
  const critical = witnessed.filter(c => c.report_tier === 'critical');
  const supporting = witnessed.filter(c => c.report_tier === 'supporting');
  const transient = conclusionRows.filter(
    c => c.report_tier === 'transient' || c.status !== 'witnessed',
  );
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

  const [planeChart, recoveryChart, screeningChart, purityChart] = await Promise.all([
    renderPlaneChart(rankingRows),
    renderRecoveryChart(rankingRows),
    renderScreeningChart(rankingRows),
    renderPurityChart(rankingRows),
  ]);

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
  registerFonts(doc);

  doc.on('data', (chunk: Buffer) => chunks.push(chunk));

  const finished = new Promise<Buffer>((resolve, reject) => {
    doc.on('end', () => resolve(Buffer.concat(chunks)));
    doc.on('error', reject);
  });

  let y = PAGE_MARGIN;

  doc.font(FONT_BOLD).fontSize(TITLE_SIZE).fillColor('#111111');
  doc.text("Simpson's Paradox — Corpus Summary", PAGE_MARGIN, y);
  y = doc.y + 4;

  y = bodyText(doc, y, tagline, { size: 11, color: '#555555' });
  y = bodyText(
    doc,
    y,
    `Generated ${generatedAt} — a status report on a live, running instrument, not a one-off analysis. Every number below is queried fresh from Postgres views computed straight off the rulebook below; nothing here is hand-typed.`,
    { size: SMALL_SIZE, color: '#666666' },
  );

  y = linkBox(
    doc,
    y,
    'The single source of truth for everything in this report — jump in any time:',
    'effortless-rulebook/simpsons-paradox-rulebook.json',
    PROJECT_LINKS.rulebook(),
  );

  y = sectionHeading(doc, y, "What is this?");
  y = bodyText(doc, y, meta.rootDescription, { size: SMALL_SIZE, color: '#333333' });
  y = bodyText(doc, y, cmccSummary, { size: SMALL_SIZE, color: '#333333' });
  if (useCases.length > 0) {
    y = bodyText(doc, y, 'What it is for:', { size: SMALL_SIZE, color: '#666666' });
    for (const useCase of useCases) {
      y = bulletDot(doc, y, useCase, { size: SMALL_SIZE, color: '#444444' });
    }
  }
  y = bodyText(doc, y, journalSeed, { size: SMALL_SIZE, color: '#555555' });

  y = sectionHeading(doc, y, 'How to read this report');
  y = bodyText(doc, y, SCOPE_BOUNDARY_TEXT, { size: SMALL_SIZE, color: '#333333' });
  y = bulletDot(doc, y, LIMITS_TEXT, { size: SMALL_SIZE, color: '#444444' });
  y = bulletDot(doc, y, SWEEP_CONTRACT, { size: SMALL_SIZE, color: '#444444' });
  y = bulletDot(
    doc,
    y,
    'Most "InvariantChecks" rows are transpiler regression checks restating one algebraic fact in a second vocabulary (definitional) — not independent empirical tests. A few cross into the one hand/LLM-encoded field (CausalRole, judgment-crossing) or are currently empty-universe (vacuous). See "Invariant health" below for the breakdown.',
    { size: SMALL_SIZE, color: '#444444' },
  );

  y = sectionHeading(doc, y, 'Epistemic summary (tiered)');
  y = bulletLine(
    doc,
    y,
    'Proved by construction (pure algebra — no CausalRole dependency)',
    String(tiers.proved),
  );
  y = bulletLine(
    doc,
    y,
    'Proved, conditional on CausalRole annotation (see caveats below — not pure algebra)',
    String(tiers.provedConditional),
  );
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
      'Latent Type-D — flips somewhere across the full f ∈ [0.05, 0.95] sweep, not necessarily near observed allocation',
      `${summary.latent_type_d_count} / ${summary.type_d_count} (${fmtPct(summary.latent_type_d_fraction)})`,
    );
    y = bulletDot(
      doc,
      y,
      'This measures mathematical fragility over a wide counterfactual range, not practical vulnerability under realistic reallocations — see conc-15.',
      { size: 7.5, color: '#884400' },
    );
  }
  y = bulletLine(
    doc,
    y,
    'Studies with a full paired-treatment encoding (StudyCount, current live N — the denominator for every ratio in this report)',
    String(summary.study_count),
  );
  if (summary.real_study_count != null && summary.real_study_count !== summary.study_count) {
    y = bulletDot(
      doc,
      y,
      `RealStudyCount=${summary.real_study_count} counts a larger superset: every non-synthetic Studies row, including ${summary.real_study_count - summary.study_count} control studies (loop-88) that exist as rows but were never encoded with paired treatment arms and so produce zero TreatmentRankings — not a data error, but a different denominator. Do not treat the two counts as interchangeable.`,
      { size: 7.5, color: '#666666' },
    );
  }
  y = bulletLine(
    doc,
    y,
    'Distortion taxonomy',
    `A ${summary.type_a_count} · B ${summary.type_b_count} · C+ ${summary.c_amplification_count} · C− ${summary.c_compression_count} · D ${summary.type_d_count}`,
  );

  y = sectionHeading(doc, y, 'The corpus at a glance');
  y = chartImage(doc, y, planeChart);

  y = sectionHeading(doc, y, `Proved conclusions (${critical.length})`);
  y = bodyText(
    doc,
    y,
    `Theorems and scope-defining findings, re-verified live against the current ${summary.study_count}-study corpus. Evidence text below may still cite the corpus size at the loop where each theorem was first witnessed (e.g. "90+ studies," "96-study corpus") — that is historical provenance, not a claim about today's N. The pass/fail counts in "Invariant health" reflect the live corpus.`,
    { size: SMALL_SIZE, color: '#666666' },
  );
  const chartAfter: Record<string, Buffer> = {
    'conc-12-signal-purity-theorem': purityChart,
    'conc-20-signal-purity-ceiling': recoveryChart,
    'conc-36-perfect-balance-theorem': screeningChart,
  };
  for (const c of critical) {
    y = renderConclusionBlock(doc, y, c);
    const chart = chartAfter[c.conclusion_id];
    if (chart) y = chartImage(doc, y, chart);
  }

  y = sectionHeading(doc, y, `Supporting findings (${supporting.length})`);
  y = bodyText(
    doc,
    y,
    'Established in the instrument or observed in this corpus — one line each. Full evidence text is in the rulebook JSON (Conclusions table).',
    { size: SMALL_SIZE, color: '#666666' },
  );
  for (const cat of CATEGORY_ORDER) {
    const group = supporting.filter(c => c.category === cat);
    if (group.length === 0) continue;
    y = ensureSpace(doc, y, 20);
    doc.font(FONT_BOLD).fontSize(SMALL_SIZE).fillColor('#555555');
    doc.text(conclusionPdfLabel(cat), PAGE_MARGIN, y);
    y = doc.y + 3;
    for (const c of group) {
      y = renderSupportingConclusion(doc, y, c);
    }
    y += 3;
  }

  y = sectionHeading(doc, y, `Other findings, in brief (${transient.length})`);
  y = bodyText(
    doc,
    y,
    'Superseded corpus patterns, build/methodology synthesis notes, and candidate (not-yet-promoted) theorems — kept for the record, not central to the current story.',
    { size: 7.5, color: '#666666' },
  );
  for (const c of transient) {
    y = renderTransientLine(doc, y, c);
  }

  y = sectionHeading(doc, y, 'Pre-registered discovery (loop-61)');
  y = bodyText(
    doc,
    y,
    'Consistency checks — confirm implementation matches stated algebra (same tier as conc-12 / inv-signal-purity-sign-flip). Not independent discoveries about nature.',
    { size: SMALL_SIZE, color: '#666666' },
  );
  for (const h of consistencyHypotheses) {
    y = renderDiscoveryHypothesis(doc, y, h, findingsByHypothesis.get(h.hypothesis_id), 'consistency check');
  }
  y += 4;
  y = bodyText(
    doc,
    y,
    'Corpus hypotheses — contingent patterns on this convenience sample. Directional inequalities only; not inferential. See conc-14.',
    { size: SMALL_SIZE, color: '#666666' },
  );
  for (const h of corpusHypotheses) {
    y = renderDiscoveryHypothesis(doc, y, h, findingsByHypothesis.get(h.hypothesis_id), 'corpus hypothesis');
  }

  y = sectionHeading(doc, y, 'Invariant health');
  y = bulletLine(
    doc,
    y,
    'Critical invariant failures',
    String(criticalFails),
    criticalFails === 0 ? PROJECT_LINKS.invariantChecks() : undefined,
  );
  const kindCounts = invariantKindCounts(criticalInvariants);
  y = bulletLine(
    doc,
    y,
    'Definitional (cannot fail by construction — see definitions above)',
    String(kindCounts.definitional),
  );
  y = bulletLine(
    doc,
    y,
    'Judgment-crossing (touch hand/LLM-encoded CausalRole)',
    String(kindCounts.judgment),
  );
  y = bulletLine(
    doc,
    y,
    'Vacuous (0 pass / 0 fail — empty universe, proves nothing yet)',
    String(kindCounts.vacuous),
  );
  const criticalFailedRows = criticalInvariants.filter(i => Number(i.fail_count) > 0);
  const warningFailedRows = invariantRows.filter(
    i => i.severity !== 'critical' && Number(i.fail_count) > 0,
  );
  const judgmentRows = criticalInvariants.filter(
    i => classifyInvariant(i) === 'judgment',
  );
  const judgmentFailedRows = judgmentRows.filter(i => Number(i.fail_count) > 0);

  if (judgmentFailedRows.length > 0) {
    y = bodyText(
      doc,
      y,
      'Judgment-crossing failures — the closest thing here to a substantive empirical finding, since these are the only checks that touch the hand/LLM-encoded CausalRole field rather than restating pure geometry:',
      { size: SMALL_SIZE, color: '#884400' },
    );
    for (const inv of judgmentFailedRows) {
      y = bulletDot(
        doc,
        y,
        `${inv.invariant_check_id}: ${inv.natural_language} (pass=${inv.pass_count}, fail=${inv.fail_count})`,
        { size: 7.5, color: '#884400' },
      );
    }
  }
  const definitionalFailedRows = criticalFailedRows.filter(
    i => classifyInvariant(i) !== 'judgment',
  );
  if (definitionalFailedRows.length > 0) {
    y = bodyText(
      doc,
      y,
      'Definitional failures — a build/encoding regression against stated algebra, not an empirical finding:',
      { size: SMALL_SIZE, color: '#666666' },
    );
    for (const inv of definitionalFailedRows) {
      y = bulletDot(
        doc,
        y,
        `${inv.invariant_check_id}: ${inv.natural_language} (fail=${inv.fail_count})`,
        { size: 7.5 },
      );
    }
  }
  if (criticalFailedRows.length === 0) {
    y = bodyText(
      doc,
      y,
      `All ${criticalInvariants.length} critical invariants pass — ${kindCounts.definitional} are transpiler ` +
        `regression checks against stated algebra (cannot fail by construction), ${kindCounts.judgment} cross ` +
        `into the hand/LLM-encoded CausalRole field, and ${kindCounts.vacuous} are currently vacuous ` +
        `(empty universe). See "How to read this report" above.`,
      { size: SMALL_SIZE },
    );
  }
  if (warningFailedRows.length > 0) {
    y = bodyText(
      doc,
      y,
      `Warning-tier geometry checks currently failing (${warningFailedRows.length}) — not critical, tracked separately, listed for completeness:`,
      { size: 7.5, color: '#666666' },
    );
    for (const inv of warningFailedRows) {
      y = bulletDot(
        doc,
        y,
        `${inv.invariant_check_id}: ${inv.natural_language} (fail=${inv.fail_count})`,
        { size: 7.5, color: '#888888' },
      );
    }
  }

  y = sectionHeading(doc, y, 'Most misleading studies (by paradox strength)');
  for (const row of spotlightRows) {
    y = ensureSpace(doc, y, 20);
    doc.font(FONT_REGULAR).fontSize(BODY_SIZE).fillColor('#333333');
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

// ===========================================================================
// cqs.ts — the per-question RESOLVERS for the Competency-Question scoreboard.
//
// The competency questions THEMSELVES (the text, the article ref, the field
// that answers each, the asserted expected answer) are first-class rows in the
// rulebook's `CompetencyQuestions` table — they arrive in the payload as
// `sit.competencyQuestions`. This file is PRESENTATION only: for each CQ id it
// knows how to (a) pull the already-computed LIVE answer out of the payload and
// (b) decide pass/fail against the rulebook's asserted ExpectedAnswer.
//
// DOCTRINE: nothing here computes a business fact. Every value read below is a
// column the substrate already populated (a verdict's IsStale, a workflow's
// InvolvesEngineeringAndLegal, a gate's GateApproverHuman, the artifact chain,
// the dataset's consuming step). We only read, format, and compare — never
// re-derive. The view IS the contract. If an answer is wrong, the substrate is
// wrong; we surface it, we don't paper over it. The pass/fail is therefore a
// real check: (substrate-computed value) vs (rulebook-asserted expectation), and
// editing a raw fact in the console can flip any of these to ✗.
// ===========================================================================

import type { Situation, CompetencyQuestion } from "../types";

export interface CqRow {
  label: string;      // the headline of a backing row
  sub?: string;       // optional secondary text
  kind?: "ai" | "human" | "pipeline" | "step" | "artifact" | "dataset" | "role";
}

export interface CqResult {
  answer: string;     // the live, computed answer rendered as a short string
  ok: boolean;        // does the live answer satisfy the rulebook's expectation?
  rows: CqRow[];      // the backing detail revealed by the disclosure
}

export type CqResolver = (sit: Situation, cq: CompetencyQuestion) => CqResult;

// --- small shared helpers (formatting / comparison only) -------------------
const norm = (s: unknown): string => (s ?? "").toString().trim().toLowerCase();
// The rulebook authors each scalar expectation as prose; its leading Yes/No is
// the asserted polarity for the boolean CQs. Reading it here keeps the rulebook
// the source of the expectation rather than re-hardcoding it.
const expectsYes = (cq: CompetencyQuestion): boolean => /^\s*yes\b/i.test(cq.expectedAnswer || "");
const kindOfExecType = (t: string): CqRow["kind"] =>
  t === "AIAgent" ? "ai" : t === "AutomatedPipeline" ? "pipeline" : t === "HumanAgent" ? "human" : "step";

const gateStep = (sit: Situation) => sit.steps.find((s) => s.isApprovalGate) || null;

export const CQ_RESOLVERS: Record<string, CqResolver> = {
  // CQ1 — all steps, IN ORDER. The question demands a definite order, which only
  // exists when the precedence closure is a TOTAL order: every pair of steps is
  // comparable. A total order over n steps has exactly n·(n−1)/2 closure pairs.
  // Fewer than that means a PARTIAL order — some steps tie on SequencePosition and
  // the left-to-right sequence on the Flow dashboard is just one arbitrary
  // linearization, not "the" order. So pass ONLY when the closure is complete.
  // (The rulebook's ExpectedAnswer asserts the same thing: "…(10 closure pairs)".)
  "cq-1": (sit) => {
    const steps = [...sit.steps].sort((a, b) => a.position - b.position);
    const n = steps.length;
    const pairs = sit.closure?.count ?? 0;
    const needed = (n * (n - 1)) / 2;          // closure-pair count of a TOTAL order
    const isTotal = n > 0 && pairs === needed;
    // Steps sharing a SequencePosition are NOT ordered relative to each other —
    // that's exactly the ambiguity that makes the dashboard order arbitrary.
    const posCounts: Record<number, number> = {};
    for (const s of steps) posCounts[s.position] = (posCounts[s.position] || 0) + 1;
    const tied = steps.filter((s) => posCounts[s.position] > 1).length;
    return {
      answer: isTotal
        ? `${n} steps · fully ordered (${pairs}/${needed} pairs)`
        : `${n} steps · PARTIAL order — ${pairs}/${needed} pairs, ${tied} step(s) ambiguous`,
      ok: isTotal,
      rows: steps.map((s) => ({
        label: `${s.position}. ${s.title}`,
        sub: posCounts[s.position] > 1 ? "⚠ tied — order not determined by the asserted edges" : undefined,
        kind: kindOfExecType(s.executingAgentType),
      })),
    };
  },

  // CQ2 — who approves: the approval gate's resolved human approver. Pass only
  // if it matches the asserted expectation (reassign the gate → mismatch → ✗).
  "cq-2": (sit, cq) => {
    const g = gateStep(sit);
    const approver = g?.gateApproverHuman || "";
    const ok = !!approver && norm(cq.expectedAnswer).includes(norm(approver));
    return {
      answer: approver || "— no human approver",
      ok,
      rows: g
        ? [
            { label: g.gateName || g.title, sub: "approval gate", kind: "step" },
            { label: g.gateRole || "—", sub: "gate role", kind: "role" },
            { label: approver || "—", sub: "filled by (human)", kind: "human" },
          ]
        : [{ label: "no approval gate in this workflow" }],
    };
  },

  // CQ3 — which steps are AI-executed vs human-required. Both classifications
  // are computed per step (ExecutingAgentType / RequiresHumanApproval). Like the
  // other scalar CQs, pass/fail is the LIVE answer vs the rulebook's asserted
  // ExpectedAnswer ("AI-executed: steps 1, 5 - human-required: steps 2, 3"): we
  // split that prose on "human" and compare the two step-number sets. Reassign a
  // step to an AI and the AI set drifts from the assertion → ✗ (toggle made
  // visible). This stays purely descriptive — it reports the true classification;
  // the AI-step-needs-human-signoff *inconsistency* is a separate constraint (the
  // per-step "rule broken" flag), not CQ3's job.
  "cq-3": (sit, cq) => {
    const steps = [...sit.steps].sort((a, b) => a.position - b.position);
    const ai = steps.filter((s) => s.executingAgentType === "AIAgent");
    const human = steps.filter((s) => s.requiresHumanApproval);
    const fmt = (xs: typeof steps) => xs.map((s) => s.position).join(", ") || "none";
    const posSet = (xs: typeof steps) => xs.map((s) => s.position).sort((a, b) => a - b);
    const nums = (t?: string) => (t?.match(/\d+/g) || []).map(Number).sort((a, b) => a - b);
    const sameSet = (x: number[], y: number[]) => x.length === y.length && x.every((v, i) => v === y[i]);
    // Split the asserted prose into its AI side and its human side on "human".
    const [expAiPart, expHumanPart] = (cq.expectedAnswer || "").split(/human/i);
    const ok = sameSet(posSet(ai), nums(expAiPart)) && sameSet(posSet(human), nums(expHumanPart));
    return {
      answer: `AI: ${fmt(ai)} · human: ${fmt(human)}`,
      ok,
      rows: steps.map((s) => ({
        label: `${s.position}. ${s.title}`,
        sub: s.executingAgentType + (s.requiresHumanApproval ? " · requires human sign-off" : ""),
        kind: kindOfExecType(s.executingAgentType),
      })),
    };
  },

  // CQ4 — artifact lineage: the wasDerivedFrom chain + who each was attributed
  // to + the downstream consumer. All from WorkflowArtifacts' columns.
  "cq-4": (sit) => {
    const arts = sit.artifacts || [];
    return {
      answer: `${arts.length} artifacts · derivation chain`,
      ok: arts.length > 0 && arts.every((a) => a.attributedTo),
      rows: arts.map((a) => ({
        label: a.title,
        sub:
          (a.derivedFromArtifact ? `from ${a.derivedFromArtifact.replace(/^artifact-/, "")} · ` : "origin · ") +
          (a.attributedTo ? `by ${a.attributedTo.name}` : "unattributed") +
          (a.requiredBySteps.length ? ` → ${a.requiredBySteps.map((r) => r.title).join(", ")}` : ""),
        kind: a.attributedTo?.kind === "human" ? "human" : a.attributedTo?.kind === "ai" ? "ai" : a.attributedTo?.kind === "pipeline" ? "pipeline" : "artifact",
      })),
    };
  },

  // CQ5 — is the workflow stale (>12mo)? The verdict is computed; the asserted
  // expectation is "not stale". Drag the StalenessBar past the threshold and
  // this flips to ✗ — the toggle made visible.
  "cq-5": (sit, cq) => {
    const w = sit.workflow;
    const isStale = !!w.isStale;
    const answer = isStale
      ? `Yes — stale (${w.monthsSinceModified}mo > ${w.stalenessThresholdMonths}mo)`
      : `No — not stale (${w.monthsSinceModified}mo ≤ ${w.stalenessThresholdMonths}mo)`;
    // The question is "which workflows are stale"; the answer's polarity is
    // IsStale. Pass when it matches the rulebook's asserted polarity.
    return {
      answer,
      ok: isStale === expectsYes(cq),
      rows: [
        { label: `Modified ${w.monthsSinceModified} months ago`, sub: `threshold ${w.stalenessThresholdMonths} months` },
        { label: isStale ? "IsStale = true" : "IsStale = false", sub: "MonthsSinceModified > StalenessThresholdMonths" },
      ],
    };
  },

  // CQ6 — escalation when the VP is unavailable: the gate role's DelegatesTo
  // chain (transitive). Already materialized in the delegation map.
  "cq-6": (sit) => {
    const g = gateStep(sit);
    const roleId = g?.roleId || "";
    const entry = sit.delegation?.[roleId];
    const fromName = entry?.from || sit.roleById[roleId]?.name || "—";
    const chain = entry?.to || [];
    const reachesCto = chain.some((t) => /cto|chief technology/i.test(t.name));
    return {
      answer: chain.length ? `${fromName} → ${chain.map((t) => t.name).join(" → ")}` : `${fromName} — no delegation`,
      ok: chain.length > 0 && reachesCto,
      rows: [
        { label: fromName, sub: "gate role", kind: "role" },
        ...chain.map((t, i) => ({ label: t.name, sub: i === chain.length - 1 ? "final escalation" : "delegates to", kind: "role" as const })),
      ],
    };
  },

  // CQ7 — does the workflow involve BOTH Engineering and Legal? A computed
  // boolean over the two department-owned step counts.
  "cq-7": (sit, cq) => {
    const w = sit.workflow;
    const both = !!w.involvesEngineeringAndLegal;
    return {
      answer: both
        ? `Yes — ${w.countEngineeringOwnedSteps} Eng + ${w.countLegalOwnedSteps} Legal step(s)`
        : `No — Eng ${w.countEngineeringOwnedSteps}, Legal ${w.countLegalOwnedSteps}`,
      ok: both === expectsYes(cq),
      rows: [
        { label: `${w.countEngineeringOwnedSteps} Engineering-owned step(s)` },
        { label: `${w.countLegalOwnedSteps} Legal-owned step(s)` },
        { label: `InvolvesEngineeringAndLegal = ${both}`, sub: "AND(Eng > 0, Legal > 0)" },
      ],
    };
  },

  // CQ8 — datasets the review consumes + the AI agent that processed each. The
  // consuming step (Datasets.ConsumedBySteps) resolves to its filling agent.
  "cq-8": (sit) => {
    const ds = sit.datasets || [];
    const flat = ds.flatMap((d) => d.consumedBySteps.map((c) => ({ d, c })));
    const answer =
      flat.length === 0
        ? "no datasets consumed"
        : flat.map(({ d, c }) => `${d.title}${c.agent ? ` · ${c.agent.name}` : ""}`).join(" ; ");
    return {
      answer,
      ok: ds.length > 0 && flat.every(({ c }) => !!c.agent),
      rows: ds.map((d) => ({
        label: d.title,
        sub: d.consumedBySteps.length
          ? d.consumedBySteps.map((c) => `${c.title}${c.agent ? ` — processed by ${c.agent.name}` : ""}`).join("; ")
          : "not consumed by any step",
        kind: "dataset",
      })),
    };
  },
};

// Resolve one CQ row to its live result. A CQ with no resolver (a newly added
// row the UI doesn't know yet) surfaces loudly rather than silently passing.
export function resolveCq(sit: Situation, cq: CompetencyQuestion): CqResult {
  const r = CQ_RESOLVERS[cq.id];
  if (!r) return { answer: "— no resolver for this CQ", ok: false, rows: [{ label: `add a resolver for ${cq.id} in cqs.ts` }] };
  return r(sit, cq);
}

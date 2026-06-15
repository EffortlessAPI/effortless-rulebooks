import React from "react";
import { ChainRibbon, type RibbonNode, type RibbonLink } from "./ChainRibbon";
import { DagCell } from "../explainer-dag";
import type { Situation, Handlers, Artifact } from "../types";

// ===========================================================================
// LINEAGE VIEW — artifact provenance (prov:wasDerivedFrom), on the SAME ribbon
// the step-precedence and role-escalation closures use.
//
// This is the surface CQ4 never had: a place to SEE the derivation chain and
// FIX a broken link. The five artifacts the release produces form one chain —
// each derived from the one before it (Risk Report → Legal Clearance → Release
// Authorization → Deployment Log → Post-Deployment Report). That chain is now a
// first-class `closure` field (DerivationClosure → vw_workflow_artifacts_closure),
// exactly like precedesStep and delegatesTo, so:
//   • a present link is a SOLID connector,
//   • the never-asserted long-range reachability (Post-Deploy ⇒ Risk Report) is
//     counted as INFERRED,
//   • a cleared link (CQ4's break-derivation simulate) is a GAP with an inline
//     "↺ restore link" button that re-points DerivedFromArtifact and re-reasons.
//
// Computes nothing: node order is the producer-step order the substrate sorted,
// link state is read from each artifact's DerivedFromArtifact and the closure.
// ===========================================================================

interface LineageViewProps {
  sit: Situation;
  handlers: Handlers;
  // When rendered INSIDE the Closure lens, the surrounding view already explains
  // "same closure machine" — so suppress this view's own intro to avoid saying it
  // twice. Standalone (its own former tab) it still shows the full intro.
  embedded?: boolean;
}

const short = (id: string | null): string => (id || "").replace(/^artifact-/, "");
const agentKind = (a: Artifact["attributedTo"]): string =>
  a?.kind === "human" ? "human" : a?.kind === "ai" ? "ai" : a?.kind === "pipeline" ? "pipeline" : "artifact";

export function LineageView({ sit, handlers, embedded = false }: LineageViewProps) {
  // Artifacts arrive sorted by producer-step position — that IS the chain order.
  const arts = sit.artifacts || [];
  const dc = sit.derivationClosure;

  const nodes: RibbonNode[] = arts.map((a) => ({
    id: a.id,
    label: "📄",
    title: a.title,
    kind: agentKind(a.attributedTo),
    sub:
      (a.attributedTo ? `by ${a.attributedTo.name}` : "unattributed") +
      (a.requiredBySteps.length ? ` → feeds ${a.requiredBySteps.map((r) => r.title).join(", ")}` : ""),
    origin: !a.hasDerivationParent,
    // Mark the chain head, and flag any *additional* origin as a break symptom.
    flag: !a.hasDerivationParent ? "⚑ origin" : undefined,
  }));

  // The ASSERTED single-hop links — the source of truth for adjacency. We read
  // them from the closure's non-inferred pairs, NOT from each artifact's
  // DerivedFromArtifact field: on the reasoner engine OWL-RL expands that FK into
  // the whole transitive closure (multi-valued), so a direct === check would
  // misfire. The closure view tags every pair is_inferred, so its is_inferred=false
  // rows are exactly the directly-asserted edges, identically on both engines.
  // (Edge direction: child wasDerivedFrom parent ⇒ pair from=child to=parent.)
  const assertedSet = new Set(
    (dc?.pairs || []).filter((p) => !p.is_inferred).map((p) => p.from_id + "→" + p.to_id),
  );

  // One connector per adjacent pair. The canonical chain says artifact[i+1] was
  // derived from artifact[i]; the link is ASSERTED when that edge is in the
  // closure, MISSING (broken) otherwise — and a missing link carries the restore-fix.
  const links: RibbonLink[] = [];
  for (let i = 0; i < arts.length - 1; i++) {
    const left = arts[i];
    const right = arts[i + 1];
    const linked = assertedSet.has(right.id + "→" + left.id);
    links.push(
      linked
        ? { state: "asserted", caption: "derived from" }
        : {
            state: "missing",
            caption: "derived from",
            fixLabel: `↺ link to ${short(left.id)}`,
            onFix: () => handlers.setDerivedFrom(right.id, left.id),
          },
    );
  }

  const broken = links.some((l) => l.state === "missing");
  const origins = arts.filter((a) => !a.hasDerivationParent).length;

  return (
    <div className="lineage">
      {!embedded && (
        <div className="ln-intro">
          <div className="ln-intro-lead">
            The release produces five artifacts in a PROV <code>wasDerivedFrom</code> chain — each
            built from the one before it. This is the same transitive-closure construct as step
            ordering and role escalation: you assert only the adjacent derivation links and the
            reachable pairs close for free. Cut one link and the chain splits — repair it right on the
            ribbon.
          </div>
        </div>
      )}

      <DagCell table="WorkflowArtifacts" field="DerivationClosure">
        <ChainRibbon
          nodes={nodes}
          links={links}
          relationLabel="prov:wasDerivedFrom"
          counts={
            dc
              ? { asserted: dc.asserted, inferred: dc.inferred, total: dc.count }
              : undefined
          }
          brokenNote={
            broken
              ? `Lineage broken — ${origins} origins (a single chain has exactly one). ` +
                `The artifact below its gap lost its derivation parent; click ↺ to restore it.`
              : undefined
          }
        />
      </DagCell>

      <p className="ln-foot muted">
        A <b>solid</b> connector is an asserted <code>wasDerivedFrom</code> link. The{" "}
        <b>{dc?.inferred ?? 0}</b> inferred pairs are the long-range reachability the closure derived
        but no one typed (e.g. Post-Deployment Report ⇒ Risk Report). A <b>✂ gap</b> is a severed
        link — the exact failure CQ4's <i>break-derivation</i> simulate creates, now visible and
        one click from fixed.
        {!embedded && (
          <>
            {" "}This is the SAME <code>closure</code> construct as{" "}
            <code>precedesStep</code> (the step ordering above) and <code>delegatesTo</code> (the
            Org tab's escalation) — three relations, one transitive-closure machine; each just picks
            the lens that fits (a ribbon here, a reachability matrix for the step DAG, an org chart for
            escalation).
          </>
        )}
      </p>
    </div>
  );
}

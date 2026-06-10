import React, { useEffect, useState, useCallback, useRef } from "react";
import { api, getBackend } from "./api";
import Diamond, { type NodeId, type LegFlow } from "./Diamond";
import type {
  StoreId,
  TriangleResponse,
  DiffResponse,
  DiffAgainst,
  BackendDescriptor,
} from "./types";

// ===========================================================================
// SyncPanel.tsx — the ONE interactive control. Nothing is privileged among the
// three STORES; the CLIENT is a separate, fourth thing (a reader, not a store).
//
// The picture is a DIAMOND (see Diamond.tsx): the Rulebook source at the BASE,
// the two engines (Reasoner / Postgres) mid-height, and the Client app at the
// TOP apex. Data flows UP: the engines are derived from the rulebook; the client
// reads exactly ONE engine.
//
// Resting state of the three STORES shows the COMPUTED drift status — who is HEAD
// right now (Rulebook when all three agree; otherwise the most-recently-edited
// store, shown purely as a status read-out). There is NO selection to make.
//
// Interaction on a STORE node: HOVER to ask "if THIS store were authoritative,
// what would the others lose, and how would I push it?" — a floaty box appears
// with the field-level diff and the ONE push, and you act straight from the
// hover. Move away and the box vanishes. Recency only decides the resting STATUS;
// every store node is equally hoverable, because none of the three is privileged.
//
// Interaction on the CLIENT node: HOVER to choose which engine the app reads. The
// client is NOT a store — it has no hash, no diff, no drift of its own; it just
// DISPLAYS whichever engine it's wired to (getBackend()). So the client popover
// is an engine picker — "Enter on Reasoner →" / "Enter on Postgres →" — each of
// which sets the backend and enters the app (onEnter). The connected engine's
// wire is lit and borrows that engine's drift color, so entering on a STALE
// engine is visible before you click.
//
// When the three stores already agree there is nothing to push: hovering the
// Rulebook offers just "Reset to baseline". The push appears only on a real diff.
//
// ONE push per store node. A node does not need the whole fan-out of "both legs /
// left only / right only" — because every push round-trips THROUGH the rulebook
// (planSync in control.js: there is no direct engine↔engine copy), a single
// directional push always ends with all three stores locked. So each store offers
// exactly one button — "make THIS store the truth everywhere":
//   • Reasoner (left)  → export up to the source, rebuild, reseed Postgres
//   • Postgres (right) → export up to the source, rebuild, reseed Reasoner
//   • Rulebook (base)  → rebuild, push up into both engines
// All three resolve to one backend action (/api/control/sync {from,targets});
// reset is /api/control/reset. Diffs are fetched per store lazily and cached
// (regeneratable, invalidated on refreshKey) so any hovered store can show "what
// it would overwrite" without a chosen-HEAD round-trip.
// ===========================================================================

const STORE_LABEL: Record<StoreId, string> = { rulebook: "Rulebook", reasoner: "Reasoner", postgres: "Postgres" };
const STORE_SUB: Record<StoreId, string> = { rulebook: "the hub", reasoner: "db.json", postgres: "Postgres tables" };

// The action payload SyncPanel hands up to its parent's onRunSync (App.jsx),
// which drives the SSE control endpoints. Two shapes: a directional sync and a
// reset; both carry the path, body, and a human label for the run banner.
interface SyncAction {
  kind: "sync";
  path: string;
  body: { from: StoreId; targets: StoreId[] };
  label: string;
}
interface ResetAction {
  kind: "reset";
  path: string;
  body: null;
  label: string;
}
type RunAction = SyncAction | ResetAction;

function fmtAgo(iso: string | null): string | null {
  if (!iso) return null;
  const ms = Date.now() - new Date(iso).getTime();
  if (ms < 0 || Number.isNaN(ms)) return null;
  const s = Math.round(ms / 1000);
  if (s < 60) return `${s}s ago`;
  const m = Math.round(s / 60);
  if (m < 60) return `${m}m ago`;
  const h = Math.round(m / 60);
  if (h < 24) return `${h}h ago`;
  return `${Math.round(h / 24)}d ago`;
}

// The store the recency markers say was touched most recently (used only to
// decide the resting STATUS when the three have genuinely diverged).
function mostRecentStore(tri: TriangleResponse): StoreId {
  const t = (s: StoreId): number => {
    const v = s === "rulebook" ? tri.legs.rulebook.lastEditAt : tri.legs[s]?.lastEditAt;
    return v ? new Date(v).getTime() : -1;
  };
  return (["reasoner", "postgres", "rulebook"] as StoreId[]).sort((a, b) => t(b) - t(a))[0];
}

// Render one value as it should appear in the diff cell.
interface ValueCellProps {
  v: unknown;
  kind: string;
}
function ValueCell({ v, kind }: ValueCellProps) {
  if (v === null || v === undefined) return <span className={`dv dv-absent ${kind}`}>∅</span>;
  let s = typeof v === "string" ? v : JSON.stringify(v);
  const long = s.length > 60;
  if (long) s = s.slice(0, 60) + "…";
  return <span className={`dv ${kind}`} title={typeof v === "string" ? v : JSON.stringify(v)}>{s}</span>;
}

interface SyncPanelProps {
  onRunSync: (action: RunAction) => void;
  running: string | null;  // the active build label while a sync runs, else null
  refreshKey: number;
  backends: BackendDescriptor[];
  onEnter: (id: string) => void;
}

export default function SyncPanel({ onRunSync, running, refreshKey, backends, onEnter }: SyncPanelProps) {
  const [tri, setTri] = useState<TriangleResponse | null>(null);
  const [diffs, setDiffs] = useState<Record<string, DiffResponse>>({});       // { store: diffResult } cache
  const [hovered, setHovered] = useState<NodeId | null>(null);  // the node currently hovered (store OR client)
  const [error, setError] = useState<string | null>(null);
  const [, tick] = useState(0);                  // keep "ago" fresh
  const pollRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const diffReqRef = useRef<Record<string, boolean>>({});                 // in-flight guards per store

  const syncing = !!running;

  const loadTriangle = useCallback(() => {
    api.triangle().then(setTri).catch((e: Error) => setError(e.message));
  }, []);

  // Poll the triangle on mount + after each build; pause while a sync runs.
  useEffect(() => { loadTriangle(); }, [loadTriangle, refreshKey]);
  useEffect(() => {
    if (syncing) return;
    pollRef.current = setInterval(loadTriangle, 4000);
    return () => { if (pollRef.current) clearInterval(pollRef.current); };
  }, [loadTriangle, syncing]);
  useEffect(() => {
    const t = setInterval(() => tick((n) => n + 1), 1000);
    return () => clearInterval(t);
  }, []);

  // Dismiss the hover box on a click OUTSIDE it. Hovering shows the box; once it's
  // up, clicking empty space (or anywhere that isn't the box) closes it — even if
  // the pointer is still technically over a node the box overlaps. Clicks INSIDE
  // the box (its push/launch/reset buttons) are left alone so they can fire.
  useEffect(() => {
    if (!hovered) return;
    const onDown = (e: PointerEvent) => {
      if (!(e.target as Element).closest(".dia-popover")) setHovered(null);
    };
    document.addEventListener("pointerdown", onDown, true);
    return () => document.removeEventListener("pointerdown", onDown, true);
  }, [hovered]);

  // The diff cache is invalidated whenever the stores may have moved (a build
  // ran, or the triangle re-polled into a new shape). Clear it so the next hover
  // re-fetches fresh "what would this store overwrite" data.
  useEffect(() => { setDiffs({}); diffReqRef.current = {}; }, [refreshKey]);

  // Lazily fetch the diff for whichever STORE is hovered (cached per store). The
  // client is not a store and has no diff — skip it.
  useEffect(() => {
    if (!hovered || hovered === "client" || diffs[hovered] || diffReqRef.current[hovered]) return;
    const store = hovered; // narrowed: StoreId
    diffReqRef.current[store] = true;
    api.diff(store)
      .then((d: DiffResponse) => setDiffs((m) => ({ ...m, [store]: d })))
      .catch((e: Error) => setError(e.message))
      .finally(() => { diffReqRef.current[store] = false; });
  }, [hovered, diffs]);

  if (error) return <div className="login-error">Sync station error: {error}</div>;
  if (!tri) return <div className="dia-loading">Reading the three stores…</div>;

  const locked = tri.aheadOf === null;

  // The resting STATUS — who is HEAD right now. Agreement ⇒ the Rulebook hub.
  // Disagreement with a single concrete leg ⇒ that leg. A genuine 3-way split
  // ⇒ the most recent store, *as a read-out only*. Nothing here privileges a
  // store for action; it just colors the resting picture.
  const status: StoreId = locked
    ? "rulebook"
    : ((["rulebook", "reasoner", "postgres"] as StoreId[]).includes(tri.aheadOf as StoreId) ? (tri.aheadOf as StoreId) : mostRecentStore(tri));

  // Is `store` in sync with the resting HEAD? (drives the resting node colors)
  const hashEq = (a: StoreId, b: StoreId): boolean => tri.hashes[a] === tri.hashes[b];
  const nodeState = (id: StoreId): "head" | "lit" | "stale" => {
    if (id === status) return "head";
    if (locked) return "lit";
    return hashEq(id, status) ? "lit" : "stale";
  };

  // Animate the legs a "push the hovered store into both others" would touch,
  // while a sync runs. If the hovered source is an engine, its own leg flows UP
  // to the hub and the other leg receives (delayed down). Rulebook ⇒ both down.
  // The client isn't a leg source — fall back to the resting HEAD store.
  const src: StoreId = hovered && hovered !== "client" ? hovered : status;
  const legPlan: { left: LegFlow | null; right: LegFlow | null } = (() => {
    if (!syncing) return { left: null, right: null };
    if (src === "rulebook") return { left: "down", right: "down" };
    if (src === "reasoner") return { left: "up", right: "down-delayed" };
    return { left: "down-delayed", right: "up" }; // postgres
  })();

  const labels = {
    rulebook: { inSync: locked || hashEq("rulebook", status), ago: fmtAgo(tri.legs.rulebook.lastEditAt) },
    reasoner: { inSync: locked || hashEq("reasoner", status), ago: fmtAgo(tri.legs.reasoner.lastEditAt) },
    postgres: { inSync: locked || hashEq("postgres", status), ago: fmtAgo(tri.legs.postgres.lastEditAt) },
  };

  const runSync = (from: StoreId, targets: StoreId[]) => {
    onRunSync({
      kind: "sync",
      path: "/api/control/sync",
      body: { from, targets },
      label: `${STORE_LABEL[from]} → ${targets.map((t) => STORE_LABEL[t]).join(" + ")}`,
    });
  };
  const runReset = () => {
    onRunSync({ kind: "reset", path: "/api/control/reset", body: null, label: "Reset to pristine baseline" });
  };

  // Which engine the CLIENT currently reads (lights one wire, tints the apex).
  // getBackend() may hold an explicit "rules:data" cross-run for the easter-egg
  // 2×2; for the diamond we only care which of the two engines it resolves to,
  // defaulting to the reasoner (the api.ts default).
  const rawBackend = getBackend();
  const connectedEngine: StoreId = rawBackend === "postgres" ? "postgres" : "reasoner";

  // The backend id we'd enter the app on for a given engine (maps through the
  // backend descriptors when present; falls back to the engine id itself).
  const launchIdFor = (store: StoreId): string => {
    const match = (backends || []).find((b) => b.id === store);
    return match ? match.id : store;
  };

  // ---- the hover box, built for whichever node is hovered ------------------
  const buildPopover = (head: StoreId): React.ReactElement => {
    const others = (["rulebook", "reasoner", "postgres"] as StoreId[]).filter((s) => s !== head);
    const diff = diffs[head];
    const totalDiff = others.reduce((acc, s) => {
      const d = diff?.against?.[s];
      return acc + (d ? d.changedFields + d.changedRows : 0);
    }, 0);
    const allInSync = locked && totalDiff === 0;
    // The push is "suggested" (recency hint) when head IS the store the triangle
    // flagged as ahead — still just a hint, never privileged.
    const suggested = !locked && tri.aheadOf === head;
    // Copy for the ONE push button, by which node is hovered. Every push makes
    // `head` the truth everywhere (it always round-trips through the hub, so it
    // lands all three locked); the glyph just shows the dominant direction.
    const PUSH = {
      rulebook: { glyph: "▾", text: <>Push down into <em>both legs</em></> },
      reasoner: { glyph: "▴▸", text: <>Make <em>Reasoner</em> the truth everywhere</> },
      postgres: { glyph: "◂▴", text: <>Make <em>Postgres</em> the truth everywhere</> },
    }[head];

    return (
      <div className="dia-pop">
        <div className="dia-pop-head">
          <span className="dia-pop-title">
            <strong>{STORE_LABEL[head]}</strong>
            <span className="dia-pop-sub">{STORE_SUB[head]}</span>
          </span>
          {allInSync
            ? <span className="dia-pop-status ok">✓ in sync</span>
            : (diff
                ? <span className="dia-pop-status warn">{totalDiff} change{totalDiff !== 1 ? "s" : ""} if authoritative</span>
                : <span className="dia-pop-status muted">reading…</span>)}
        </div>

        {allInSync ? (
          // Nothing to push — offer only the action that still makes sense here.
          <div className="dia-pop-insync">
            <p className="dia-pop-insync-note">
              All three stores agree — nothing to push from {STORE_LABEL[head]}.
            </p>
            {head === "rulebook" && (
              <button type="button" className="sync-btn sync-btn-reset" onClick={runReset} disabled={syncing}>
                <span className="sync-btn-glyph">⟲▾</span>
                <span className="sync-btn-text">Reset to the committed baseline, then rebuild both</span>
              </button>
            )}
          </div>
        ) : (
          <>
            {/* DIFF: what each other store would lose if `head` were authoritative */}
            <div className="dia-pop-diff">
              {!diff ? (
                <div className="dia-loading">Reading what {STORE_LABEL[head]} would overwrite…</div>
              ) : others.map((store) => {
                const d: DiffAgainst | undefined = diff?.against?.[store];
                const empty = !d || (d.changedRows === 0 && d.changedFields === 0);
                return (
                  <div key={store} className={`diff-block ${empty ? "is-empty" : ""}`}>
                    <div className="diff-block-head">
                      <span className="diff-arrow">{STORE_LABEL[store]} <em>becomes</em> {STORE_LABEL[head]}</span>
                      {empty
                        ? <span className="diff-count diff-count-ok">in sync</span>
                        : <span className="diff-count">{d.changedRows} row{d.changedRows !== 1 ? "s" : ""}, {d.changedFields} field{d.changedFields !== 1 ? "s" : ""}</span>}
                    </div>
                    {!empty && (
                      <div className="diff-tables">
                        {d.tables.map((t) => (
                          <div key={t.table} className="diff-table">
                            <div className="diff-table-name">{t.table}</div>
                            {t.rows.map((row) => (
                              <div key={row.pk} className={`diff-row diff-${row.kind}`}>
                                <div className="diff-row-pk">
                                  <span className={`diff-kind diff-kind-${row.kind}`}>{row.kind}</span>
                                  {row.pk}
                                </div>
                                {row.kind === "changed" && row.fields.map((f) => (
                                  <div key={f.field} className="diff-field">
                                    <span className="diff-field-name">{f.field}</span>
                                    <ValueCell v={f.other} kind="dv-old" />
                                    <span className="diff-to">→</span>
                                    <ValueCell v={f.head} kind="dv-new" />
                                  </div>
                                ))}
                                {row.kind === "added" && <div className="diff-note">created in {STORE_LABEL[head]} — would be inserted</div>}
                                {row.kind === "removed" && <div className="diff-note">absent in {STORE_LABEL[head]} — would be deleted</div>}
                              </div>
                            ))}
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                );
              })}
            </div>

            {/* THE PUSH — one per node. Always makes `head` the truth everywhere;
                it round-trips through the hub, so it lands all three locked. */}
            <div className="dia-pop-pushes">
              <button type="button"
                className={`sync-btn sync-btn-both ${suggested ? "is-suggested" : ""}`}
                onClick={() => runSync(head, others)} disabled={syncing}>
                <span className="sync-btn-glyph">{PUSH.glyph}</span>
                <span className="sync-btn-text">{PUSH.text}</span>
                {suggested && <span className="sync-btn-suggest">suggested</span>}
              </button>

              {/* the escape hatch: git reset, then push down. Quarantined + risk-colored. */}
              <button type="button" className="sync-btn sync-btn-reset" onClick={runReset} disabled={syncing}>
                <span className="sync-btn-glyph">⟲▾</span>
                <span className="sync-btn-text">Reset to the committed baseline, then rebuild both</span>
              </button>
            </div>
          </>
        )}
      </div>
    );
  };

  // ---- the CLIENT hover box: an engine picker, not a diff ------------------
  // The client is a reader, not a store. Picking an engine sets the backend AND
  // enters the app (onEnter). Each option shows that engine's drift state so you
  // can see you're about to read a STALE engine before committing.
  const buildClientPopover = (): React.ReactElement => {
    const ENGINE_SUB: Record<StoreId, string> = {
      reasoner: "OWL/SHACL reasoner", postgres: "Postgres vw_* views", rulebook: "",
    };
    return (
      <div className="dia-pop dia-pop-client">
        <div className="dia-pop-head">
          <span className="dia-pop-title">
            <strong>Client</strong>
            <span className="dia-pop-sub">the app reads exactly one engine</span>
          </span>
          <span className="dia-pop-status muted">wired to {STORE_LABEL[connectedEngine]}</span>
        </div>
        <p className="dia-pop-insync-note">
          The same UI renders whatever engine it's wired to. Pick the engine to read,
          and enter — the engine has already computed every derived field.
        </p>
        <div className="dia-pop-pushes">
          {(["reasoner", "postgres"] as StoreId[]).map((eng) => {
            const st = nodeState(eng);
            const isWired = eng === connectedEngine;
            return (
              <button key={eng} type="button"
                className={`sync-btn sync-btn-enter st-${st} ${isWired ? "is-wired" : ""}`}
                disabled={syncing}
                onClick={() => onEnter(launchIdFor(eng))}>
                <span className="sync-btn-glyph">▸</span>
                <span className="sync-btn-text">
                  Enter on <em>{STORE_LABEL[eng]}</em>
                  <span className="sync-btn-sub">{ENGINE_SUB[eng]}</span>
                </span>
                {isWired && <span className="sync-btn-suggest">connected</span>}
                {st === "stale" && <span className="sync-btn-stale">trailing</span>}
              </button>
            );
          })}
        </div>
      </div>
    );
  };

  const popover = !hovered
    ? null
    : hovered === "client"
      ? buildClientPopover()
      : buildPopover(hovered);

  return (
    <div className="sync-station">
      <Diamond
        nodeState={nodeState}
        status={status}
        hovered={hovered}
        legPlan={legPlan}
        labels={labels}
        syncing={syncing}
        connectedEngine={connectedEngine}
        onHover={setHovered}
        disabled={syncing}
        popover={popover}
      />
    </div>
  );
}

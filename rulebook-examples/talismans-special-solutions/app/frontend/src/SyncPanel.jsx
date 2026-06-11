import React, { useEffect, useState, useCallback, useRef } from "react";
import { api } from "./api.js";
import Triangle from "./Triangle.jsx";

// ===========================================================================
// SyncPanel.jsx — the ONE interactive control. Nothing is privileged.
//
// Resting state: the triangle shows the COMPUTED drift status — who is HEAD
// right now (Rulebook when all three agree; otherwise the most-recently-edited
// store, shown purely as a status read-out). There is NO selection to make.
//
// Interaction: HOVER any node to ask "if THIS store were authoritative, what
// would the others lose, and how would I push it?" — a floaty box appears next
// to that node with the field-level diff and the directional pushes, and you act
// straight from the hover. Move away and the box vanishes (so it can never cover
// the other nodes — the old sticky box trapped you on one HEAD). Recency only
// decides the resting STATUS; every node is equally hoverable and equally
// actionable, because none of the three is privileged.
//
// When the three already agree there is nothing to push: hovering an engine leg
// offers just "Launch the console on X"; hovering the Rulebook offers just
// "Reset to baseline". Pushes appear only when there's a real diff to resolve.
//
// All pushes compose from ONE backend action (/api/control/sync {from,targets});
// reset is /api/control/reset. Diffs are fetched per store lazily and cached
// (regeneratable, invalidated on refreshKey) so any hovered node can show "what
// it would overwrite" without a chosen-HEAD round-trip.
// ===========================================================================

const STORE_LABEL = { rulebook: "Rulebook", reasoner: "Reasoner", postgres: "Postgres" };
const STORE_SUB = { rulebook: "the hub", reasoner: "db.json", postgres: "Postgres tables" };
const ENGINES = new Set(["reasoner", "postgres"]); // legs you can launch the console on

function fmtAgo(iso) {
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
function mostRecentStore(tri) {
  const t = (s) => {
    const v = s === "rulebook" ? tri.legs.rulebook.lastEditAt : tri.legs[s]?.lastEditAt;
    return v ? new Date(v).getTime() : -1;
  };
  return ["reasoner", "postgres", "rulebook"].sort((a, b) => t(b) - t(a))[0];
}

// Render one value as it should appear in the diff cell.
function ValueCell({ v, kind }) {
  if (v === null || v === undefined) return <span className={`dv dv-absent ${kind}`}>∅</span>;
  let s = typeof v === "string" ? v : JSON.stringify(v);
  const long = s.length > 60;
  if (long) s = s.slice(0, 60) + "…";
  return <span className={`dv ${kind}`} title={typeof v === "string" ? v : JSON.stringify(v)}>{s}</span>;
}

export default function SyncPanel({ onRunSync, running, refreshKey, backends, onEnter }) {
  const [tri, setTri] = useState(null);
  const [diffs, setDiffs] = useState({});       // { store: diffResult } cache
  const [hovered, setHovered] = useState(null);  // the store currently hovered
  const [error, setError] = useState(null);
  const [, tick] = useState(0);                  // keep "ago" fresh
  const pollRef = useRef(null);
  const diffReqRef = useRef({});                 // in-flight guards per store

  const syncing = !!running;

  const loadTriangle = useCallback(() => {
    api.triangle().then(setTri).catch((e) => setError(e.message));
  }, []);

  // Poll the triangle on mount + after each build; pause while a sync runs.
  useEffect(() => { loadTriangle(); }, [loadTriangle, refreshKey]);
  useEffect(() => {
    if (syncing) return;
    pollRef.current = setInterval(loadTriangle, 4000);
    return () => clearInterval(pollRef.current);
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
    const onDown = (e) => {
      if (!e.target.closest(".tri-popover")) setHovered(null);
    };
    document.addEventListener("pointerdown", onDown, true);
    return () => document.removeEventListener("pointerdown", onDown, true);
  }, [hovered]);

  // The diff cache is invalidated whenever the stores may have moved (a build
  // ran, or the triangle re-polled into a new shape). Clear it so the next hover
  // re-fetches fresh "what would this store overwrite" data.
  useEffect(() => { setDiffs({}); diffReqRef.current = {}; }, [refreshKey]);

  // Lazily fetch the diff for whichever store is hovered (cached per store).
  useEffect(() => {
    if (!hovered || diffs[hovered] || diffReqRef.current[hovered]) return;
    diffReqRef.current[hovered] = true;
    api.diff(hovered)
      .then((d) => setDiffs((m) => ({ ...m, [hovered]: d })))
      .catch((e) => setError(e.message))
      .finally(() => { diffReqRef.current[hovered] = false; });
  }, [hovered, diffs]);

  if (error) return <div className="login-error">Sync station error: {error}</div>;
  if (!tri) return <div className="tri-loading">Reading the three stores…</div>;

  const locked = tri.aheadOf === null;

  // The resting STATUS — who is HEAD right now. Agreement ⇒ the Rulebook hub.
  // Disagreement with a single concrete leg ⇒ that leg. A genuine 3-way split
  // ⇒ the most recent store, *as a read-out only*. Nothing here privileges a
  // store for action; it just colors the resting picture.
  const status = locked
    ? "rulebook"
    : (["rulebook", "reasoner", "postgres"].includes(tri.aheadOf) ? tri.aheadOf : mostRecentStore(tri));

  // Is `store` in sync with the resting HEAD? (drives the resting node colors)
  const hashEq = (a, b) => tri.hashes[a] === tri.hashes[b];
  const nodeState = (id) => {
    if (id === status) return "head";
    if (locked) return "lit";
    return hashEq(id, status) ? "lit" : "stale";
  };

  // Animate the legs a "push the hovered store into both others" would touch,
  // while a sync runs. If the hovered source is an engine, its own leg flows UP
  // to the hub and the other leg receives (delayed down). Rulebook ⇒ both down.
  const src = hovered || status;
  const legPlan = (() => {
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

  const runSync = (from, targets) => {
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

  // The backend id we'd launch the console on for a hovered engine leg.
  const launchIdFor = (store) => {
    if (!ENGINES.has(store)) return null;
    const match = (backends || []).find((b) => b.id === store);
    return match ? match.id : store;
  };

  // ---- the hover box, built for whichever node is hovered ------------------
  const buildPopover = (head) => {
    const others = ["rulebook", "reasoner", "postgres"].filter((s) => s !== head);
    const diff = diffs[head];
    const inSyncWith = (store) => {
      if (store === head) return true;
      const d = diff?.against?.[store];
      return d ? d.changedRows === 0 && d.changedFields === 0 : hashEq(store, head);
    };
    const totalDiff = others.reduce((acc, s) => {
      const d = diff?.against?.[s];
      return acc + (d ? d.changedFields + d.changedRows : 0);
    }, 0);
    const allInSync = locked && totalDiff === 0;
    const launchId = launchIdFor(head);
    // "push into both others" is the recency suggestion when head IS the store
    // the triangle flagged as ahead (still just a hint — never privileged).
    const suggestBoth = !locked && tri.aheadOf === head && ["rulebook", "reasoner", "postgres"].includes(tri.aheadOf);

    return (
      <div className="tri-pop">
        <div className="tri-pop-head">
          <span className="tri-pop-title">
            <strong>{STORE_LABEL[head]}</strong>
            <span className="tri-pop-sub">{STORE_SUB[head]}</span>
          </span>
          {allInSync
            ? <span className="tri-pop-status ok">✓ in sync</span>
            : (diff
                ? <span className="tri-pop-status warn">{totalDiff} change{totalDiff !== 1 ? "s" : ""} if authoritative</span>
                : <span className="tri-pop-status muted">reading…</span>)}
        </div>

        {allInSync ? (
          // Nothing to push — offer only the action that still makes sense here.
          <div className="tri-pop-insync">
            <p className="tri-pop-insync-note">
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
            <div className="tri-pop-diff">
              {!diff ? (
                <div className="tri-loading">Reading what {STORE_LABEL[head]} would overwrite…</div>
              ) : others.map((store) => {
                const d = diff?.against?.[store];
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

            {/* PUSHES — named by geometry; direction implied by which node is the source */}
            <div className="tri-pop-pushes">
              {head === "rulebook" ? (
                <>
                  <button type="button"
                    className={`sync-btn sync-btn-both ${suggestBoth ? "is-suggested" : ""}`}
                    onClick={() => runSync("rulebook", others)} disabled={syncing}>
                    <span className="sync-btn-glyph">▾</span>
                    <span className="sync-btn-text">Push down into <em>both legs</em></span>
                    {suggestBoth && <span className="sync-btn-suggest">suggested</span>}
                  </button>
                  <button type="button" className="sync-btn sync-btn-one"
                    onClick={() => runSync("rulebook", ["reasoner"])} disabled={syncing || inSyncWith("reasoner")}>
                    <span className="sync-btn-glyph">◂</span>
                    <span className="sync-btn-text">Push left into <em>Reasoner only</em></span>
                  </button>
                  <button type="button" className="sync-btn sync-btn-one"
                    onClick={() => runSync("rulebook", ["postgres"])} disabled={syncing || inSyncWith("postgres")}>
                    <span className="sync-btn-glyph">▸</span>
                    <span className="sync-btn-text">Push right into <em>Postgres only</em></span>
                  </button>
                </>
              ) : (
                <>
                  <button type="button"
                    className={`sync-btn sync-btn-both ${suggestBoth ? "is-suggested" : ""}`}
                    onClick={() => runSync(head, others)} disabled={syncing}>
                    <span className="sync-btn-glyph">▴▾</span>
                    <span className="sync-btn-text">Promote up &amp; sync <em>the other leg</em></span>
                    {suggestBoth && <span className="sync-btn-suggest">suggested</span>}
                  </button>
                  <button type="button" className="sync-btn sync-btn-one"
                    onClick={() => runSync(head, ["rulebook"])} disabled={syncing || inSyncWith("rulebook")}>
                    <span className="sync-btn-glyph">▴</span>
                    <span className="sync-btn-text">Promote up into <em>the Rulebook hub only</em></span>
                  </button>
                  {others.filter((s) => s !== "rulebook").map((leg) => (
                    <button key={leg} type="button" className="sync-btn sync-btn-one"
                      onClick={() => runSync(head, [leg])} disabled={syncing || inSyncWith(leg)}>
                      <span className="sync-btn-glyph">{leg === "reasoner" ? "◂" : "▸"}</span>
                      <span className="sync-btn-text">Promote up, then overwrite <em>{STORE_LABEL[leg]} only</em></span>
                    </button>
                  ))}
                </>
              )}

              {/* the 4th choice: git reset, then push down. Quarantined + risk-colored. */}
              <button type="button" className="sync-btn sync-btn-reset" onClick={runReset} disabled={syncing}>
                <span className="sync-btn-glyph">⟲▾</span>
                <span className="sync-btn-text">Reset to the committed baseline, then rebuild both</span>
              </button>
            </div>
          </>
        )}

        {/* LAUNCH — engine legs only; the hub has nothing to run the console on. */}
        {launchId && (
          <button type="button" className="tri-pop-launch" disabled={syncing}
            onClick={() => onEnter(launchId)}>
            Launch the console on <strong>{STORE_LABEL[head]}</strong> →
          </button>
        )}
      </div>
    );
  };

  const popover = hovered ? buildPopover(hovered) : null;

  return (
    <div className="sync-station">
      <Triangle
        nodeState={nodeState}
        status={status}
        hovered={hovered}
        legPlan={legPlan}
        labels={labels}
        syncing={syncing}
        onHover={setHovered}
        disabled={syncing}
        popover={popover}
      />
    </div>
  );
}

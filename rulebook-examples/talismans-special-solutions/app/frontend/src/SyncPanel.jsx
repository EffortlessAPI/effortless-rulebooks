import React, { useEffect, useState, useCallback, useRef } from "react";
import { api } from "./api.js";
import Triangle from "./Triangle.jsx";

// ===========================================================================
// SyncPanel.jsx — the ONE interactive control.
//
// Everything the login page used to stack in three sections is folded into the
// triangle itself:
//
//   • Click any node → it becomes HEAD (the former HEAD chips, gone).
//   • A floaty box pops next to the focused node holding:
//       - the field-level DIFF for that HEAD (what each other store would lose),
//       - the directional PUSHES, named by geometry from the apex:
//           ◂ overwrite the left leg (reasoner)
//           ▸ overwrite the right leg (postgres)
//           ▾ overwrite BOTH legs
//           ▴ promote a leg UP into the hub
//         (which of these show depends on which node is HEAD),
//       - a quarantined, risk-colored "Reset to baseline, then rebuild" — the
//         git-reset-then-down 4th choice,
//       - and, for an ENGINE leg only, a "Launch console as X →" button. The hub
//         has no launch (you don't run the UI on the rulebook).
//
// All pushes compose from ONE backend action (/api/control/sync {from,targets});
// reset is /api/control/reset. The component fetches its own triangle + diff and
// hands the parent ready-to-run descriptors via onRunSync; the parent streams
// the build into the shared console below.
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

// The store the recency markers say was touched most recently (fallback when the
// triangle's `aheadOf` isn't a single concrete store, e.g. "diverged").
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
  const [diff, setDiff] = useState(null);
  const [head, setHead] = useState(null);          // chosen authoritative store
  const [userPicked, setUserPicked] = useState(false);
  const [error, setError] = useState(null);
  const [, tick] = useState(0);                    // keep "ago" fresh
  const pollRef = useRef(null);

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

  // Default HEAD follows the recency suggestion until the user picks one.
  const suggestion = tri
    ? (["rulebook", "reasoner", "postgres"].includes(tri.aheadOf) ? tri.aheadOf : mostRecentStore(tri))
    : null;
  useEffect(() => {
    if (!userPicked && suggestion) setHead(suggestion);
  }, [suggestion, userPicked]);

  // Load the diff whenever the chosen HEAD changes (or after a build relocks).
  useEffect(() => {
    if (!head) return;
    api.diff(head).then(setDiff).catch((e) => setError(e.message));
  }, [head, refreshKey]);

  if (error) return <div className="login-error">Sync station error: {error}</div>;
  if (!tri) return <div className="tri-loading">Reading the three stores…</div>;

  const locked = tri.aheadOf === null;
  const others = ["rulebook", "reasoner", "postgres"].filter((s) => s !== head);

  // Which stores are in sync with the chosen HEAD (zero diff). Drives node state.
  const inSyncWith = (store) => {
    if (store === head) return true;
    const d = diff?.against?.[store];
    return d ? d.changedRows === 0 && d.changedFields === 0 : tri.hashes[store] === tri.hashes[head];
  };

  // View model for the Triangle. HEAD is the focus (gold ring); stores that match
  // HEAD are "lit"; stores that differ are "stale" (they'd be overwritten).
  const nodeState = (id) => {
    if (id === head) return "head";
    if (locked) return "lit";
    return inSyncWith(id) ? "lit" : "stale";
  };
  // Animate the legs a "push HEAD into both others" would touch. If HEAD is an
  // engine, its own leg flows UP to the hub and the other leg receives (delayed
  // down). If HEAD is the rulebook, both legs flow DOWN. Only while syncing.
  const legPlan = (() => {
    if (!syncing) return { left: null, right: null };
    if (head === "rulebook") return { left: "down", right: "down" };
    if (head === "reasoner") return { left: "up", right: "down-delayed" };
    return { left: "down-delayed", right: "up" }; // postgres HEAD
  })();
  const labels = {
    rulebook: { inSync: inSyncWith("rulebook"), ago: fmtAgo(tri.legs.rulebook.lastEditAt) },
    reasoner: { inSync: inSyncWith("reasoner"), ago: fmtAgo(tri.legs.reasoner.lastEditAt) },
    postgres: { inSync: inSyncWith("postgres"), ago: fmtAgo(tri.legs.postgres.lastEditAt) },
  };

  const pickHead = (s) => { setHead(s); setUserPicked(true); };

  const runSync = (targets) => {
    onRunSync({
      kind: "sync",
      path: "/api/control/sync",
      body: { from: head, targets },
      label: `${STORE_LABEL[head]} → ${targets.map((t) => STORE_LABEL[t]).join(" + ")}`,
    });
  };
  const runReset = () => {
    onRunSync({ kind: "reset", path: "/api/control/reset", body: null, label: "Reset to pristine baseline" });
  };

  // Is "push HEAD into both others" the recency suggestion?
  const suggestBoth = !locked && tri.aheadOf === head && ["rulebook", "reasoner", "postgres"].includes(tri.aheadOf);

  const totalDiff = others.reduce((acc, s) => {
    const d = diff?.against?.[s];
    return acc + (d ? d.changedFields + d.changedRows : 0);
  }, 0);

  // The backend id we'd launch the console on for the focused engine leg. The
  // server's backend list (api.backends) names them; we match by id so the easter-
  // egg cross-runs still resolve. Falls back to the bare store id.
  const launchId = (() => {
    if (!ENGINES.has(head)) return null;
    const match = (backends || []).find((b) => b.id === head);
    return match ? match.id : head;
  })();

  // ---- the floaty box rendered next to the focused node --------------------
  // Diff (compact) + geometric pushes + quarantined reset + (engine) launch.
  const popover = (
    <div className="tri-pop">
      <div className="tri-pop-head">
        <span className="tri-pop-title">
          <strong>{STORE_LABEL[head]}</strong> is HEAD
          <span className="tri-pop-sub">{STORE_SUB[head]}</span>
        </span>
        {locked && totalDiff === 0
          ? <span className="tri-pop-status ok">✓ all in sync</span>
          : <span className="tri-pop-status warn">{totalDiff} change{totalDiff !== 1 ? "s" : ""} pending</span>}
      </div>

      {/* DIFF: what each other store would lose if HEAD wins */}
      <div className="tri-pop-diff">
        {locked && totalDiff === 0 ? (
          <div className="sync-locked">All three stores agree — nothing to push.</div>
        ) : (
          others.map((store) => {
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
          })
        )}
      </div>

      {/* PUSHES — named by geometry. Direction is implied by which node is HEAD. */}
      <div className="tri-pop-pushes">
        {head === "rulebook" ? (
          <>
            <button type="button"
              className={`sync-btn sync-btn-both ${suggestBoth ? "is-suggested" : ""}`}
              onClick={() => runSync(others)} disabled={syncing || (locked && totalDiff === 0)}>
              <span className="sync-btn-glyph">▾</span>
              <span className="sync-btn-text">Push down into <em>both legs</em></span>
              {suggestBoth && <span className="sync-btn-suggest">suggested</span>}
            </button>
            <button type="button" className="sync-btn sync-btn-one"
              onClick={() => runSync(["reasoner"])} disabled={syncing || inSyncWith("reasoner")}>
              <span className="sync-btn-glyph">◂</span>
              <span className="sync-btn-text">Push left into <em>Reasoner only</em></span>
            </button>
            <button type="button" className="sync-btn sync-btn-one"
              onClick={() => runSync(["postgres"])} disabled={syncing || inSyncWith("postgres")}>
              <span className="sync-btn-glyph">▸</span>
              <span className="sync-btn-text">Push right into <em>Postgres only</em></span>
            </button>
          </>
        ) : (
          <>
            <button type="button"
              className={`sync-btn sync-btn-both ${suggestBoth ? "is-suggested" : ""}`}
              onClick={() => runSync(others)} disabled={syncing || (locked && totalDiff === 0)}>
              <span className="sync-btn-glyph">▴▾</span>
              <span className="sync-btn-text">Promote up &amp; sync <em>the other leg</em></span>
              {suggestBoth && <span className="sync-btn-suggest">suggested</span>}
            </button>
            <button type="button" className="sync-btn sync-btn-one"
              onClick={() => runSync(["rulebook"])} disabled={syncing || inSyncWith("rulebook")}>
              <span className="sync-btn-glyph">▴</span>
              <span className="sync-btn-text">Promote up into <em>the Rulebook hub only</em></span>
            </button>
            {others.filter((s) => s !== "rulebook").map((leg) => (
              <button key={leg} type="button" className="sync-btn sync-btn-one"
                onClick={() => runSync([leg])} disabled={syncing || inSyncWith(leg)}>
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

      {/* LAUNCH — engine legs only; the hub has nothing to run the console on. */}
      {launchId && (
        <button type="button" className="tri-pop-launch" disabled={syncing}
          onClick={() => onEnter(launchId)}>
          Launch the console on <strong>{STORE_LABEL[head]}</strong> →
        </button>
      )}
    </div>
  );

  return (
    <div className="sync-station">
      <Triangle
        nodeState={nodeState}
        legPlan={legPlan}
        labels={labels}
        focus={head}
        syncing={syncing}
        onPick={pickHead}
        disabled={syncing}
        popover={popover}
      />
    </div>
  );
}

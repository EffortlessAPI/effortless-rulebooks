import React, { useEffect, useState, useCallback, useRef } from "react";
import { api } from "./api.js";
import Triangle from "./Triangle.jsx";

// ===========================================================================
// SyncPanel.jsx — the unified "sync station".
//
// One surface that combines everything the user asked for:
//
//   1. The Head & Legs TRIANGLE (visual drift indicator).
//   2. A HEAD selector — ANY of the three stores can be made authoritative at
//      any time, regardless of which is most recent. Default = the recency
//      suggestion, but the user is free to override.
//   3. The field-level DIFF for the chosen HEAD: for each OTHER store, exactly
//      which values would be replaced (other-value → HEAD-value), so you can see
//      what a push would do BEFORE you do it.
//   4. Directional SYNC buttons driven by the chosen HEAD: overwrite both other
//      stores (V), overwrite just one leg (< / >), or push up into the rulebook
//      (^). The recency-suggested direction is highlighted, but every direction
//      is always available. Plus "Reset to baseline" (reset root).
//
// All three stores and all directions compose from ONE backend action
// (/api/control/sync {from, targets}); the panel just decides the target set.
// ===========================================================================

const STORE_LABEL = { rulebook: "Rulebook", reasoner: "Reasoner", postgres: "Postgres" };
const STORE_SUB = { rulebook: "the hub", reasoner: "db.json", postgres: "Postgres tables" };
// Left-to-right layout order, for choosing ◂ vs ▸ glyphs on single-leg pushes.
const NODES_ORDER = { reasoner: 0, rulebook: 1, postgres: 2 };

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
  const long = s.length > 80;
  if (long) s = s.slice(0, 80) + "…";
  return <span className={`dv ${kind}`} title={typeof v === "string" ? v : JSON.stringify(v)}>{s}</span>;
}

export default function SyncPanel({ onRunSync, running, refreshKey }) {
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

  // Default HEAD follows the recency suggestion until the user picks one. When
  // the suggestion changes (a new edit lands) and the user hasn't overridden, we
  // follow it; once they pick, we respect their choice.
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

  // View model for the (presentational) Triangle. HEAD is the focus (gold ring);
  // stores that already match HEAD are "lit"; stores that differ are "stale"
  // (they'd be overwritten). When everything's locked, all three read "lit".
  const nodeState = (id) => {
    if (id === head) return "head";
    if (locked) return "lit";
    return inSyncWith(id) ? "lit" : "stale";
  };
  // Animate the legs that a "push HEAD into both others" would touch: from HEAD
  // toward each store that differs. If HEAD is an engine, its leg flows UP to the
  // hub; the hub→other-leg flows DOWN. If HEAD is the rulebook, both legs flow
  // DOWN. Only animate while a sync is actually running.
  const legPlan = (() => {
    if (!syncing) return { left: null, right: null };
    if (head === "rulebook") return { left: "down", right: "down" };
    // engine HEAD: its own leg pushes up, the other leg receives (delayed down)
    if (head === "reasoner") return { left: "up", right: "down-delayed" };
    return { left: "down-delayed", right: "up" }; // postgres HEAD
  })();
  const labels = {
    rulebook: { inSync: inSyncWith("rulebook"), ago: fmtAgo(tri.legs.rulebook.lastEditAt) },
    reasoner: { inSync: inSyncWith("reasoner"), ago: fmtAgo(tri.legs.reasoner.lastEditAt) },
    postgres: { inSync: inSyncWith("postgres"), ago: fmtAgo(tri.legs.postgres.lastEditAt) },
  };

  // The directional buttons for the chosen HEAD. Each maps to one /api/control
  // call. The recency-suggested direction is flagged is-suggested.
  // - overwrite ONE other store  → sync {from:head, targets:[that store]}
  // - overwrite BOTH others      → sync {from:head, targets:[both]}
  // - reset root                 → the existing reset action
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

  // Is "push HEAD into both others" the recency suggestion? (It is when HEAD is
  // exactly the store the triangle flagged as ahead.)
  const suggestBoth = !locked && tri.aheadOf === head && ["rulebook", "reasoner", "postgres"].includes(tri.aheadOf);

  const totalDiff = others.reduce((acc, s) => {
    const d = diff?.against?.[s];
    return acc + (d ? d.changedFields + d.changedRows : 0);
  }, 0);

  return (
    <div className="sync-station">
      <Triangle nodeState={nodeState} legPlan={legPlan} labels={labels} focus={head} syncing={syncing} />

      {/* HEAD selector — anything can be authoritative at any time */}
      <div className="sync-headpick">
        <span className="sync-headpick-label">Make authoritative (HEAD):</span>
        <div className="sync-headpick-chips">
          {["rulebook", "reasoner", "postgres"].map((s) => (
            <button key={s} type="button"
              className={`headchip ${head === s ? "is-head" : ""} ${suggestion === s ? "is-suggested" : ""}`}
              onClick={() => pickHead(s)} disabled={syncing}
              title={suggestion === s ? "Most recently edited — suggested" : ""}>
              <span className="headchip-name">{STORE_LABEL[s]}</span>
              <span className="headchip-sub">{STORE_SUB[s]}</span>
              {suggestion === s && <span className="headchip-suggest">suggested</span>}
            </button>
          ))}
        </div>
      </div>

      {/* the diff — what each OTHER store would lose if HEAD wins */}
      <div className="sync-diff">
        {locked && totalDiff === 0 ? (
          <div className="sync-locked">✓ All three stores agree — nothing to sync.</div>
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

      {/* directional sync buttons — every direction always available */}
      <div className="sync-actions">
        <button type="button"
          className={`sync-btn sync-btn-both ${suggestBoth ? "is-suggested" : ""}`}
          onClick={() => runSync(others)} disabled={syncing || (locked && totalDiff === 0)}>
          <span className="sync-btn-glyph">▾</span>
          <span className="sync-btn-text">{STORE_LABEL[head]} → overwrite both
            <em> ({others.map((s) => STORE_LABEL[s]).join(" + ")})</em></span>
          {suggestBoth && <span className="sync-btn-suggest">suggested</span>}
        </button>

        {others.map((store) => (
          <button key={store} type="button"
            className="sync-btn sync-btn-one"
            onClick={() => runSync([store])} disabled={syncing || inSyncWith(store)}>
            <span className="sync-btn-glyph">{store === "rulebook" ? "▴" : (NODES_ORDER[store] < NODES_ORDER[head] ? "◂" : "▸")}</span>
            <span className="sync-btn-text">{STORE_LABEL[head]} → overwrite {STORE_LABEL[store]} only</span>
          </button>
        ))}

        <button type="button" className="sync-btn sync-btn-reset" onClick={runReset} disabled={syncing}>
          <span className="sync-btn-glyph">⟲</span>
          <span className="sync-btn-text">Reset everything to the committed baseline</span>
        </button>
      </div>
    </div>
  );
}

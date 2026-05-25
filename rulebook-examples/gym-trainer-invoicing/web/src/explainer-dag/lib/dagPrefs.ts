// Persistent UI prefs for the DAG explorer (localStorage). One source of
// truth so DagCell and DagToggle stay in sync via a custom event.

import { useSyncExternalStore } from "react";

const STORAGE_KEY = "dag.glyphsOn";
const EVENT = "dag-prefs-changed";

// Default: OFF. Provenance is opt-in via the DagToggle so the host app
// looks like a normal admin UI by default; double-click on any DagCell
// still navigates to the field's DAG page regardless of this setting.
function getGlyphsOnFromStorage(): boolean {
  const v = localStorage.getItem(STORAGE_KEY);
  if (v === null) return false;
  return v === "1";
}

export function setGlyphsOn(on: boolean): void {
  localStorage.setItem(STORAGE_KEY, on ? "1" : "0");
  window.dispatchEvent(new Event(EVENT));
}

function subscribe(cb: () => void): () => void {
  const handler = () => cb();
  window.addEventListener(EVENT, handler);
  // Also react to storage events from other tabs.
  window.addEventListener("storage", handler);
  return () => {
    window.removeEventListener(EVENT, handler);
    window.removeEventListener("storage", handler);
  };
}

export function useGlyphsOn(): boolean {
  return useSyncExternalStore(subscribe, getGlyphsOnFromStorage, () => true);
}

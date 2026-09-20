import { createContext, useCallback, useContext, useEffect, useState, type ReactNode } from "react";
import * as api from "./api";

type Toast = { id: number; tone: "green" | "red" | "amber"; title: string; detail?: string };
type Ctx = {
  shell: api.Shell | null; loading: boolean;
  signIn: (appUserId: string, principalId: string) => Promise<api.Shell>; signOut: () => void;
  action: (id: string) => api.Action; toasts: Toast[]; toast: (t: Omit<Toast, "id">) => void;
  /** bump to make every page re-read its views after a write */
  version: number; bump: () => void;
  /** Show the explainers: every key value can say where it came from. Set on the sign-in page. */
  explain: boolean; setExplain: (on: boolean) => void;
};

// A preference, not domain data, so it lives in the browser and not in the
// rulebook. It is deliberately on by default: this register's whole claim is
// that nobody typed the worked-out values, and a reader cannot check that claim
// with the explainers switched off.
const EXPLAIN_KEY = "pko.mobile.explain";
const C = createContext<Ctx>(null as unknown as Ctx);
export const useSession = () => useContext(C);

export function SessionProvider({ children }: { children: ReactNode }) {
  const [shell, setShell] = useState<api.Shell | null>(null);
  const [loading, setLoading] = useState(!!api.getToken());
  const [toasts, setToasts] = useState<Toast[]>([]);
  const [version, setVersion] = useState(0);
  const [explain, setExplainState] = useState(() => localStorage.getItem(EXPLAIN_KEY) !== "off");
  const setExplain = useCallback((on: boolean) => {
    localStorage.setItem(EXPLAIN_KEY, on ? "on" : "off"); setExplainState(on);
  }, []);

  useEffect(() => {
    if (!api.getToken()) return;
    api.shell().then(setShell).catch(() => api.setToken(null)).finally(() => setLoading(false));
  }, []);

  const signIn = useCallback(async (u: string, p: string) => {
    const { token } = await api.signIn(u, p); api.setToken(token);
    const s = await api.shell(); setShell(s); return s;
  }, []);
  const signOut = useCallback(() => { api.setToken(null); setShell(null); }, []);
  const toast = useCallback((t: Omit<Toast, "id">) => {
    const id = Date.now() + Math.random();
    setToasts([{ ...t, id }]);   // one at a time: the newest replaces the last, so the screen is never buried
    setTimeout(() => setToasts((x) => x.filter((y) => y.id !== id)), 3800);
  }, []);
  const action = useCallback((id: string) => {
    const a = shell?.actions.find((x) => x.app_action_id === id);
    if (!a) throw new Error(`This sign-in has no action ${id}. Actions are rows in AppActions for the signed-in role.`);
    return a;
  }, [shell]);

  return <C.Provider value={{ shell, loading, signIn, signOut, action, toasts, toast, version, bump: () => setVersion((v) => v + 1), explain, setExplain }}>{children}</C.Provider>;
}

/** Read rows from my schema; re-read whenever anything is written. Errors are shown, never swallowed. */
export function useRows(table: string | null, filters: Record<string, string | boolean | number> = {}, order?: string) {
  const { version } = useSession();
  const [data, setData] = useState<api.Row[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const key = JSON.stringify([table, filters, order]);
  useEffect(() => {
    if (!table) { setData(null); return; }
    let live = true;
    api.rows(table, filters, order).then((r) => live && (setData(r), setError(null))).catch((e) => live && setError(e.message));
    return () => { live = false; };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [key, version]);
  return { rows: data, error };
}

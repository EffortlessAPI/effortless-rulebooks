import { useEffect, useLayoutEffect, useRef } from 'react';
import { Link, Outlet, useNavigate, useParams } from 'react-router-dom';

declare global {
  interface Window {
    EffortlessExplainer: {
      init: (opts: Record<string, unknown>) => { enhanceCells: (r?: ParentNode) => void };
      renderFieldPage: (el: HTMLElement, table: string, field: string) => void;
      renderTablePage: (el: HTMLElement, table: string) => void;
      renderTablesIndex: (el: HTMLElement) => void;
      enhanceCells: (root?: ParentNode) => void;
      mountToggle: (container?: ParentNode | null) => HTMLButtonElement;
    };
  }
}

let dagInited = false;

export function useExplainerRouting() {
  const navigate = useNavigate();
  useEffect(() => {
    if (dagInited || !window.EffortlessExplainer) return;
    dagInited = true;
    window.EffortlessExplainer.init({
      mode: 'callback',
      enhance: false,
      routing: {
        navigate: (t: string, f: string) => navigate(`/dag/${encodeURIComponent(t)}/${encodeURIComponent(f)}`),
        navigateTable: (t: string) => navigate(`/dag/${encodeURIComponent(t)}`),
        fieldHref: (t: string, f: string) => `/dag/${encodeURIComponent(t)}/${encodeURIComponent(f)}`,
        tableHref: (t: string) => `/dag/${encodeURIComponent(t)}`,
        onBack: () => navigate(-1),
        onHome: () => navigate('/overview'),
      },
    });
  }, [navigate]);
}

/** Mount the provenance ƒ toggle once the host div is in the DOM (Shell or DAG layout). */
export function DagToggle() {
  const hostRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    let interval: number | undefined;

    const mount = () => {
      const host = hostRef.current;
      if (!host || !window.EffortlessExplainer?.mountToggle) return false;
      host.replaceChildren();
      window.EffortlessExplainer.mountToggle(host);
      return true;
    };

    if (!mount()) {
      interval = window.setInterval(() => {
        if (mount()) window.clearInterval(interval);
      }, 50);
    }

    return () => {
      if (interval) window.clearInterval(interval);
      hostRef.current?.replaceChildren();
    };
  }, []);

  return <div ref={hostRef} className="dag-toggle-host" />;
}

export function DagShell() {
  return (
    <div className="dag-shell">
      <header className="dag-app-topbar">
        <Link to="/overview" className="dag-back-link">
          ← Back to dashboard
        </Link>
        <DagToggle />
      </header>
      <Outlet />
    </div>
  );
}

function useDagRenderer(render: (el: HTMLElement) => void, deps: unknown[]) {
  const ref = useRef<HTMLDivElement>(null);
  useEffect(() => {
    if (ref.current) render(ref.current);
  }, deps); // eslint-disable-line react-hooks/exhaustive-deps
  return ref;
}

export function DagIndexPage() {
  const ref = useDagRenderer((el) => window.EffortlessExplainer.renderTablesIndex(el), []);
  return <div ref={ref} className="dag-page" />;
}

export function DagTablePage() {
  const { table = '' } = useParams();
  const t = decodeURIComponent(table);
  const ref = useDagRenderer((el) => window.EffortlessExplainer.renderTablePage(el, t), [t]);
  return <div ref={ref} className="dag-page" />;
}

export function DagFieldPage() {
  const { table = '', field = '' } = useParams();
  const t = decodeURIComponent(table);
  const f = decodeURIComponent(field);
  const ref = useDagRenderer((el) => window.EffortlessExplainer.renderFieldPage(el, t, f), [t, f]);
  return <div ref={ref} className="dag-page" />;
}

export function ExplainerEnhance({ root }: { root?: HTMLElement | null }) {
  useLayoutEffect(() => {
    window.EffortlessExplainer?.enhanceCells(root ?? document);
  });
  return null;
}

/** Re-scan the whole document when the route changes (view swap). */
export function RouteDagScan({ pathname }: { pathname: string }) {
  useLayoutEffect(() => {
    window.EffortlessExplainer?.enhanceCells(document);
  }, [pathname]);
  return null;
}

import { useEffect, useLayoutEffect, useRef } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

declare global {
  interface Window {
    EffortlessExplainer: {
      init: (opts: Record<string, unknown>) => { enhanceCells: (r?: ParentNode) => void };
      renderFieldPage: (el: HTMLElement, table: string, field: string) => void;
      renderTablePage: (el: HTMLElement, table: string) => void;
      renderTablesIndex: (el: HTMLElement) => void;
      enhanceCells: (root?: ParentNode) => void;
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
      mountToggle: '#effortless-dag-toggle',
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

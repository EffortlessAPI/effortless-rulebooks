import { useEffect, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../api';
import './DownloadMenu.css';

async function downloadUrl(url: string, fallbackName: string) {
  const res = await fetch(url);
  if (!res.ok) {
    const err = await res.json().catch(() => ({})) as { error?: string };
    throw new Error(err.error ?? `Download failed (${res.status})`);
  }
  const cd = res.headers.get('content-disposition') ?? '';
  const match = /filename="?([^"]+)"?/.exec(cd);
  const blob = await res.blob();
  const anchor = document.createElement('a');
  const objUrl = URL.createObjectURL(blob);
  anchor.href = objUrl;
  anchor.download = match?.[1] ?? fallbackName;
  anchor.click();
  URL.revokeObjectURL(objUrl);
}

export function DownloadMenu() {
  const [open, setOpen] = useState(false);
  const [busy, setBusy] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const rootRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) return;
    function onDocClick(e: MouseEvent) {
      if (rootRef.current && !rootRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    }
    function onKey(e: KeyboardEvent) {
      if (e.key === 'Escape') setOpen(false);
    }
    document.addEventListener('mousedown', onDocClick);
    document.addEventListener('keydown', onKey);
    return () => {
      document.removeEventListener('mousedown', onDocClick);
      document.removeEventListener('keydown', onKey);
    };
  }, [open]);

  async function run(label: string, fn: () => Promise<void>) {
    setBusy(label);
    setError(null);
    try {
      await fn();
      setOpen(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : String(err));
    } finally {
      setBusy(null);
    }
  }

  return (
    <div className="download-menu" ref={rootRef}>
      <button
        type="button"
        className="download-menu-trigger"
        aria-haspopup="menu"
        aria-expanded={open}
        onClick={() => setOpen((v) => !v)}
        title="Downloads & exports"
      >
        ⬇
      </button>
      {open && (
        <div className="download-menu-pop" role="menu">
          <Link to="/dag" className="download-menu-item" role="menuitem" onClick={() => setOpen(false)}>
            ƒ Rulebook DAG
            <span className="download-menu-hint">Browse every table and derived field</span>
          </Link>
          <a
            className="download-menu-item"
            role="menuitem"
            href="/rulespeak/rulespeak.html"
            target="_blank"
            rel="noopener noreferrer"
            onClick={() => setOpen(false)}
          >
            RuleSpeak (HTML)
            <span className="download-menu-hint">Plain-English business rules from the rulebook</span>
          </a>
          <button
            type="button"
            className="download-menu-item"
            role="menuitem"
            disabled={busy !== null}
            onClick={() => run('rulespeak-pdf', () => downloadUrl('/rulespeak/rulespeak.pdf', 'simpsons-paradox-rulespeak.pdf'))}
          >
            {busy === 'rulespeak-pdf' ? 'Building RuleSpeak PDF…' : 'RuleSpeak (PDF)'}
            <span className="download-menu-hint">Print-ready RuleSpeak document</span>
          </button>
          <button
            type="button"
            className="download-menu-item"
            role="menuitem"
            disabled={busy !== null}
            onClick={() => run('xlsx', () => api.downloadXlsx())}
          >
            {busy === 'xlsx' ? 'Exporting Excel…' : 'Export to Excel'}
            <span className="download-menu-hint">Live vw_* data — one sheet per entity</span>
          </button>
          <button
            type="button"
            className="download-menu-item"
            role="menuitem"
            disabled={busy !== null}
            onClick={() => run('summary-pdf', () => api.downloadSummaryPdf())}
          >
            {busy === 'summary-pdf' ? 'Building summary PDF…' : 'Corpus summary (PDF)'}
            <span className="download-menu-hint">Conclusions, findings, and invariant health</span>
          </button>
          <a
            className="download-menu-item"
            role="menuitem"
            href="/simpsons-paradox-explorer.html"
            target="_blank"
            rel="noopener noreferrer"
            onClick={() => setOpen(false)}
          >
            Email-ready HTML
            <span className="download-menu-hint">Standalone explorer snapshot</span>
          </a>
        </div>
      )}
      {error && <span className="download-menu-error" title={error}>!</span>}
    </div>
  );
}

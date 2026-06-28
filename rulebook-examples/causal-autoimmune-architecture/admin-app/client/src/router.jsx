// router.jsx — a tiny, dependency-free URL layer over the History API.
//
// THE PRINCIPLE: the URL is the single source of truth for what's on screen.
// Nothing in this app is "selected" in React state that isn't also in the URL.
// That buys the three hard requirements:
//   1) every thing (and every list of things) has a stable, unique URL,
//   2) F5 restores the exact same view — down to the selected tab / open panel,
//   3) any piece of the puzzle is reachable by link alone (no click-click-click).
//
// Path  = WHICH thing, in its parent context   (/diagnosis/case/pred-a/mechanism)
// Query = which TAB / which leaf POPOVER         (?tab=summary, ?panel=evidence:ev-a-1)
//
// This pairs with routeMatch.js (template <-> concrete URL) and the rulebook's
// RoutingAndNavigation route templates + each entity row's RelativePath.
import React, { createContext, useContext, useCallback, useEffect, useState } from 'react';

const LocationContext = createContext(null);

// Normalize a concrete path: ensure a single leading slash, strip a trailing one
// (so /diagnosis/ and /diagnosis match the same template). Root stays '/'.
export function normalizePath(p) {
  if (!p) return '/';
  let s = p.startsWith('/') ? p : `/${p}`;
  if (s.length > 1) s = s.replace(/\/+$/, '');
  return s || '/';
}

// Provider — owns the live {path, query} and re-renders the tree on every
// pushState/replaceState/back/forward. Mount once at the app root.
export function LocationProvider({ children }) {
  const read = () => ({
    path: normalizePath(window.location.pathname),
    query: new URLSearchParams(window.location.search),
  });
  const [loc, setLoc] = useState(read);

  useEffect(() => {
    const onPop = () => setLoc(read());
    window.addEventListener('popstate', onPop);
    return () => window.removeEventListener('popstate', onPop);
  }, []);

  // navigate({ path?, query? }, { replace }) — push (or replace) a new URL and
  // re-render. `query` may be a plain object, a URLSearchParams, or a string.
  const navigate = useCallback((to, opts = {}) => {
    const cur = read();
    const path = normalizePath(to.path ?? cur.path);
    let qs = '';
    if (to.query !== undefined) {
      const sp = to.query instanceof URLSearchParams ? to.query
        : typeof to.query === 'string' ? new URLSearchParams(to.query)
          : new URLSearchParams(Object.entries(to.query).filter(([, v]) => v != null && v !== ''));
      qs = sp.toString();
    } else {
      qs = cur.query.toString();
    }
    const url = path + (qs ? `?${qs}` : '');
    window.history[opts.replace ? 'replaceState' : 'pushState']({}, '', url);
    setLoc(read());
  }, []);

  return (
    <LocationContext.Provider value={{ ...loc, navigate }}>
      {children}
    </LocationContext.Provider>
  );
}

// useLocation() — { path, query (URLSearchParams), navigate }.
export function useLocation() {
  const ctx = useContext(LocationContext);
  if (!ctx) throw new Error('useLocation must be used within <LocationProvider>');
  return ctx;
}

// useQueryParam('tab', 'chain') — read one query param (with a default) and a
// setter that updates ONLY that param (replaceState, so tab/panel changes don't
// pollute back-history). Setting it to null/'' removes the param.
export function useQueryParam(key, fallback = null) {
  const { query, navigate } = useLocation();
  const value = query.has(key) ? query.get(key) : fallback;
  const setValue = useCallback((v) => {
    const next = new URLSearchParams(query);
    if (v == null || v === '' || v === fallback) next.delete(key);
    else next.set(key, v);
    navigate({ query: next }, { replace: true });
  }, [query, navigate, key, fallback]);
  return [value, setValue];
}

// <Link to="/diagnosis/case/pred-a" query={{tab:'summary'}}> — a REAL anchor
// (so copy-link, middle-click, ⌘-click all behave) that intercepts plain
// left-click to pushState instead of a full reload. `to` is a path; `query`
// optionally sets the search string for the destination.
export function Link({ to, query, replace, onClick, children, style, ...rest }) {
  const { navigate } = useLocation();
  let href = normalizePath(to);
  if (query !== undefined) {
    const sp = query instanceof URLSearchParams ? query
      : typeof query === 'string' ? new URLSearchParams(query)
        : new URLSearchParams(Object.entries(query).filter(([, v]) => v != null && v !== ''));
    const qs = sp.toString();
    if (qs) href += `?${qs}`;
  }
  const handle = (e) => {
    onClick?.(e);
    // let modified clicks / non-left buttons fall through to the browser
    if (e.defaultPrevented || e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
    e.preventDefault();
    navigate({ path: normalizePath(to), query }, { replace });
  };
  return (
    <a href={href} onClick={handle} style={{ color: 'inherit', textDecoration: 'none', ...style }} {...rest}>
      {children}
    </a>
  );
}

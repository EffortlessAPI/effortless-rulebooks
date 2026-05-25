import { createContext, useCallback, useContext, useMemo, useState } from 'react';
import ExplainOverlay from './ExplainOverlay.jsx';

const Ctx = createContext(null);

export function useExplain() {
  const c = useContext(Ctx);
  if (!c) throw new Error('useExplain outside provider');
  return c;
}

// Stack-based navigation: clicking a ref inside the modal pushes a new
// frame so the user can drill into upstream calculations.
export function ExplainProvider({ children, resolveRef }) {
  const [stack, setStack] = useState([]);

  const open = useCallback((frame) => setStack([frame]), []);
  const close = useCallback(() => setStack([]), []);

  const push = useCallback(
    async (ref) => {
      const resolved = await resolveRef(ref);
      if (resolved) setStack((s) => [...s, resolved]);
    },
    [resolveRef]
  );

  const pop = useCallback(() => setStack((s) => s.slice(0, -1)), []);

  const value = useMemo(() => ({ open, close, push, pop, stack }), [open, close, push, pop, stack]);

  return (
    <Ctx.Provider value={value}>
      {children}
      {stack.length > 0 && <ExplainOverlay />}
    </Ctx.Provider>
  );
}

import { useState, useEffect, useCallback } from "react";
import { api } from "../lib/api.js";
import { toast } from "../lib/toast.js";

export function usePortal() {
  const [me, setMe]                           = useState(null);
  const [projectRulebook, setProjectRulebook] = useState(null);
  const [rulebook, setRulebook]               = useState(null);
  const [projects, setProjects]               = useState({ active: null, projects: [] });
  const [domainState, setDomainState]         = useState({ states: [], currentRevisions: {} });

  const reload = useCallback(async () => {
    try {
      const [m, prb, rb, pj, ds] = await Promise.all([
        api.get("/api/me"),
        api.get("/api/project-rulebook"),
        api.get("/api/rulebook"),
        api.get("/api/projects"),
        api.get("/api/portal/me/domain-state").catch(() => ({ states: [], currentRevisions: {} })),
      ]);
      setMe(m);
      setProjectRulebook(prb);
      setRulebook(rb);
      setProjects(pj);
      setDomainState(ds);
    } catch (e) {
      toast("Failed to load portal: " + e.message, "error");
    }
  }, []);

  // Lightweight refresh of just the domain-state slice. Used after a navigate
  // call records "I was here" so the picker chips update without re-fetching
  // the rulebook.
  const reloadDomainState = useCallback(async () => {
    try {
      const ds = await api.get("/api/portal/me/domain-state");
      setDomainState(ds);
    } catch { /* non-fatal */ }
  }, []);

  useEffect(() => { reload(); }, [reload]);

  return { me, projectRulebook, rulebook, projects, domainState, reload, reloadDomainState };
}

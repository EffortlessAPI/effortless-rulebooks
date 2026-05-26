import { useState, useEffect, useCallback } from "react";
import { api } from "../lib/api.js";
import { toast } from "../lib/toast.js";

export function usePortal() {
  const [me, setMe]                           = useState(null);
  const [projectRulebook, setProjectRulebook] = useState(null);
  const [rulebook, setRulebook]               = useState(null);
  const [projects, setProjects]               = useState({ active: null, projects: [] });

  const reload = useCallback(async () => {
    try {
      const [m, prb, rb, pj] = await Promise.all([
        api.get("/api/me"),
        api.get("/api/project-rulebook"),
        api.get("/api/rulebook"),
        api.get("/api/projects"),
      ]);
      setMe(m);
      setProjectRulebook(prb);
      setRulebook(rb);
      setProjects(pj);
    } catch (e) {
      toast("Failed to load portal: " + e.message, "error");
    }
  }, []);

  useEffect(() => { reload(); }, [reload]);

  return { me, projectRulebook, rulebook, projects, reload };
}

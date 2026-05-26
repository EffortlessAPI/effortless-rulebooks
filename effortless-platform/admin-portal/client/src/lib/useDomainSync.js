import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { api } from "./api.js";
import { toast } from "./toast.js";

// Keeps the server's active project in sync with the :domain URL param.
// Without this, navigating to /developer/star-trek would render screens
// that still fetch from the previously-active domain — every "Pick Domain"
// click would silently show the wrong data.
export function useDomainSync(reload, projects) {
  const { domain } = useParams();
  const active     = projects?.active;
  const known      = projects?.projects || [];

  useEffect(() => {
    if (!domain) return;                    // no :domain in URL — nothing to do
    if (domain === active) return;          // already matches
    if (!known.some((p) => p.id === domain)) return;  // unknown domain — let route show fallback

    let cancelled = false;
    (async () => {
      try {
        await api.post(`/api/projects/${encodeURIComponent(domain)}/activate`);
        if (!cancelled) {
          await reload();
        }
      } catch (e) {
        if (!cancelled) toast("Couldn't switch to " + domain + ": " + e.message, "error");
      }
    })();
    return () => { cancelled = true; };
  }, [domain, active]);
}

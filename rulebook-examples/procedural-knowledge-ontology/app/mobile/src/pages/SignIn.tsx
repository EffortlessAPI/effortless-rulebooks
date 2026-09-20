import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import * as api from "../api";
import { useSession } from "../session";
import { Err, human, initials } from "../ui/kit";

const TONE: Record<string, string> = { "acme-plant": "#0f766e", "acme-corp": "#1d4ed8", "acme-home-brands": "#0369a1", "acme-engineering": "#4338ca", "acme-finance": "#1d4ed8", "acme-people": "#b45309", "acme-legal": "#6d28d9" };
// The order the story meets them in; everyone else follows alphabetically.
const ORDER = ["acme-plant", "acme-corp", "acme-home-brands", "acme-engineering", "acme-finance", "acme-people", "acme-legal"];

export default function SignIn() {
  const { signIn } = useSession();
  const nav = useNavigate();
  const [people, setPeople] = useState<api.SignIn[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  useEffect(() => { api.signIns().then(setPeople).catch((e) => setError(e.message)); }, []);
  const orgs = [...new Set((people || []).map((p) => p.organization))].sort((a, b) => (ORDER.indexOf(a) + 99) % 99 - (ORDER.indexOf(b) + 99) % 99);

  return (
    <div className="content signin">
      <div className="brand">
        <h1>ACME Procedure Register</h1>
        <p>One card for each person at ACME. Each sign-in is its own database role, with its own schema: what you see, and what you may change, depends on who you are.</p>
      </div>
      <Err error={error} />
      {orgs.map((org) => (
        <div key={org}>
          <div className="section">{human(org).replace("Acme", "ACME")}</div>
          <div className="people">
            {people!.filter((p) => p.organization === org).map((p) => (
              <button key={p.appUserId + p.principalId} className="person"
                onClick={() => signIn(p.appUserId, p.principalId).then(() => nav("/")).catch((e) => setError(e.message))}>
                <span className="av" style={{ background: p.agentKind === "Human" ? TONE[org] || "#334155" : "#7c3aed" }}>{p.agentKind === "Human" ? initials(p.displayName) : "AI"}</span>
                <span className="grow"><span className="nm">{p.displayName}</span><span className="rl" style={{ display: "block" }}>{p.principalLabel}{p.isAdministrator ? " · administrator" : ""}</span></span>
              </button>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}

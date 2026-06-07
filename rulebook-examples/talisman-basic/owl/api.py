"""
OWL-powered API for the Talisman BASIC workflow ontology.

Every endpoint answers a competency question by querying the REASONED graph
(TBox + ABox + OWL-RL closure) built in graph.py. The transitive-closure and
inverse answers are produced by the reasoner, not by this code — the API just
SELECTs them. This is the OWL analogue of "read from vw_<entity>": the reasoner
already ran; we open the door and walk in.

Run:  uvicorn api:app --port 8077      (or ./run.sh api)
"""
from __future__ import annotations

from fastapi import FastAPI, HTTPException

import graph

app = FastAPI(
    title="Talisman BASIC — OWL Workflow API",
    description="Competency questions answered by an OWL-RL reasoner over the "
                "rulebook-generated ontology.",
    version="1.0.0",
)


@app.get("/health")
def health():
    g = graph.reasoned_graph()
    return {"status": "ok", "reasoned_triples": len(g)}


@app.get("/steps/{step_id}/precedes")
def steps_precedes(step_id: str):
    """Competency Q: which steps does this step precede (transitively)?

    Returns the reasoner-derived closure. e.g. prod-deploy-step-1 returns
    steps 2,3,4,5 — including pairs never asserted as direct edges.
    """
    reachable = graph.steps_after(step_id)
    if not reachable and step_id not in _all_step_ids():
        raise HTTPException(404, f"Unknown step: {step_id}")
    return {"step": step_id, "precedes": reachable}


@app.get("/precedence/closure")
def precedence_closure():
    """The full step-precedence closure (mirrors vw_step_precedence_closure)."""
    rows = graph.step_precedence_closure()
    return {
        "count": len(rows),
        "inferred": sum(1 for r in rows if r["is_inferred"]),
        "asserted": sum(1 for r in rows if not r["is_inferred"]),
        "pairs": rows,
    }


@app.get("/roles/{role_id}/delegates-to")
def role_delegates(role_id: str):
    """Competency Q: full delegation chain for a role (transitive)."""
    chain = graph.delegation_closure(role_id)
    return {"role": role_id, "delegates_to": chain}


@app.get("/roles/{role_id}/filled-by")
def role_filled_by(role_id: str):
    """Competency Q: which agent (and type) fills this role?"""
    return {"role": role_id, **graph.role_filled_by(role_id)}


@app.get("/ontology/disjoint-classes")
def disjoint_classes():
    """The mutually-disjoint agent classes (Human / AI / Pipeline)."""
    return {"disjoint_pairs": graph.disjoint_classes()}


def _all_step_ids():
    rows = graph._q(
        "SELECT DISTINCT ?s WHERE { ?s a erb:WorkflowSteps }"
    )
    return {graph._local(r[0]) for r in rows}

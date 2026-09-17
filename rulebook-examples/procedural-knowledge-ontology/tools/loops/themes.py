"""Which build-out theme owns each article claim.

A theme is one witness loop. Its author writes tools/loops/loopNN_<theme>.py (the spec
applied by tools/apply_loop_spec.py) and tools/loops/evidence_<theme>.py (the ClaimEvidence
rows). Ownership is decided here once, so two authors never model the same claim.

  A  loop-06  procedure structure, the full PKO class model, lockout/tagout and deployment scenarios
  B  loop-07  collecting knowledge: elicitation methods, tacit knowledge, social dimension, apprenticeship
  C  loop-08  organizing: vocabularies, taxonomy, context and applicability, granularity, lenses, strategy
  D  loop-09  encoding and use: retrieval, AI grounding, projections, annotation, usage and outcome feedback
  E  loop-10  governance: ownership, versioning, change control, competency-question review, ontology lifecycle
  F  loop-11  sourcing and operations: outsourcing, knowledge audit, AI system registry, ground-truth routing
  G  loop-12  the social side of collection: brokers, communities of practice, apprenticeship, culture
  H  loop-13  provenance and traceability: sources, documents, mining, conflicting accounts, failures to redesign

  I  loop-14  gaps: claims earlier themes could not witness, closed afterwards

Theme B was split into B, G and H after its author twice exceeded a single response's output cap.

Run `python3 tools/loops/themes.py` to write tools/loops/claims_<theme>.txt.
"""
from __future__ import annotations

import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE.parent))
import article_claims  # noqa: E402

# (article, section) -> theme; a claim id in OVERRIDES wins.
SECTION_THEME = {
    ("pkm-1", "The Nature of Process Knowledge"): "C",
    ("pkm-1", "Process Knowledge Frameworks"): "D",
    ("pkm-1", "Semantic Ecosystems and Process"): "A",
    ("pkm-1", "Foundation for AI Systems and Workfows"): "D",
    ("pkm-1", "Its Not Easy, But it's Worth It"): "B",
    ("pkm-1", "Integrated Process Intelligence"): "E",
    ("pkm-1", "Conclusion"): "D",

    ("pkm-2", "Introduction"): "B",
    ("pkm-2", "The Challenge of Elicitation"): "B",
    ("pkm-2", "Elicitation Methodologies: Knowledge Collection"): "B",
    ("pkm-2", "The Anthropologist"): "B",
    ("pkm-2", "Process Mining"): "B",
    ("pkm-2", "Collaborative Workshops"): "B",
    ("pkm-2", "Document Analysis"): "B",
    ("pkm-2", "Capturing Tacit Knowledge"): "B",
    ("pkm-2", "The Social Dimension of Collection Strategies"): "B",
    ("pkm-2", "From Collection to Structure"): "C",
    ("pkm-2", "Levels of Organization"): "A",
    ("pkm-2", "Organizing Principles"): "C",
    ("pkm-2", "Taxonomies and Controlled Vocabularies"): "C",
    ("pkm-2", "From Organization to Representation"): "D",
    ("pkm-2", "Encoding for AI Systems"): "D",
    ("pkm-2", "Yeah—That Context Window Won’t Cut It"): "D",
    ("pkm-2", "Encoding for Communication and Collaboration"): "D",
    ("pkm-2", "Standards and Interoperability"): "D",
    ("pkm-2", "Integration: Making Process Knowledge Work"): "E",
    ("pkm-2", "Feedback Loops"): "D",
    ("pkm-2", "Conclusion"): "D",

    ("pkm-3", "Introduction"): "B",
    ("pkm-3", "The Great Unbundling"): "F",
    ("pkm-3", "Shenzhen and Process Knowledge"): "B",
    ("pkm-3", "From Engineering State to Lawyerly Society"): "F",
    ("pkm-3", "Death of Apprenticeship and Institutional Memory"): "B",
    ("pkm-3", "Why We Stopped Documenting"): "F",
    ("pkm-3", "Socio-technical Ethos"): "B",
    ("pkm-3", "Process Knowledge is Foundational Infrastructure"): "D",
    ("pkm-3", "Rebuilding Process Knowledge Infrastructure"): "F",
    ("pkm-3", "Conclusion"): "B",

    ("pkm-4", "Introduction"): "D",
    ("pkm-4", "The Problem PKO Addresses"): "B",
    ("pkm-4", "Why Do I Care About Industrial Processes?"): "D",
    ("pkm-4", "PKO's Architecture and Design"): "A",
    ("pkm-4", "PKO Ontology Specifics"): "A",
    ("pkm-4", "LOTO: Safety Procedures at Beko Europe"): "A",
    ("pkm-4", "Before: Tacit Knowledge and Limitations"): "A",
    ("pkm-4", "After: Explicit, Actionable Procedural Knowledge"): "A",
    ("pkm-4", "Practical Elicitation and Exploitation"): "A",
    ("pkm-4", "Measured Impact: What the Evidence Shows"): "E",
    ("pkm-4", "Implications for AI Workflows"): "D",
    ("pkm-4", "Building A Procedural Knowledge Infrastructure"): "E",
    ("pkm-4", "Procedural Knowlege Is Not Boring Afterall"): "E",
    ("pkm-4", "PKO class diagram"): "A",
    ("pkm-4", "PKO concept map"): "A",

    ("ont-4", "Governance, Maintenance, and AI"): "E",
    ("ont-4", "Why Ontologies Decay"): "E",
    ("ont-4", "Ownership"): "E",
    ("ont-4", "Versioning"): "E",
    ("ont-4", "Change Management"): "E",
    ("ont-4", "Competency Questions as a Governance Instrument"): "E",
    ("ont-4", "The Relationship Has Two Directions"): "D",
    ("ont-4", "How AI Systems Consume an Ontology"): "D",
    ("ont-4", "AI Contributes to Ontology Development"): "E",
    ("ont-4", "The NTWF Graph as an AI System Registry"): "F",
    ("ont-4", "Ontology Is the Ground Truth"): "F",
    ("ont-4", "RAG Over Structured Graphs"): "D",
    ("ont-4", "AI-Assisted Ontology Maintenance"): "E",
    ("ont-4", "Ontology as Organizational Memory"): "D",
    ("ont-4", "Conclusion"): "E",
}

OVERRIDES = {
    # pkm-1: structural classes live in A, tacit and collection in B, levels and lenses in C
    **{f"pkm1-c{n:02d}": "A" for n in (1, 2, 3, 31)},
    **{f"pkm1-c{n:02d}": "B" for n in (4, 5, 6, 16, 28, 30)},
    "pkm1-c15": "B", "pkm1-s07": "B", "pkm1-s08": "B", "pkm1-q08": "B", "pkm1-p01": "B",
    "pkm1-p04": "B", "pkm1-p05": "B", "pkm1-p08": "B", "pkm1-c33": "E", "pkm1-c34": "C",
    "pkm1-p28": "C", "pkm1-s06": "C", "pkm1-p27": "E", "pkm1-p30": "E", "pkm1-q14": "E",
    "pkm1-p06": "C", "pkm1-p14": "C", "pkm1-p15": "C", "pkm1-p40": "C", "pkm1-p09": "E",
    "pkm1-q01": "A", "pkm1-q02": "A", "pkm1-q03": "A", "pkm1-q10": "A", "pkm1-i02": "A",
    "pkm1-q04": "A", "pkm1-p16": "A",
    **{f"pkm1-p{n:02d}": "B" for n in (36, 37, 38, 39)},
    "pkm1-c35": "B", "pkm1-c36": "B", "pkm1-p31": "D", "pkm1-s02": "D", "pkm1-i07": "D",
    "pkm1-c37": "D", "pkm1-p13": "D", "pkm1-p17": "D", "pkm1-p18": "D", "pkm1-p19": "D",
    "pkm1-s01": "D", "pkm1-s03": "D", "pkm1-s04": "D", "pkm1-c29": "A",
    # pkm-2: contextual level and instances categorization in C, conditions/failure/decisions in A
    **{f"pkm2-c{n:02d}": "C" for n in range(62, 69)},
    "pkm2-p21": "C", "pkm2-q21": "C", "pkm2-c59": "C", "pkm2-s23": "C",
    **{f"pkm2-c{n:02d}": "A" for n in range(88, 93)},
    **{f"pkm2-q{n:02d}": "A" for n in range(22, 28)},
    "pkm2-s12": "A",
    "pkm2-c103": "B", "pkm2-c104": "B", "pkm2-q38": "B", "pkm2-q39": "B",
    "pkm2-q12": "B", "pkm2-q13": "B", "pkm2-q14": "B", "pkm2-p29": "B", "pkm2-p30": "B",
    "pkm2-p31": "B", "pkm2-p32": "B", "pkm2-p33": "B", "pkm2-i11": "B",
    "pkm2-p56": "B", "pkm2-p76": "B", "pkm2-p78": "B", "pkm2-c102": "B",
    "pkm2-q32": "D", "pkm2-q33": "D", "pkm2-p57": "D", "pkm2-p58": "D", "pkm2-p59": "D",
    "pkm2-c101": "E", "pkm2-p79": "E", "pkm2-c22": "B",
    "pkm2-i14": "C", "pkm2-s09": "C", "pkm2-s22": "C",
    # pkm-3
    "pkm3-c03": "A", "pkm3-q01": "A", "pkm3-p26": "A", "pkm3-c16": "A",
    "pkm3-c15": "B", "pkm3-p04": "B", "pkm3-q11": "B", "pkm3-c01": "B", "pkm3-c02": "B",
    "pkm3-p02": "B", "pkm3-c22": "C", "pkm3-q04": "C", "pkm3-s02": "C",
    "pkm3-c28": "F", "pkm3-c29": "F",
    **{f"pkm3-c{n:02d}": "D" for n in (39, 40, 41)},
    **{f"pkm3-p{n:02d}": "D" for n in (23, 27, 28, 29, 30)}, "pkm3-s01": "D",
    **{f"pkm3-p{n:02d}": "B" for n in (17, 18, 19)},
    # pkm-4
    "pkm4-c13": "B", "pkm4-p04": "B", "pkm4-p05": "B", "pkm4-p08": "B", "pkm4-q13": "B",
    "pkm4-q14": "B", "pkm4-p12": "B", "pkm4-p06": "D", "pkm4-p07": "D", "pkm4-p30": "E",
    "pkm4-i01": "A", "pkm4-c09": "D", "pkm4-p15": "D", "pkm4-p16": "D", "pkm4-s12": "D",
    "pkm4-p34": "D", "pkm4-p35": "D", "pkm4-p37": "D", "pkm4-p11": "E", "pkm4-p10": "A",
    "pkm4-s17": "E", "pkm4-s18": "E", "pkm4-s19": "E", "pkm4-i02": "D", "pkm4-i05": "D", "pkm4-i08": "C", "pkm4-c12": "D",
    "pkm4-c88": "E",
    # ont-4: vocabulary and external-dependency drift in C, the step-ordering chain in A
    **{f"ont4-c{n:02d}": "C" for n in (4, 8, 9, 10, 11, 12, 15, 32, 33, 34, 35)},
    **{f"ont4-p{n:02d}": "C" for n in (1, 3, 4, 5, 6, 7, 59, 62)},
    "ont4-q26": "C", "ont4-s03": "C", "ont4-s08": "C", "ont4-s09": "C", "ont4-s10": "C",
    "ont4-s12": "C", "ont4-s14": "C", "ont4-i09": "C", "ont4-i12": "C", "ont4-i13": "C",
    "ont4-i16": "C", "ont4-i17": "C",
    "ont4-c36": "A", "ont4-c45": "A", "ont4-c27": "A", "ont4-q28": "A", "ont4-q07": "A",
    "ont4-q08": "A",
    "ont4-p92": "E", "ont4-s11": "C",
    **{f"ont4-x{n:02d}": "A" for n in range(1, 6)},
    "pkm3-c26": "E", "pkm3-q05": "E", "pkm3-p24": "E", "pkm3-p25": "E", "pkm3-p31": "E",
}

_G = ("pkm3-c10 pkm3-c11 pkm3-c12 pkm3-c13 pkm3-c14 pkm3-q09 pkm3-i05 pkm3-i08 pkm3-i09 "
      "pkm2-c32 pkm2-c33 pkm2-c34 pkm2-c35 pkm2-c36 pkm2-c37 pkm2-c38 pkm2-c39 pkm2-p15 pkm2-p16 pkm2-p17 pkm2-p18 "
      "pkm2-q06 pkm2-q07 pkm2-q08 pkm2-s06 pkm3-c19 pkm3-c20 pkm3-c21 pkm3-i04 pkm3-i07 "
      "pkm3-c27 pkm3-c30 pkm3-c31 pkm3-p05 pkm3-p06 pkm3-p07 pkm3-q06 pkm3-q10 pkm3-i06 pkm3-p17 pkm3-p18 pkm3-p19 "
      "pkm1-c35 pkm1-c36 pkm1-p36 pkm1-p37 pkm1-p38 pkm1-p39 pkm2-p78 pkm3-c42 pkm3-p32 pkm3-c04 pkm3-c05 pkm3-p01 "
      "pkm4-p04 pkm4-p05 pkm4-q14 pkm4-p08 pkm1-c16 pkm1-p08").split()
_H = ("pkm2-p29 pkm2-p30 pkm2-p31 pkm2-p32 pkm2-p33 pkm2-q12 pkm2-q13 pkm2-q14 pkm2-i11 "
      "pkm2-c103 pkm2-c104 pkm2-q38 pkm2-q39 pkm1-c30 pkm1-q08 "
      "pkm2-c19 pkm2-c20 pkm2-c21 pkm2-c22 pkm2-q15 pkm2-q16 pkm2-s16 "
      "pkm2-c24 pkm2-c25 pkm2-c26 pkm2-p13 pkm2-p14 pkm2-s14 pkm3-c15 pkm3-p04 pkm3-q11 "
      "pkm2-c102 pkm2-p56 pkm2-p76 pkm1-c32 pkm1-p25 pkm1-p26 pkm1-p29 pkm1-q09 pkm1-q13 pkm1-s05 pkm4-q13").split()
OVERRIDES.update({cid: "G" for cid in _G})
OVERRIDES.update({cid: "H" for cid in _H})

OVERRIDES.update({cid: "I" for cid in ("pkm4-p23", "pkm4-p43", "pkm4-p45", "pkm4-p25", "ont4-i31", "pkm4-i06", "ont4-i27")})

LOOP_OF = {"A": ("loop-06", "foundation"), "B": ("loop-07", "collection"), "C": ("loop-08", "organizing"),
           "D": ("loop-09", "encoding"), "E": ("loop-10", "governance"), "F": ("loop-11", "sourcing"),
           "G": ("loop-12", "social"), "H": ("loop-13", "provenance"), "I": ("loop-14", "gaps")}


def theme_of(claim) -> str:
    cid, art, _kind, section, _text = claim
    if cid in OVERRIDES:
        return OVERRIDES[cid]
    try:
        return SECTION_THEME[(art, section)]
    except KeyError:
        raise SystemExit(f"{cid}: no theme for section {section!r} of {art}")


def main() -> int:
    by: dict[str, list] = {t: [] for t in LOOP_OF}
    for c in article_claims.CLAIMS:
        by[theme_of(c)].append(c)
    for t, claims in by.items():
        loop, name = LOOP_OF[t]
        lines = [f"# Theme {t} ({loop}, {name}): {len(claims)} claims", ""]
        for cid, art, kind, section, text in claims:
            lines.append(f"{cid}\t{kind}\t{section}\t{text}")
        (HERE / f"claims_{name}.txt").write_text("\n".join(lines) + "\n")
        kinds = {}
        for c in claims:
            kinds[c[2]] = kinds.get(c[2], 0) + 1
        print(f"{t} {loop} {name:11s} {len(claims):4d}  {kinds}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

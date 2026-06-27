#!/usr/bin/env python3
"""
Loop: add the 3-panel witness to the rulebook.

Panel 1  = Individuals.CaseNarrative      (the LLM's input: a plausible case text)
Panel 2  = each leaf row's SourceQuote     (the pointer back into Panel 1)
Panel 3  = the existing derived DAG        (unchanged)

ALL added fields are raw (no formula) => no derived value can change =>
the witnessed harness oracle stays green by construction.

Every SourceQuote MUST be a literal substring of its patient's CaseNarrative.
This script asserts that before writing — a non-substring quote aborts the run.
"""
import json, sys

P = '/Users/eejai42/effortlessapi-app-root/users/user_ee42ai73-18a9-47d5-8f99-954b00f6c041/my-projects/jholla-problem-to-solve/effortless-rulebook/effortless-rulebook.json'
d = json.load(open(P))

# ---------------------------------------------------------------------------
# 1. CASE NARRATIVES (Panel 1) — one per patient.
#    Each is a clinic-style intake note. Crucially, every number a leaf row
#    extracts appears VERBATIM in the prose, so the quote pointer is checkable.
#    Loci/drugs/thresholds are literature-aligned (see bootstrap/clinical-reference.md).
# ---------------------------------------------------------------------------
NARRATIVES = {
"ind-a-reyes": (
"Ana Reyes, a 34-year-old woman of European ancestry (cohort is in the training set), "
"presents with photosensitive malar rash, arthralgia, and a positive ANA, raising concern for SLE onset. "
"Family history is notable for lupus in a maternal aunt. Genotyping finds the IRF5 regulatory variant "
"rs2004640 at an allele frequency of 0.006 (rare), and allele-specific expression is present at the locus. "
"Functional workup of the IRF5 -> type-I interferon -> SLE-risk edge returns a cis-eQTL in blood RNA-seq with "
"effect 0.8 (SE 0.2), a caQTL in blood ATAC-seq with effect 0.7 (SE 0.2), and a pQTL in blood proteomics with "
"effect 0.6 (SE 0.2) — three assays across two omics modalities, all ancestry-PC and batch adjusted. "
"The edge replicates with a positive effect sign in an East Asian cohort (p=0.01) and in an African cohort (p=0.02), "
"while a European replication is directionally consistent but not significant (p=0.2). "
"Two ancestry-permutation negative controls collapse to the null (permutation effect 0.01 and 0.02, well inside the +/-0.1 null band). "
"The IL-interferon target is druggable (anifrolumab, anti-IFNAR1), giving a perturbable intervention target. "
"For SLE-onset risk, the model's reliability bins each carry coverage of 30 held-out individuals, with observed "
"event rates tracking predicted bands closely (e.g. 0.08 observed at the 0.1 band, 0.92 at the 0.9 band). "
"No cryptic-relatedness or assortative-mating leakage was flagged."
),
"ind-b-okafor": (
"Bili Okafor, a 41-year-old man of European ancestry (in the training set), presents with serositis, fatigue, and "
"a high-titre ANA suggestive of SLE onset. Genotyping finds the IRF5 regulatory variant rs2004640 at an allele "
"frequency of 0.006 (rare), with allele-specific expression present. The IRF5 -> type-I interferon -> SLE-risk edge "
"is supported by a cis-eQTL in blood RNA-seq with effect 0.8 (SE 0.2), a caQTL in blood ATAC-seq with effect 0.7 (SE 0.2), "
"and a pQTL in blood proteomics with effect 0.6 (SE 0.2), spanning two omics modalities and adjusted for ancestry PCs and batch. "
"It replicates positive in an East Asian cohort (p=0.01) and an African cohort (p=0.02), with a non-significant European "
"replication (p=0.2). Two ancestry-permutation controls collapse to the null (permutation effect 0.01 and 0.02 inside +/-0.1). "
"The interferon target is druggable. "
"The mechanism is as solid as Ana's — but the calibration substrate is thin: this site only accrued coverage of 8 held-out "
"individuals per reliability bin, far below the 20-count floor needed to trust the predicted-vs-observed match. "
"No cryptic-relatedness leakage was flagged."
),
"ind-c-chen": (
"Chen Wei, a 47-year-old woman of East Asian ancestry (in the training set), presents with arthritis, oral ulcers, and "
"anti-dsDNA positivity concerning for SLE onset. Genotyping finds the STAT4 regulatory variant rs7574865 at an allele "
"frequency of 0.008 (rare), with allele-specific expression present. The STAT4 -> Th1/IFN -> SLE-risk edge shows a cis-eQTL "
"in blood RNA-seq with effect 0.8 (SE 0.2), a caQTL in blood ATAC-seq with effect 0.7 (SE 0.2), and a pQTL in blood "
"proteomics with effect 0.6 (SE 0.2), across two modalities, ancestry-PC and batch adjusted. "
"Replication is troubling: the effect is positive in an East Asian cohort (p=0.01) but the SIGN FLIPS to negative in an "
"African cohort (p=0.02), and the European replication is non-significant (p=0.3) — only one concordant cohort. "
"Worse, while one ancestry-permutation control collapses (permutation effect 0.01), a second negative control does NOT "
"collapse: its permutation effect is 0.5, far outside the +/-0.1 null band, so the signal is not separable from permuted noise. "
"Reliability bins carry coverage of 30 per bin. No cryptic-relatedness leakage was flagged."
),
"ind-d-santos": (
"Diego Santos, a 29-year-old man of European ancestry (in the training set), presents with nephritis and a malar rash "
"concerning for SLE onset. Genotyping finds the IRF5 regulatory variant rs2004640 at an allele frequency of 0.006 (rare), "
"with allele-specific expression present. The IRF5 -> type-I interferon -> SLE-risk edge is fully supported: a cis-eQTL in "
"blood RNA-seq with effect 0.8 (SE 0.2), a caQTL in blood ATAC-seq with effect 0.7 (SE 0.2), and a pQTL in blood proteomics "
"with effect 0.6 (SE 0.2), across two modalities and adjusted for ancestry PCs and batch. It replicates positive in an East "
"Asian cohort (p=0.01) and an African cohort (p=0.02), with a non-significant European replication (p=0.2). Two ancestry-"
"permutation controls collapse to the null (permutation effect 0.01 and 0.02 inside +/-0.1). The interferon target is druggable, "
"and reliability bins carry coverage of 30 per bin. "
"However, sample QC raised a cryptic-relatedness / assortative-mating flag for this participant: kinship estimates indicate "
"undeclared relatives inside the cohort, so the prediction may be confounded by leakage rather than the mechanism alone."
),
"ind-e-mensah": (
"Esi Mensah, a 38-year-old woman of African ancestry (in the training set), presents with arthralgia and cytopenias "
"concerning for SLE onset. Genotyping finds the CTLA4 enhancer variant rs3087243 at an allele frequency of 0.007 (rare), "
"with allele-specific expression present. The CTLA4 -> T-cell costimulation -> SLE-risk edge is well supported: a cis-eQTL "
"in blood RNA-seq with effect 0.8 (SE 0.2), a caQTL in blood ATAC-seq with effect 0.7 (SE 0.2), and a pQTL in blood "
"proteomics with effect 0.6 (SE 0.2), across two modalities, ancestry-PC and batch adjusted. It replicates positive in an "
"East Asian cohort (p=0.01) and a European cohort (p=0.02), with a non-significant African replication (p=0.2). Two ancestry-"
"permutation controls collapse to the null (permutation effect 0.01 and 0.02 inside +/-0.1), and reliability bins carry "
"coverage of 30 per bin. "
"The gap is experimental: no perturbable intervention target has been mapped for this edge — there is no agent or assay that "
"could knock the costimulation node down to test it, so the mechanism is not experimentally falsifiable as stated."
),
"ind-f-haidar": (
"Faisal Haidar, a 31-year-old man of Indigenous American ancestry — an ancestry deliberately HELD OUT of the training set — "
"presents with dactylitis, enthesitis, and nail pitting concerning for psoriatic-arthritis (PsA) onset. Genotyping finds the "
"IL23R regulatory variant rs11209026 at an allele frequency of 0.009 (rare), with allele-specific expression present. The "
"IL23R -> IL-17 axis -> PsA-risk edge is well supported: a cis-eQTL in blood RNA-seq with effect 0.8 (SE 0.2), a caQTL in "
"blood ATAC-seq with effect 0.7 (SE 0.2), and a pQTL in blood proteomics with effect 0.6 (SE 0.2), across two modalities and "
"adjusted for ancestry PCs and batch. The IL-17 target is druggable (secukinumab). Two ancestry-permutation controls collapse "
"to the null (permutation effect 0.01 and 0.02 inside +/-0.1), and reliability bins carry coverage of 30 per bin. "
"But every replication of this edge was run in an Indigenous American cohort: p=0.01, p=0.02, and p=0.03, all positive, all in "
"the SAME ancestry. There is no cohort in a DIFFERENT ancestry confirming the effect — so for a patient whose ancestry is "
"absent from training, nothing demonstrates the mechanism transports. No cryptic-relatedness leakage was flagged."
),
"ind-g-lin": (
"Grace Lin, a 36-year-old woman of Indigenous American ancestry — again an ancestry HELD OUT of the training set — presents "
"with plaque psoriasis and asymmetric oligoarthritis concerning for psoriatic-arthritis (PsA) onset. Genotyping finds the "
"IL23R regulatory variant rs11209026 at an allele frequency of 0.009 (rare), with allele-specific expression present. The "
"IL23R -> IL-17 axis -> PsA-risk edge is well supported: a cis-eQTL in blood RNA-seq with effect 0.8 (SE 0.2), a caQTL in "
"blood ATAC-seq with effect 0.7 (SE 0.2), and a pQTL in blood proteomics with effect 0.6 (SE 0.2), across two modalities and "
"adjusted for ancestry PCs and batch. The IL-17 target is druggable (secukinumab). Two ancestry-permutation controls collapse "
"to the null (permutation effect 0.01 and 0.02 inside +/-0.1), and reliability bins carry coverage of 30 per bin. "
"Crucially — and unlike Faisal — this edge was MEASURED to replicate ACROSS ancestries: positive in a European cohort (p=0.01), "
"positive in an East Asian cohort (p=0.02), and positive in an Indigenous American cohort (p=0.03). Because the mechanism "
"demonstrably holds in ancestries other than the held-out one, the prediction transports. No cryptic-relatedness leakage was flagged."
),
}

# ---------------------------------------------------------------------------
# 2. PER-LEAF SOURCE QUOTES (Panel 2 -> Panel 1 pointer).
#    Keyed by row id. Each value MUST be a literal substring of the patient's
#    narrative. We assert that below before writing anything.
#    Variant rows, evidence rows, replication rows, control rows, and one
#    representative calibration quote per prediction.
# ---------------------------------------------------------------------------
QUOTES = {}

# Helper to register a quote and remember which patient's narrative it belongs to.
def q(rowid, patient, text):
    QUOTES[rowid] = (patient, text)

# --- Variants: the allele-frequency + ASE sentence fragment ---
q("var-a-irf5","ind-a-reyes","IRF5 regulatory variant rs2004640 at an allele frequency of 0.006 (rare), and allele-specific expression is present")
q("var-b-irf5","ind-b-okafor","IRF5 regulatory variant rs2004640 at an allele frequency of 0.006 (rare), with allele-specific expression present")
q("var-c-stat4","ind-c-chen","STAT4 regulatory variant rs7574865 at an allele frequency of 0.008 (rare), with allele-specific expression present")
q("var-d-irf5","ind-d-santos","IRF5 regulatory variant rs2004640 at an allele frequency of 0.006 (rare), with allele-specific expression present")
q("var-e-ctla4","ind-e-mensah","CTLA4 enhancer variant rs3087243 at an allele frequency of 0.007 (rare), with allele-specific expression present")
q("var-f-il23r","ind-f-haidar","IL23R regulatory variant rs11209026 at an allele frequency of 0.009 (rare), with allele-specific expression present")
q("var-g-il23r","ind-g-lin","IL23R regulatory variant rs11209026 at an allele frequency of 0.009 (rare), with allele-specific expression present")

# --- Evidence rows: the per-assay effect/SE fragment ---
for pid, pat in [("a","ind-a-reyes"),("b","ind-b-okafor"),("c","ind-c-chen"),("d","ind-d-santos"),("e","ind-e-mensah"),("f","ind-f-haidar"),("g","ind-g-lin")]:
    q(f"ev-{pid}-1", pat, "cis-eQTL in blood RNA-seq with effect 0.8 (SE 0.2)")
    q(f"ev-{pid}-2", pat, "caQTL in blood ATAC-seq with effect 0.7 (SE 0.2)")
    q(f"ev-{pid}-3", pat, "pQTL in blood proteomics with effect 0.6 (SE 0.2)")

# --- Replication rows: the per-cohort sign + p fragment ---
q("rep-a-1","ind-a-reyes","positive effect sign in an East Asian cohort (p=0.01)")
q("rep-a-2","ind-a-reyes","in an African cohort (p=0.02)")
q("rep-a-3","ind-a-reyes","European replication is directionally consistent but not significant (p=0.2)")
q("rep-b-1","ind-b-okafor","replicates positive in an East Asian cohort (p=0.01)")
q("rep-b-2","ind-b-okafor","an African cohort (p=0.02)")
q("rep-b-3","ind-b-okafor","non-significant European replication (p=0.2)")
q("rep-c-1","ind-c-chen","positive in an East Asian cohort (p=0.01)")
q("rep-c-2","ind-c-chen","SIGN FLIPS to negative in an African cohort (p=0.02)")
q("rep-c-3","ind-c-chen","European replication is non-significant (p=0.3)")
q("rep-d-1","ind-d-santos","replicates positive in an East Asian cohort (p=0.01)")
q("rep-d-2","ind-d-santos","an African cohort (p=0.02)")
q("rep-d-3","ind-d-santos","non-significant European replication (p=0.2)")
q("rep-e-1","ind-e-mensah","replicates positive in an East Asian cohort (p=0.01)")
q("rep-e-2","ind-e-mensah","a European cohort (p=0.02)")
q("rep-e-3","ind-e-mensah","non-significant African replication (p=0.2)")
q("rep-f-1","ind-f-haidar","run in an Indigenous American cohort: p=0.01")
q("rep-f-2","ind-f-haidar","p=0.02")
q("rep-f-3","ind-f-haidar","p=0.03, all positive, all in the SAME ancestry")
q("rep-g-1","ind-g-lin","positive in a European cohort (p=0.01)")
q("rep-g-2","ind-g-lin","positive in an East Asian cohort (p=0.02)")
q("rep-g-3","ind-g-lin","positive in an Indigenous American cohort (p=0.03)")

# --- Negative control rows: the permutation-effect fragment ---
# A uses fuller phrasing; the other passing patients share one "0.01 and 0.02 inside +/-0.1" span.
q("nct-a-1","ind-a-reyes","permutation effect 0.01 and 0.02, well inside the +/-0.1 null band")
q("nct-a-2","ind-a-reyes","permutation effect 0.01 and 0.02, well inside the +/-0.1 null band")
for pid, pat in [("b","ind-b-okafor"),("d","ind-d-santos"),("e","ind-e-mensah"),("f","ind-f-haidar"),("g","ind-g-lin")]:
    q(f"nct-{pid}-1", pat, "permutation effect 0.01 and 0.02 inside +/-0.1")
    q(f"nct-{pid}-2", pat, "permutation effect 0.01 and 0.02 inside +/-0.1")
# C is special — one control collapses, one does NOT
q("nct-c-1","ind-c-chen","one ancestry-permutation control collapses (permutation effect 0.01)")
q("nct-c-2","ind-c-chen","its permutation effect is 0.5, far outside the +/-0.1 null band")

# --- Calibration bins: one representative coverage quote per bin row ---
cov30 = "reliability bins each carry coverage of 30 held-out individuals"
for pid, pat, cov in [
    ("a","ind-a-reyes","reliability bins each carry coverage of 30 held-out individuals"),
    ("b","ind-b-okafor","coverage of 8 held-out\nindividuals per reliability bin"),
    ("c","ind-c-chen","Reliability bins carry coverage of 30 per bin"),
    ("d","ind-d-santos","reliability bins carry coverage of 30 per bin"),
    ("e","ind-e-mensah","reliability bins carry coverage of 30 per bin"),
    ("f","ind-f-haidar","reliability bins carry coverage of 30 per bin"),
    ("g","ind-g-lin","reliability bins carry coverage of 30 per bin"),
]:
    for b in range(5):
        q(f"cb-{pid}-{b}", pat, cov)

# ---------------------------------------------------------------------------
# 3. VERIFY every quote is a literal substring of its patient's narrative.
# ---------------------------------------------------------------------------
def norm(s):  # collapse internal newlines/whitespace to single spaces for the substring check
    return ' '.join(s.split())

errors = []
for rowid,(pat,text) in QUOTES.items():
    nar = NARRATIVES.get(pat)
    if nar is None:
        errors.append(f"{rowid}: unknown patient {pat}"); continue
    if norm(text) not in norm(nar):
        errors.append(f"{rowid}: quote NOT a substring of {pat} narrative:\n    quote={text!r}")
if errors:
    print("ABORT — quote/narrative mismatches:\n" + "\n".join(errors))
    sys.exit(1)
print(f"OK: all {len(QUOTES)} quotes are literal substrings of their narratives.")

# ---------------------------------------------------------------------------
# 4. ADD SCHEMA FIELDS (raw, no formula) if not present.
# ---------------------------------------------------------------------------
def add_field(table, field_def):
    sch = d[table]['schema']
    if any(f['name']==field_def['name'] for f in sch):
        return False
    # insert raw fields right after the last raw field, before calculated ones,
    # so generated DDL keeps inputs grouped. Simplest: append; transpiler orders by type anyway.
    sch.append(field_def)
    return True

CASE_NARRATIVE = {
    "name":"CaseNarrative","datatype":"string","type":"raw","nullable":True,
    "Description":"PANEL 1 of the witness (raw leaf, read by nothing downstream): the natural-language intake case the LLM was handed. Every raw observation below is extracted from THIS text; each leaf's SourceQuote points back into it so the extraction is human-checkable independently of the verdict. Synthetic but transparent — invented case, literature-aligned loci/thresholds."
}
SOURCE_QUOTE = lambda noun: {
    "name":"SourceQuote","datatype":"string","type":"raw","nullable":True,
    "Description":f"PANEL 2->1 provenance pointer (raw leaf, read by nothing downstream): the literal span of the patient's CaseNarrative from which this {noun}'s raw value was extracted. A human verifies the EXTRACTION was faithful here, separately from trusting the derived diagnosis — which is exactly what defeats 'a hallucination laundered through a deterministic function'."
}

changed = []
if add_field("Individuals", CASE_NARRATIVE): changed.append("Individuals.CaseNarrative")
for t, noun in [("GenomicVariants","variant"),("EvidenceItems","evidence assay"),
                ("CohortReplications","replication"),("NegativeControlTests","negative control"),
                ("CalibrationBins","calibration bin")]:
    if add_field(t, SOURCE_QUOTE(noun)): changed.append(f"{t}.SourceQuote")
print("Schema fields added:", changed)

# ---------------------------------------------------------------------------
# 5. WRITE DATA: narratives onto Individuals, quotes onto leaf rows.
# ---------------------------------------------------------------------------
def find_row(table, idfield, idval):
    for r in d[table]['data']:
        if r.get(idfield)==idval: return r
    return None

# narratives
for iid, nar in NARRATIVES.items():
    r = find_row("Individuals","IndividualId",iid)
    if r is None: print("WARN no individual", iid); continue
    r["CaseNarrative"] = nar

# quotes (store with newlines collapsed so the UI highlight matches the rendered narrative)
LEAF_TABLES = {
    "GenomicVariants":"GenomicVariantId",
    "EvidenceItems":"EvidenceItemId",
    "CohortReplications":"CohortReplicationId",
    "NegativeControlTests":"NegativeControlTestId",
    "CalibrationBins":"CalibrationBinId",
}
applied = 0; missing = []
for rowid,(pat,text) in QUOTES.items():
    placed = False
    for t, idf in LEAF_TABLES.items():
        r = find_row(t, idf, rowid)
        if r is not None:
            r["SourceQuote"] = norm(text)
            applied += 1; placed = True; break
    if not placed: missing.append(rowid)
print(f"Quotes applied: {applied}; missing rows: {missing}")

# ---------------------------------------------------------------------------
# 6. SAVE
# ---------------------------------------------------------------------------
with open(P,'w') as f:
    json.dump(d, f, indent=2, ensure_ascii=False)
print("WROTE", P)

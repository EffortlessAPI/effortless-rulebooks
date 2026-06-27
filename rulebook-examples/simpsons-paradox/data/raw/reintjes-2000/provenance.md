# Reintjes et al. 2000 — Provenance

**Citation:** Reintjes R, de Boer A, van Pelt W, Mintjes-de Groot J.
"Simpson's paradox: an example from hospital epidemiology."
*Epidemiology.* 2000;11(1):81–83.
PubMed PMID: 10615849
DOI: 10.1097/00001648-200001000-00017

**Source table:** Table 1 in the paper. Extracted manually 2026-06-27.
No transformation applied — numbers are the exact integers from the published table.

**Domain:** Hospital infection control. 8 Dutch hospitals, 1990s.
N = 3,519 patients total (1,279 prophylaxis + 2,240 control).

**Groups:**
- A = antibiotic prophylaxis given
- B = no antibiotic prophylaxis (control)

**Strata:** Hospital baseline UTI incidence level
- low  = hospitals with baseline UTI rate ≤ 2.5%
- high = hospitals with baseline UTI rate > 2.5%

**Outcome:** UTI acquired (success = UTI event — NOTE: "success" here means infection occurred;
higher rate = worse outcome. The DAG treats successes as positive events; in this study a
"success" is a bad clinical outcome. AllocationDistortion and DistortionType are geometrically
correct regardless — the direction of "better" is stored in context, not the geometry.)

**Raw counts (from Table 1):**

| Stratum | Prophylaxis (A) |        | Control (B) |        |
|---------|----------------|--------|-------------|--------|
|         | UTI (successes)| Total  | UTI (successes)| Total |
| low     | 20             | 1,113  | 5           | 720    |
| high    | 22             | 166    | 99          | 1,520  |
| pooled  | 42             | 1,279  | 104         | 2,240  |

**Verification:**
- low:  A = 20/1113 = 1.797%,  B = 5/720  = 0.694%  → A worse  (RR = 2.59)
- high: A = 22/166  = 13.253%, B = 99/1520 = 6.513% → A worse  (RR = 2.03)
- pool: A = 42/1279 = 3.284%,  B = 104/2240 = 4.643% → A appears BETTER (RR = 0.71)

Full reversal. Expected DistortionType = A, PolicyImplication = stratify-immediately.

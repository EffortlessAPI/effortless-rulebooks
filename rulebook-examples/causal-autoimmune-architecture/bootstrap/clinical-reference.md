# Autoimmune Causal-Inference Demo — Literature-Aligned Reference Facts

> **Demonstration of inference structure, NOT validated clinical decision support.**
> Effect sizes vary by population/study; values below are rough literature-aligned ranges used
> only to make the synthetic mock data plausible. Do not use for any clinical purpose.

| # | Item | Realistic value / range | Rationale | Source |
|---|------|------------------------|-----------|--------|
| 1a | **HLA-DRB1\*15:01 & SLE** | Risk; **OR ≈ 2.1–2.4** | DRB1\*15:01 raises lupus risk (OR 2.17, 95% CI 1.69–2.79, Japanese SLE) | medRxiv 2023.04.05.23288103 |
| 1b | **HLA-DRB1 shared epitope & RA** | Risk; **OR ≈ 4–6** for ACPA+ RA | SE alleles = strongest genetic RA risk factor, chiefly anti-CCP/ACPA-positive | PMC3446463 |
| 2 | **PTPN22 R620W (rs2476601) & RA** | **OR ≈ 1.75–2.6** het, ~4.5 hom; also SLE/T1D. **EUR risk-allele freq ~5–10%**; **rare/absent in African & East-Asian** (monomorphic in Japanese) | Strong non-HLA autoimmunity locus; classic European-specific common risk allele → key ancestry-transfer example | Nature Genes&Immunity 6364159; PMC3737240; AJHG 63322-9 |
| 3a | **IRF5 — SLE/RA** | Confirmed risk locus (type-I IFN pathway) | IRF5 (with STAT4, BLK) associated with SLE and RA | PubMed 20453440 |
| 3b | **STAT4 — SLE/RA** | Confirmed; tied to severe SLE (dsDNA Ab, nephritis, early onset); epistatic with IRF5 | Established SLE risk factor for severe disease | PMC2377340 |
| 3c | **CTLA4 — RA/autoimmunity** | Confirmed (rs231775, rs3087243) | T-cell costimulation gene; meta-analyses link to RA (Caucasian & Asian) | aging-us 203349 |
| 4 | **IL-17 / IL-23 pathway** | Central to psoriasis/PsA/spondyloarthritis; **IL-17A blockade (secukinumab) approved** (also ixekizumab, brodalumab) | IL-23/IL-17 axis drives psoriatic skin & joint inflammation | PMC6129988 |
| 5 | **"Rare variant" AF cutoff** | **MAF < 0.01** (rare); **< 0.001** stricter; **< 0.0001** highly penetrant; gnomAD MAF is the reference filter | Standard pop-gen convention | PMC6117313; Wikipedia MAF |
| 6 | **Calibration of risk models** | Predicted prob = observed rate. Assess via calibration slope/intercept, **Expected Calibration Error (ECE)** (0=perfect), **prediction-interval coverage** (90/95% intervals contain truth ~90/95%) | Lets clinicians set action thresholds; over/under-confidence breaks utility | PMC8627243 |
| 7 | **PRS ancestry portability** | EUR-trained PRS lose accuracy: **~1.7× lower (South Asian), ~2.5× (East Asian), ~4.5–4.9× lower R² (African)**; Indigenous-American/admixed also reduced | Allele-freq & LD differences; documented equity gap → basis for ancestry-holdout gate | Nat Med s41591-022-01835-x; Hum Genomics 40246-024-00664-y |
| 8a | **Rituximab (anti-CD20) RA** | ~50% ACR20 wk24 in TNFi failures (REFLEX 51% vs 18% pbo; ACR50 27%, ACR70 12%) = **partial** | Approved B-cell depletion for TNFi-inadequate RA | A&R 10.1002/art.22025 |
| 8b | **TNF inhibitors RA** | First-line biologic; ~30–40% fail/partial → switching | Establishes partial-response reality | Arthritis Res Ther ar2666 |
| 8c | **Anifrolumab & belimumab SLE** | Real approved SLE biologics; partial (belimumab SRI-4 ~54% vs ~40% pbo wk52; anifrolumab significant SRI/BICLA, TULIP) | Anti-IFNAR1 / anti-BLyS; modest absolute benefit | PMC10186457; PMC11299632 |
| 9a | **SLEDAI (lupus activity)** | SLEDAI-2K **0–105**; severe flare commonly **total > 12** (or Δ≥7 non-renal) | Most-used lupus activity index → anchors "high activity" | Front Lupus 2024.1442013 |
| 9b | **DAS28 (RA activity)** | ~**0–9.4**; **>5.1 high**, 3.2–5.1 moderate, ≤3.2 low, **<2.6 remission** | Standard RA composite-activity cutoffs | PMC4235977 |

**Caveats:** ORs are population/study-dependent (use ranges as anchors). HLA-DRB1\*15:01 SLE OR varies by ancestry. Shared-epitope & PTPN22 effects concentrate in seropositive (ACPA/RF+) RA. Therapy figures are trial averages vs placebo = *partial* response, appropriate for modeling incomplete-treatment scenarios.

**How these map to the four keystone gates:**
- **Causally-grounded** ← evidence strength (real loci above + omics support), rare-variant cutoff (row 5).
- **Calibrated** ← ECE / interval-coverage conventions (row 6).
- **Not-spurious** ← confounding / population-stratification (PTPN22 ancestry-specificity, row 2 & 7).
- **Ancestry-generalizable** ← PRS portability loss (row 7); fails for ancestries absent from training (Indigenous-American, African for EUR-derived effects).

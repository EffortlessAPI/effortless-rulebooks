# Radelet 1981 — Provenance

**Citation:** Radelet ML.
"Racial characteristics and the imposition of the death penalty."
*American Sociological Review.* 1981;46(6):918–927.
JSTOR: https://www.jstor.org/stable/2095513

**Source table:** Table 1 in the paper. Extracted manually 2026-06-27.
Counts also reproduced in statistics textbooks (e.g. Statistics LibreTexts 10.5).
No transformation applied — numbers are the exact integers from the published table.

**Domain:** Criminal justice. 326 homicide cases from 20 Florida counties, 1976–1977.

**Groups:**
- A = white defendant
- B = black defendant

**Strata:** Victim race
- white-victim = homicide case with a white victim
- black-victim = homicide case with a black victim

**Outcome:** Death penalty imposed (success = death sentence handed down)

**Raw counts (from Table 1):**

| Stratum      | White defendant (A) |       | Black defendant (B) |       |
|--------------|---------------------|-------|---------------------|-------|
|              | Death (successes)   | Total | Death (successes)   | Total |
| white-victim | 19                  | 151   | 11                  | 63    |
| black-victim | 0                   | 9     | 6                   | 103   |
| pooled       | 19                  | 160   | 17                  | 166   |

**Verification:**
- white-victim: A = 19/151 = 12.58%, B = 11/63 = 17.46% → B (black def) MORE likely  (+4.88pp)
- black-victim: A =  0/9  =  0.00%, B =  6/103 =  5.83% → B (black def) MORE likely  (+5.83pp)
- pooled:       A = 19/160 = 11.88%, B = 17/166 = 10.24% → A (white def) APPEARS more likely

Full reversal. Confounder: victim race predicts both defendant race (cross-race homicides rare)
and death penalty probability (white-victim cases sentenced to death ~5× more often).
Expected DistortionType = A, PolicyImplication = stratify-immediately.

**Note on 0/9 cell:** White defendants with black victims — only 9 cases, 0 death sentences.
The zero numerator is the actual published count, not imputed. The StratumGap for black-victim
stratum is well-defined as 0% − 5.83% = −5.83pp (B is worse). Postgres division is safe (9 ≠ 0).

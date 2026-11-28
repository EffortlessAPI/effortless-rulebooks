# 📘 Does God Exist — Hitchens vs Turek (VCU) — Docs

_Rulebook formalizing the VCU debate 'Does God Exist?' between Christopher Hitchens (atheism/anti-theism) and Frank Turek (theism), and every philosophical concept raised. Built via the Shadle steps: raw text -> vocabulary -> glossary -> narrative -> mock data -> normalized schema -> this DAG-structured hub._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Debater** | The two principals and the moderator of the debate. | — |
| Label | A defined attribute. | _Display name of the participant._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Side | A defined attribute. | _Role in the debate: affirmative, negative, or moderator._ |
| Description | A defined attribute. | _Biographical note and stance. (Worldviews defended are the reverse of Worldviews.ChampionedBy.)_ |
| Argument Count | The number of arguments related to the debater. | _1st-order: number of arguments advanced._ |
| Claim Count | The number of claims related to the debater. | _1st-order: number of claims made._ |
| Thinkers Cited | The number of thinkers related to the debater. | _1st-order: distinct thinkers pressed into service._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Side | Description | Argument Count ƒ | Claim Count ƒ | Thinkers Cited ƒ |
|---|---|---|---|---|---|---|
| Frank Turek | frank-turek | affirmative | Christian apologist, founder of CrossExamined.org, co-author of 'I Don't Have Enough Faith to Be an Atheist'. Argues the affirmative for a generic theism. | 9 | 9 | 16 |
| Christopher Hitchens | christopher-hitchens | negative | Author of 'God Is Not Great'. Argues the negative; self-describes as anti-theist, not merely atheist. | 4 | 8 | 19 |
| Timothy Hulley | timothy-hulley | moderator | Dean of the VCU Honors College; moderates phases and relays audience questions for the event hosted by the United Secular Alliance. | 0 | 0 | 0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Worldview** | Competing positions on God and reality contested in the debate. | — |
| Label | A defined attribute. | _Name of the worldview._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Affirms God | True when an empty string. | _Whether the worldview affirms the existence of God._ |
| Championed by | A defined attribute. | _Debater who principally champions this worldview._ |
| Description | A defined attribute. | _What the worldview holds in this debate's context._ |
| Argument Count | The number of arguments related to the worldview. | _1st-order: arguments fielded under this worldview._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Affirms God | Description | Argument Count ƒ |
|---|---|---|---|---|
| Theism | theism | true | A personal God who created and intervenes in the universe, answers prayer, and grounds morality. | 9 |
| Deism | deism | true | A designer who set the cosmos in order but does not intervene or reveal itself; Hitchens treats it as separable from theism. | 0 |
| Atheism | atheism | false | No advanced argument for God has survived rebuttal; not the dogmatic claim no God could exist. | 2 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Argument** | The named arguments advanced by each side, with derived development and contestation metrics. | — |
| Label | A defined attribute. | _Name of the argument._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Argument Type | A defined attribute. | _primary, sub, supporting, or counter._ |
| Advanced by | A defined attribute. | _Debater who advances the argument._ |
| Fields Worldview | A defined attribute. | _Worldview the argument serves._ |
| Conclusion | A defined attribute. | _What the argument purports to establish._ |
| Description | A defined attribute. | _Summary of the argument as presented._ |
| Premise Count | The number of premises related to the argument. | _1st-order: premises composing the argument._ |
| Evidence Count | The number of evidence related to the argument. | _1st-order: evidence items cited._ |
| Claim Count | The number of claims related to the argument. | _1st-order: claims supporting the argument._ |
| Is Fully Developed | True when the premise count is at least 2. | _2nd-order: has at least two premises._ |
| Total Rebuttals | The total rebuttal count across the claims related to the argument. | _2nd-order: rebuttals accumulated across supporting claims._ |
| Is Contested | True when the total rebuttals is greater than 0. | _3rd-order: at least one supporting claim was rebutted._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Argument Type | Conclusion | Description | Premise Count ƒ | Evidence Count ƒ | Claim Count ƒ | Is Fully Developed ƒ | Total Rebuttals ƒ | Is Contested ƒ |
|---|---|---|---|---|---|---|---|---|---|---|
| Cosmological Argument | cosmological-argument | primary | The universe had a beginning, so it had a beginner. | From the Big Bang: a beginning implies a beginner; SURGE supplies the evidence. | 3 | 5 | 1 | true | 1 | true |
| Teleological Argument | teleological-argument | primary | Order and apparent purpose imply a designer. | The design argument; contains fine-tuning and design-of-life as sub-arguments. | 0 | 0 | 1 | false | 1 | true |
| Argument From Fine-Tuning | argument-from-fine-tuning | sub | Life-permitting constants imply a tuner. | Constants on a razor's edge (the gravitational force to 1 part in 10^40, expansion rate, axial tilt, Jupiter). | 3 | 0 | 1 | true | 1 | true |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Premise** | Ordered premises composing each argument. | — |
| Label | A defined attribute. | _Short name of the premise._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Supports Argument | A defined attribute. | _Argument this premise belongs to._ |
| Ordinal | A defined attribute. | _Position of the premise within its argument._ |
| Statement | A defined attribute. | _The premise as stated._ |
| Description | A defined attribute. | _Note on the premise._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Ordinal | Statement | Description |
|---|---|---|---|---|
| Universe Began | universe-began | 1 | The universe had a beginning (SURGE). | Premise 1 of the cosmological argument. |
| Beginning Needs Beginner | beginning-needs-beginner | 2 | Whatever begins to exist has a cause/beginner. | Premise 2 of the cosmological argument. |
| Cause Is Transcendent | cause-is-transcendent | 3 | The creator-cause is spaceless, timeless, immaterial, intelligent, powerful, and personal. | Premise 3 of the cosmological argument. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Evidence** | Empirical and theoretical evidence items cited in support of arguments. | — |
| Label | A defined attribute. | _Name of the evidence item._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Supports Argument | A defined attribute. | _Argument the evidence supports._ |
| Evidence Kind | A defined attribute. | _observational or theoretical._ |
| Description | A defined attribute. | _What the evidence is and who found it._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Evidence Kind | Description |
|---|---|---|---|
| SURGE: Second Law | surge:-second-law | theoretical | Second law of thermodynamics — the universe runs toward heat death, so it cannot be eternal. |
| SURGE: Universe Expanding | surge:-universe-expanding | observational | Hubble's 1929 discovery of expansion; reversed, it collapses to a singularity. |
| SURGE: Radiation Echo | surge:-radiation-echo | observational | Cosmic afterglow found by Penzias and Wilson; remnant heat of the Big Bang. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Concept** | Every philosophical concept raised in the debate, with category and introducer. | — |
| Label | A defined attribute. | _Name of the concept._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Category | A defined attribute. | _cosmology, design, moral, theological, method, or epistemic._ |
| Introduced by | A defined attribute. | _Debater who introduced the concept._ |
| Description | A defined attribute. | _Meaning of the concept in this debate._ |
| Claim Count | The number of claims related to the concept. | _1st-order: claims that touch this concept._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Category | Description | Claim Count ƒ |
|---|---|---|---|---|
| Big Bang | big-bang | cosmology | The accepted origin of the universe; Turek accepts it and infers a 'banger'. | 1 |
| Heat Death | heat-death | cosmology | The far-future maximal-entropy state; used by both (Turek vs eternity, Hitchens for poor design). | 1 |
| Singularity | singularity | cosmology | The point reversed expansion collapses to; Turek equates it with 'nothing'. | 0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Claim** | Specific assertions made by each debater, with derived rebuttal status. | — |
| Label | A defined attribute. | _Short name of the claim._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Made by | A defined attribute. | _Debater who made the claim._ |
| Phase | A defined attribute. | _opening, rebuttal, cross-examination, or closing._ |
| Supports Argument | A defined attribute. | _Argument the claim supports._ |
| Touches Concept | A defined attribute. | _Primary concept the claim engages._ |
| Description | A defined attribute. | _The claim in brief._ |
| Rebuttal Count | The number of rebuttals related to the claim. | _1st-order: rebuttals directed at this claim._ |
| Is Rebutted | True when the rebuttal count is greater than 0. | _2nd-order: the claim drew at least one rebuttal._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Phase | Description | Rebuttal Count ƒ | Is Rebutted ƒ |
|---|---|---|---|---|---|
| Beginning Implies Beginner | beginning-implies-beginner | opening | The Big Bang's beginning requires a transcendent beginner. | 1 | true |
| Constants Are Fine-Tuned | constants-are-fine-tuned | opening | Life-permitting constants sit on a razor's edge, implying a tuner. | 1 | true |
| Information Implies Mind | information-implies-mind | opening | DNA's specified complexity could only come from a mind. | 1 | true |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Rebuttal** | Direct responses aimed at specific claims. | — |
| Label | A defined attribute. | _Short name of the rebuttal._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Made by | A defined attribute. | _Debater making the rebuttal._ |
| Rebuts Claim | A defined attribute. | _Claim being rebutted._ |
| Description | A defined attribute. | _The rebuttal in brief._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Description |
|---|---|---|
| Cause Need Not Choose | cause-need-not-choose | 'I don't have to know' the origin; positing a choosing being just relabels the mystery. |
| Accelerating Expansion | accelerating-expansion | Krauss: the red-shift rate is increasing — the cosmos is flying apart, hardly careful tuning. |
| Information Argument Is Faith | information-argument-is-faith | The origin-of-life quotes are all statements of uncertainty, not evidence of a mind. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Thinker** | Authorities invoked by each side, including hostile witnesses. | — |
| Label | A defined attribute. | _Name of the thinker._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Field | A defined attribute. | _Primary field (physics, biology, philosophy, etc.)._ |
| Era | A defined attribute. | _Rough period._ |
| Cited by | A defined attribute. | _Debater who invokes them._ |
| Cited As Hostile Witness | True when an empty string. | _Cited against their own sympathies (e.g., an atheist conceding fine-tuning)._ |
| Description | A defined attribute. | _Why they appear in the debate._ |
| Quotation Count | The number of quotations related to the thinker. | _1st-order: quotations attributed to them._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Field | Era | Cited As Hostile Witness | Description | Quotation Count ƒ |
|---|---|---|---|---|---|---|
| Edwin Hubble | edwin-hubble | physics | 20th c. | false | Discovered cosmic expansion (1929). | 0 |
| Arno Penzias | arno-penzias | physics | 20th c. | false | Co-discovered the radiation echo; quoted on a balanced, supernatural-seeming universe. | 0 |
| Robert Wilson | robert-wilson | physics | 20th c. | false | Co-discovered the cosmic afterglow at Bell Labs. | 0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Quotation** | Citations used by debaters, stored as paraphrased gists to respect copyright. | — |
| Label | A defined attribute. | _Short name of the quotation._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Logical primary key — slug of Label._ |
| Speaker | A defined attribute. | _Thinker the quotation is attributed to._ |
| Cited by | A defined attribute. | _Debater who cited it._ |
| Gist | A defined attribute. | _Paraphrased substance of the quotation._ |
| Description | A defined attribute. | _Context note._ |

<details open>
<summary>Example data</summary>

| Label | Name ƒ | Gist | Description |
|---|---|---|---|
| Smoot On Looking At God | smoot-on-looking-at-god | Paraphrase: a religious person might see the COBE result as a glimpse of the divine. | Paraphrased citation used in the debate (not verbatim). |
| Jastrow's Mountain Of Ignorance | jastrow's-mountain-of-ignorance | Paraphrase: the scientist scaling the peak finds theologians already seated there. | Paraphrased citation used in the debate (not verbatim). |
| Eddington On The Beginning | eddington-on-the-beginning | Paraphrase: the cosmic beginning seems frankly supernatural. | Paraphrased citation used in the debate (not verbatim). |

_ƒ marks a computed column._

</details>

## 2 Fact Types

- a **worldview** may reference one **debater**
- an **argument** references exactly one **debater**
- an **argument** references exactly one **worldview**
- a **premise** references exactly one **argument**
- an **evidence** references exactly one **argument**
- a **concept** may reference one **debater**
- a **claim** references exactly one **debater**
- a **claim** may reference one **argument**
- a **claim** may reference one **concept**
- a **rebuttal** references exactly one **debater**
- a **rebuttal** references exactly one **claim**
- a **thinker** may reference one **debater**
- a **quotation** references exactly one **thinker**
- a **quotation** references exactly one **debater**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A debater **must** have a label and a side.
- A worldview **must** have a label, and record whether it is affirms god.
- An argument **must** reference exactly one debater as its advanced by.
- An argument **must** reference exactly one worldview as its fields worldview.
- An argument **must** have a label and an argument type.
- A premise **must** reference exactly one argument as its supports argument.
- A premise **must** have a label and an ordinal.
- An evidence **must** reference exactly one argument as its supports argument.
- An evidence **must** have a label and an evidence kind.
- A concept **must** have a label and a category.
- A claim **must** reference exactly one debater as its made by.
- A claim **must** have a label and a phase.
- A rebuttal **must** reference exactly one debater as its made by.
- A rebuttal **must** reference exactly one claim as its rebuts claim.
- A rebuttal **must** have a label.
- A thinker **must** have a label, and record whether it is cited as hostile witness.
- A quotation **must** reference exactly one thinker as its speaker.
- A quotation **must** reference exactly one debater as its cited by.
- A quotation **must** have a label.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A debater's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-2 Argument Count** | A debater's argument count is the number of arguments related to the debater. |
| **DR-3 Claim Count** | A debater's claim count is the number of claims related to the debater. |
| **DR-4 Thinkers Cited** | A debater's thinkers cited is the number of thinkers related to the debater. |
| **DR-5 Name** | A worldview's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-6 Argument Count** | A worldview's argument count is the number of arguments related to the worldview. |
| **DR-7 Name** | An argument's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-8 Premise Count** | An argument's premise count is the number of premises related to the argument. |
| **DR-9 Evidence Count** | An argument's evidence count is the number of evidence related to the argument. |
| **DR-10 Claim Count** | An argument's claim count is the number of claims related to the argument. |
| **DR-11 Is Fully Developed** | An argument is considered fully-developed if the premise count is at least 2. |
| **DR-12 Total Rebuttals** | An argument's total rebuttals is the total rebuttal count across the claims related to the argument. |
| **DR-13 Is Contested** | An argument is considered contested if the total rebuttals is greater than 0. |
| **DR-14 Name** | A premise's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-15 Name** | An evidence's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-16 Name** | A concept's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-17 Claim Count** | A concept's claim count is the number of claims related to the concept. |
| **DR-18 Name** | A claim's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-19 Rebuttal Count** | A claim's rebuttal count is the number of rebuttals related to the claim. |
| **DR-20 Is Rebutted** | A claim is considered rebutted if the rebuttal count is greater than 0. |
| **DR-21 Name** | A rebuttal's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-22 Name** | A thinker's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-23 Quotation Count** | A thinker's quotation count is the number of quotations related to the thinker. |
| **DR-24 Name** | A quotation's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |

## 5 Traceability to Schema

_The expression column is the rule's definition in Docs notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Debaters.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **Debaters.ArgumentCount** | rollup | `Count(Arguments via AdvancedBy)` |
| **Debaters.ClaimCount** | rollup | `Count(Claims via MadeBy)` |
| **Debaters.ThinkersCited** | rollup | `Count(Thinkers via CitedBy)` |
| **Worldviews.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **Worldviews.ArgumentCount** | rollup | `Count(Arguments via FieldsWorldview)` |
| **Arguments.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **Arguments.PremiseCount** | rollup | `Count(Premises via SupportsArgument)` |
| **Arguments.EvidenceCount** | rollup | `Count(Evidence via SupportsArgument)` |
| **Arguments.ClaimCount** | rollup | `Count(Claims via SupportsArgument)` |
| **Arguments.IsFullyDeveloped** | formula | `If(PremiseCount >= 2, True(), False())` |
| **Arguments.TotalRebuttals** | rollup | `Sum(Claims.RebuttalCount via SupportsArgument)` |
| **Arguments.IsContested** | formula | `If(TotalRebuttals > 0, True(), False())` |
| **Premises.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **Evidence.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **Concepts.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **Concepts.ClaimCount** | rollup | `Count(Claims via TouchesConcept)` |
| **Claims.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **Claims.RebuttalCount** | rollup | `Count(Rebuttals via RebutsClaim)` |
| **Claims.IsRebutted** | formula | `If(RebuttalCount > 0, True(), False())` |
| **Rebuttals.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **Thinkers.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **Thinkers.QuotationCount** | rollup | `Count(Quotations via Speaker)` |
| **Quotations.Name** | formula | `Replace(Lower(Label), " ", "-")` |

---

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)

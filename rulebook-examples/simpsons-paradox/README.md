# Build Brief: Two Witnessed-DAG Demonstrations

_A standalone starting prompt for a new session. You have no prior context, and that's intentional — I want your independent read on this plan, including whether it's wrong. Read it, then push back before we build anything._

---

## What you need to know first (the framework)

There's an open-source framework (`effortlessapi/effortless-rulebooks`) built on a simple, strong idea: business/domain rules are defined **once**, as structural data in a JSON "rulebook," and then mechanically compiled into many execution substrates — Postgres, Python, Go, OWL, Excel, English, even COBOL and ARM64 — that **provably return identical results** against a shared answer key. The rulebook is the single source of truth; the substrates are interchangeable backends.

Every derived value in a rulebook decomposes into five first-class kinds:

- **Schema** — the tables and their fields
- **Data** — raw, entered facts (the "leaves")
- **Lookups** — pull a value from a related row (INDEX/MATCH-style)
- **Aggregations** — count/sum/min/max over related rows (COUNTIFS-style)
- **Functions** — pure formulas combining the above

Call it **S/D/L/A/F over a bitemporal ACID DAG.** The critical property: there is a **trust boundary** drawn as a line in the dependency graph. Anything *raw* is entered by a human (or an LLM acting as an overridable transcriber). Anything with a *formula* is the model's — pure, inspectable, deterministic. No inference is ever hidden inside generated prose. Every conclusion traces down to raw facts by a path you can read.

This is why conformance matters: if the same rulebook produces identical results in Postgres, Python, Go, and OWL, an error in the rules shows up as **divergence across substrates** (caught mechanically) or as **every substrate agreeing on the same wrong answer** (a specification defect, visible in the rules, catchable by a human reading them). You can't get a silent, localized bug. Errors are loud and global, which makes them cheap to find and fix-once-fix-everywhere.

## The Leopold loop

The development cycle, named for how it feels in practice:

1. Look at the rulebook.
2. Hypothesize a change (a new concept, a new angle on existing facts).
3. Update the schema.
4. Push the model to Postgres (or any substrate).
5. It computes all the higher-order inferences deterministically.
6. Import those computed values back into the rulebook as first-class data.
7. Repeat — now with the new concept's inferences witnessed and available as columns you can group, sort, filter, pivot, chart, and build the *next* inference on top of.

Each loop adds one first-class concept. After 25–50 loops you have a richly populated conceptual model where **the interesting patterns live in the relationships *between* inferences that only coexist once many are columns in the same substrate.** That's the engine: not "analyze forest fires" with bespoke fire code, but "analyze *systems*" where forest-fire-ness is a value in a column, and the same generic operations (group/sort/chart/cluster) work across every domain that lands in the substrate.

## The one distinction that governs everything

Two kinds of proposition can be witnessed in this substrate, and they have **different epistemic status**:

**Entailed-kind.** The pattern is logically latent in facts you already have. The loop surfaces what your premises commit you to but you didn't know they did. This is discovery in the *mathematical* sense — like finding a theorem, or like spotting that a power law breaks in a specific regime when you project the fixed data onto a windowed-slope angle. **These cannot be wrong** (they're entailed) and **reality gets no vote** (no new measurement is needed). Maximally defensible; surprises the modeler, not the world.

**Instrument-kind.** The loop builds and *hardens* a witnessed inference machine on realistic synthetic leaves — validating, angle by angle, that the structure computes the inferences we already know are correct. Then, **once, at the end**, real data flows through the identical DAG and the world-claim gets made — traceably, every clause citing a raw fact. The synthetic data never carries the answer; it calibrates the instrument. The truth enters through the real leaves at the final step. These *can* be wrong (reality vetoes), but they fail **informatively** — because it's a witnessed DAG, you can see *which node* diverged from reality.

The discipline the framework demands: **know which kind of proposition a given witness is about.** A beautifully-witnessed entailed pattern and a beautifully-witnessed synthetic pattern look identical as artifacts. The only error available is reading an entailed witness ("this pattern is in my rows") as a world-claim ("this pattern is in reality"). The framework makes *both* kinds traceable; it does not by itself tell you which one you're holding. That's the modeler's job.

---

## The plan: two demonstrations, deliberately paired

We screened ten candidate domains against one criterion — **most defensible, uncontroversial, potentially-really-solid DAG** — and two won, for opposite reasons. We want to build both, flagship first.

### Flagship — Simpson's-paradox / stratification-reversal detector (entailed-kind)

**Why it wins.** It is the **only** candidate whose step-7 output is a *proof* rather than a *prediction*. A witnessed stratification reversal — an aggregate trend that flips inside every stratum — is **deductively true given the rows**. Reality can't veto it. No threshold can be argued. No replication is needed, because it's arithmetic.

**Why it's the purest demonstration of the whole thesis.** The mechanism is two aggregations over the *same fixed rows* — an aggregate trend, and a per-stratum trend — and a comparison. Angle A says X; angle C says not-X; and **only building both witnesses reveals the contradiction.** Nothing measured enters. The non-randomness was latent the whole time and became *knowable* only because someone built the projection that makes it legible. That is the entire conjecture, in its cleanest possible form.

**Why it's pedagogically perfect.** The punchline — "the trend reverses inside every group" — is viscerally surprising the first time everyone sees it, and *undeniable* once shown. The witness is the explanation.

**The one honest caveat to design around.** Over a textbook dataset it's a *demo*, not a *discovery*. Pointed at a **fresh real dataset with enough strata**, it can surface a *previously-unnoticed* reversal — and that finding is provable on the spot. So the build should target the structure that ingests an arbitrary real dataset (admissions, hiring, treatment outcomes, sports, anything stratifiable) and emits, for every (outcome, grouping-variable) pair, a witnessed verdict: *no reversal / reversal present*, with the deciding strata named.

### Deep follow-up — Tax-and-transfer marginal-rate cliffs (entailed-kind, deep DAG)

**Why it's the best *shape*.** Each benefit program is a set of rules. A marginal-rate cliff — a point where earning one more dollar *lowers* net income — is entailed by the **interaction** of separately-designed programs and lives in **no single one of them**. It's invisible program-by-program and only exists in the combinatorial layer many hops above ground truth. Adding a program doesn't just add rows; it adds *interaction surface* with every program already present. This is the clearest example on the whole list of **why depth matters** — the cliff is a property that only materializes 20–30 hops up, after enough programs coexist. The leaves multiply each generation. That's the deep-DAG shape we actually want to demonstrate.

**Why it's still defensible.** The cliff is *entailed* (a fact about how the phase-outs compose), so it's as deductively solid as Simpson's. And step 7 has abundant real data — real program parameters are public.

**The honest tension to name up front.** Cliffs are arithmetically unimpeachable but **politically legible**. "A family at \$40k loses money by earning more" is true and also the kind of true that people argue about — not because the arithmetic is wrong but because the *parameters* are someone's policy. So: deductively solid, loud in reception. We accept that knowingly; it's a different thing from being controversial *in fact*.

### Why this specific pair

One **unarguable** (Simpson's — proves the thesis deductively, reality has no vote) and one **ambitious with real depth and real stakes** (cliffs — the exploding-leaves shape on a domain people care about). They share machinery: both are **aggregation-reversal over stratified rows**. A cliff is, structurally, a reversal in the net-income-vs-gross-income relationship across the stratification by program-eligibility. So **the second is cheap once the first exists** — same engine, deeper data.

---

## What I want from you (the actual ask)

Not a rubber stamp. I want your independent read on whether this plan is sound *before* we write any schema. Specifically:

**On the flagship choice.** Is Simpson's actually the right flagship, or am I overrating "deductively certain" against "interesting"? A reversal detector that only confirms known textbook reversals is a demo. What's the cheapest path to a *fresh* real dataset where a reversal is *plausibly unnoticed* — so the first run is a genuine (if small) discovery and not a recital? If you think a different one of the ten screened domains is a stronger flagship, say so and say why.

**On the entailed-vs-instrument framing.** Both chosen domains are *entailed-kind* — deliberately, for defensibility. Is that the right call for a *first* demonstration, or does leaning entirely entailed undersell the framework by avoiding the harder, more impressive *instrument-kind* claim (build on synthetic, validate on real)? Where's the line — should the deep follow-up reach for an instrument-kind result instead of staying entailed?

**On the shared-machinery claim.** I'm asserting a cliff is "structurally a reversal" and therefore the second build reuses the first's engine. Stress-test that. If it's actually a different shape, the pairing logic collapses and we should pick a different second domain.

**On the witness design.** For *either* domain, the whole value is in witnesses that make the situation legible to a non-expert and traceable to ground truth for an expert. What do those witnesses need to look like? What's the failure mode where the DAG is correct but the *presentation* of the reversal/cliff leaves a reasonable viewer unconvinced or confused?

**On scope and loop count.** Is "25–50 loops" right for these, or does Simpson's saturate at far fewer (it's a small mechanism) while cliffs genuinely need the depth? What's the minimum loop count where each demonstrates its point honestly?

Start wherever you have the strongest opinion. If you think the whole plan is aimed wrong, say that first.
# Trust the Artifact, Not the Autocomplete
## Article 8: A Student's Guide to Rumors, Receipts, and Reasoning

**Series position:** 8 of 10
**Audience:** High school and college students, teachers, curious nontechnical readers, science communicators
**Central metaphor:** School rumors versus receipts. "Sounds right" is not "proved."
**Tone:** Conversational, uses school social examples, builds toward statistical intuition without formulas

---

## Original Outline

**Title:** *Rumors, Receipts, and the Machine That Checks the Work*

**Central metaphor:** School rumors versus receipts.

**Focus:** Why "sounds right" is not the same as "proved."

**Translate terms this way:**

| Technical term      | Student version             |
| ------------------- | --------------------------- |
| Hallucination       | Confident rumor             |
| Rulebook            | Receipts notebook           |
| Invariant check     | Fact-check rule             |
| Predicate invention | Inventing a useful category |
| Loop history        | Revision history            |

**Article angle:**
Use examples like "everyone says this team is better" or "this study proves X," then show how hidden groupings can change the story. The LLM is like a student who notices possible patterns, but the rulebook is the notebook that shows the math.

**Key lesson:**
"Reasoning is not just having an answer. Reasoning is being able to show how the answer survives checking."

---

## Source Material to Weave In

### The Berkeley admissions story (the accessible version)

In 1973, UC Berkeley looked at its admissions data. The numbers were stark: men were being admitted at a much higher rate than women. The aggregate statistics said so clearly. It looked like discrimination.

But when researchers looked at the numbers by department — not just the total — something strange appeared. In nearly every individual department, women were being admitted at *higher* rates than men. Both things were true at the same time. How?

Answer: women were applying in larger numbers to departments that were harder to get into for everyone. The aggregate number was hiding this detail. Once you broke the data down by department, the pattern reversed. The conclusion that looked obvious was backwards.

This is Simpson's Paradox. And it shows up everywhere: medical studies, sports statistics, legal cases, economic reports.

### How it feels in high school: the team ranking example

Imagine two basketball players arguing about which of their teams is better. One says: "Look at the stats — our team has a 60% win rate and your team only has a 55% win rate. We're better." The other says: "That's because you play half your games at home. When we compare home vs. away separately, we win more in both situations."

Both players are looking at the same game history. But one is combining numbers that shouldn't be combined. The 5-point difference in win rate is an artifact of scheduling — not evidence of quality.

This is exactly what AllocationDistortion measures in the project: how much of the apparent gap between two treatments is caused by unequal distribution of patients between groups, not by the actual difference in treatment effectiveness.

### The AI rumor machine

An LLM is very good at generating confident-sounding descriptions of patterns. Ask it to describe a dataset and it will produce a polished paragraph. The problem: the paragraph sounds like a conclusion whether the math supports it or not. This is what researchers call hallucination — not lying, but generating plausible text that hasn't been checked.

The project uses the LLM like a student who's good at brainstorming but hasn't done the homework yet. The model proposes names: `IsSignFlip`, `AllocationDistortion`, `SignalPurity`. These are useful ideas. But they go into a receipts notebook — the rulebook — and the formulas are checked against every study in the dataset before anyone believes them.

### The receipts notebook: how checking actually works

The rulebook is the receipts notebook. Every concept has:
- A name
- A formula (what it actually computes)
- A fact-check rule (an invariant that must pass)

Example receipt for `SignalPurity`:
- **Name**: SignalPurity
- **Formula**: how much of the observed gap between treatments is real vs. noise, expressed as a fraction from 0 to 1
- **Fact-check rule**: Studies where the pooled result is backwards always have SignalPurity below 0.5 — meaning the noise is doing more than half the work

That fact-check rule was tested against all 238 studies in the dataset. It passed every time. That's not a rumor — that's a receipt.

### The revision history

The project also keeps a complete revision history — 78 loops of improvements, each one recorded in the model. Loop 4 introduced the first classification. Loop 21 extended it to five types. Loop 34 proved that the corrected estimate doesn't change when you shuffle patients between groups.

Every step was witnessed and recorded. You can go back to loop 17 and see exactly what was true at that point: which studies showed a paradox, what the numbers were, what question was still open. This is like the edit history on a shared document — but for a scientific conclusion.

### The three grades of knowledge

Not all facts are equally well-established. The project grades them explicitly:

**Grade A: Theorem** — mathematically proven from the definition. Example: "The corrected estimate of the gap between treatments is the same no matter how patients are allocated between groups." This is provably true. You don't need more studies to be sure.

**Grade B: Discovery finding** — tested against real data, with the prediction written down before looking at the data. Example: "We predicted that economics studies would show no sign-flip effects. We then looked. Confirmed." This is good evidence. More data could theoretically change it.

**Grade C: Old observation** — something that seemed true with fewer studies but was revised when more were added. The project keeps these labeled and archived, not deleted. "We thought Type D studies were dominant at N=64. At N=238, the picture was more nuanced."

This grading system is what good science is supposed to do. The problem is most AI output has no grades at all — every statement comes out looking equally confident.

### The punchline: reasoning is showing your work

The easiest way to sound smart is to be confident. The hardest way to be right is to show your work in a way that can be checked and, if necessary, corrected.

AI systems are very good at the first thing. The architecture in this project is specifically designed to force the second thing. Every conclusion is backed by a formula. Every formula is tested. Every test is recorded. Every revision is archived.

"Reasoning is not just having an answer. Reasoning is being able to show how the answer survives checking."

---

## Structural Outline for the Article

1. **Hook (3 paragraphs):** The Berkeley story. Data that looks one way in aggregate, the opposite way in subgroups.
2. **A familiar example (2 paragraphs):** The basketball team ranking. How hidden groupings change the story.
3. **The AI rumor machine (2 paragraphs):** LLMs produce confident prose, not checked conclusions. The brainstormer analogy.
4. **The receipts notebook (3 paragraphs):** The rulebook. Name, formula, fact-check rule. The SignalPurity example with all three.
5. **The revision history (2 paragraphs):** 78 loops. What was known at each step. The edit-history analogy.
6. **Three grades of knowledge (3 paragraphs):** Theorem, discovery finding, old observation. Why grades matter.
7. **Close (2 paragraphs):** Sounding confident vs. being checkable. The key lesson.

**Target length:** 1000–1300 words
**Suggested Medium tags:** Statistics, Simpson's Paradox, Critical Thinking, AI Literacy, Data Literacy

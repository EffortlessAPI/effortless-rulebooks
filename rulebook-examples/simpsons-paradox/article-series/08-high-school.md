# Trust the Artifact, Not the Autocomplete
## Article 8: A Student's Guide to Rumors, Receipts, and Reasoning

**Series position:** 8 of 10
**Audience:** High school and college students, teachers, curious nontechnical readers, science communicators
**Central metaphor:** School rumors versus receipts. "Sounds right" is not "proved."
**Tone:** Conversational, uses school social examples, builds toward statistical intuition without formulas

---

*This is part 8 of a 10-part series exploring what it looks like to build trustworthy AI systems with verifiable outputs.*

# Rumors, Receipts, and the Machine That Checks the Work

In 1973, researchers at UC Berkeley looked at the university's admissions numbers and got alarmed. Men were being admitted at a significantly higher rate than women. The aggregate statistics were stark. The conclusion seemed obvious: the university was discriminating.

Then someone asked a different question. Instead of looking at the total numbers, they looked at each department separately.

In almost every individual department, women were being admitted at *higher* rates than men. Not the same rate. Higher. The same data that seemed to show discrimination showed the opposite when you broke it down correctly.

How is that possible? Both things can be true at once because of a hidden factor: women were applying in much larger numbers to the most competitive departments — the ones where admission rates were low for everyone. That concentration in hard-to-enter departments dragged down the overall women's admission rate, even though women were outperforming men inside each individual department. The aggregate number looked like one story. The detailed breakdown told the opposite story. One of them was right and one was misleading — and you couldn't tell which until you looked underneath.

This is Simpson's Paradox. And it has been fooling researchers, journalists, and policymakers ever since.

---

## The basketball team argument

You've probably had a version of this argument yourself.

Two friends are arguing about which of their basketball teams is better. One says: "Our team has a 60% win rate and your team only has 55%. We're better." The other says: "That's because you play half your games at home. When you look at home games and away games separately, we win more in both situations."

Both friends are looking at exactly the same game history. But one is combining numbers that don't belong together — home games and away games aren't the same thing. Mixing them produces a 60% overall win rate that means something different than it appears to. The five-point difference is real. But it isn't telling you what you think it's telling you.

This is exactly the kind of thing a statistics research project is designed to catch. The technical name for "how much of the apparent difference is caused by mixing groups that shouldn't be mixed" is `AllocationDistortion`. A high AllocationDistortion means the comparison is more like comparing home-game and away-game performance without labeling them. It doesn't necessarily mean anyone was trying to mislead you. It means the groups weren't fair to compare directly, and combining them created a false picture.

The hard part is that the combined number almost always sounds authoritative. Numbers feel like receipts. But a number you haven't checked the grouping on is more like a rumor — it might be true, it might not, and it definitely sounds more certain than it is.

---

## The AI rumor machine

Here is where artificial intelligence comes in — and where it can go wrong.

Large language models are extraordinarily good at generating confident-sounding statements about patterns. You can show one a dataset and it will produce a polished summary. The problem: the summary sounds like a conclusion whether the underlying math supports it or not.

Researchers call this hallucination — not lying, but generating plausible text that hasn't been verified. The model isn't trying to deceive. It's doing exactly what it was trained to do: produce fluent, confident-sounding text in response to a prompt. The problem is that "fluent and confident" is not the same as "checked and true."

Think of it like a classmate who's great at brainstorming but hasn't done the homework yet. Ask them to name the categories for your project and they'll generate five great-sounding names instantly. That's useful! But those names aren't answers yet — they're proposals. The homework is figuring out whether the categories actually work when you apply them to real data.

In the Simpson's Paradox research project, the AI was used exactly like that classmate. It proposed names: `IsSignFlip`, `AllocationDistortion`, `SignalPurity`. Useful ideas. But they went into a receipts notebook — and the receipts were checked against every study in the dataset before anyone treated them as real conclusions.

---

## The receipts notebook

The receipts notebook in this project is called the rulebook. Every concept in it has three things:

**A name**: what we're calling this idea. `SignalPurity` is the name for how much of the apparent gap between treatments is real versus noise.

**A formula**: what it actually computes. Not a description — the exact recipe. "Take the real gap, divide it by the real gap plus the noise. The answer is a fraction between 0 and 1."

**A fact-check rule**: a test that must pass for the concept to be considered valid. For SignalPurity, the rule is: "Every study where the pooled result is backwards must have a SignalPurity below 0.5." That means in every study where mixing the groups gave you the wrong answer, the noise was doing more than half the work. If even one such study had SignalPurity above 0.5, something would be wrong with the formula.

That fact-check rule was tested against all 238 studies in the dataset. Every study where the pooled result was backwards had SignalPurity below 0.5. Zero exceptions. That's not a rumor — that's a receipt. The formula was checked, the rule held, and now `SignalPurity` is a concept the research can rely on.

Compare this to how most AI tools work. The model generates the name and an explanation of what it should measure. Then it moves on. Nobody runs the check. Nobody verifies that the formula holds on every real example. The name goes into a document, people start using it, and gradually it drifts away from what it was supposed to mean because there was never a receipt — just a confident-sounding description.

---

## The revision history

The project keeps a complete revision history of every step. There are 78 build iterations, each one recorded as a row in a table. Step 4 introduced the first version of the classification. Step 21 extended it to five types. Step 34 proved that the corrected estimate of the gap doesn't change when you shuffle how the patients are distributed.

Every step was witnessed and recorded. You can look at step 17 and see exactly what was known at that point: which studies showed the paradox, what the numbers were, and what question was still unanswered. This is like the edit history on a shared document — but for a scientific conclusion. You can see not just what the project currently believes, but what it believed at every earlier point and why it changed.

This matters because good reasoning is not just about reaching the right answer. It is about being able to show how you got there, what you checked along the way, and what you were wrong about before. The revision history is the receipts notebook for the reasoning process itself.

---

## Three grades of knowledge

Not all conclusions are equally solid, and a good reasoning system should know the difference. This project grades conclusions explicitly into three categories.

**Grade A: Theorem** — proven from the definition by pure logic. Example: "The corrected estimate of the treatment gap is the same no matter how patients happen to be distributed between groups." This is mathematically provable from the formula. You don't need more studies to be certain. Adding a thousand more studies will not make it false. It is as solid as 2+2=4.

**Grade B: Discovery finding** — tested against real data, but with the prediction written down before looking at the data. Example: "We predicted that economics studies would show no cases of the paradox. We then looked. Confirmed." This is strong evidence. It was committed before the evidence was collected, which means it wasn't reverse-engineered from the answer. But it is not a theorem — a bigger dataset with different economics studies could theoretically show a different pattern.

**Grade C: Old observation** — something that appeared to be true with fewer studies and was revised when more data was added. The project keeps these labeled and archived, not deleted. "We thought this type of study was rare at 64 studies. At 238 studies, the picture was more complex." An honest system remembers what it used to believe and when it changed its mind.

Most AI output has no grades. Every sentence comes out at the same apparent confidence level — the model's consistent, measured tone makes theorems and guesses sound identical. This project makes the grades explicit so that anyone reading the conclusions knows what kind of claim they are looking at.

---

## Reasoning is showing your work

The easiest way to sound smart is to be confident. Confident people are persuasive. Confident AI systems are even more persuasive — they do not hesitate, they do not second-guess themselves, and they generate text that feels authoritative even when it has not been checked.

The hard thing — and the more important thing — is being able to show your work in a way that can be verified, and if necessary, corrected. That is what this project forces. Every conclusion is backed by a formula. Every formula is tested against real data. Every test result is recorded. Every revision is archived. If you want to know whether a finding is a theorem or a provisional observation, the information is in the record.

Reasoning is not just having an answer. Reasoning is being able to show how the answer survives checking — what rules it must satisfy, what data it was tested against, and what would have to be true for it to be wrong.

The goal is not to distrust AI. The goal is to build systems where the AI's proposals get checked the same way any other proposal gets checked — with specific tests, recorded results, and the willingness to say "this held in 238 cases and failed in zero" rather than "this sounds right to me."

---

**Also in this series:**
- *Part 1: A Public Guide to Auditable AI* — the same paradox, the courtroom analogy, and why AI outputs need a rules-of-evidence layer
- *Part 9: A Story for Children About Guessing and Checking* — the ideas in this article, told as a story for younger readers

*Full series overview: Trust the Artifact, Not the Autocomplete (10 articles)*

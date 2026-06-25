#!/usr/bin/env python3
"""Assemble effortless-rulebook.json for the Hitchens vs Turek debate.
Follows ERB schema: $schema, Name, Description, one key per table (Description/schema/data), _meta.
Conventions: PascalCase plural tables, Name = slug(Label) formula, 1-to-many FKs only, Description on every field."""
import json, collections

NAME_FORMULA = '=SUBSTITUTE(LOWER({{Label}}), " ", "-")'

def f(name, datatype, type_, nullable, desc, formula=None, related=None):
    d = {"name": name, "datatype": datatype, "type": type_, "nullable": nullable, "Description": desc}
    if formula: d["formula"] = formula
    if related: d["RelatedTo"] = related
    return d

def name_field():
    return f("Name", "string", "calculated", False,
             "Logical primary key — slug of Label.", formula=NAME_FORMULA)

def label_field(desc):
    return f("Label", "string", "raw", False, desc)

rb = {
    "$schema": "https://example.com/cmcc-schema/v1",
    "Name": "Does God Exist — Hitchens vs Turek (VCU)",
    "Description": ("Rulebook formalizing the VCU debate 'Does God Exist?' between Christopher "
                    "Hitchens (atheism/anti-theism) and Frank Turek (theism), and every philosophical "
                    "concept raised. Built via the Shadle steps: raw text -> vocabulary -> glossary -> "
                    "narrative -> mock data -> normalized schema -> this DAG-structured hub."),
}

# ---------------- Debaters ----------------
rb["Debaters"] = {
 "Description": "The two principals and the moderator of the debate.",
 "schema": [
   label_field("Display name of the participant."), name_field(),
   f("Side","string","raw",False,"Role in the debate: affirmative, negative, or moderator."),
   f("Description","string","raw",True,"Biographical note and stance. (Worldviews defended are the reverse of Worldviews.ChampionedBy.)"),
   f("ArgumentCount","integer","aggregation",False,"1st-order: number of arguments advanced.",
     formula='=COUNTIFS(Arguments!{{AdvancedBy}}, {{Name}})'),
   f("ClaimCount","integer","aggregation",False,"1st-order: number of claims made.",
     formula='=COUNTIFS(Claims!{{MadeBy}}, {{Name}})'),
   f("ThinkersCited","integer","aggregation",False,"1st-order: distinct thinkers pressed into service.",
     formula='=COUNTIFS(Thinkers!{{CitedBy}}, {{Name}})'),
 ],
 "data": [
   {"Label":"Frank Turek","Side":"affirmative",
    "Description":"Christian apologist, founder of CrossExamined.org, co-author of 'I Don't Have Enough Faith to Be an Atheist'. Argues the affirmative for a generic theism."},
   {"Label":"Christopher Hitchens","Side":"negative",
    "Description":"Author of 'God Is Not Great'. Argues the negative; self-describes as anti-theist, not merely atheist."},
   {"Label":"Timothy Hulley","Side":"moderator",
    "Description":"Dean of the VCU Honors College; moderates phases and relays audience questions for the event hosted by the United Secular Alliance."},
 ]
}

# ---------------- Worldviews ----------------
rb["Worldviews"] = {
 "Description":"Competing positions on God and reality contested in the debate.",
 "schema":[
   label_field("Name of the worldview."), name_field(),
   f("AffirmsGod","boolean","raw",False,"Whether the worldview affirms the existence of God."),
   f("ChampionedBy","string","relationship",True,"Debater who principally champions this worldview.",related="Debaters"),
   f("Description","string","raw",True,"What the worldview holds in this debate's context."),
   f("ArgumentCount","integer","aggregation",False,"1st-order: arguments fielded under this worldview.",
     formula='=COUNTIFS(Arguments!{{FieldsWorldview}}, {{Name}})'),
 ],
 "data":[
   {"Label":"Theism","AffirmsGod":True,"ChampionedBy":"frank-turek","Description":"A personal God who created and intervenes in the universe, answers prayer, and grounds morality."},
   {"Label":"Deism","AffirmsGod":True,"Description":"A designer who set the cosmos in order but does not intervene or reveal itself; Hitchens treats it as separable from theism."},
   {"Label":"Atheism","AffirmsGod":False,"ChampionedBy":"christopher-hitchens","Description":"No advanced argument for God has survived rebuttal; not the dogmatic claim no God could exist."},
   {"Label":"Anti-theism","AffirmsGod":False,"ChampionedBy":"christopher-hitchens","Description":"Hitchens's stronger stance: relief that no celestial dictatorship exists, which he would oppose if it did."},
   {"Label":"Agnosticism","AffirmsGod":False,"Description":"Suspension of judgment; embodied by Jastrow, cited by Turek as a hostile witness for the Big Bang."},
   {"Label":"Materialism","AffirmsGod":False,"ChampionedBy":"christopher-hitchens","Description":"Only matter exists; mind, reason, and morality reduce to physical processes. Closely tied to naturalism; Turek's main target."},
   {"Label":"Humanism","AffirmsGod":False,"ChampionedBy":"christopher-hitchens","Description":"Ethics grounded in innate solidarity, empathy, and conscience without divine sanction."},
   {"Label":"Christianity","AffirmsGod":True,"ChampionedBy":"frank-turek","Description":"The specific tradition Turek personally holds but brackets in favor of generic theism for the debate."},
 ]
}

# ---------------- Arguments ----------------
arg = lambda label, atype, by, wv, concl, desc: {
   "Label":label,"ArgumentType":atype,"AdvancedBy":by,"FieldsWorldview":wv,"Conclusion":concl,"Description":desc}
rb["Arguments"] = {
 "Description":"The named arguments advanced by each side, with derived development and contestation metrics.",
 "schema":[
   label_field("Name of the argument."), name_field(),
   f("ArgumentType","string","raw",False,"primary, sub, supporting, or counter."),
   f("AdvancedBy","string","relationship",False,"Debater who advances the argument.",related="Debaters"),
   f("FieldsWorldview","string","relationship",False,"Worldview the argument serves.",related="Worldviews"),
   f("Conclusion","string","raw",True,"What the argument purports to establish."),
   f("Description","string","raw",True,"Summary of the argument as presented."),
   f("PremiseCount","integer","aggregation",False,"1st-order: premises composing the argument.",
     formula='=COUNTIFS(Premises!{{SupportsArgument}}, {{Name}})'),
   f("EvidenceCount","integer","aggregation",False,"1st-order: evidence items cited.",
     formula='=COUNTIFS(Evidence!{{SupportsArgument}}, {{Name}})'),
   f("ClaimCount","integer","aggregation",False,"1st-order: claims supporting the argument.",
     formula='=COUNTIFS(Claims!{{SupportsArgument}}, {{Name}})'),
   f("IsFullyDeveloped","boolean","calculated",False,"2nd-order: has at least two premises.",
     formula='=IF({{PremiseCount}} >= 2, TRUE(), FALSE())'),
   f("TotalRebuttals","integer","aggregation",False,"2nd-order: rebuttals accumulated across supporting claims.",
     formula='=SUMIFS(Claims!{{RebuttalCount}}, Claims!{{SupportsArgument}}, {{Name}})'),
   f("IsContested","boolean","calculated",False,"3rd-order: at least one supporting claim was rebutted.",
     formula='=IF({{TotalRebuttals}} > 0, TRUE(), FALSE())'),
 ],
 "data":[
   arg("Cosmological Argument","primary","frank-turek","theism","The universe had a beginning, so it had a beginner.","From the Big Bang: a beginning implies a beginner; SURGE supplies the evidence."),
   arg("Teleological Argument","primary","frank-turek","theism","Order and apparent purpose imply a designer.","The design argument; contains fine-tuning and design-of-life as sub-arguments."),
   arg("Argument From Fine-Tuning","sub","frank-turek","theism","Life-permitting constants imply a tuner.","Constants on a razor's edge (the gravitational force to 1 part in 10^40, expansion rate, axial tilt, Jupiter)."),
   arg("Argument From Design Of Life","sub","frank-turek","theism","DNA's specified complexity implies a mind.","Information only comes from minds; random mutation and macro-evolution cannot supply first life; panspermia concedes natural origin fails."),
   arg("Moral Argument","primary","frank-turek","theism","Objective moral values require a moral lawgiver.","A prescription needs a prescriber; the standard is God's nature."),
   arg("Argument From Reason","supporting","frank-turek","theism","Trustworthy reason cannot be mere chemistry.","If thoughts are only chemical reactions, we cannot trust them (Darwin's doubt)."),
   arg("Argument From Mathematics","supporting","frank-turek","theism","Mathematical order comes from mind, not matter.","Rationality and mathematics do not arise from randomness."),
   arg("Argument From Free Will","supporting","frank-turek","theism","Real freedom is impossible under materialism.","Molecules in motion (per Provine) afford no genuine human freedom."),
   arg("Argument From Consciousness","supporting","frank-turek","theism","Consciousness cannot reduce to chemicals.","Why is some carbon-based matter conscious? Materialism (Dennett) calls it an illusion."),
   arg("Argument From Poor Design","counter","christopher-hitchens","atheism","Waste and ruin suggest capricious or absent design.","Extinction, collision courses, heat death — not the marks of a careful tuner."),
   arg("Deism-Theism Gap","counter","christopher-hitchens","atheism","A designer is not yet a prayer-answering God.","Even granting design, the theist still owes virgin birth, resurrection, vicarious redemption."),
   arg("Morality Without God","counter","christopher-hitchens","humanism","Ethics needs no divine permission.","No moral act is unique to believers; some wicked acts are unique to them; conscience is innate."),
   arg("Religion Poisons Everything","counter","christopher-hitchens","anti-theism","Religion is a corrupting totalitarian tyranny.","Its eschatology yearns for the world's end; it demands a master-slave submission."),
 ]
}

# ---------------- Premises ----------------
prem = lambda label, a, n, stmt: {"Label":label,"SupportsArgument":a,"Ordinal":n,"Statement":stmt,
    "Description":"Premise %d of the %s." % (n, a.replace('-',' '))}
rb["Premises"] = {
 "Description":"Ordered premises composing each argument.",
 "schema":[
   label_field("Short name of the premise."), name_field(),
   f("SupportsArgument","string","relationship",False,"Argument this premise belongs to.",related="Arguments"),
   f("Ordinal","integer","raw",False,"Position of the premise within its argument."),
   f("Statement","string","raw",True,"The premise as stated."),
   f("Description","string","raw",True,"Note on the premise."),
 ],
 "data":[
   prem("Universe Began","cosmological-argument",1,"The universe had a beginning (SURGE)."),
   prem("Beginning Needs Beginner","cosmological-argument",2,"Whatever begins to exist has a cause/beginner."),
   prem("Cause Is Transcendent","cosmological-argument",3,"The creator-cause is spaceless, timeless, immaterial, intelligent, powerful, and personal."),
   prem("Fine-Tuning Observed","argument-from-fine-tuning",1,"Physical constants are set on a razor's edge."),
   prem("Chance Insufficient","argument-from-fine-tuning",2,"Such precision is not plausibly due to chance."),
   prem("Tuner Inferred","argument-from-fine-tuning",3,"A fine-tuner best explains the balance."),
   prem("Life Carries Information","argument-from-design-of-life",1,"DNA encodes specified complexity."),
   prem("Information From Minds","argument-from-design-of-life",2,"Information only comes from minds."),
   prem("Designer Inferred","argument-from-design-of-life",3,"A mind best explains biological information."),
   prem("Objective Values Exist","moral-argument",1,"Objective moral values and duties exist."),
   prem("Values Need Lawgiver","moral-argument",2,"Objective values require a standard beyond humanity."),
   prem("Lawgiver Is God","moral-argument",3,"That standard is the moral lawgiver, God's nature."),
   prem("Atrocity From Permission","morality-without-god",1,"The worst atrocities flow from felt divine permission."),
   prem("No Unique Believer Virtue","morality-without-god",2,"No moral act is available only to believers."),
   prem("Conscience Is Innate","morality-without-god",3,"Conscience (Socrates, Smith) is innate, not granted."),
   prem("Designer Not Pray-Answerer","deism-theism-gap",1,"A designer need not answer prayer or perform miracles."),
   prem("Burden Remains","deism-theism-gap",2,"The theist must still bridge from designer to revealed God."),
   prem("Waste And Extinction","argument-from-poor-design",1,"Profuse creation is mostly wiped out; 99.8% of species are extinct."),
   prem("Capricious Or Absent","argument-from-poor-design",2,"Heat death and collisions imply capricious or no design."),
 ]
}

# ---------------- Evidence ----------------
ev = lambda label, a, kind, desc: {"Label":label,"SupportsArgument":a,"EvidenceKind":kind,"Description":desc}
rb["Evidence"] = {
 "Description":"Empirical and theoretical evidence items cited in support of arguments.",
 "schema":[
   label_field("Name of the evidence item."), name_field(),
   f("SupportsArgument","string","relationship",False,"Argument the evidence supports.",related="Arguments"),
   f("EvidenceKind","string","raw",False,"observational or theoretical."),
   f("Description","string","raw",True,"What the evidence is and who found it."),
 ],
 "data":[
   ev("SURGE: Second Law","cosmological-argument","theoretical","Second law of thermodynamics — the universe runs toward heat death, so it cannot be eternal."),
   ev("SURGE: Universe Expanding","cosmological-argument","observational","Hubble's 1929 discovery of expansion; reversed, it collapses to a singularity."),
   ev("SURGE: Radiation Echo","cosmological-argument","observational","Cosmic afterglow found by Penzias and Wilson; remnant heat of the Big Bang."),
   ev("SURGE: Galaxy Seeds","cosmological-argument","observational","COBE's one-part-in-100,000 temperature variations that let galaxies form."),
   ev("SURGE: Einstein Relativity","cosmological-argument","theoretical","General relativity makes space-time-matter co-relative, originating together."),
   ev("DNA Information","argument-from-design-of-life","observational","DNA's information content, likened to a thousand encyclopedias (citing Dawkins)."),
   ev("Red Shift Increasing","argument-from-poor-design","observational","Krauss: the Hubble red-light-shift rate is increasing; the universe dissipates at speed."),
 ]
}

# ---------------- Concepts ----------------
con = lambda label, cat, by, desc: {"Label":label,"Category":cat,"IntroducedBy":by,"Description":desc}
T,H = "frank-turek","christopher-hitchens"
rb["Concepts"] = {
 "Description":"Every philosophical concept raised in the debate, with category and introducer.",
 "schema":[
   label_field("Name of the concept."), name_field(),
   f("Category","string","raw",False,"cosmology, design, moral, theological, method, or epistemic."),
   f("IntroducedBy","string","relationship",True,"Debater who introduced the concept.",related="Debaters"),
   f("Description","string","raw",True,"Meaning of the concept in this debate."),
   f("ClaimCount","integer","aggregation",False,"1st-order: claims that touch this concept.",
     formula='=COUNTIFS(Claims!{{TouchesConcept}}, {{Name}})'),
 ],
 "data":[
   con("Big Bang","cosmology",T,"The accepted origin of the universe; Turek accepts it and infers a 'banger'."),
   con("Heat Death","cosmology",T,"The far-future maximal-entropy state; used by both (Turek vs eternity, Hitchens for poor design)."),
   con("Singularity","cosmology",T,"The point reversed expansion collapses to; Turek equates it with 'nothing'."),
   con("Fine-Tuning","design",T,"Constants on a razor's edge permitting life."),
   con("Specified Complexity","design",T,"Information in living systems Turek says only minds produce."),
   con("Watchmaker","design",H,"Paley's image: design implies a designer."),
   con("Panspermia","design",T,"Crick/Hoyle's seeded-life proposal; Turek calls it a backhanded concession."),
   con("Objective Moral Values","moral",T,"Mind-independent moral facts at the center of the moral exchange."),
   con("Moral Lawgiver","moral",T,"The prescriber Turek infers from the prescription of morality."),
   con("Is-Ought Problem","epistemic",H,"Hume's bar on deriving an 'ought' from an 'is'."),
   con("Divine Permission","moral",H,"Felt divine authorization Hitchens links to atrocity."),
   con("Conscience","moral",H,"Innate moral sense (Socrates's daemon, Smith's internal witness)."),
   con("Empathy","moral",H,"Biologically grounded basis for morality raised in audience Q&A."),
   con("Vicarious Redemption","theological",H,"Salvation by another's suffering through human sacrifice; Hitchens's central moral objection."),
   con("Scapegoating","theological",H,"Casting sins onto a substitute; called primitive and barbaric."),
   con("Miracle","theological",H,"A suspension of natural order (the regular course of nature), per Hume."),
   con("Virgin Birth","theological",H,"Parthenogenesis: a miracle a design argument can never reach, even if a male-free pregnancy could be tracked."),
   con("Resurrection","theological",H,"Hitchens notes it was 'commonplace' in the gospels."),
   con("Eschatology","theological",H,"End-times doctrine; the rapture, Armageddon, the apocalypse, a 'cult of death' set against a promised new Heaven and new earth."),
   con("Mind-Forged Manacles","theological",H,"Blake's phrase for self-imposed religious tyranny (limbo, hell)."),
   con("Celestial Dictatorship","theological",H,"Theism framed as totalitarian, master-slave submission — the oldest enemy of our moral autonomy."),
   con("Materialism","method",T,"Only matter exists; Turek's target across the four supporting arguments."),
   con("Free Will","method",T,"Genuine choice Turek says materialism cannot supply."),
   con("Consciousness","method",T,"Subjective experience Turek says chemicals cannot explain."),
   con("Occam's Razor","method",H,"Dispose of needless assumptions — Hitchens removes the prime mover."),
   con("Hume On Miracles","method",H,"Error is always likelier than a suspension of nature in one's favor."),
   con("Non-Overlapping Magisteria","method",H,"Gould's thesis that science and religion address separate domains."),
   con("Burden Of Proof","method",T,"Who must establish the claim; both accept a share."),
   con("Ontological Argument","method",H,"To conceive a thing is for it to be 'real in the mind' (parodied via limbo)."),
   con("Borrowing From Theism","moral",T,"Turek's charge: Hitchens must 'sit in God's lap to slap his face'."),
   con("Problem Of Evil","theological",H,"Implicit in 'created sick, commanded to be well' — suffering counts against a good designer."),
   con("Choice Argument","cosmology",T,"Turek's claim that moving from non-existence to existence requires a choice, and only persons choose."),
   con("Divine Command","moral",T,"The divine-command standard: right and wrong fixed by God's nature and command."),
   con("Naturalism","method",H,"Natural causes suffice to explain the world; every origin question now has a natural or sufficient explanation."),
   con("Determinism","method",T,"Physical law fixes all events (Einstein, Provine's Darwinism); Turek says it undercuts trust in reasoning."),
   con("Probability Argument","epistemic",T,"Turek's cross-examination framing: not a proof of God but a cumulative case that God is the more probable explanation."),
   con("Irreducible Complexity","design",T,"Raised in audience Q&A: whether such biological structures could have formed by chance."),
   con("Prime Mover","method",H,"The first-cause designer Hitchens applies Occam's razor to remove — 'we don't need the prime mover at all'."),
   con("Optimal Design","design",T,"Turek's closing: design need not be optimal; you can't call it suboptimal without knowing the designer's purpose (vs. Gould's panda's thumb)."),
   con("Event Horizon","cosmology",H,"Hawking's 'lip of the black hole'; Hitchens uses it to show our God-talk is the vocabulary of our infancy."),
   con("Andromeda Collision","cosmology",H,"The Andromeda galaxy on a direct collision course — Hitchens's exhibit for capricious or absent design."),
   con("Termination Of Pregnancy","moral",T,"The abortion exchange: Turek presses Hitchens's book on the unborn child / fetus; embryology and early viability are invoked."),
   con("Contraception","moral",H,"Hitchens calls it nonsensical and immoral for the Church to equate contraception with abortion; cites the ectopic pregnancy as a humanist no-starter."),
   con("Good Samaritan","moral",H,"The Samaritan helped without religious instruction and predates Christianity — morality 'written on your heart'."),
   con("Parable Of The Talents","theological",T,"Turek's counter that Jesus did not simply preach 'take no thought for tomorrow'."),
   con("Original Sin","theological",T,"'We were well in the beginning but then we messed up'; the Fall, which Hitchens parodies as 'created sick, commanded to be well'."),
   con("Atonement","theological",T,"Christ's sacrifice is retroactive to everyone before him, so God 'always had a witness' (creation, conscience, Christ)."),
   con("Theocracy","theological",H,"Hitchens's post-9/11 cause: opposing theocracy and its messianic, end-times depredations internationally."),
   con("Amalekites Genocide","theological",H,"God's Old Testament injunction to Moses that too many Amalekite children were spared — Hitchens's exhibit of divinely-commanded atrocity, not equivalent to a hard medical decision."),
   con("Suicide Bombing","moral",H,"Wickedness Hitchens says is characteristically religious, alongside genital mutilation and circumcision dictated 'in God's name'."),
   con("Enlightenment Values","method",H,"The post-religious values Turek says Hitchens wants all to adopt — which Turek reframes as 'the dictatorship of Christopher Hitchens'."),
   con("Solipsism","epistemic",H,"Religion's appeal to self-regard — wanting to be objects of a divine plan — likened to a quick peek at one's horoscope."),
 ]
}

# ---------------- Claims ----------------
clm = lambda label, by, phase, sup, con_, desc: {"Label":label,"MadeBy":by,"Phase":phase,
    "SupportsArgument":sup,"TouchesConcept":con_,"Description":desc}
rb["Claims"] = {
 "Description":"Specific assertions made by each debater, with derived rebuttal status.",
 "schema":[
   label_field("Short name of the claim."), name_field(),
   f("MadeBy","string","relationship",False,"Debater who made the claim.",related="Debaters"),
   f("Phase","string","raw",False,"opening, rebuttal, cross-examination, or closing."),
   f("SupportsArgument","string","relationship",True,"Argument the claim supports.",related="Arguments"),
   f("TouchesConcept","string","relationship",True,"Primary concept the claim engages.",related="Concepts"),
   f("Description","string","raw",True,"The claim in brief."),
   f("RebuttalCount","integer","aggregation",False,"1st-order: rebuttals directed at this claim.",
     formula='=COUNTIFS(Rebuttals!{{RebutsClaim}}, {{Name}})'),
   f("IsRebutted","boolean","calculated",False,"2nd-order: the claim drew at least one rebuttal.",
     formula='=IF({{RebuttalCount}} > 0, TRUE(), FALSE())'),
 ],
 "data":[
   clm("Beginning Implies Beginner",T,"opening","cosmological-argument","big-bang","The Big Bang's beginning requires a transcendent beginner."),
   clm("Constants Are Fine-Tuned",T,"opening","argument-from-fine-tuning","fine-tuning","Life-permitting constants sit on a razor's edge, implying a tuner."),
   clm("Information Implies Mind",T,"opening","argument-from-design-of-life","specified-complexity","DNA's specified complexity could only come from a mind."),
   clm("Objective Morality Needs God",T,"opening","moral-argument","moral-lawgiver","Objective moral values require a moral lawgiver beyond humanity."),
   clm("Materialism Defeats Reason",T,"opening","argument-from-reason","materialism","If thought is mere chemistry, reasoning cannot be trusted."),
   clm("Consciousness Defies Chemistry",T,"opening","argument-from-consciousness","consciousness","No materialist account explains why matter is conscious."),
   clm("Hitchens Borrows Morality",T,"rebuttal","moral-argument","borrowing-from-theism","Hitchens must assume God's moral standard to argue against God."),
   clm("Design Need Not Be Optimal",T,"closing","teleological-argument","fine-tuning","Suboptimal design is still design; you can't judge it without the designer's purpose."),
   clm("Everyone Is An Atheist",H,"opening","deism-theism-gap","ontological-argument","Everyone disbelieves in thousands of gods; the theist must justify the one exception."),
   clm("Designer Is Not Enough",H,"opening","deism-theism-gap","watchmaker","Even a designer doesn't yield answered prayer, virgin birth, or resurrection."),
   clm("Atrocity From Divine Permission",H,"opening","morality-without-god","divine-permission","The most brutal acts come from believing one has divine permission."),
   clm("No Virtue Unique To Believers",H,"opening","morality-without-god","conscience","No moral act exists that only a believer could perform."),
   clm("Fine-Tuning Cuts Both Ways",H,"opening","argument-from-poor-design","heat-death","Accelerating dissipation and extinction suggest capricious or absent design."),
   clm("Miracles Fail Humes Test",H,"cross-examination","deism-theism-gap","hume-on-miracles","A reported miracle is always less likely than the witness being mistaken."),
   clm("Vicarious Redemption Is Immoral",H,"cross-examination","morality-without-god","vicarious-redemption","One may pay a debt but cannot assume another's moral responsibility."),
   clm("Religion Poisons Everything",H,"closing","religion-poisons-everything","eschatology","Religion demands servile submission and yearns for the end of the world."),
   clm("Abortion Is A Double Standard",T,"cross-examination","moral-argument","termination-of-pregnancy","If God taking a life prematurely is an outrage, why is Hitchens taking one via abortion a moral right?"),
 ]
}

# ---------------- Rebuttals ----------------
reb = lambda label, by, claim, desc: {"Label":label,"MadeBy":by,"RebutsClaim":claim,"Description":desc}
rb["Rebuttals"] = {
 "Description":"Direct responses aimed at specific claims.",
 "schema":[
   label_field("Short name of the rebuttal."), name_field(),
   f("MadeBy","string","relationship",False,"Debater making the rebuttal.",related="Debaters"),
   f("RebutsClaim","string","relationship",False,"Claim being rebutted.",related="Claims"),
   f("Description","string","raw",True,"The rebuttal in brief."),
 ],
 "data":[
   reb("Cause Need Not Choose",H,"beginning-implies-beginner","'I don't have to know' the origin; positing a choosing being just relabels the mystery."),
   reb("Accelerating Expansion",H,"constants-are-fine-tuned","Krauss: the red-shift rate is increasing — the cosmos is flying apart, hardly careful tuning."),
   reb("Information Argument Is Faith",H,"information-implies-mind","The origin-of-life quotes are all statements of uncertainty, not evidence of a mind."),
   reb("Morality Predates Atoms",H,"objective-morality-needs-god","The question of the good predates Lucretius, Democritus, and Epicurus and the atomic theory; it needs no deity."),
   reb("I Am A Primate And Still Reason",H,"materialism-defeats-reason","Being a primate doesn't bar reasoning about heroism and self-sacrifice."),
   reb("Borrowing Charge Is False",H,"hitchens-borrows-morality","Conscience is innate (Socrates, Smith); no borrowing from theism is required."),
   reb("Optimality Objection Misfires",H,"design-need-not-be-optimal","Profuse extinction and a heat-death trajectory are not what design predicts."),
   reb("Religious People Aren't The Point",T,"religion-poisons-everything","Bad believers say nothing about whether God exists; everything poisons religion."),
   reb("Hume's Premise Is Wrong",T,"miracles-fail-humes-test","Hume's 'regular beats rare' rule would deny the Big Bang and one's own birth — singular events happen."),
   reb("Not All Pardons Are Immoral",H,"vicarious-redemption-is-immoral","Hitchens clarifies: not all pardons — only assuming another's responsibility — are immoral."),
   reb("Nature Is The Greater Abortion Provider",H,"abortion-is-a-double-standard","A false distinction: nature, not God, ends most pregnancies; the unborn has a real claim, but that is no moral equivalent to a commanded genocide."),
 ]
}

# ---------------- Thinkers ----------------
thk = lambda label, field_, era, by, hostile, desc: {"Label":label,"Field":field_,"Era":era,
    "CitedBy":by,"CitedAsHostileWitness":hostile,"Description":desc}
rb["Thinkers"] = {
 "Description":"Authorities invoked by each side, including hostile witnesses.",
 "schema":[
   label_field("Name of the thinker."), name_field(),
   f("Field","string","raw",True,"Primary field (physics, biology, philosophy, etc.)."),
   f("Era","string","raw",True,"Rough period."),
   f("CitedBy","string","relationship",True,"Debater who invokes them.",related="Debaters"),
   f("CitedAsHostileWitness","boolean","raw",False,"Cited against their own sympathies (e.g., an atheist conceding fine-tuning)."),
   f("Description","string","raw",True,"Why they appear in the debate."),
   f("QuotationCount","integer","aggregation",False,"1st-order: quotations attributed to them.",
     formula='=COUNTIFS(Quotations!{{Speaker}}, {{Name}})'),
 ],
 "data":[
   thk("Edwin Hubble","physics","20th c.",T,False,"Discovered cosmic expansion (1929)."),
   thk("Arno Penzias","physics","20th c.",T,False,"Co-discovered the radiation echo; quoted on a balanced, supernatural-seeming universe."),
   thk("Robert Wilson","physics","20th c.",T,False,"Co-discovered the cosmic afterglow at Bell Labs."),
   thk("George Smoot","physics","20th c.",T,False,"Led the COBE result; quoted comparing it to looking at God."),
   thk("Stephen Hawking","physics","20th-21st c.",H,False,"Invoked by both; Hitchens warns against drafting him as a deist."),
   thk("Robert Jastrow","astronomy","20th c.",T,True,"Agnostic astronomer; Turek's hostile witness for an abrupt creation."),
   thk("Arthur Eddington","physics","20th c.",T,False,"Called the cosmic beginning 'frankly supernatural'."),
   thk("Albert Einstein","physics","20th c.",T,False,"General relativity; a determinist invoked on both sides."),
   thk("Aristotle","philosophy","ancient",T,False,"Defined nothing as 'what rocks dream about'."),
   thk("Richard Dawkins","biology","contemporary",T,True,"Hostile witness on DNA's information content."),
   thk("Francis Crick","biology","20th c.",T,True,"Co-discoverer of DNA; floated panspermia."),
   thk("Fred Hoyle","astronomy","20th c.",T,True,"Coined 'Big Bang'; spoke of a superintellect monkeying with physics."),
   thk("Chandra Wickramasinghe","astronomy","contemporary",T,True,"Called primordial-soup belief 'an article of faith'."),
   thk("Antony Flew","philosophy","contemporary",T,False,"Prominent atheist turned deist over DNA evidence."),
   thk("Steven Weinberg","physics","contemporary",T,True,"Atheist physicist on the delicate balance required for life."),
   thk("William Provine","biology","contemporary",T,True,"Materialist Darwinist who denies free will."),
   thk("Daniel Dennett","philosophy","contemporary",T,True,"Materialist who treats consciousness as an illusion."),
   thk("Charles Darwin","biology","19th c.",H,False,"Natural selection; Hitchens's 'greater emancipator'."),
   thk("William Paley","philosophy","18th-19th c.",H,False,"The watchmaker analogy: design implies a designer."),
   thk("Thomas Aquinas","philosophy","medieval",H,False,"Source of the ontological move Hitchens parodies."),
   thk("Thomas Jefferson","statesmanship","18th c.",H,False,"Author of Virginia's statute on religious freedom; a deist."),
   thk("David Hume","philosophy","18th c.",H,False,"On miracles and the is-ought problem."),
   thk("Socrates","philosophy","ancient",H,False,"His inner daemon as a model of conscience."),
   thk("Adam Smith","philosophy","18th c.",H,False,"The 'internal witness' in Theory of Moral Sentiments."),
   thk("C. S. Lewis","theology","20th c.",H,False,"Attributed conscience to the divine; Hitchens says without improving on Smith."),
   thk("Lawrence Krauss","physics","contemporary",H,False,"On the increasing red-shift rate."),
   thk("Stephen Jay Gould","biology","20th c.",H,False,"Panda's thumb (poor design) and non-overlapping magisteria."),
   thk("Georges Lemaitre","physics","20th c.",H,False,"Catholic physicist who first proposed the Big Bang."),
   thk("H. L. Mencken","letters","20th c.",H,False,"Listed thousands of dead gods in Hitchens's anthology."),
   thk("Fyodor Dostoevsky","literature","19th c.",H,False,"Brothers Karamazov; his character Smerdyakov voices the 'everything is permitted' line."),
   thk("Blaise Pascal","philosophy","17th c.",H,False,"Addressed those 'so made that they cannot believe'."),
   thk("Lucretius","philosophy","ancient",H,False,"Atomic theory predating the morality debate; cited with Democritus and Epicurus."),
   thk("Francis Collins","biology","contemporary",H,False,"Supervisor of the Human Genome Project; Hitchens's 'occasional debating enemy', invoked on the age of Homo sapiens."),
   thk("William Gladstone","statesmanship","19th c.",H,False,"Scholar of Latin and Greek; cited as having shown Greek moral precepts as 'prefigurations of Christianity'."),
   thk("Omar Khayyam","literature","medieval",H,False,"Persian poet whose quatrain Hitchens borrows to ask Turek 'what is your authority for saying you know something I don't?'."),
 ]
}

# ---------------- Quotations (paraphrased gists only) ----------------
quo = lambda label, sp, by, gist: {"Label":label,"Speaker":sp,"CitedBy":by,"Gist":gist,
    "Description":"Paraphrased citation used in the debate (not verbatim)."}
rb["Quotations"] = {
 "Description":"Citations used by debaters, stored as paraphrased gists to respect copyright.",
 "schema":[
   label_field("Short name of the quotation."), name_field(),
   f("Speaker","string","relationship",False,"Thinker the quotation is attributed to.",related="Thinkers"),
   f("CitedBy","string","relationship",False,"Debater who cited it.",related="Debaters"),
   f("Gist","string","raw",True,"Paraphrased substance of the quotation."),
   f("Description","string","raw",True,"Context note."),
 ],
 "data":[
   quo("Smoot On Looking At God","george-smoot",T,"Paraphrase: a religious person might see the COBE result as a glimpse of the divine."),
   quo("Jastrow's Mountain Of Ignorance","robert-jastrow",T,"Paraphrase: the scientist scaling the peak finds theologians already seated there."),
   quo("Eddington On The Beginning","arthur-eddington",T,"Paraphrase: the cosmic beginning seems frankly supernatural."),
   quo("Hoyle's Superintellect","fred-hoyle",T,"Paraphrase: a superintellect appears to have arranged physics, chemistry, and biology."),
   quo("Wickramasinghe On Faith","chandra-wickramasinghe",T,"Paraphrase: belief in life from a primordial soup is an article of faith."),
   quo("Dostoevsky's Permission","fyodor-dostoevsky",H,"Paraphrase: without God, everything is permitted — voiced by the story's basest character."),
   quo("Hume On Probability","david-hume",H,"Paraphrase: weigh whether nature was suspended for you, or whether you simply erred."),
   quo("Mencken's Dead Gods","h.-l.-mencken",H,"Paraphrase: a long catalogue of gods once worshipped and now believed by no one."),
   quo("Khayyam On Authority","omar-khayyam",H,"Paraphrase: would God give a secret to a fanatic crew and deny it to me? — borrowed to press Turek on his authority."),
 ]
}

# ---------------- __meta__ (first-class project-metadata table; transpiler-ignored) ----------------
def meta_row(key, vtype, sval=None, jval=None):
    return {"MetaKey":key, "ValueType":vtype, "StringValue":sval, "JsonValue":jval}
rb["__meta__"] = {
 "Description":"Project-level metadata for the debate rulebook (presentation hints + provenance). Typed-row key/value bag; ignored by execution-substrate transpilers.",
 "schema":[
   f("MetaKey","string","raw",False,"Logical primary key — the metadata key."),
   f("Name","string","calculated",False,"Mirrors MetaKey.",formula='={{MetaKey}}'),
   f("ValueType","string","raw",False,"One of: string, object, array."),
   f("StringValue","string","raw",True,"Value when ValueType is string."),
   f("JsonValue","string","raw",True,"JSON-encoded value when ValueType is object or array."),
 ],
 "data":[
   meta_row("tagline","string","Does God exist? — Christopher Hitchens vs. Frank Turek at VCU, modeled as a DAG."),
   meta_row("source","string","VCU 'Does God Exist?' debate transcript (Hitchens vs Turek), bootstrap/raw-input.txt."),
   meta_row("method","string","effortless-bootstrap (Shadle steps) 1-10, Rulebook-First."),
   meta_row("tool_version","string","shadle-manual-1.1"),
   meta_row("dag_depth","string","3"),
   meta_row("copyright_note","string","Quotations stored as paraphrased gists, never long verbatim."),
   meta_row("motif","string","debate / philosophy of religion"),
 ]
}

rb["_meta"] = {
 "_CMCC_Summary":"Shadle-steps bootstrap of a philosophical debate into a DAG-structured rulebook hub.",
 "_conversion_metadata":{
   "source":"VCU 'Does God Exist?' debate transcript (Hitchens vs Turek)",
   "method":"effortless-bootstrap (Shadle steps) 1-10, Rulebook-First",
   "table_count":11,
   "tool_version":"shadle-manual-1.1",
   "dag_depth":3,
   "field_type_mapping":"raw, calculated, relationship, aggregation",
   "copyright_note":"Quotations stored as paraphrased gists, never long verbatim."
 }
}

import os
# Write to this project's rulebook hub, resolved relative to this script (bootstrap/ -> ../effortless-rulebook/).
out = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "effortless-rulebook", "effortless-rulebook.json")
out = os.path.normpath(out)
os.makedirs(os.path.dirname(out), exist_ok=True)
with open(out,"w") as fh:
    json.dump(rb, fh, indent=2, ensure_ascii=False)

# quick stats
tables = [k for k in rb if k not in ("$schema","Name","Description","_meta")]
print("Tables:", len(tables))
for t in tables:
    print(f"  {t}: {len(rb[t]['schema'])} fields, {len(rb[t]['data'])} rows")
print("Wrote", out)

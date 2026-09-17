"""Evidence for theme G (loop-12): the social side of collection.

(kind, target, justification). Validity is computed by ClaimEvidence.IsValid from Postgres
measurements; nothing here asserts coverage by itself.
"""


def fld(target, why):
    return ("Field", target, why)


def tbl(target, why):
    return ("Table", target, why)


def q(target, why):
    return ("RoleQuestion", target, why)


EVIDENCE = {
    # ---------------------------------------------------------------- pkm-1
    "pkm1-c16": [fld("KnowledgeTransfers.IsTraditionalChannel",
                     "TRUE for a recorded transfer that went through a document, a training program or mentoring: the release notes "
                     "Grace documented for Leo, the lockout training Lin gave Aisha, and Walter's and Tomas's mentoring.")],
    "pkm1-c35": [tbl("KnowledgeWorkforcePositions",
                     "Each row is a position an organization budgets specifically for knowledge engineering, information architecture "
                     "or ontology work: roles devoted to managing process knowledge.")],
    "pkm1-c36": [tbl("SharingRecognitions",
                     "Each row is an incentive given for sharing knowledge: the plant's teaching award to Tomas for training apprentices "
                     "and a cluster peer award to Joao for spreading servo timing practice.")],
    "pkm1-p08": [fld("Procedures.HasDecayedTransferChannel",
                     "Detects both kinds of decay the claim names: TRUE when a procedure's repository entry is past its review interval or "
                     "some of its know-how is held only by someone who left with nothing transferred or captured. It fires on production "
                     "deployment (stale go/no-go notes and a departed SRE's health-check judgment) and customer incident response "
                     "(a departed incident lead's triage heuristics), and is FALSE on the other eight procedures.")],
    "pkm1-p36": [fld("Procedures.HasUnengagedStakeholder",
                     "TRUE when a stakeholder department of a procedure was never engaged in shaping its knowledge. It fires on production "
                     "deployment, whose compliance stakeholder was never engaged, and is FALSE on lockout/tagout, where maintenance and "
                     "production were both engaged; an unengaged stakeholder is exactly the breach the prescription forbids.")],
    "pkm1-p37": [fld("KnowledgeRepositoryEntries.IsUncreditedExpertKnowHow",
                     "TRUE when an entry records another person's expertise without crediting them. It fires on the lockout quick "
                     "reference card Lin wrote from Ken's know-how without naming him, and is FALSE on entries that credit their source "
                     "or record the author's own expertise.")],
    "pkm1-p38": [fld("DepartmentProcessAccounts.IsUnresolvedDisagreement",
                     "TRUE when two departments give contradicting accounts of the same process and no resolution practice was used. "
                     "Engineering's and customer support's accounts of deployment rollback fire; the maintenance/production conflict over "
                     "bleed time, settled by a timed joint walk-down, does not.")],
    "pkm1-p39": [fld("Organizations.MemoryLeavesWithStaff",
                     "TRUE when some know-how of people who left was neither transferred nor captured, so the organization's memory did "
                     "not stay intact through turnover. acme-engineering fires (two of three departed carriers' know-how lost); "
                     "acme-plant does not, because Walter's die-shim know-how passed to Ken before he retired.")],
    # ---------------------------------------------------------------- pkm-2: concepts
    "pkm2-c32": [fld("Agents.IsBoundarySpanner",
                     "TRUE for a person who belongs to two or more communities of practice: Lin (maintenance guild and portal community), "
                     "Ken (maintenance guild and Riverbend cluster) and Joao (Riverbend and Eastvale clusters).")],
    "pkm2-c33": [fld("Agents.LocatedKnowHowCount",
                     "Counts know-how held by someone else that a broker can point seekers to. Lin knows that the Press 7 bleed-down know-how "
                     "lives with Tomas without holding it herself: meta-knowledge of where process knowledge sits.")],
    "pkm2-c34": [fld("CommunitiesOfPractice.HasOwnVocabularyAndNorms",
                     "TRUE for a community that speaks its own vocabulary and states its own norms: the maintenance guild (lockout "
                     "vocabulary; deviations talked through at the Friday huddle) and the release guild (release vocabulary; a written note "
                     "for every rollback).")],
    "pkm2-c35": [fld("KnowledgeBrokerLinks.TranslationBetweenVocabularies",
                     "The broker's translation between the words of the two communities a link connects, e.g. production's 'line stop' "
                     "(plant quality vocabulary) = maintenance's 'energy isolation' (lockout vocabulary), and support's 'outage' = release "
                     "engineering's 'rollback'.")],
    "pkm2-c36": [fld("SourceRelationships.WithholdingMotive",
                     "Records why a source holds information back: Ken fears documenting his die-shim tricks makes him replaceable (job "
                     "security); Grace guards the go/no-go judgment that gives her release authority (status).")],
    "pkm2-c37": [fld("DepartmentProcessAccounts.ShapingInterest",
                     "Each department's account of a process carries the interest that shapes it: maintenance's twenty-minute bleed is shaped "
                     "by technician safety, production's five minutes by uptime targets; engineering's ten-minute rollback by release velocity, "
                     "support's hour of errors by incident load.")],
    "pkm2-c38": [fld("SourceRelationships.TrustLevel",
                     "The state of trust between the knowledge engineer and each source: Established with Tomas and Lin, Building with Grace, "
                     "Low with Ken.")],
    "pkm2-c39": [fld("SourceRelationships.PowerDynamic",
                     "The power dynamic shaping what each source shares: Tomas outranks the engineer, Grace's sessions had a supervisor "
                     "present, Ken's knowledge is threatened by automation, Lin's relationship is balanced.")],
    # ---------------------------------------------------------------- pkm-2: prescriptions, questions, standard
    "pkm2-p15": [fld("KnowHowCarriers.IsOverlookedLivingHolder",
                     "TRUE when uncaptured know-how is carried by someone still in a role whom no collection relationship has approached on "
                     "that procedure. It fires on Tomas's improvised press setup, Ken's belt-drift diagnosis and Lin's forklift checks, and is "
                     "FALSE on Tomas's bleed-down know-how, which a knowledge engineer is collecting from him: collection that follows "
                     "documents rather than the social holder breaks the claim.")],
    "pkm2-p16": [fld("Agents.IsUnidentifiedBoundarySpanner",
                     "TRUE when a person who connects two communities was never identified by a social network analysis. The plant analysis "
                     "found Lin, so she reads FALSE; Ken (maintenance guild and Riverbend cluster) and Joao (two clusters) connect groups "
                     "and no analysis found them.")],
    "pkm2-p17": [fld("SourceRelationships.IsUnnegotiatedPowerGap",
                     "TRUE when a power imbalance shapes a source relationship and nothing was negotiated about how the knowledge is "
                     "collected and used. It fires on Ken, whose job is threatened by automation, and is FALSE on Tomas and Grace, where "
                     "review rights and supervisor-free sessions were negotiated, and on Lin's balanced relationship.")],
    "pkm2-p18": [fld("SourceRelationships.IsExtractiveRelationship",
                     "TRUE when trust with a source is not established and the engineer records no trust-building practice. It fires on "
                     "Grace (trust still building, nothing done to build it) and is FALSE on Ken, whose low trust is being addressed by "
                     "returning transcripts, and on the established relationships.")],
    "pkm2-p78": [fld("Procedures.IsCapturedByAutomationAlone",
                     "TRUE when a procedure's knowledge was captured by machine-authored entries and no practitioner relationship exists. "
                     "Conveyor maintenance fires (only a copilot summary of vendor manuals); lockout/tagout, captured through relationships "
                     "with Tomas and Lin, does not.")],
    "pkm2-q06": [q("aq-pkm2-q06",
                   "A knowledge engineer asks who holds each piece of know-how; KnowHowCarriers.IsHeldByCurrentPractitioner answers it per "
                   "carrier, TRUE for know-how carried by a person still in a role and FALSE for plants, systems and people who left.")],
    "pkm2-q07": [q("aq-pkm2-q07",
                   "A knowledge engineer asks who knows where know-how is held; KnowledgeBrokerLinks.LocatesOtherHolder is TRUE when a broker "
                   "points a seeker to know-how someone else carries (Lin pointing Aisha to Tomas).")],
    "pkm2-q08": [q("aq-pkm2-q08",
                   "The steward asks which departments give conflicting accounts of one process; DepartmentProcessAccounts.IsConflictingAccount "
                   "is TRUE for the maintenance/production and engineering/support pairs and FALSE for compliance's uncontested account.")],
    "pkm2-s06": [("KnowledgeMethod", "SocialNetworkAnalysis",
                  "Applied by MethodApplications row ma12-sna-plant: a who-asks-whom network of Plant North that identified Lin as a "
                  "connector between communities.")],
    # ---------------------------------------------------------------- pkm-3: concepts
    "pkm3-c04": [fld("KnowledgeRepositoryEntries.FedFromExecution",
                     "Links a repository entry to the execution whose experience it wrote back: the 2026-07-14 lockout run into the "
                     "isolation map, the 2026-01-05 deployment into the go/no-go notes, the Q2 close into the FX-timing retrospective.")],
    "pkm3-c05": [tbl("CommunitiesOfPractice",
                     "Communities that generate and pass on process knowledge, from the close guild to the plant maintenance guild and "
                     "the Riverbend and Eastvale clusters; KnowledgeTransfers records know-how passing inside them.")],
    "pkm3-c10": [fld("KnowHowCarriers.KnowHowKind",
                     "Classifies practitioner know-how as what fails (which die shims crack), diagnosis (hearing a bled accumulator, belt "
                     "drift), improvisation (press setup without the cart, servo timing) or refinement.")],
    "pkm3-c11": [fld("KnowHowCarriers.IsHeldInBothForms",
                     "TRUE when the same know-how is at once written into a procedure and carried as trained skill: Walter's die-shim "
                     "know-how, Hector's hand-build method and Grace's go/no-go judgment.")],
    "pkm3-c12": [fld("KnowledgeTransfers.IsSocialNetworkChannel",
                     "TRUE for transfers through collaboration (Grace pairing with Ravi), design-production iteration (Joao and Petra) and "
                     "people changing employers (Joao and Petra moving between firms).")],
    "pkm3-c13": [fld("CommunitiesOfPractice.InterconnectedKnowHowCount",
                     "Counts know-how in a community that builds on related know-how in the same community: in the maintenance guild the "
                     "bleed-down diagnosis builds on the plant's isolation layout and the improvised setup on the die-shim know-how.")],
    "pkm3-c14": [fld("CommunitiesOfPractice.SpansPhysicalAndDigitalWithHumans",
                     "TRUE when a community's know-how spans physical and digital work and people still carry it: the maintenance guild "
                     "holds press and valve know-how, PLC interlock logic, and technicians' skill.")],
    "pkm3-c19": [fld("Mentorships.MentorshipForm",
                     "Marks apprenticeships: Aisha and Bea placed with Tomas, and Ken with Walter, learning by watching, supervised "
                     "practice and guided correction, as their learning objectives state.")],
    "pkm3-c20": [fld("Organizations.CapturedOwnKnowHowCount",
                     "Counts the organization's own know-how of how its processes are carried out that is kept in its repository: the "
                     "plant's isolation layout, and engineering's hand-build method and go/no-go judgment.")],
    "pkm3-c21": [fld("Mentorships.EmployerWorkerObligation",
                     "Records the obligations that make training worth investing in across generations: a guaranteed post and paid "
                     "certification against a commitment to stay, a retention bonus at completion.")],
    "pkm3-c27": [fld("KnowledgeRepositoryEntries.WrittenForAudience",
                     "Records whether an entry was written for later colleagues as part of the craft (the bleed fix, the build runbook) or "
                     "to satisfy compliance (the quick reference card, the copilot summary).")],
    "pkm3-c30": [fld("KnowledgeRepositoryEntries.OutlivesAuthorTenure",
                     "TRUE when an entry is still current after its author left: Hector's hand-build runbook remains in use since his "
                     "departure at the end of 2024.")],
    "pkm3-c31": [fld("ProblemOccurrences.IsRelearnedSolvedProblem",
                     "TRUE when a problem solved before came back and the earlier fix had not been recorded: stale CDN assets, fixed by a "
                     "contractor in 2025 without a note, returned in May 2026 and had to be solved again.")],
    "pkm3-c42": [fld("KnowHowCarriers.LostAccumulationYears",
                     "The years of practice lost when a departed person was the only carrier: 12.2 years of incident triage heuristics left "
                     "with Dmitri. A new hire starts at zero, which is why such knowledge cannot be recruited back.")],
    # ---------------------------------------------------------------- pkm-3: prescriptions
    "pkm3-p01": [fld("Procedures.IsExecutedWithoutFeedbackLoop",
                     "TRUE when a procedure is executed and no repository entry wrote back what an execution taught. Workforce policy "
                     "notification fires (executed, nothing fed back); lockout, deployment and the close each had an execution written back "
                     "into their documentation and read FALSE.")],
    "pkm3-p05": [fld("OnboardingRecords.IsStartingFromNothing",
                     "TRUE when a newcomer joins a procedure whose previous carriers left with nothing captured, so the work must be relearned "
                     "from nothing. Leah's onboarding into customer incident response fires (Dmitri's triage heuristics left with him); "
                     "starters on lockout and deployment, which have captured entries, read FALSE.")],
    "pkm3-p06": [fld("ProblemOccurrences.IsSolvedWithoutRecordedSolution",
                     "TRUE when a problem was solved and the solution was not recorded as part of the work. The contractor's 2025 CDN cache "
                     "fix fires; the 2026 cache fix and both Press 7 bleed fixes, each linked to a repository entry, do not.")],
    "pkm3-p07": [fld("CommunitiesOfPractice.IsMandatedWithoutSharingNorm",
                     "TRUE when a community was mandated or came with a purchased platform and no sharing takes place in it. The portal "
                     "community created when the knowledge portal was licensed fires; the grassroots maintenance guild and clusters, where "
                     "know-how is passed on, do not.")],
    "pkm3-p17": [fld("Agents.LacksTimeToMentor",
                     "TRUE when a mentor's protected weekly hours fall short of what their active mentorships need. Tomas has 4 protected "
                     "hours against 12 needed for two apprentices; Devon's 3 hours cover his 2-hour mentorship.")],
    "pkm3-p18": [fld("Agents.IsUnrewardedSharer",
                     "TRUE when someone has passed know-how on repeatedly and never been recognized for it. Grace documented and paired to "
                     "pass on her go/no-go judgment with no recognition; Joao, who shares more, received a peer award and reads FALSE.")],
    "pkm3-p19": [fld("Organizations.TreatsKnowledgeWorkAsUnvalued",
                     "TRUE when most of an organization's documentation and knowledge transfer is done outside allotted time, a culture that "
                     "does not value it. acme-engineering fires (four of five pieces on people's own time); acme-plant, which gives time for "
                     "entries and mentoring, does not.")],
    "pkm3-p32": [fld("KnowHowCarriers.IsDelegatedToUnfitSource",
                     "TRUE when the plan to replace know-how relies on public references or AI generation although the know-how is not in "
                     "public references. Ken's belt-drift diagnosis, slated for AI generation, fires; Lin's forklift checklist, which really "
                     "is the manufacturer's public checklist, does not.")],
    # ---------------------------------------------------------------- pkm-3: questions
    "pkm3-q06": [q("aq-pkm3-q06",
                   "The release manager asks whether this problem was solved before; ProblemOccurrences.HasBeenSolvedBefore is TRUE when an "
                   "earlier occurrence of the same problem was solved, and FALSE for first occurrences.")],
    "pkm3-q09": [q("aq-pkm3-q09",
                   "The release manager asks who handled a comparable problem and whether they can be consulted; "
                   "ProblemOccurrences.HasConsultableSpecialist is TRUE when the earlier solver still holds a role.")],
    "pkm3-q10": [q("aq-pkm3-q10",
                   "The VP asks what would be relearned if the current team left; KnowHowCarriers.MustBeRelearnedIfHolderLeaves is TRUE for "
                   "know-how carried by a serving person with nothing captured for a successor.")],
    # ---------------------------------------------------------------- pkm-3: illustrations
    "pkm3-i04": [fld("Organizations.ErodedInStages",
                     "TRUE when facility investment declined, then engineer training after it, then upkeep of how-to-make knowledge after "
                     "that. acme-plant fires (tooling budget frozen 2011, engineer rotation cut 2015, how-to-make binders abandoned 2020); "
                     "acme-engineering's isolated training pause does not.")],
    "pkm3-i05": [fld("CommunitiesOfPractice.IsCrossFirmPracticeCluster",
                     "TRUE when members from several firms share practice, know-how moves with people changing employers, and specialists "
                     "are at hand. The Riverbend press cluster fires (members from two supplier firms, Joao's move, Petra as specialist); "
                     "single-firm guilds do not.")],
    "pkm3-i06": [fld("ProblemOccurrences.IsTurnoverRegression",
                     "TRUE when a fix that was never recorded comes back after the person who made it left. The contractor who fixed stale "
                     "CDN assets in 2025 left without a note, and the bug returned in May 2026; the Press 7 bleed stall, whose fix was "
                     "recorded, does not fire.")],
    "pkm3-i07": [fld("CommunitiesOfPractice.IsCirculationEndingForLackOfApprentices",
                     "TRUE when know-how circulates through people moving between firms but offered apprenticeship places have gone unfilled "
                     "for three years. The Eastvale packaging cluster fires (Petra's move, four open places, last apprentice in 2014); "
                     "Riverbend, with no unfilled places, does not.")],
    "pkm3-i08": [fld("CommunitiesOfPractice.HasAmbientTradeKnowHow",
                     "TRUE when a trade concentrated in one place passes its know-how even to people who do not practise it. Riverbend "
                     "fires: Nina, a school leaver with no role, absorbed servo timing by growing up among the works; no other community "
                     "records such absorption.")],
    "pkm3-i09": [fld("Organizations.HoldsKnowHowInPeoplePlantsAndSystems",
                     "TRUE when an organization's how-to-build knowledge is carried at once by people, plants and systems. acme-plant fires "
                     "(technicians, the Plant North isolation layout, the PLC interlock logic); acme-engineering, whose know-how is carried "
                     "only by people, does not.")],
    # ---------------------------------------------------------------- pkm-4
    "pkm4-p04": [fld("Procedures.HasTransferShortfallExposure",
                     "TRUE when a wave of three or more new starters meets untransferred veteran know-how, or a veteran about to leave "
                     "carries untransferred know-how. Lockout/tagout fires on both counts: Aisha, Bea and Carlos started within 180 days and "
                     "Tomas retires on 2026-09-30 with his bleed-down know-how untransferred. No other procedure is exposed.")],
    "pkm4-p05": [fld("KnowHowCarriers.TransferStopsWithoutVeteran",
                     "TRUE when a serving veteran's know-how has only been passed person to person and nothing is captured, so the next "
                     "newcomer still needs that veteran. Tomas's improvised press setup, taught only to Aisha, fires; Grace's go/no-go "
                     "judgment, also written into the repository, does not.")],
    "pkm4-p08": [fld("Procedures.FormalizationDoesNotEaseOnboarding",
                     "TRUE when starters who learned from captured knowledge took at least as long to reach proficiency as those who did "
                     "not. Production deployment fires (55 days with the stale go/no-go notes against 40 without); lockout/tagout, where "
                     "captured knowledge cut onboarding from 70 to 21 days, does not.")],
    "pkm4-q14": [q("aq-pkm4-q14",
                   "The plant operations manager asks which know-how only veterans hold unpassed, and whether they are leaving; "
                   "KnowHowCarriers.IsUntransferredVeteranKnowHow and IsAtRiskOfImminentLoss answer it, TRUE for Tomas's bleed-down know-how "
                   "(retiring in 73 days) and, without the departure, Lin's checks.")],
}

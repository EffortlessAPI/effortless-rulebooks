"""Evidence for theme B (loop-07): elicitation methods and tacit knowledge.

(kind, target, justification). Validity is computed by ClaimEvidence.IsValid from Postgres
measurements; nothing here asserts coverage by itself.
"""


def fld(target, why):
    return ("Field", target, why)


def tbl(target, why):
    return ("Table", target, why)


def q(target, why):
    return ("RoleQuestion", target, why)


def km(target, why):
    return ("KnowledgeMethod", target, why)


EVIDENCE = {
    # ---------------------------------------------------------------- pkm-1
    "pkm1-c04": [fld("KnowledgeFragments.CognitiveBasis",
                     "Each fragment records whether it rests on intuition, pattern recognition or judgment (tacit) or was articulated "
                     "outright (explicit): the bleed cue is PatternRecognition, the retrofit zero-reading distrust is Intuition, the "
                     "retyped manual note is Articulated.")],
    "pkm1-c05": [fld("ProcedureVersions.JudgmentHeldOutsideSopCount",
                     "Counts the version's unformalized knowledge that its written SOP says nothing about; the four-step loto-v1.0.0 SOP "
                     "left four pieces of expert handling outside it, and even loto-v2.0.0 still leaves one.")],
    "pkm1-c06": [fld("KnowledgeHoldings.IsVeteranDiscretion",
                     "TRUE where a veteran resolves an exception or discretionary decision that nothing written encodes: Tomas's bleed cue "
                     "and Victor's twin-drive isolation before v2, and Tomas's distrust of zero readings on retrofits still today.")],
    "pkm1-c15": [fld("KnowledgeConversions.ConversionMode",
                     "Records each shift of the lockout bleed knowledge by mode: socialization (Ken learning beside Tomas), externalization "
                     "(the cue stated in an interview), combination (merged into the v2 rulebook) and internalization (Aisha rehearsing it).")],
    "pkm1-c28": [fld("KnowledgeFragments.TacitnessDegree",
                     "Places each fragment on a 1-5 spectrum from fully articulated to not articulable instead of a binary: the manual "
                     "note is 1, the shift hand-off 2, the twin-drive rule 3, the bleed cue 4, the retrofit zero-reading distrust 5.")],
    "pkm1-p01": [fld("KnowledgeFragments.IsFlattenedToBrittleRule",
                     "Fires when tacit or situated knowledge was encoded as a hard rule with no conditions saying when it holds. It fires "
                     "on 'Always isolate both drives of mixer 2', which reduced Victor's judgment about retrofits to one machine's name; "
                     "it does not fire on the bleed cue, encoded as guidance with its conditions, nor on the explicit manual note.")],
    "pkm1-p04": [fld("ElicitationSessions.IsGenericOrUnskilledCapture",
                     "Fires when a session captured knowledge with a method outside the elicitation family or without the organization's "
                     "knowledge engineer running it. It fires on the three earlier sessions a finance analyst, a communications manager "
                     "and a policy owner ran, and on none of the lockout sessions Sam Adeyemi ran with elicitation methods.")],
    "pkm1-p05": [fld("KnowledgeFragments.RestsOnSingleDataPoint",
                     "Fires on approved knowledge that no second session or practitioner supports. It fires on the mixer 2 twin-drive "
                     "rule, whose only other data point (Ken in think-aloud) disagrees, and on the retyped manual note; it does not fire "
                     "on the bleed cue, corroborated by shadowing and by an incident, nor on the shift hand-off corroborated in the workshop.")],
    "pkm1-s07": [km("SeciConversion",
                    "Applied by method application ma7-loto-seci, which typed the lockout bleed knowledge's conversions by SECI mode in "
                    "KnowledgeConversions.")],
    "pkm1-s08": [km("PolanyiTacitExplicitDistinction",
                    "Applied by method application ma7-loto-polanyi, which classified the lockout fragments and knowledge holdings as "
                    "tacit or explicit before loto-v2.0.0 was encoded.")],
    # ---------------------------------------------------------------- pkm-2 concepts
    "pkm2-c01": [fld("KnowledgeFragments.LostInTranslation",
                     "Records, for each elicited fragment, what its explicit statement fails to carry of the practice: the pitch change "
                     "behind the bleed cue, Victor's reasons behind the twin-drive rule, what makes Tomas distrust a zero reading.")],
    "pkm2-c02": [fld("PractitionerExpertise.UnstatedBasis",
                     "Records, as a property of the practitioner holding the expertise, the basis they could not put into words: Tomas "
                     "knows a bleed has not taken before the gauge settles and cannot say what in the sound tells him.")],
    "pkm2-c03": [fld("KnowledgeMethods.ElicitationTradeoff",
                     "States for each elicitation method what it gives up and for what: interviews give up accuracy for practicality, "
                     "shadowing gives up reach for depth, workshops give up depth for reach, critical incidents give up accuracy for "
                     "practicality.")],
    "pkm2-c04": [fld("ElicitationParticipants.IsPractitioner",
                     "TRUE for participants who hold the process knowledge by doing the work: Tomas Reyes and Rosa Delgado in the lockout "
                     "workshop, Amina Yusuf in the policy workshop.")],
    "pkm2-c05": [fld("ElicitationParticipants.IsSubjectMatterExpert",
                     "TRUE for the subject matter experts in the room: the plant safety officer and the operations manager in the lockout "
                     "workshop, the people policy owner in the policy workshop.")],
    "pkm2-c06": [fld("ElicitationParticipants.IsKnowledgeEngineer",
                     "TRUE for Sam Adeyemi, the knowledge engineer collecting the process knowledge in the lockout workshop.")],
    "pkm2-c07": [fld("ElicitationParticipants.IsKnowledgeProducer",
                     "TRUE for participants who produce the knowledge being collected, such as the technician Tomas Reyes.")],
    "pkm2-c08": [fld("ElicitationParticipants.IsKnowledgeConsumer",
                     "TRUE for participants who consume it, such as the operations manager and the knowledge engineer.")],
    "pkm2-c09": [fld("InterviewProbes.WhyAnswer",
                     "Holds the reason a practitioner gave when an interview asked why they make a choice: Tomas waits before bleeding "
                     "again because the press 3 accumulator refills from the dryer.")],
    "pkm2-c10": [fld("InterviewProbes.ShortfallAnswer",
                     "Holds what the practitioner does when the standard procedure proves insufficient: on a capacitor-bank retrofit "
                     "Tomas waits five minutes and re-tests before anyone reaches in.")],
    "pkm2-c11": [fld("CriticalIncidents.Outcome",
                     "Each collected episode is typed WentWell, WentBadly or Improvised: the mixer 2 second-drive restart went badly, the "
                     "press 3 gauge episode was improvised, the conveyor 5 hand-off went well.")],
    "pkm2-c12": [fld("CriticalIncidents.JudgmentFragment",
                     "Links each episode to the knowledge fragment recording the judgment call it brought out: the mixer 2 restart to the "
                     "twin-drive isolation fragment, the press 3 gauge episode to the bleed cue.")],
    "pkm2-c13": [fld("KnowledgeGaps.IsGatekeepingOrSabotage",
                     "TRUE for gaps caused by gatekeeping or knowledge sabotage, each recording the silo it created (SiloedWithin): the press "
                     "3 day crew withholding its bleed check, and the override card removed from the retrofit binder.")],
    "pkm2-c14": [fld("KnowledgeFragments.KnowledgeForm",
                     "Every knowledge fragment is classified by form: Tacit, Explicit, Implicit or SituatedJudgment.")],
    "pkm2-c15": [fld("ElicitationSessions.ObserverStance",
                     "Records how the observer took part: NonParticipantObservation in the close shadowing, Shadowing of Ken on press line 3, "
                     "and LegitimatePeripheralParticipation when Sam fetched locks and staged tools for Tomas's crew while asking about each move.")],
    "pkm2-c16": [fld("ObservedActions.IsSmallChoice",
                     "TRUE for small choices seen during actual practice: Ken resting a hand on the valve body after a bleed, Devon "
                     "rechecking the FX rate before posting.")],
    "pkm2-c17": [fld("ObservedActions.IsUnofficialWorkaround",
                     "TRUE for unofficial workarounds seen in practice: Ken hanging a spare personal lock on the cart for a colleague.")],
    "pkm2-c18": [fld("ObservedActions.IsOmittedFromOwnAccount",
                     "TRUE when observation saw an action the practitioner's own account left out, such as trying the start button "
                     "after lockout, which 'everybody knows'.")],
    "pkm2-c23": [fld("ElicitationSessions.GathersWholeProcessChain",
                     "TRUE for a workshop whose participants include people who start the process, carry it out and depend on its "
                     "results: the lockout workshop had the safety officer, technicians, the operations manager and a material handler.")],
    "pkm2-c27": [fld("ExpertCognitions.IsMentalModel",
                     "TRUE for a mental model an expert applies: Tomas treats a machine as a set of energy reservoirs to drain, not a "
                     "switch to turn off.")],
    "pkm2-c28": [fld("ExpertCognitions.IsAutomaticHeuristic",
                     "TRUE for decision heuristics experts apply without deliberating: 'lock a retrofitted machine out as two machines', "
                     "'trust zero only once the needle stops'.")],
    "pkm2-c29": [fld("ElicitationSessions.IsReviewedRecording",
                     "TRUE when a recording of the practitioner's work was reviewed by that practitioner afterward to explain their "
                     "reasoning: Tomas's press 3 lockout recorded on 3 April and reviewed with him on 4 April.")],
    "pkm2-c30": [tbl("ConceptLadderRungs",
                     "Holds the ladder built from a concrete action: from bleeding the pneumatic supply up to the goal 'no trapped air can "
                     "move the clamp' and the value 'nobody's hand is ever inside a machine that can move', and down to its subprocess "
                     "and condition.")],
    "pkm2-c31": [fld("RepertoryGridConstructs.IsNeverStatedDimension",
                     "TRUE for a dimension an expert uses to tell situations apart that only the grid comparison brought out: 'retrofitted "
                     "vs original drive' and 'audible vs silent bleed', which Tomas never named unprompted.")],
}

EVIDENCE.update({
    # ---------------------------------------------------------------- pkm-2 prescriptions
    "pkm2-p01": [fld("ExpertCognitions.IsHeuristicOversimplified",
                     "Fires when an expert said when a heuristic does not hold and the capture kept only the bare rule. It fires on Victor's "
                     "'lock a retrofitted machine out as two machines', whose stated exceptions were dropped; it does not fire on Ken's "
                     "needle heuristic, captured with its press 3 exception, nor on a mental model.")],
    "pkm2-p02": [fld("ProcedureVersions.ReliesOnSingleMethod",
                     "Fires when a version's knowledge was elicited with only one family of method. It fires on close-v1.1.0 (shadowing "
                     "only) and deploy-v3.2.0 (one remote interview); it does not fire on loto-v2.0.0, elicited by interview, observation, "
                     "workshop, protocols and incidents, nor on policy-v1.0.0 (interview and workshop).")],
    "pkm2-p03": [fld("ElicitationSessions.IsInterviewWithoutWhyProbe",
                     "Fires on an interview that never asked why a choice is made: the policy interview, which has no probes; it does not "
                     "fire on the lockout and deployment interviews, which both asked why.")],
    "pkm2-p04": [fld("ElicitationSessions.IsInterviewWithoutShortfallProbe",
                     "Fires on an interview that never asked what happens when the procedure falls short: the deployment interview and "
                     "the policy interview; it does not fire on the lockout interview, which asked what Tomas does when the zero-energy "
                     "check is not enough.")],
    "pkm2-p05": [fld("ProcedureVersions.JudgmentUnprobedByIncidents",
                     "Fires when a version relies on tacit or situated judgment yet no critical incident was collected to surface the "
                     "judgment calls its documentation hides. It fires on close-v1.1.0 and policy-v1.0.0; it does not fire on loto-v2.0.0, "
                     "whose incidents surfaced the twin-drive and gauge judgments, nor on versions with no tacit judgment.")],
    "pkm2-p06": [fld("KnowledgeGaps.IsUnattributedGatekeeping",
                     "Fires when an elicitation recorded the holder declining to share, yet the gap is not identified as gatekeeping or "
                     "sabotage: the night crew's tag-removal habit, left with no cause. It does not fire on the two gaps identified as "
                     "gatekeeping and sabotage, nor on gaps with no refusal.")],
    "pkm2-p07": [fld("KnowledgeGaps.IsRequiredGatekeptUncodified",
                     "Fires on blocking (required) gatekept or sabotaged knowledge never drawn out and codified: the retrofit override "
                     "card removed from the binder. It does not fire on the press 3 crew's gatekept bleed check, which a critical incident "
                     "session drew out and codified as the bleed cue fragment.")],
    "pkm2-p08": [fld("ObservedActions.IsMissedStepLeftUncaptured",
                     "Fires when observation saw a step the practitioner's own account left out and it was never captured as knowledge: "
                     "trying the start button after lockout. It does not fire on the hand-on-valve pause or the FX recheck, which were "
                     "also missing from the accounts but were captured, nor on steps the practitioner mentioned.")],
    "pkm2-p09": [fld("ObservedActions.IsWatchedNotQuestioned",
                     "Fires when a small choice or workaround was watched but no reason was asked for and recorded against it: the spare "
                     "lock hung on the cart. It does not fire on the hand-on-valve pause or the FX recheck, whose reasons are recorded.")],
    "pkm2-p10": [fld("ElicitationSessions.IsWorkshopWithoutUsualOutsiders",
                     "Fires on a workshop in which nobody normally left out took part: the policy workshop of its usual invitees. It does "
                     "not fire on the lockout workshop, which brought in Rosa Delgado, a material handler who depends on the lockouts.")],
    "pkm2-p11": [fld("ProcedureVersions.IsApprovedWithoutSmeSignoff",
                     "Fires on an approved version no affected subject matter expert approved the representation of: deploy-v3.2.0 (approved "
                     "only by the VP, who is not affected) and the other approved versions with no SME approval. It does not fire on "
                     "loto-v2.0.0, approved by the technician and the safety officer whose work it governs.")],
    "pkm2-p12": [fld("WorkflowViewDivergences.IsSurfacedButUnreconciled",
                     "Fires when a workshop brought two differing views of how work flows into the open but never reconciled them: when "
                     "operators are notified of a shutdown. It does not fire on the last-tag disagreement, settled and recorded as a fragment.")],
    # ---------------------------------------------------------------- pkm-2 competency questions
    "pkm2-q01": [q("aq-pkm2-q01", "The knowledge engineer's question; ObservedActions.HasRecordedReason reads each observed action's recorded reason.")],
    "pkm2-q02": [q("aq-pkm2-q02", "The knowledge engineer's question; ObservedActions.HasCounterfactualAnswer reads what the practitioner said they would do under a different condition.")],
    "pkm2-q03": [q("aq-pkm2-q03", "The safety officer's question; ConceptLadderRungs.IsUltimateGoal picks the top rung laddered up from each action.")],
    "pkm2-q04": [q("aq-pkm2-q04", "The knowledge engineer's question; ConceptLadderRungs.IsDecompositionRung picks the subprocess and condition rungs below an action.")],
    "pkm2-q05": [q("aq-pkm2-q05", "The knowledge engineer's question; RepertoryGridConstructs.IsRecordedDiscriminatingDimension picks the recorded dimensions that split the compared situations.")],
    "pkm2-q10": [q("aq-pkm2-q10", "The knowledge authority's question; KnowledgeGaps.IsKnownAndUnresolved picks identified gaps still open.")],
    "pkm2-q11": [q("aq-pkm2-q11", "The knowledge authority's question; KnowledgeGaps.IsGatekeepingOrSabotage picks gaps caused by gatekeeping or sabotage.")],
    "pkm2-q17": [q("aq-pkm2-q17", "The safety officer's question; CriticalIncidents.Outcome types each episode and IsAdverseOrImprovised picks the bad and improvised ones.")],
    "pkm2-q18": [q("aq-pkm2-q18", "The safety officer's question; CriticalIncidents.RevealedJudgment and JudgmentFragment give each episode's judgment, HasSurfacedJudgment says whether it revealed one.")],
    # ---------------------------------------------------------------- pkm-2 standards (each applied by a seeded session or application)
    "pkm2-s01": [km("CriticalIncidentTechnique", "Applied by session el7-loto-cit, which collected the mixer 2, press 3 and conveyor 5 incidents.")],
    "pkm2-s02": [km("ThinkAloudProtocol", "Applied by session el7-loto-thinkaloud: Ken verbalized his reasoning while locking out mixer 2.")],
    "pkm2-s03": [km("RetrospectiveProtocol", "Applied by session el7-loto-retro: Tomas's recorded lockout reviewed with him the next day.")],
    "pkm2-s04": [km("ConceptLaddering", "Applied by session el7-loto-ladder, which produced the ConceptLadderRungs rows.")],
    "pkm2-s05": [km("RepertoryGrid", "Applied by session el7-loto-grid, which produced the RepertoryGridConstructs rows.")],
    "pkm2-s07": [km("ProcessMapping", "Applied by ma14-loto-process-map, which drew the lockout step by step with technicians and the safety officer before any value stream analysis.")],
    "pkm2-s08": [km("ValueStreamAnalysis", "Applied by ma7-loto-value-stream, which exposed the day-to-night hand-off and the wait on operator release.")],
    "pkm2-s15": [km("Shadowing", "Applied by sessions el7-loto-shadow and el7-loto-lpp on press line 3 and by elicit-close-shadow.")],
    "pkm2-s17": [km("PractitionerInterview", "The structured practitioner interview; applied by el7-loto-interview with typed why, what-if and shortfall probes.")],
    "pkm2-s18": [km("FacilitatedWorkshop", "Applied by el7-loto-workshop, whose participants span technicians, the safety officer, operations and a material handler.")],
    # ---------------------------------------------------------------- pkm-2 illustrations
    "pkm2-i01": [fld("PractitionerExpertise.KnowsMoreThanCanSay",
                     "The plant's counterpart of the adjuster sensing fraud: fires when a practitioner reliably detects a condition whose "
                     "basis they cannot state. It fires on Tomas, right fourteen times about a bleed that did not take; it does not fire on "
                     "Aisha, who cannot say why either but has one reliable call, nor on Ken, who states his basis.")],
    "pkm2-i02": [fld("PractitionerExpertise.IsUnexplainedForesight",
                     "The software architect case: fires when a design judgment later proved right although its holder could not say why. "
                     "It fires on Ravi's sense that the shared build cache would be costly to maintain; it does not fire on Omar's "
                     "confirmed canary judgment, whose basis he stated, nor on detection expertise.")],
    "pkm2-i07": [fld("ProcedureVersions.TacitShareExceedsExplicit",
                     "Compares tacit holdings (operator memory, departing staff, crew agreements) with explicit carriers (SOP, manual "
                     "notes). It fires on loto-v1.0.0, where three of five holdings were tacit; it does not fire on loto-v2.0.0 after encoding.")],
    "pkm2-i08": [fld("ProcedureVersions.IsStudiedOnlyFromTheDesk",
                     "The field-researcher comparison as a check: fires when the knowledge engineer elicited a version but never went "
                     "where the work happens. It fires on deploy-v3.2.0, elicited by one remote interview; it does not fire on loto-v2.0.0, "
                     "elicited on the plant floor.")],
    "pkm2-i09": [fld("ProcedureVersions.ExpertsEvaluateAiNotRepresentation",
                     "Fires when experts were recruited to evaluate AI output on a version but none to approve the representation it "
                     "rests on: Ravi Menon, an affected platform engineer, rated the deployment AI's output while no affected expert approved deploy-v3.2.0. It does "
                     "not fire on loto-v2.0.0, approved by its experts.")],
    "pkm2-i12": [fld("ProcedureVersions.LivesInHandsSilenceAndNegotiation",
                     "Fires when a version's knowledge lives at once in operators' memory, in what its SOP leaves unsaid, and in unspoken "
                     "crew agreements. It fires on loto-v1.0.0 (Tomas's bleed cue, the manual margin note, the night crew's tag habit); "
                     "it does not fire on loto-v2.0.0, where the crew agreement was settled and encoded.")],
    "pkm2-i15": [fld("KnowledgeTestOutcomes.IsImproved",
                     "Measures each outcome the lockout knowledge tests were meant to improve, before and after: competency gaps, audit "
                     "quality, minutes per lockout and consistency improved; job satisfaction did not move, and the field reads FALSE there.")],
    # ---------------------------------------------------------------- pkm-3 / pkm-4
    "pkm3-c01": [fld("KnowledgeHoldings.IsUnformalizedProcessKnowledge",
                     "TRUE for lived, unformalized knowledge of how the lockout work gets done, whether tacit (Tomas's bleed cue), explicit "
                     "(a manual margin note) or implicit (the night crew's unspoken tag agreement).")],
    "pkm3-c02": [fld("KnowledgeHoldings.IsProceduralKnowledge",
                     "TRUE once a holding is formalized as a typed fragment of the procedure ontology: the v2 bleed cue, twin-drive "
                     "isolation and shift hand-off.")],
    "pkm3-p02": [fld("KnowledgeHoldings.IsFormalizedWithoutElicitationWork",
                     "Fires when knowledge was formalized into a fragment that no observation, interview or other elicitation produced: "
                     "the press 3 margin note retyped into the rulebook. It does not fire on the bleed cue, twin-drive or hand-off "
                     "holdings, formalized through an interview, an incident session and a workshop.")],
    "pkm4-c13": [fld("KnowledgeHoldings.Carrier",
                     "Records where each piece of lockout knowledge lives, before and after encoding: operators' memory, formal documents, "
                     "notes in an outdated manual, the memory of a technician about to retire, and, after, the procedure rulebook.")],
    "pkm4-p12": [fld("ProcedureVersions.MissesARequiredElicitationMode",
                     "Fires for an elicited version lacking a workshop, a structured interview, observation, or a session that settled a "
                     "disagreement. It fires on close-v1.1.0, policy-v1.0.0 and deploy-v3.2.0; it does not fire on loto-v2.0.0, which had "
                     "all four, including the workshop that settled the last-tag disagreement.")],
})

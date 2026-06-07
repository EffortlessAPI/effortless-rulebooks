# 📘 Mechanical Kitchen Timer — RuleSpeak

_Five-part mechanical timer modeled with every README noun as a table — a hardware-ontology stress test._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **User** | People who wind timers. |
| Wind Count | Total wind actions performed by this user. |
| Cook Count | Total cooks this user has prepared today. |
| Completed Cook Count | Cooks this user has prepared that have finished (their timer has rung). |
| Activity Level | Fuzzy take on how active this user is today: idle if no winds, casual for one or two, busy for three or more. |
| Total Recipe Minutes Attempted | Sum of recommended-minutes across recipes this user has cooked today. Zig-zags User -> Cook -> Recipe.RecommendedMinutes. |
| Avg Recipe Minutes Attempted | Mean recommended-minutes across this user's cooks today. Zig-zags User -> Cook -> Recipe.RecommendedMinutes. |
| Cooking Style | Fuzzy verdict on this user's day, off the zig-zag aggregate AvgRecipeMinutesAttempted: 'snacking' under 10 min average, 'short-order' under 30, 'slow-cooking' beyond. |
| **Timer** | Top-level timer assembly. Holds the raw mechanical parameters and the headline runtime DAG: stored torque, remaining minutes, wind state, and whether the bell has rung. |
| Wind Fraction | Fraction of full wind currently stored. |
| Stored Torque Nm | Torque currently stored in the mainspring (§2). |
| Remaining Minutes | Minutes left until the cam trips §5. |
| Wind State | Coarse state: Unwound, Partial, or Full. |
| Is Armed | True while the mainspring still holds energy. |
| Has Rung | True once §5 has fired. |
| Tick Count | Total escapement ticks recorded for this timer. |
| Ring Count | Total ding events recorded for this timer. |
| Cook Count | Number of cooks currently or recently timed on this timer. |
| Total Recipe Minutes Used | Sum of recommended minutes across recipes the cooks on this timer are following. Zig-zags through Cook to Recipe. |
| Avg Recipe Minutes Used | Average recommended-minutes across the recipes any cook on this timer is following. Goes Timer -> Cook (rollup) -> Recipe (the underlying lookup). |
| Wind Duration Minutes | How many minutes one full wind buys on this timer. Derived from MaxWindDegrees at 6 deg per minute. |
| Expected Ticks Per Minute | Escapement ticks expected per minute on this timer. |
| Expected Total Ticks | Ticks expected across a full wind, end to end. |
| Ticks Elapsed Expected | Ticks that should have occurred so far on this timer based on how much the spring has unwound. |
| Tick Efficiency Percent | Observed ticks divided by expected ticks so far, as a percentage. A timer that has just been wound returns 100 since no ticks were expected yet. |
| Has Sparse Tick Record | True when the recorded tick count is well below what should have happened by now. |
| Mechanical Phase | Fuzzy lifecycle phase. A timer is freshly-wound if barely any of its wind has been spent, mid-run if it is well into its travel, nearly-done if it is almost out of energy, and rung once it has fired. |
| **Cas** | The outer housing of a timer. The bell is mounted on top of the case. |
| **Bell** | The bell mounted on top of the case. Struck by the hammer striker when §5 fires. |
| Times Rung | Number of ding events for this bell. |
| Has Ever Rung | True if at least one ding has been recorded. |
| **Winding Knob** | §1 — the dial on the front. Shaft is fixed to the mainspring arbor. Rim carries a printed scale 0..60 read against a fixed pointer on the case. |
| Timer Wind Angle | Parent timer's current wind angle. |
| Timer Max Wind | Parent timer's max wind angle. |
| Scale Reading | Number the printed scale currently shows against the pointer. |
| **Arbor** | The shaft driven by the mainspring. Connects §1 (knob shaft) to §2 (mainspring), and carries the §5 cam. |
| **Mainspring** | §2 — flat coiled steel spring in a drum. Inner end anchors to the arbor; outer end anchors to the case. |
| **Gear Train** | §3 — the three-gear stack between mainspring arbor and escapement. Carries the output shaft that drives the dial pointer. |
| Timer Escapement Hz | Parent timer's escapement frequency. |
| Output Shaft RPM | Speed of the output shaft (the one carrying the dial pointer). |
| Gear Count | Number of gears in this train. Always 3 on this design. |
| Total Teeth | Sum of tooth counts across the three gears. |
| Average Tooth Count | Mean tooth count across the gears in this train. |
| **Gear** | Individual gears in a gear train. Each has a tooth count and a position in the chain (1, 2, or 3). |
| Is First in Chain | True for the gear nearest the arbor. |
| Is Last in Chain | True for the gear nearest the escapement. |
| Size Class | Fuzzy size based on tooth count: large 40+, medium 25-39, small under 25. |
| On Timer | Timer the gear's gear-train belongs to (chained lookup through GearTrains). |
| **Output Shaft** | The shaft at the end of the gear train. Carries the dial pointer. |
| Current RPM | Inherits the gear train's output RPM. |
| **Dial Pointer** | Pointer mounted on the output shaft. Reads against the printed scale on the knob rim. |
| **Escapement** | §4 — escapement assembly: toothed wheel plus two-pronged pallet plus hairspring. Defines the time base. |
| Tick Count | Ticks recorded at this escapement. |
| Has Ticked | True if this escapement has recorded at least one tick. |
| **Escapement Wheel** | The toothed wheel of the escapement. Each pallet rock releases exactly one tooth. |
| **Pallet** | Pivoting piece sitting across the escapement-wheel teeth. Two prongs (the README's 'two-pronged pallet'). Rocking is set by pallet mass plus the hairspring. |
| **Hairspring** | Tiny coiled spring attached to the pallet. Sets the pallet's rocking rate together with pallet mass. |
| **Cam** | Profile on the arbor of §2. Its notch aligns with the hammer catch at the zero position; that alignment is what trips §5. |
| **Bell Hammer** | §5 — spring-loaded arm with a steel striker, held back by the hammer catch. When the cam notch reaches the catch, the catch releases and the arm's own spring snaps the striker against the bell. |
| Timer Has Rung | Whether the parent timer has rung. |
| Release State | Held while the cam still restrains the catch; Released once the timer has rung. |
| **Hammer Arm** | The spring-loaded arm of the bell hammer. Carries the striker at its tip. |
| **Striker** | The small steel head at the tip of the hammer arm. Whatever actually contacts the bell. |
| **Hammer Catche** | Catch that holds the hammer back until the cam notch aligns. Once released, the arm spring snaps the striker against the bell. |
| Hammer Release State | Inherits parent hammer's release state. |
| Is Engaged | True while the catch still restrains the hammer. |
| **Wind Action** | Event: a user rotates the winding knob to a target angle at a point in time. The README's §1 input event. |
| Applied to Timer | Timer the turned knob is fixed to. |
| Resulted in Ring | True if the timer this wind action wound has since rung. |
| **Tick Event** | Event: the escapement releases one tooth — one 'tick' from the README. |
| Timer Label | Parent timer's label. |
| On Timer That Has Rung | Whether the parent timer has rung. |
| On Timer in Phase | Parent timer's current mechanical phase. |
| **Ring Event** | Event: the bell hammer strikes the bell — the README's 'ding'. Recorded once per fired §5. |
| **Cook** | A dish being timed on a timer. Each cook started at the moment a user wound the timer; its status mirrors the timer's lifecycle. |
| Timer Has Rung | Whether the cook's timer has rung yet. |
| Timer is Armed | Whether the cook's timer is still armed. |
| Timer Remaining Min | Minutes the cook's timer has left. |
| Status | Done if the timer has rung, Cooking if it is still armed, otherwise Pending. |
| Recommended Minutes | Minutes the recipe calls for. |
| Timer Wind Duration | Maximum minutes the chosen timer can run. |
| Timer Can Cover Recipe | True if the chosen timer can run for at least as long as the recipe calls for. |
| Suitability Verdict | Fuzzy verdict on the cook's choice of timer: insufficient if the timer is too short for the recipe, overkill if it can run more than three times as long as needed, otherwise appropriate. |
| Prepared by Name | Display name of the user preparing this cook. |
| Prepared by Activity Level | Activity-level verdict on the user preparing this cook (chained lookup through Users). |
| Recipe Duration Category | Duration-category of the recipe being followed (chained lookup through Recipes). |
| Recipe Temp Profile | Cooking-temp profile of the recipe (chained lookup through Recipes). |
| Timer Mechanical Phase | Mechanical phase of the timer this cook is on (chained lookup through Timers). |
| **Recipe** | Cooking recipes a cook can follow. Each names a dish and the minutes it should run. |
| Times Used Today | Cooks following this recipe today. |
| Duration Category | A loose verbal classification of how long this recipe takes: quick under 5 min, short under 15, medium under 45, long beyond that. |
| Cooking Temp Profile | Loose verbal classification of cooking temperature: boiling, hot oven, warm oven, or low. |
| Avg Timer Capacity | Average wind-duration of the timers cooks have used for this recipe. Zig-zags Recipe -> Cook -> Timer.WindDurationMinutes. |
| Total Timer Capacity | Sum of timer wind-durations allocated to this recipe today. Zig-zags Recipe -> Cook -> Timer.WindDurationMinutes. |
| Assignment Verdict | Fuzzy verdict on how well this recipe's actual timer assignments match what it needs. Drives off the zig-zag aggregate AvgTimerCapacity vs the recipe's own RecommendedMinutes. |
| **Kitchen** | A kitchen, top of the rollup tree. Aggregates everything below it. |
| Total Timers | Count of timers in the world. |
| Total Cases | Count of cases. |
| Total Bells | Count of bells. |
| Total Users | Count of users. |
| Total Cooks | Count of cooks today. |
| Total Recipes | Count of recipes. |
| Total Wind Actions | Wind actions today. |
| Total Tick Events | Tick events recorded today. |
| Total Ring Events | Ring events recorded today. |
| Timers Freshly Wound | Timers currently freshly-wound. |
| Timers Mid Run | Timers currently mid-run. |
| Timers Nearly Done | Timers currently nearly-done. |
| Timers Rung | Timers that have rung. |
| Cooks Done | Cooks whose status is Done. |
| Cooks Cooking | Cooks whose status is Cooking. |
| Recipes Unused | Recipes nobody cooked today. |
| Recipes Under Provisioned | Recipes whose chosen timer can't cover them. |
| Recipes Well Matched | Recipes whose timer assignment was well-matched. |
| Busy Users | Users with activity-level busy. |
| Idle Users | Users with activity-level idle. |
| Total Recipe Minutes | Sum of recommended-minutes across all recipes. |
| Avg Recipe Minutes | Mean recommended-minutes across all recipes. |
| Min Recipe Minutes | Shortest recipe in minutes. |
| Max Recipe Minutes | Longest recipe in minutes. |
| Has Any Timer Rung | True if any timer has rung today. |
| Has Misprovisioned Recipe | True if any recipe is currently under-provisioned. |
| Operational Status | Fuzzy overall status: 'quiet' if no cooks active, 'busy' if many, 'active' otherwise. |

## 2 Fact Types

- a **cas** references exactly one **timer**
- a **bell** references exactly one **cas**
- a **winding knob** references exactly one **timer**
- an **arbor** references exactly one **timer**
- a **mainspring** references exactly one **arbor**
- a **gear train** references exactly one **timer**
- a **gear** references exactly one **gear train**
- an **output shaft** references exactly one **gear train**
- a **dial pointer** references exactly one **output shaft**
- an **escapement** references exactly one **timer**
- an **escapement wheel** references exactly one **escapement**
- a **pallet** references exactly one **escapement**
- a **hairspring** references exactly one **pallet**
- a **cam** references exactly one **arbor**
- a **bell hammer** references exactly one **timer**
- a **hammer arm** references exactly one **bell hammer**
- a **striker** references exactly one **hammer arm**
- a **hammer catche** references exactly one **bell hammer**
- a **hammer catche** references exactly one **cam**
- a **wind action** references exactly one **user**
- a **wind action** references exactly one **winding knob**
- a **tick event** references exactly one **timer**
- a **tick event** references exactly one **escapement**
- a **ring event** references exactly one **timer**
- a **ring event** references exactly one **bell**
- a **cook** references exactly one **timer**
- a **cook** references exactly one **user**
- a **cook** references exactly one **recipe**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Wind Count** | A user's wind count is the number of wind actions related to the user. |
| **DR-2 Cook Count** | A user's cook count is the number of cooks related to the user. |
| **DR-3 Completed Cook Count** | A user's completed cook count is the number of the user's cooks that are timer has rung. |
| **DR-4 Activity Level** | The user's activity level is determined by the following priority:<br>1. the literal “idle”, if the wind count is 0;<br>2. the literal “casual”, if the wind count is at most 2;<br>3. otherwise the literal “busy”. |
| **DR-5 Total Recipe Minutes Attempted** | A user's total recipe minutes attempted is the total recommended minutes across the cooks related to the user. |
| **DR-6 Avg Recipe Minutes Attempted** | A user's avg recipe minutes attempted is the average recommended minutes across the cooks related to the user. |
| **DR-7 Cooking Style** | The user's cooking style is determined by the following priority:<br>1. the literal “not cooking”, if the cook count is 0;<br>2. the literal “snacking”, if the avg recipe minutes attempted is less than 10;<br>3. the literal “short-order”, if the avg recipe minutes attempted is less than 30;<br>4. otherwise the literal “slow-cooking”. |
| **DR-8 Wind Fraction** | A timer's wind fraction is computed as `WindAngleDegrees / MaxWindDegrees`. |
| **DR-9 Stored Torque Nm** | A timer's stored torque nm is computed as `WindFraction * MaxSpringTorqueNm`. |
| **DR-10 Remaining Minutes** | A timer's remaining minutes is computed as `WindAngleDegrees / 6`. |
| **DR-11 Wind State** | The timer's wind state is determined by the following priority:<br>1. the literal “Unwound”, if the wind angle degrees is at most 0;<br>2. the literal “Full”, if the wind angle degrees is at least the max wind degrees;<br>3. otherwise the literal “Partial”. |
| **DR-12 Is Armed** | A timer is considered armed if the remaining minutes is greater than 0. |
| **DR-13 Has Rung** | A timer is considered to have a rung if it is not the case that the is armed flag is set. |
| **DR-14 Tick Count** | A timer's tick count is the number of tick events related to the timer. |
| **DR-15 Ring Count** | A timer's ring count is the number of ring events related to the timer. |
| **DR-16 Cook Count** | A timer's cook count is the number of cooks related to the timer. |
| **DR-17 Total Recipe Minutes Used** | A timer's total recipe minutes used is the total recommended minutes across the cooks related to the timer. |
| **DR-18 Avg Recipe Minutes Used** | A timer's avg recipe minutes used is the average recommended minutes across the cooks related to the timer. |
| **DR-19 Wind Duration Minutes** | A timer's wind duration minutes is computed as `MaxWindDegrees / 6`. |
| **DR-20 Expected Ticks Per Minute** | A timer's expected ticks per minute is computed as `EscapementBaseHz * 60`. |
| **DR-21 Expected Total Ticks** | A timer's expected total ticks is computed as `ExpectedTicksPerMinute * WindDurationMinutes`. |
| **DR-22 Ticks Elapsed Expected** | A timer's ticks elapsed expected is computed as `ExpectedTicksPerMinute * (WindDurationMinutes - RemainingMinutes)`. |
| **DR-23 Tick Efficiency Percent** | The timer's tick efficiency percent is determined by the following priority:<br>1. `(TickCount / TicksElapsedExpected) * 100`, if the ticks elapsed expected is greater than 0;<br>2. otherwise 100. |
| **DR-24 Has Sparse Tick Record** | A timer is considered to have a sparse tick record if `If(TicksElapsedExpected` is greater than `0, TickCount < TicksElapsedExpected, False())`. |
| **DR-25 Mechanical Phase** | The timer's mechanical phase is determined by the following priority:<br>1. the literal “rung”, if the has rung flag is set;<br>2. the literal “freshly-wound”, if the wind fraction is greater than 0.8;<br>3. the literal “mid-run”, if the wind fraction is greater than 0.25;<br>4. otherwise the literal “nearly-done”. |
| **DR-26 Times Rung** | A bell's times rung is the number of ring events related to the bell. |
| **DR-27 Has Ever Rung** | A bell is considered to have an ever rung if the times rung is greater than 0. |
| **DR-28 Timer Wind Angle** | A winding knob's timer wind angle is the wind angle degrees of the winding knob's fixed to timer. |
| **DR-29 Timer Max Wind** | A winding knob's timer max wind is the max wind degrees of the winding knob's fixed to timer. |
| **DR-30 Scale Reading** | A winding knob's scale reading is computed as `ScaleMin + (TimerWindAngle / TimerMaxWind) * (ScaleMax - ScaleMin)`. |
| **DR-31 Timer Escapement Hz** | A gear train's timer escapement hz is the escapement base hz of the gear train's in timer. |
| **DR-32 Output Shaft RPM** | A gear train's output shaft RPM is computed as `(TimerEscapementHz * 60) / ReductionRatio`. |
| **DR-33 Gear Count** | A gear train's gear count is the number of gears related to the gear train. |
| **DR-34 Total Teeth** | A gear train's total teeth is the total tooth count across the gears related to the gear train. |
| **DR-35 Average Tooth Count** | A gear train's average tooth count is the average tooth count across the gears related to the gear train. |
| **DR-36 Is First in Chain** | A gear is considered a first in chain if the position in chain is 1. |
| **DR-37 Is Last in Chain** | A gear is considered a last in chain if the position in chain is 3. |
| **DR-38 Size Class** | The gear's size class is determined by the following priority:<br>1. the literal “large”, if the tooth count is at least 40;<br>2. the literal “medium”, if the tooth count is at least 25;<br>3. otherwise the literal “small”. |
| **DR-39 On Timer** | A gear's on timer is the in timer of the gear's in gear train. |
| **DR-40 Current RPM** | An output shaft's current RPM is the output shaft RPM of the output shaft's in gear train. |
| **DR-41 Tick Count** | An escapement's tick count is the number of tick events related to the escapement. |
| **DR-42 Has Ticked** | An escapement is considered to have ticked if the tick count is greater than 0. |
| **DR-43 Timer Has Rung** | A bell hammer's timer has rung is true when the bell hammer's in timer has a rung. |
| **DR-44 Release State** | The bell hammer's release state is determined by the following priority:<br>1. the literal “Released”, if the timer has rung flag is set;<br>2. otherwise the literal “Held”. |
| **DR-45 Hammer Release State** | A hammer catche's hammer release state is the release state of the hammer catche's of bell hammer. |
| **DR-46 Is Engaged** | A hammer catche is considered engaged if `If(HammerReleaseState` is `"Held", True(), False())`. |
| **DR-47 Applied to Timer** | A wind action's applied to timer is the fixed to timer of the wind action's turned knob. |
| **DR-48 Resulted in Ring** | A wind action's resulted in ring is true when the wind action's applied to timer has a rung. |
| **DR-49 Timer Label** | A tick event's timer label is the label of the tick event's happened on timer. |
| **DR-50 On Timer That Has Rung** | A tick event's on timer that has rung is true when the tick event's happened on timer has a rung. |
| **DR-51 On Timer in Phase** | A tick event's on timer in phase is the mechanical phase of the tick event's happened on timer. |
| **DR-52 Timer Has Rung** | A cook's timer has rung is true when the cook's on timer has a rung. |
| **DR-53 Timer is Armed** | A cook's timer is armed is true when the cook's on timer is armed. |
| **DR-54 Timer Remaining Min** | A cook's timer remaining min is the remaining minutes of the cook's on timer. |
| **DR-55 Status** | The cook's status is determined by the following priority:<br>1. the literal “Done”, if the timer has rung flag is set;<br>2. the literal “Cooking”, if the timer is armed flag is set;<br>3. otherwise the literal “Pending”. |
| **DR-56 Recommended Minutes** | A cook's recommended minutes is the recommended minutes of the cook's follows recipe. |
| **DR-57 Timer Wind Duration** | A cook's timer wind duration is the wind duration minutes of the cook's on timer. |
| **DR-58 Timer Can Cover Recipe** | A cook is flagged timer can cover recipe if the timer wind duration is at least the recommended minutes. |
| **DR-59 Suitability Verdict** | The cook's suitability verdict is determined by the following priority:<br>1. the literal “insufficient”, if the timer wind duration is less than the recommended minutes;<br>2. the literal “overkill”, if the timer wind duration is greater than `(RecommendedMinutes * 3)`;<br>3. otherwise the literal “appropriate”. |
| **DR-60 Prepared by Name** | A cook's prepared by name is the display name of the cook's prepared by. |
| **DR-61 Prepared by Activity Level** | A cook's prepared by activity level is the activity level of the cook's prepared by. |
| **DR-62 Recipe Duration Category** | A cook's recipe duration category is the duration category of the cook's follows recipe. |
| **DR-63 Recipe Temp Profile** | A cook's recipe temp profile is the cooking temp profile of the cook's follows recipe. |
| **DR-64 Timer Mechanical Phase** | A cook's timer mechanical phase is the mechanical phase of the cook's on timer. |
| **DR-65 Times Used Today** | A recipe's times used today is the number of cooks related to the recipe. |
| **DR-66 Duration Category** | The recipe's duration category is determined by the following priority:<br>1. the literal “quick”, if the recommended minutes is less than 5;<br>2. the literal “short”, if the recommended minutes is less than 15;<br>3. the literal “medium”, if the recommended minutes is less than 45;<br>4. otherwise the literal “long”. |
| **DR-67 Cooking Temp Profile** | The recipe's cooking temp profile is determined by the following priority:<br>1. the literal “hot oven”, if the ideal temperature c is at least 200;<br>2. the literal “warm oven”, if the ideal temperature c is at least 150;<br>3. the literal “boiling”, if the ideal temperature c is at least 90;<br>4. otherwise the literal “low”. |
| **DR-68 Avg Timer Capacity** | A recipe's avg timer capacity is the average timer wind duration across the cooks related to the recipe. |
| **DR-69 Total Timer Capacity** | A recipe's total timer capacity is the total timer wind duration across the cooks related to the recipe. |
| **DR-70 Assignment Verdict** | The recipe's assignment verdict is determined by the following priority:<br>1. the literal “unused”, if the times used today is 0;<br>2. the literal “under-provisioned”, if the avg timer capacity is less than the recommended minutes;<br>3. the literal “over-provisioned”, if the avg timer capacity is greater than `(RecommendedMinutes * 3)`;<br>4. otherwise the literal “well-matched”. |
| **DR-71 Total Timers** | A kitchen's total timers is rolled up from its related records (`Count(Timers.TimerId)`). |
| **DR-72 Total Cases** | A kitchen's total cases is rolled up from its related records (`Count(Cases.CaseId)`). |
| **DR-73 Total Bells** | A kitchen's total bells is rolled up from its related records (`Count(Bells.BellId)`). |
| **DR-74 Total Users** | A kitchen's total users is rolled up from its related records (`Count(Users.UserId)`). |
| **DR-75 Total Cooks** | A kitchen's total cooks is rolled up from its related records (`Count(Cooks.CookId)`). |
| **DR-76 Total Recipes** | A kitchen's total recipes is rolled up from its related records (`Count(Recipes.RecipeId)`). |
| **DR-77 Total Wind Actions** | A kitchen's total wind actions is rolled up from its related records (`Count(WindActions.WindActionId)`). |
| **DR-78 Total Tick Events** | A kitchen's total tick events is rolled up from its related records (`Count(TickEvents.TickEventId)`). |
| **DR-79 Total Ring Events** | A kitchen's total ring events is rolled up from its related records (`Count(RingEvents.RingEventId)`). |
| **DR-80 Timers Freshly Wound** | A kitchen's timers freshly wound is rolled up from its related records (`Count(Timers.MechanicalPhase, "freshly-wound")`). |
| **DR-81 Timers Mid Run** | A kitchen's timers mid run is rolled up from its related records (`Count(Timers.MechanicalPhase, "mid-run")`). |
| **DR-82 Timers Nearly Done** | A kitchen's timers nearly done is rolled up from its related records (`Count(Timers.MechanicalPhase, "nearly-done")`). |
| **DR-83 Timers Rung** | A kitchen's timers rung is rolled up from its related records (`Count(Timers.MechanicalPhase, "rung")`). |
| **DR-84 Cooks Done** | A kitchen's cooks done is rolled up from its related records (`Count(Cooks.Status, "Done")`). |
| **DR-85 Cooks Cooking** | A kitchen's cooks cooking is rolled up from its related records (`Count(Cooks.Status, "Cooking")`). |
| **DR-86 Recipes Unused** | A kitchen's recipes unused is rolled up from its related records (`Count(Recipes.AssignmentVerdict, "unused")`). |
| **DR-87 Recipes Under Provisioned** | A kitchen's recipes under provisioned is rolled up from its related records (`Count(Recipes.AssignmentVerdict, "under-provisioned")`). |
| **DR-88 Recipes Well Matched** | A kitchen's recipes well matched is rolled up from its related records (`Count(Recipes.AssignmentVerdict, "well-matched")`). |
| **DR-89 Busy Users** | A kitchen's busy users is rolled up from its related records (`Count(Users.ActivityLevel, "busy")`). |
| **DR-90 Idle Users** | A kitchen's idle users is rolled up from its related records (`Count(Users.ActivityLevel, "idle")`). |
| **DR-91 Total Recipe Minutes** | A kitchen's total recipe minutes is rolled up from its related records (`Sum(Recipes.RecommendedMinutes)`). |
| **DR-92 Avg Recipe Minutes** | A kitchen's avg recipe minutes is rolled up from its related records (`Average(Recipes.RecommendedMinutes)`). |
| **DR-93 Min Recipe Minutes** | A kitchen's min recipe minutes is rolled up from its related records (`Min(Recipes.RecommendedMinutes)`). |
| **DR-94 Max Recipe Minutes** | A kitchen's max recipe minutes is rolled up from its related records (`Max(Recipes.RecommendedMinutes)`). |
| **DR-95 Has Any Timer Rung** | A kitchen is considered to have any timer rung if the timers rung is greater than 0. |
| **DR-96 Has Misprovisioned Recipe** | A kitchen is considered to have a misprovisioned recipe if the recipes under provisioned is greater than 0. |
| **DR-97 Operational Status** | The kitchen's operational status is determined by the following priority:<br>1. the literal “quiet”, if the cooks cooking is 0;<br>2. the literal “busy”, if the cooks cooking is at least 3;<br>3. otherwise the literal “active”. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Users.WindCount** | rollup | `Count(WindActions via PerformedBy)` |
| **Users.CookCount** | rollup | `Count(Cooks via PreparedBy)` |
| **Users.CompletedCookCount** | rollup | `Count(Cooks.PreparedBy, Users.UserId, Cooks.TimerHasRung, True())` |
| **Users.ActivityLevel** | formula | `If(WindCount = 0, "idle", If(WindCount <= 2, "casual", "busy"))` |
| **Users.TotalRecipeMinutesAttempted** | rollup | `Sum(Cooks.RecommendedMinutes via PreparedBy)` |
| **Users.AvgRecipeMinutesAttempted** | rollup | `Average(Cooks.RecommendedMinutes via PreparedBy)` |
| **Users.CookingStyle** | formula | `If(CookCount = 0, "not cooking", If(AvgRecipeMinutesAttempted < 10, "snacking", If(AvgRecipeMinutesAttempted < 30, "short-order", "slow-cooking")))` |
| **Timers.WindFraction** | formula | `WindAngleDegrees / MaxWindDegrees` |
| **Timers.StoredTorqueNm** | formula | `WindFraction * MaxSpringTorqueNm` |
| **Timers.RemainingMinutes** | formula | `WindAngleDegrees / 6` |
| **Timers.WindState** | formula | `If(WindAngleDegrees <= 0, "Unwound", If(WindAngleDegrees >= MaxWindDegrees, "Full", "Partial"))` |
| **Timers.IsArmed** | formula | `RemainingMinutes > 0` |
| **Timers.HasRung** | formula | `Not(IsArmed)` |
| **Timers.TickCount** | rollup | `Count(TickEvents via HappenedOnTimer)` |
| **Timers.RingCount** | rollup | `Count(RingEvents via HappenedOnTimer)` |
| **Timers.CookCount** | rollup | `Count(Cooks via OnTimer)` |
| **Timers.TotalRecipeMinutesUsed** | rollup | `Sum(Cooks.RecommendedMinutes via OnTimer)` |
| **Timers.AvgRecipeMinutesUsed** | rollup | `Average(Cooks.RecommendedMinutes via OnTimer)` |
| **Timers.WindDurationMinutes** | formula | `MaxWindDegrees / 6` |
| **Timers.ExpectedTicksPerMinute** | formula | `EscapementBaseHz * 60` |
| **Timers.ExpectedTotalTicks** | formula | `ExpectedTicksPerMinute * WindDurationMinutes` |
| **Timers.TicksElapsedExpected** | formula | `ExpectedTicksPerMinute * (WindDurationMinutes - RemainingMinutes)` |
| **Timers.TickEfficiencyPercent** | formula | `If(TicksElapsedExpected > 0, (TickCount / TicksElapsedExpected) * 100, 100)` |
| **Timers.HasSparseTickRecord** | formula | `If(TicksElapsedExpected > 0, TickCount < TicksElapsedExpected, False())` |
| **Timers.MechanicalPhase** | formula | `If(HasRung, "rung", If(WindFraction > 0.8, "freshly-wound", If(WindFraction > 0.25, "mid-run", "nearly-done")))` |
| **Bells.TimesRung** | rollup | `Count(RingEvents via Struck)` |
| **Bells.HasEverRung** | formula | `TimesRung > 0` |
| **WindingKnobs.TimerWindAngle** | lookup | `Lookup(Timers.WindAngleDegrees via FixedToTimer)` |
| **WindingKnobs.TimerMaxWind** | lookup | `Lookup(Timers.MaxWindDegrees via FixedToTimer)` |
| **WindingKnobs.ScaleReading** | formula | `ScaleMin + (TimerWindAngle / TimerMaxWind) * (ScaleMax - ScaleMin)` |
| **GearTrains.TimerEscapementHz** | lookup | `Lookup(Timers.EscapementBaseHz via InTimer)` |
| **GearTrains.OutputShaftRPM** | formula | `(TimerEscapementHz * 60) / ReductionRatio` |
| **GearTrains.GearCount** | rollup | `Count(Gears via InGearTrain)` |
| **GearTrains.TotalTeeth** | rollup | `Sum(Gears.ToothCount via InGearTrain)` |
| **GearTrains.AverageToothCount** | rollup | `Average(Gears.ToothCount via InGearTrain)` |
| **Gears.IsFirstInChain** | formula | `PositionInChain = 1` |
| **Gears.IsLastInChain** | formula | `PositionInChain = 3` |
| **Gears.SizeClass** | formula | `If(ToothCount >= 40, "large", If(ToothCount >= 25, "medium", "small"))` |
| **Gears.OnTimer** | lookup | `Lookup(GearTrains.InTimer via InGearTrain)` |
| **OutputShafts.CurrentRPM** | lookup | `Lookup(GearTrains.OutputShaftRPM via InGearTrain)` |
| **Escapements.TickCount** | rollup | `Count(TickEvents via AtEscapement)` |
| **Escapements.HasTicked** | formula | `TickCount > 0` |
| **BellHammers.TimerHasRung** | lookup | `Lookup(Timers.HasRung via InTimer)` |
| **BellHammers.ReleaseState** | formula | `If(TimerHasRung, "Released", "Held")` |
| **HammerCatches.HammerReleaseState** | lookup | `Lookup(BellHammers.ReleaseState via OfBellHammer)` |
| **HammerCatches.IsEngaged** | formula | `If(HammerReleaseState = "Held", True(), False())` |
| **WindActions.AppliedToTimer** | lookup | `Lookup(WindingKnobs.FixedToTimer via TurnedKnob)` |
| **WindActions.ResultedInRing** | lookup | `Lookup(Timers.HasRung via AppliedToTimer)` |
| **TickEvents.TimerLabel** | lookup | `Lookup(Timers.Label via HappenedOnTimer)` |
| **TickEvents.OnTimerThatHasRung** | lookup | `Lookup(Timers.HasRung via HappenedOnTimer)` |
| **TickEvents.OnTimerInPhase** | lookup | `Lookup(Timers.MechanicalPhase via HappenedOnTimer)` |
| **Cooks.TimerHasRung** | lookup | `Lookup(Timers.HasRung via OnTimer)` |
| **Cooks.TimerIsArmed** | lookup | `Lookup(Timers.IsArmed via OnTimer)` |
| **Cooks.TimerRemainingMin** | lookup | `Lookup(Timers.RemainingMinutes via OnTimer)` |
| **Cooks.Status** | formula | `If(TimerHasRung, "Done", If(TimerIsArmed, "Cooking", "Pending"))` |
| **Cooks.RecommendedMinutes** | lookup | `Lookup(Recipes.RecommendedMinutes via FollowsRecipe)` |
| **Cooks.TimerWindDuration** | lookup | `Lookup(Timers.WindDurationMinutes via OnTimer)` |
| **Cooks.TimerCanCoverRecipe** | formula | `TimerWindDuration >= RecommendedMinutes` |
| **Cooks.SuitabilityVerdict** | formula | `If(TimerWindDuration < RecommendedMinutes, "insufficient", If(TimerWindDuration > (RecommendedMinutes * 3), "overkill", "appropriate"))` |
| **Cooks.PreparedByName** | lookup | `Lookup(Users.DisplayName via PreparedBy)` |
| **Cooks.PreparedByActivityLevel** | lookup | `Lookup(Users.ActivityLevel via PreparedBy)` |
| **Cooks.RecipeDurationCategory** | lookup | `Lookup(Recipes.DurationCategory via FollowsRecipe)` |
| **Cooks.RecipeTempProfile** | lookup | `Lookup(Recipes.CookingTempProfile via FollowsRecipe)` |
| **Cooks.TimerMechanicalPhase** | lookup | `Lookup(Timers.MechanicalPhase via OnTimer)` |
| **Recipes.TimesUsedToday** | rollup | `Count(Cooks via FollowsRecipe)` |
| **Recipes.DurationCategory** | formula | `If(RecommendedMinutes < 5, "quick", If(RecommendedMinutes < 15, "short", If(RecommendedMinutes < 45, "medium", "long")))` |
| **Recipes.CookingTempProfile** | formula | `If(IdealTemperatureC >= 200, "hot oven", If(IdealTemperatureC >= 150, "warm oven", If(IdealTemperatureC >= 90, "boiling", "low")))` |
| **Recipes.AvgTimerCapacity** | rollup | `Average(Cooks.TimerWindDuration via FollowsRecipe)` |
| **Recipes.TotalTimerCapacity** | rollup | `Sum(Cooks.TimerWindDuration via FollowsRecipe)` |
| **Recipes.AssignmentVerdict** | formula | `If(TimesUsedToday = 0, "unused", If(AvgTimerCapacity < RecommendedMinutes, "under-provisioned", If(AvgTimerCapacity > (RecommendedMinutes * 3), "over-provisioned", "well-matched")))` |
| **Kitchens.TotalTimers** | rollup | `Count(Timers.TimerId)` |
| **Kitchens.TotalCases** | rollup | `Count(Cases.CaseId)` |
| **Kitchens.TotalBells** | rollup | `Count(Bells.BellId)` |
| **Kitchens.TotalUsers** | rollup | `Count(Users.UserId)` |
| **Kitchens.TotalCooks** | rollup | `Count(Cooks.CookId)` |
| **Kitchens.TotalRecipes** | rollup | `Count(Recipes.RecipeId)` |
| **Kitchens.TotalWindActions** | rollup | `Count(WindActions.WindActionId)` |
| **Kitchens.TotalTickEvents** | rollup | `Count(TickEvents.TickEventId)` |
| **Kitchens.TotalRingEvents** | rollup | `Count(RingEvents.RingEventId)` |
| **Kitchens.TimersFreshlyWound** | rollup | `Count(Timers.MechanicalPhase, "freshly-wound")` |
| **Kitchens.TimersMidRun** | rollup | `Count(Timers.MechanicalPhase, "mid-run")` |
| **Kitchens.TimersNearlyDone** | rollup | `Count(Timers.MechanicalPhase, "nearly-done")` |
| **Kitchens.TimersRung** | rollup | `Count(Timers.MechanicalPhase, "rung")` |
| **Kitchens.CooksDone** | rollup | `Count(Cooks.Status, "Done")` |
| **Kitchens.CooksCooking** | rollup | `Count(Cooks.Status, "Cooking")` |
| **Kitchens.RecipesUnused** | rollup | `Count(Recipes.AssignmentVerdict, "unused")` |
| **Kitchens.RecipesUnderProvisioned** | rollup | `Count(Recipes.AssignmentVerdict, "under-provisioned")` |
| **Kitchens.RecipesWellMatched** | rollup | `Count(Recipes.AssignmentVerdict, "well-matched")` |
| **Kitchens.BusyUsers** | rollup | `Count(Users.ActivityLevel, "busy")` |
| **Kitchens.IdleUsers** | rollup | `Count(Users.ActivityLevel, "idle")` |
| **Kitchens.TotalRecipeMinutes** | rollup | `Sum(Recipes.RecommendedMinutes)` |
| **Kitchens.AvgRecipeMinutes** | rollup | `Average(Recipes.RecommendedMinutes)` |
| **Kitchens.MinRecipeMinutes** | rollup | `Min(Recipes.RecommendedMinutes)` |
| **Kitchens.MaxRecipeMinutes** | rollup | `Max(Recipes.RecommendedMinutes)` |
| **Kitchens.HasAnyTimerRung** | formula | `TimersRung > 0` |
| **Kitchens.HasMisprovisionedRecipe** | formula | `RecipesUnderProvisioned > 0` |
| **Kitchens.OperationalStatus** | formula | `If(CooksCooking = 0, "quiet", If(CooksCooking >= 3, "busy", "active"))` |

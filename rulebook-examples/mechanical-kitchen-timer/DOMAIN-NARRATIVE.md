# Domain Narrative — One Morning in Main Kitchen

> A long phone-call transcript from the line manager of Main Kitchen, walking a colleague through the morning of May 21, 2026. Every entity in the database gets its own paragraph or multiple paragraphs; every raw fact about every entity is stated in flowing English with all the relationships spelled out as cross-references; the same fact may be restated several times from different angles. Inferred values — counts, averages, fuzzy verdicts, categorical labels — are deliberately omitted; those are the gaps the rulebook is meant to fill in. The substrate-equivalence claim is that a reader with this narrative plus the rulebook should be able to recompute everything the export contains, without ever looking at the export.

---

## Setting the scene

Okay, let me ground you before I get into the actual play-by-play, because there's a lot moving through here and you'll want to be able to find each piece when we go to look records up. Roughly speaking: eight people on staff today, four mechanical wind-up timers sitting out on the counter with twenty little parts inside each of them, nineteen recipes in the book the kitchen pulls from, eighteen actual cooks logged this morning, twenty-three tick events sampled off the escapements, five wind actions on the knobs, and one single ding from one of the bells. Those are the rough counts you'd hit if you started counting rows; everything below is going to walk through one of those groups.

And just to head off the question — we're talking about one kitchen. Just the one. Internally it goes by `Main`, plain and simple, just that one word capitalized. On any screen or printout where it shows up by its display name, it reads "Main Kitchen", capital M, capital K. We don't run a sister kitchen across town, we don't have a satellite location we share staff with, and we don't carve out the prep area as its own kitchen for accounting or anything. One kitchen, named `Main`, displayed as "Main Kitchen". Every single thing I'm about to describe — every cook, every wind, every tick, every ring — all of it happened inside `Main`.

About the day and the timestamps. The day was Wednesday, May the twenty-first of twenty twenty-six. ISO it down and that's 2026-05-21, which is the date prefix you'll see hanging off the front of every single timestamp I'm going to quote from here on. The clocks in here run on Central Daylight Time — the summer version of Central, which sits five hours back of UTC — and the database logs match what the clocks say, so every timestamp string you'll see ends with the offset "-05:00" on its right-hand side. The shape to anchor on: take two forty-five in the morning of that day and it comes out written as `2026-05-21T02:45:00-05:00`. The capital T in the middle is just the separator between the date side and the time-of-day side. The "-05:00" trailing off is the timezone offset, not somebody subtracting five hours after the fact. Any time you pull a record from any table and see a timestamp ending in "-05:00", that's why — that's just the format every timestamp around here happens to use.

## The people on the roster

Eight people clocked in for the kitchen session today, which gives us eight people sitting on the user roster. Let me run through them one at a time — each of them gets a paragraph — because every person around here has two names you'll need to know. There's the internal identifier the system uses to track them, and there's the display-name version that's basically their job title in the kitchen. They line up one-to-one — no two people share an identifier, and no two people share a display name — so once you know either one you've got the person.

Top of the pile is the Chef. We just call him Marcus around here, because that's his name, and the system files him under `Marcus` for exactly that reason. His display name on the roster is the single word "Chef", capital C — that's the role, that's the label, that's what shows up next to his identifier on any list you pull. Marcus is the Chef, the Chef is Marcus, same person told from two angles. He runs the whole place top to bottom, and his name is going to crop up all over the cook log and the wind log when I get to the morning's events.

Right behind him in the early shift is Lena, who's the Baker. Identifier `Lena`; display name "Baker", capital B, just the one word. Bread, breakfast prep, the whole quiet morning routine — that's her territory. The row that says `Lena` is the row that says "Baker", and the row that says "Baker" is the row that says `Lena`. And one thing worth flagging early: Lena's the only person other than Marcus who actually winds a timer today.

Then there's Eli, who's our Apprentice — `Eli` on the identifier side, "Apprentice" on the display-name side, capital A. He's only been with us a couple of weeks, still mostly watching and learning, and that shows up in the data: zero cook records attributed to him, zero wind actions attributed to him. But he's on staff and he was clocked in for the session, so he gets his row on the roster the same as everybody else.

Marcus's number two is Carmen. Internally that's `Carmen`; her display name is "Sous Chef" with both words capitalized. The row whose identifier is `Carmen` is the same row whose display name is "Sous Chef" — one row, two columns, one person. Carmen will turn up cooking three of the morning's dishes herself and orbiting Marcus on a fourth.

Down at the sink end is Tomas, who's the Dishwasher — `Tomas` on the identifier, "Dishwasher" (capital D, single word) on the display. He does what dishwashers do all morning. Like Eli, though, he doesn't show up in either the cook log or the wind log — no cook row anywhere has him as the preparer, no wind action anywhere has him as the performer. He's just on the roster, present and counted.

Working a station on the line is Diego, our Line Cook. Identifier `Diego`; display name "Line Cook", capitals on both words. He runs one of the actual cooking stations through service, and his identifier will pop up on a couple of cook rows when things get busy. Same row, two columns — `Diego` and "Line Cook" sit on the same line in the user table.

Over on the sweet side is Iris, the Pastry Chef. `Iris` is what the system has her down as; "Pastry Chef" (capital P, capital C) is the display name on her row. She handles cakes, croissants, custards — anything that needs real precision on temperature and timing. You'll see her on two cooks later in the morning.

And the eighth person on the roster is a slightly funny one — Wendell. Identifier `Wendell`; display name "Food Critic", capital F and capital C. Wendell isn't actually on staff. He's a journalist sitting in to observe service so he can publish a write-up of it later. But the system books every human present during a tracked kitchen session as a user row, no exceptions, so there he sits as the eighth on the roster, with both his identifier and his display name filled in just like the rest.

Run the eight together as a single line and you get: `Marcus` is the Chef, `Lena` is the Baker, `Eli` is the Apprentice, `Carmen` is the Sous Chef, `Tomas` is the Dishwasher, `Diego` is the Line Cook, `Iris` is the Pastry Chef, `Wendell` is the Food Critic. Eight identifiers, eight display names, every identifier paired with exactly one display name and vice versa. Those are the eight people the rest of this morning's records are going to point back at — every cook will name one of them as the preparer, every wind action will name one of them as the person who turned the knob.

## The four timers

Now the timers. There are four of them — four mechanical wind-up timers sitting in a row out on the counter, four physical objects, four rows in the timers table. They each have a name (that's the identifier we file them under), a little label somebody scratched on them at some point, and a handful of mechanical facts that describe how each one is configured and where each one's spring is sitting right at this moment. I'm going to walk through the top-level facts on each of the four right here, and then in their own sections below I'll come back and cover every one of the twenty little sub-components inside each of them. The four of them, in the order I'll talk through, are Bessie, Olive, Rufus, and Twigs — and the labels riding along on those four are "Egg Timer", "Pasta Timer", "Roast Timer", and "Tea Timer".

So first up, Bessie. Identifier `Bessie`, label "Egg Timer". Bessie is sitting on the counter right now fully unwound — zero degrees of wind on her spring, nothing queued up, dial pointer sitting at the bottom of the scale. Crank her all the way back up and you'd be at three hundred and sixty degrees, which is the maximum she'll take — one complete turn of the knob, a full hour of run-time queued. At that fully-wound position the spring inside her holds about eight hundredths of a newton-meter of torque. Her escapement — the little regulator buried inside her that keeps the spring from just snapping back all at once — beats at four hertz. The label "Egg Timer" is purely descriptive; somebody at some point decided Bessie was the one we used for eggs, so they wrote "Egg Timer" on her, and that's it. The system doesn't care what the label says — it's just a string sitting on her row.

Second one over, Olive. Identifier `Olive`, label "Pasta Timer". Olive's got a hundred and twenty degrees of wind on her right now — roughly a third of a full wind, which works out to about twenty minutes of remaining run-time at the standard six-degrees-buys-you-one-minute conversion. Other than the wind reading, Olive looks just like Bessie on the timer row: same three hundred and sixty degrees of maximum wind, same eight hundredths of a newton-meter of stored torque at full wind, same four-hertz escapement. The only thing that distinguishes the two of them at this level — once you set aside the obvious things, their names and labels — is the current wind angle. Bessie's at zero. Olive's at one twenty. Olive's spring still has some tension on it. Bessie's doesn't.

Third one, Rufus, the big one of the morning. Identifier `Rufus`, label "Roast Timer". Rufus is the fully-wound timer on the counter right now — three hundred and sixty degrees of wind, parked right up at his mechanical maximum, a full sixty minutes of run-time queued up and waiting to go. He's the only one of the four right now whose current wind matches his max. The other top-level numbers on him are exactly the same as the rest of the timers — three hundred and sixty degrees of maximum wind, eight hundredths of a newton-meter of max spring torque, four hertz at the escapement. Big sturdy timer ready for a long cook, basically.

And the fourth one, Twigs, the little one. Identifier `Twigs`, label "Tea Timer". Twigs is wound to thirty degrees right now, which translates to five minutes of run-time left at the six-degrees-per-minute conversion. Of the three timers on the counter that still have any wind at all on them — Olive, Rufus, and Twigs — Twigs is the one carrying the least. (Bessie's already at zero, so she doesn't count.) Top-level she matches the other three in every other respect: three hundred and sixty degrees of maximum wind, eight hundredths of a newton-meter of max spring torque, four-hertz escapement. Just a touch of wind sitting on her, but enough for the short brew you'd use a tea timer for.

So to put the four of them next to each other in one summary breath: Bessie is at zero degrees of wind right now, Olive is at one twenty, Rufus is fully wound at three sixty, Twigs is at thirty. Those four numbers — zero, one twenty, three sixty, thirty — are what varies across the four timer rows at the top level. Everything else about them matches across the four: the maximum wind is three hundred and sixty on every timer, the maximum stored spring torque is eight hundredths of a newton-meter on every timer, the escapement beats at four hertz on every timer. And the underlying mechanical geometry — six degrees of knob rotation buying one minute of run-time, which just falls out of three sixty degrees mapping to sixty minutes — applies the same way to all four of them.

## How a timer is built

Before I name all eighty sub-components, let me describe how the mechanism actually works once, because it's the same on all four timers and I want you to have the structural picture in mind when we get to the parts. You pick a timer up by the case — the case is the outer housing, the thing your hand grips. On the front of the case is the winding knob. The winding knob has a shaft running down through the case, and that shaft is four millimeters in diameter on every one of our four knobs. You turn the knob clockwise to wind it; every knob in our kitchen winds clockwise. A full turn is three hundred and sixty degrees and that full turn buys exactly sixty minutes of run-time, so the geometry is six degrees of knob travel per minute of timer run-time. Around the rim of each knob is a printed scale that runs from zero to sixty — minutes — and it reads against a fixed pointer on the case. The pointer is fixed on every knob, meaning the pointer doesn't move; the knob rotates underneath it as the spring unwinds.

The knob shaft is bolted to a central arbor inside the case. Each arbor is three millimeters in diameter. As the knob rotates the arbor rotates with it, and that rotation winds up the mainspring. The mainspring is a flat coiled steel spring sitting in a drum on the arbor; the drum is eighteen millimeters in diameter on every one of our four mainsprings. The spring material is spring steel on every one of them. The inner coil end of every mainspring anchors to an Arbor — that's the role label "Arbor" with a capital A, stored as a string in the database, not a foreign key. The outer coil end of every mainspring anchors to a Case — likewise stored as the string "Case", not a foreign key. So the mainspring's two ends are anchored to "Arbor" and "Case" respectively, on every mainspring. When the spring is wound all the way up, it stores about zero-point-zero-eight newton-meters of torque — that's the same max-spring-torque value that sits on each timer row.

The arbor also carries a cam — a profiled piece of metal that rides around with the arbor as the spring unwinds. Every cam in our kitchen has its notch positioned at zero degrees of arbor rotation. The notch's position matters because when the arbor unwinds all the way back to zero, the cam's notch is positioned exactly where the hammer catch is sitting. At that moment the catch — which had been riding on the smooth outer profile of the cam — drops into the notch, and that release lets the bell hammer go.

The bell hammer assembly on each timer carries its own arm-spring storing two hundredths of a newton-meter of torque. The hammer arm is twenty-two millimeters long on every one of the four arms. On the tip of the arm sits a striker — a small chunk of steel; the striker material is the single word "steel" in the database on every one of them. While the catch holds the arm, the arm is loaded and stationary. The moment the catch drops, the arm snaps forward and the striker hits the bell mounted on top of the case. The bells are forty millimeters in diameter and have a dominant tone of one thousand eight hundred hertz, on every one of our four bells.

Between the arbor and the dial pointer that the user reads sits a three-gear gear train with a total reduction ratio of two hundred and forty on every one of our four gear trains. Inside each gear train are three gears that mesh in series. In chain order from the arbor side out to the escapement side, the three gears have tooth counts of forty-eight, then thirty-six, then twenty-four; those are the standard tooth counts on every gear train. At the far end of the train sits the output shaft, and on the output shaft is mounted the dial pointer. Every dial pointer is fourteen millimeters long. With this combination of mainspring torque, gear ratio, and escapement frequency, the output shaft turns at one revolution per minute, which is what a sixty-minute dial pointer needs.

The escapement is the regulator. Each escapement contains a thirty-toothed escapement wheel and a two-pronged pallet that pivots back and forth across the teeth. Every pallet has two prongs and weighs eight tenths of a gram. Attached to each pallet is a hairspring with a torsional stiffness of four ten-thousandths of a newton-meter per radian — that's zero-point-zero-zero-zero-four in numeric form. Pallet mass and hairspring stiffness together set the pallet's natural rocking rate, which is four hertz on every one of our four escapements. Four rocks per second, two hundred and forty per minute, fourteen thousand four hundred over a full hour wind. Each rock releases one tooth of the wheel.

So that's the mechanism. Every timer in our kitchen has that structure inside it. The only mechanical thing that varies from timer to timer — beyond the current wind angle that I'll get back to — is the case material on one of them. Bessie's case, Rufus's case, and Twigs's case are all chromed steel. Olive's case is brushed aluminum. So the case material is "chromed steel" on three of the four cases and "brushed aluminum" on one. Olive is the only timer in the kitchen with a non-chromed-steel case.

## Bessie's twenty sub-components

Now let me walk through Bessie's twenty sub-components, one paragraph each, naming each identifier and stating its relationships explicitly.

Bessie's outer housing is the case `Bessie's case`. The case `Bessie's case` houses the timer `Bessie`. Its material is "chromed steel". So `Bessie's case` is a chromed-steel case that houses the timer `Bessie`. Conversely the timer `Bessie` is housed by the case `Bessie's case`. Same relationship told from both sides.

Mounted on top of `Bessie's case` is the bell `Bessie's bell`. The bell `Bessie's bell` is mounted on the case `Bessie's case`. Its diameter is forty millimeters. Its tone is one thousand eight hundred hertz. So `Bessie's bell` is a forty-millimeter, eighteen-hundred-hertz bell mounted on the chromed-steel case `Bessie's case`, which houses Bessie.

Bessie's winding knob is `Bessie's knob`. The knob `Bessie's knob` is fixed to the timer `Bessie`. Its scale runs from zero (the scale-min) to sixty (the scale-max). Its wind direction is "clockwise". Its shaft diameter is four millimeters. Its pointer-fixed flag is true — the pointer on Bessie's case doesn't move; the knob rotates under it. So `Bessie's knob` is the clockwise-winding, four-millimeter-shafted, fixed-pointer knob fixed to Bessie, with a zero-to-sixty scale on the rim.

Inside Bessie sits her arbor, `Bessie's arbor`. The arbor `Bessie's arbor` is inside the timer `Bessie`. Its diameter is three millimeters. So `Bessie's arbor` is a three-millimeter shaft inside Bessie.

Wrapped on `Bessie's arbor` is the mainspring `Bessie's mainspring`. The mainspring `Bessie's mainspring` is wrapped on the arbor `Bessie's arbor`. Its drum is eighteen millimeters in diameter. Its material is "spring steel". Its inner-end anchor is "Arbor" (the role label string). Its outer-end anchor is "Case". So `Bessie's mainspring` is a spring-steel coil in an eighteen-millimeter drum, wrapped on Bessie's arbor, with its inner end anchored to the arbor role and its outer end anchored to the case role.

Riding on the same `Bessie's arbor` is the cam `Bessie's cam`. The cam `Bessie's cam` is on the arbor `Bessie's arbor`. Its notch angle is zero degrees. So `Bessie's cam` is the cam riding on Bessie's arbor with its notch positioned at the zero-degree point of arbor rotation — and when the arbor unwinds back to zero, that notch will line up with `Bessie's hammer catch`.

Inside Bessie also sits her gear train, `Bessie's gear train`. The gear train `Bessie's gear train` is in the timer `Bessie`. Its reduction ratio is two hundred and forty. So `Bessie's gear train` is the two-hundred-forty-ratio three-gear stack inside Bessie.

Inside `Bessie's gear train` are three gears. The first one is `Bessie's first gear`. The gear `Bessie's first gear` is in the gear train `Bessie's gear train`. Its position in the chain is one — meaning it's nearest the arbor side. Its tooth count is forty-eight. So `Bessie's first gear` is the forty-eight-toothed gear at position one in Bessie's gear train.

The second gear in `Bessie's gear train` is `Bessie's second gear`. The gear `Bessie's second gear` is in the gear train `Bessie's gear train`. Its position in the chain is two. Its tooth count is thirty-six. So `Bessie's second gear` is the thirty-six-toothed gear at position two in Bessie's gear train, meshing between Bessie's first gear and Bessie's third gear.

The third gear in `Bessie's gear train` is `Bessie's third gear`. The gear `Bessie's third gear` is in the gear train `Bessie's gear train`. Its position in the chain is three — meaning it's nearest the escapement side. Its tooth count is twenty-four. So `Bessie's third gear` is the twenty-four-toothed gear at position three in Bessie's gear train.

The output shaft of `Bessie's gear train` is `Bessie's output shaft`. The output shaft `Bessie's output shaft` is in the gear train `Bessie's gear train`. So `Bessie's output shaft` is the shaft at the end of Bessie's gear train.

Mounted on `Bessie's output shaft` is the dial pointer `Bessie's dial pointer`. The dial pointer `Bessie's dial pointer` is mounted on the output shaft `Bessie's output shaft`. Its length is fourteen millimeters. So `Bessie's dial pointer` is a fourteen-millimeter pointer mounted on Bessie's output shaft.

Inside Bessie also sits her escapement, `Bessie's escapement`. The escapement `Bessie's escapement` is in the timer `Bessie`. So `Bessie's escapement` is the regulator inside Bessie that holds back the mainspring.

Inside `Bessie's escapement` sits her escapement wheel, `Bessie's escapement wheel`. The escapement wheel `Bessie's escapement wheel` is in the escapement `Bessie's escapement`. Its tooth count is thirty. So `Bessie's escapement wheel` is the thirty-toothed wheel inside Bessie's escapement.

Inside `Bessie's escapement` also sits her pallet, `Bessie's pallet`. The pallet `Bessie's pallet` is in the escapement `Bessie's escapement`. Its prong count is two. Its mass is eight tenths of a gram. So `Bessie's pallet` is the two-pronged, eight-tenths-of-a-gram pallet inside Bessie's escapement.

Attached to `Bessie's pallet` is the hairspring `Bessie's hairspring`. The hairspring `Bessie's hairspring` is attached to the pallet `Bessie's pallet`. Its torsional stiffness is four ten-thousandths of a newton-meter per radian. So `Bessie's hairspring` is the standard-stiffness hairspring attached to Bessie's pallet.

Inside Bessie also sits her bell hammer subsystem, `Bessie's bell hammer`. The bell hammer `Bessie's bell hammer` is in the timer `Bessie`. Its arm-spring torque is two hundredths of a newton-meter. So `Bessie's bell hammer` is the bell hammer subsystem inside Bessie with the standard arm-spring torque.

The hammer arm of `Bessie's bell hammer` is `Bessie's hammer arm`. The hammer arm `Bessie's hammer arm` is of the bell hammer `Bessie's bell hammer`. Its length is twenty-two millimeters. So `Bessie's hammer arm` is the twenty-two-millimeter arm belonging to Bessie's bell hammer.

On `Bessie's hammer arm` sits the striker `Bessie's striker`. The striker `Bessie's striker` is on the arm `Bessie's hammer arm`. Its material is "steel". So `Bessie's striker` is the steel striker mounted on Bessie's hammer arm.

And the catch holding `Bessie's hammer arm` back is `Bessie's hammer catch`. The hammer catch `Bessie's hammer catch` is of the bell hammer `Bessie's bell hammer`. It is engaged with the cam `Bessie's cam`. So `Bessie's hammer catch` belongs to Bessie's bell hammer on one side and is engaged against Bessie's cam on the other. That catch-cam engagement is the one cross-cutting relationship in Bessie's assembly tree — everything else hangs straight down from Bessie, but this one link goes sideways between two subsystems.

## Olive's twenty sub-components

Olive's twenty sub-components mirror Bessie's, with the same relationship pattern but Olive-prefixed identifiers and one important material difference on the case.

Olive's outer housing is the case `Olive's case`. The case `Olive's case` houses the timer `Olive`. Its material is "brushed aluminum" — Olive's case is the only case in the kitchen that is brushed aluminum rather than chromed steel. So `Olive's case` is the brushed-aluminum case that houses the timer `Olive`, the lone non-chromed-steel case in our four.

Mounted on top of `Olive's case` is the bell `Olive's bell`. The bell `Olive's bell` is mounted on the case `Olive's case`. Its diameter is forty millimeters. Its tone is one thousand eight hundred hertz. So `Olive's bell` is a standard forty-millimeter, eighteen-hundred-hertz bell mounted on the brushed-aluminum case `Olive's case`, which houses Olive.

Olive's winding knob is `Olive's knob`. The knob `Olive's knob` is fixed to the timer `Olive`. Its scale runs from zero to sixty. Its wind direction is "clockwise". Its shaft diameter is four millimeters. Its pointer-fixed flag is true. So `Olive's knob` is the standard zero-to-sixty, clockwise, four-millimeter-shafted, fixed-pointer knob fixed to Olive.

Inside Olive sits her arbor, `Olive's arbor`. The arbor `Olive's arbor` is inside the timer `Olive`. Its diameter is three millimeters. So `Olive's arbor` is a three-millimeter shaft inside Olive.

Wrapped on `Olive's arbor` is the mainspring `Olive's mainspring`. The mainspring `Olive's mainspring` is wrapped on the arbor `Olive's arbor`. Its drum is eighteen millimeters in diameter. Its material is "spring steel". Its inner-end anchor is "Arbor". Its outer-end anchor is "Case". So `Olive's mainspring` is a spring-steel coil in an eighteen-millimeter drum, wrapped on Olive's arbor, with standard inner-and-outer anchoring.

Riding on the same `Olive's arbor` is the cam `Olive's cam`. The cam `Olive's cam` is on the arbor `Olive's arbor`. Its notch angle is zero degrees. So `Olive's cam` is the cam on Olive's arbor with its notch at zero.

Inside Olive sits her gear train, `Olive's gear train`. The gear train `Olive's gear train` is in the timer `Olive`. Its reduction ratio is two hundred and forty. So `Olive's gear train` is the standard three-gear stack inside Olive.

Inside `Olive's gear train` are three gears. The first one is `Olive's first gear`. The gear `Olive's first gear` is in the gear train `Olive's gear train`. Its position in the chain is one. Its tooth count is forty-eight. So `Olive's first gear` is the forty-eight-toothed gear at position one in Olive's gear train.

The second gear in `Olive's gear train` is `Olive's second gear`. The gear `Olive's second gear` is in `Olive's gear train`. Its position is two. Its tooth count is thirty-six. So `Olive's second gear` is the thirty-six-toothed gear at position two in Olive's gear train.

The third gear in `Olive's gear train` is `Olive's third gear`. The gear `Olive's third gear` is in `Olive's gear train`. Its position is three. Its tooth count is twenty-four. So `Olive's third gear` is the twenty-four-toothed gear at position three in Olive's gear train.

The output shaft of `Olive's gear train` is `Olive's output shaft`. The output shaft `Olive's output shaft` is in the gear train `Olive's gear train`. So `Olive's output shaft` is the shaft at the end of Olive's gear train.

Mounted on `Olive's output shaft` is the dial pointer `Olive's dial pointer`. The dial pointer `Olive's dial pointer` is mounted on `Olive's output shaft`. Its length is fourteen millimeters. So `Olive's dial pointer` is a fourteen-millimeter pointer mounted on Olive's output shaft.

Inside Olive sits her escapement, `Olive's escapement`. The escapement `Olive's escapement` is in the timer `Olive`. So `Olive's escapement` is Olive's regulator.

Inside `Olive's escapement` sits her escapement wheel, `Olive's escapement wheel`. The escapement wheel `Olive's escapement wheel` is in `Olive's escapement`. Its tooth count is thirty. So `Olive's escapement wheel` is the thirty-toothed wheel inside Olive's escapement.

Inside `Olive's escapement` also sits her pallet, `Olive's pallet`. The pallet `Olive's pallet` is in `Olive's escapement`. Its prong count is two. Its mass is eight tenths of a gram. So `Olive's pallet` is the standard two-pronged, eight-tenths-of-a-gram pallet inside Olive's escapement.

Attached to `Olive's pallet` is the hairspring `Olive's hairspring`. The hairspring `Olive's hairspring` is attached to `Olive's pallet`. Its torsional stiffness is four ten-thousandths of a newton-meter per radian. So `Olive's hairspring` is the standard-stiffness hairspring attached to Olive's pallet.

Inside Olive also sits her bell hammer subsystem, `Olive's bell hammer`. The bell hammer `Olive's bell hammer` is in the timer `Olive`. Its arm-spring torque is two hundredths of a newton-meter. So `Olive's bell hammer` is Olive's standard bell hammer.

The hammer arm of `Olive's bell hammer` is `Olive's hammer arm`. The hammer arm `Olive's hammer arm` is of the bell hammer `Olive's bell hammer`. Its length is twenty-two millimeters. So `Olive's hammer arm` is the twenty-two-millimeter arm belonging to Olive's bell hammer.

On `Olive's hammer arm` sits the striker `Olive's striker`. The striker `Olive's striker` is on the arm `Olive's hammer arm`. Its material is "steel". So `Olive's striker` is the steel striker on Olive's hammer arm.

And the catch holding `Olive's hammer arm` back is `Olive's hammer catch`. The hammer catch `Olive's hammer catch` is of the bell hammer `Olive's bell hammer`. It is engaged with the cam `Olive's cam`. So `Olive's hammer catch` belongs to Olive's bell hammer and is engaged with Olive's cam, the cross-cutting link in Olive's assembly.

## Rufus's twenty sub-components

Rufus's twenty sub-components mirror Bessie's and Olive's. His case is back to chromed steel.

Rufus's outer housing is the case `Rufus's case`. The case `Rufus's case` houses the timer `Rufus`. Its material is "chromed steel". So `Rufus's case` is a chromed-steel case housing the timer `Rufus`.

Mounted on top of `Rufus's case` is the bell `Rufus's bell`. The bell `Rufus's bell` is mounted on `Rufus's case`. Its diameter is forty millimeters. Its tone is one thousand eight hundred hertz. So `Rufus's bell` is a standard forty-millimeter, eighteen-hundred-hertz bell on Rufus's case.

Rufus's winding knob is `Rufus's knob`. The knob `Rufus's knob` is fixed to the timer `Rufus`. Its scale runs from zero to sixty. Its wind direction is "clockwise". Its shaft diameter is four millimeters. Its pointer-fixed flag is true. So `Rufus's knob` is the standard knob fixed to Rufus.

Inside Rufus sits his arbor, `Rufus's arbor`. The arbor `Rufus's arbor` is inside the timer `Rufus`. Its diameter is three millimeters. So `Rufus's arbor` is the standard three-millimeter arbor inside Rufus.

Wrapped on `Rufus's arbor` is the mainspring `Rufus's mainspring`. The mainspring `Rufus's mainspring` is wrapped on `Rufus's arbor`. Its drum is eighteen millimeters in diameter. Its material is "spring steel". Its inner-end anchor is "Arbor". Its outer-end anchor is "Case". So `Rufus's mainspring` is the standard spring-steel mainspring wrapped on Rufus's arbor, with standard anchoring.

Riding on the same `Rufus's arbor` is the cam `Rufus's cam`. The cam `Rufus's cam` is on the arbor `Rufus's arbor`. Its notch angle is zero degrees. So `Rufus's cam` is the cam on Rufus's arbor with its notch at zero.

Inside Rufus sits his gear train, `Rufus's gear train`. The gear train `Rufus's gear train` is in the timer `Rufus`. Its reduction ratio is two hundred and forty. So `Rufus's gear train` is the standard three-gear stack inside Rufus.

Inside `Rufus's gear train` are three gears. The first one is `Rufus's first gear`. The gear `Rufus's first gear` is in `Rufus's gear train`. Its position in the chain is one. Its tooth count is forty-eight. So `Rufus's first gear` is the forty-eight-toothed gear at position one in Rufus's gear train.

The second gear is `Rufus's second gear`. The gear `Rufus's second gear` is in `Rufus's gear train`. Its position is two. Its tooth count is thirty-six. So `Rufus's second gear` is the thirty-six-toothed gear at position two in Rufus's gear train.

The third gear is `Rufus's third gear`. The gear `Rufus's third gear` is in `Rufus's gear train`. Its position is three. Its tooth count is twenty-four. So `Rufus's third gear` is the twenty-four-toothed gear at position three in Rufus's gear train.

The output shaft of `Rufus's gear train` is `Rufus's output shaft`. The output shaft `Rufus's output shaft` is in `Rufus's gear train`. So `Rufus's output shaft` is the shaft at the end of Rufus's gear train.

Mounted on `Rufus's output shaft` is the dial pointer `Rufus's dial pointer`. The dial pointer `Rufus's dial pointer` is mounted on `Rufus's output shaft`. Its length is fourteen millimeters. So `Rufus's dial pointer` is a fourteen-millimeter pointer on Rufus's output shaft.

Inside Rufus sits his escapement, `Rufus's escapement`. The escapement `Rufus's escapement` is in the timer `Rufus`. So `Rufus's escapement` is Rufus's regulator.

Inside `Rufus's escapement` sits his escapement wheel, `Rufus's escapement wheel`. The escapement wheel `Rufus's escapement wheel` is in `Rufus's escapement`. Its tooth count is thirty. So `Rufus's escapement wheel` is the standard thirty-toothed wheel in Rufus's escapement.

Inside `Rufus's escapement` also sits his pallet, `Rufus's pallet`. The pallet `Rufus's pallet` is in `Rufus's escapement`. Its prong count is two. Its mass is eight tenths of a gram. So `Rufus's pallet` is Rufus's standard pallet.

Attached to `Rufus's pallet` is the hairspring `Rufus's hairspring`. The hairspring `Rufus's hairspring` is attached to `Rufus's pallet`. Its torsional stiffness is four ten-thousandths of a newton-meter per radian. So `Rufus's hairspring` is Rufus's standard hairspring.

Inside Rufus sits his bell hammer subsystem, `Rufus's bell hammer`. The bell hammer `Rufus's bell hammer` is in the timer `Rufus`. Its arm-spring torque is two hundredths of a newton-meter. So `Rufus's bell hammer` is Rufus's standard bell hammer.

The hammer arm of `Rufus's bell hammer` is `Rufus's hammer arm`. The hammer arm `Rufus's hammer arm` is of `Rufus's bell hammer`. Its length is twenty-two millimeters. So `Rufus's hammer arm` is the twenty-two-millimeter arm belonging to Rufus's bell hammer.

On `Rufus's hammer arm` sits the striker `Rufus's striker`. The striker `Rufus's striker` is on `Rufus's hammer arm`. Its material is "steel". So `Rufus's striker` is the steel striker on Rufus's hammer arm.

And the catch holding `Rufus's hammer arm` back is `Rufus's hammer catch`. The hammer catch `Rufus's hammer catch` is of the bell hammer `Rufus's bell hammer`. It is engaged with the cam `Rufus's cam`. So `Rufus's hammer catch` belongs to Rufus's bell hammer and is engaged with Rufus's cam.

## Twigs's twenty sub-components

Twigs's twenty sub-components mirror Bessie's, Olive's, and Rufus's. Twigs's case is chromed steel like Bessie's and Rufus's.

Twigs's outer housing is the case `Twigs's case`. The case `Twigs's case` houses the timer `Twigs`. Its material is "chromed steel". So `Twigs's case` is a chromed-steel case housing Twigs.

Mounted on top of `Twigs's case` is the bell `Twigs's bell`. The bell `Twigs's bell` is mounted on `Twigs's case`. Its diameter is forty millimeters. Its tone is one thousand eight hundred hertz. So `Twigs's bell` is a standard forty-millimeter, eighteen-hundred-hertz bell on Twigs's case.

Twigs's winding knob is `Twigs's knob`. The knob `Twigs's knob` is fixed to the timer `Twigs`. Its scale runs from zero to sixty. Its wind direction is "clockwise". Its shaft diameter is four millimeters. Its pointer-fixed flag is true. So `Twigs's knob` is the standard knob fixed to Twigs.

Inside Twigs sits her arbor, `Twigs's arbor`. The arbor `Twigs's arbor` is inside the timer `Twigs`. Its diameter is three millimeters. So `Twigs's arbor` is the three-millimeter arbor inside Twigs.

Wrapped on `Twigs's arbor` is the mainspring `Twigs's mainspring`. The mainspring `Twigs's mainspring` is wrapped on `Twigs's arbor`. Its drum is eighteen millimeters in diameter. Its material is "spring steel". Its inner-end anchor is "Arbor". Its outer-end anchor is "Case". So `Twigs's mainspring` is the standard spring-steel mainspring wrapped on Twigs's arbor.

Riding on the same `Twigs's arbor` is the cam `Twigs's cam`. The cam `Twigs's cam` is on the arbor `Twigs's arbor`. Its notch angle is zero degrees. So `Twigs's cam` is the cam on Twigs's arbor with its notch at zero.

Inside Twigs sits her gear train, `Twigs's gear train`. The gear train `Twigs's gear train` is in the timer `Twigs`. Its reduction ratio is two hundred and forty. So `Twigs's gear train` is the standard three-gear stack inside Twigs.

Inside `Twigs's gear train` are three gears. The first one is `Twigs's first gear`. The gear `Twigs's first gear` is in `Twigs's gear train`. Its position in the chain is one. Its tooth count is forty-eight. So `Twigs's first gear` is the forty-eight-toothed gear at position one in Twigs's gear train.

The second gear is `Twigs's second gear`. The gear `Twigs's second gear` is in `Twigs's gear train`. Its position is two. Its tooth count is thirty-six. So `Twigs's second gear` is the thirty-six-toothed gear at position two in Twigs's gear train.

The third gear is `Twigs's third gear`. The gear `Twigs's third gear` is in `Twigs's gear train`. Its position is three. Its tooth count is twenty-four. So `Twigs's third gear` is the twenty-four-toothed gear at position three in Twigs's gear train.

The output shaft of `Twigs's gear train` is `Twigs's output shaft`. The output shaft `Twigs's output shaft` is in `Twigs's gear train`. So `Twigs's output shaft` is the shaft at the end of Twigs's gear train.

Mounted on `Twigs's output shaft` is the dial pointer `Twigs's dial pointer`. The dial pointer `Twigs's dial pointer` is mounted on `Twigs's output shaft`. Its length is fourteen millimeters. So `Twigs's dial pointer` is a fourteen-millimeter pointer on Twigs's output shaft.

Inside Twigs sits her escapement, `Twigs's escapement`. The escapement `Twigs's escapement` is in the timer `Twigs`. So `Twigs's escapement` is Twigs's regulator.

Inside `Twigs's escapement` sits her escapement wheel, `Twigs's escapement wheel`. The escapement wheel `Twigs's escapement wheel` is in `Twigs's escapement`. Its tooth count is thirty. So `Twigs's escapement wheel` is the standard thirty-toothed wheel in Twigs's escapement.

Inside `Twigs's escapement` also sits her pallet, `Twigs's pallet`. The pallet `Twigs's pallet` is in `Twigs's escapement`. Its prong count is two. Its mass is eight tenths of a gram. So `Twigs's pallet` is Twigs's standard pallet.

Attached to `Twigs's pallet` is the hairspring `Twigs's hairspring`. The hairspring `Twigs's hairspring` is attached to `Twigs's pallet`. Its torsional stiffness is four ten-thousandths of a newton-meter per radian. So `Twigs's hairspring` is Twigs's standard hairspring.

Inside Twigs sits her bell hammer subsystem, `Twigs's bell hammer`. The bell hammer `Twigs's bell hammer` is in the timer `Twigs`. Its arm-spring torque is two hundredths of a newton-meter. So `Twigs's bell hammer` is Twigs's standard bell hammer.

The hammer arm of `Twigs's bell hammer` is `Twigs's hammer arm`. The hammer arm `Twigs's hammer arm` is of `Twigs's bell hammer`. Its length is twenty-two millimeters. So `Twigs's hammer arm` is the twenty-two-millimeter arm belonging to Twigs's bell hammer.

On `Twigs's hammer arm` sits the striker `Twigs's striker`. The striker `Twigs's striker` is on `Twigs's hammer arm`. Its material is "steel". So `Twigs's striker` is the steel striker on Twigs's hammer arm.

And the catch holding `Twigs's hammer arm` back is `Twigs's hammer catch`. The hammer catch `Twigs's hammer catch` is of the bell hammer `Twigs's bell hammer`. It is engaged with the cam `Twigs's cam`. So `Twigs's hammer catch` belongs to Twigs's bell hammer and is engaged with Twigs's cam.

## The recipe book — nineteen recipes

Now the recipe book. We have nineteen recipes catalogued in the database. Each one specifies a dish name, a recommended cook time in minutes, and an ideal cooking temperature in degrees Celsius. Let me walk through every one of them, each in its own paragraph.

The recipe `soft-boiled egg recipe` specifies the dish "soft-boiled egg", a recommended cook time of ten minutes, and an ideal temperature of one hundred degrees Celsius. So you put your egg in water at one hundred degrees — boiling — for ten minutes. The recipe `soft-boiled egg recipe` is the ten-minute, one-hundred-degree soft-boiled-egg recipe.

The recipe `spaghetti recipe` specifies the dish "spaghetti al pomodoro", a recommended cook time of twelve minutes, and an ideal temperature of one hundred degrees Celsius. So the spaghetti goes into boiling water for twelve minutes. The recipe `spaghetti recipe` is the twelve-minute, one-hundred-degree spaghetti-al-pomodoro recipe.

The recipe `rib roast recipe` specifies the dish "rib roast", a recommended cook time of ninety minutes, and an ideal temperature of one hundred and seventy-five degrees Celsius. So the rib roast goes into the oven at one hundred and seventy-five for ninety minutes. The recipe `rib roast recipe` is the ninety-minute, one-seventy-five rib-roast recipe — the longest meat dish in the book and a big commit when you start it.

The recipe `broccoli recipe` specifies the dish "steamed broccoli", a recommended cook time of twenty-five minutes, and an ideal temperature of one hundred degrees Celsius. So the broccoli steams at boiling for twenty-five minutes. The recipe `broccoli recipe` is the twenty-five-minute, one-hundred-degree steamed-broccoli recipe.

The recipe `tea recipe` specifies the dish "Earl Grey tea", a recommended cook time of four minutes, and an ideal temperature of ninety-five degrees Celsius. So the Earl Grey steeps at ninety-five — just below boiling — for four minutes. The recipe `tea recipe` is the four-minute, ninety-five-degree Earl-Grey-tea recipe.

The recipe `cake recipe` specifies the dish "vanilla cake", a recommended cook time of thirty-five minutes, and an ideal temperature of one hundred and eighty degrees Celsius. So the vanilla cake bakes at one-eighty for thirty-five minutes. The recipe `cake recipe` is the thirty-five-minute, one-eighty vanilla-cake recipe.

The recipe `pizza recipe` specifies the dish "pizza margherita", a recommended cook time of eight minutes, and an ideal temperature of two hundred and forty degrees Celsius. So the pizza margherita bakes at two hundred and forty — much hotter than the cake — for eight minutes. The recipe `pizza recipe` is the eight-minute, two-hundred-forty-degree pizza-margherita recipe; the high temperature is for the crust.

The recipe `yogurt recipe` specifies the dish "homemade yogurt", a recommended cook time of three hundred and sixty minutes, and an ideal temperature of forty degrees Celsius. So the yogurt incubates at forty for six full hours. The recipe `yogurt recipe` is the three-hundred-sixty-minute, forty-degree homemade-yogurt recipe — the marathon of the book; it's fermentation rather than cooking, which is why the temperature is so low.

The recipe `instant coffee recipe` specifies the dish "instant coffee", a recommended cook time of two minutes, and an ideal temperature of ninety degrees Celsius. So you brew at ninety for two minutes. The recipe `instant coffee recipe` is the two-minute, ninety-degree instant-coffee recipe — the fastest one in the book.

The recipe `croissant recipe` specifies the dish "butter croissant", a recommended cook time of eighteen minutes, and an ideal temperature of two hundred degrees Celsius. So the croissants bake at two hundred for eighteen minutes. The recipe `croissant recipe` is the eighteen-minute, two-hundred-degree butter-croissant recipe.

The recipe `risotto recipe` specifies the dish "mushroom risotto", a recommended cook time of thirty minutes, and an ideal temperature of one hundred degrees Celsius. So the risotto simmers at one hundred for thirty minutes. The recipe `risotto recipe` is the thirty-minute, one-hundred-degree mushroom-risotto recipe.

The recipe `sourdough recipe` specifies the dish "sourdough loaf", a recommended cook time of forty-five minutes, and an ideal temperature of two hundred and twenty degrees Celsius. So the sourdough bakes at two-twenty for forty-five minutes — hotter than the croissant because sourdough wants more crust. The recipe `sourdough recipe` is the forty-five-minute, two-twenty sourdough-loaf recipe.

The recipe `creme brulee recipe` specifies the dish "creme brulee", a recommended cook time of fifty-five minutes, and an ideal temperature of one hundred and sixty-five degrees Celsius. So the creme brulee bakes at one-sixty-five for fifty-five minutes — low and slow for the custard. The recipe `creme brulee recipe` is the fifty-five-minute, one-sixty-five creme-brulee recipe.

The recipe `paella recipe` specifies the dish "paella valenciana", a recommended cook time of forty minutes, and an ideal temperature of one hundred degrees Celsius. So the paella cooks at boiling for forty minutes. The recipe `paella recipe` is the forty-minute, one-hundred-degree paella-valenciana recipe.

The recipe `ramen recipe` specifies the dish "tonkotsu ramen", a recommended cook time of five minutes, and an ideal temperature of one hundred degrees Celsius. So the ramen noodles cook at boiling for five minutes — that's the noodles-and-assembly step; the broth has been simmering for hours separately and isn't tracked here. The recipe `ramen recipe` is the five-minute, one-hundred-degree tonkotsu-ramen recipe.

The recipe `salad recipe` specifies the dish "garden salad", a recommended cook time of seven minutes, and an ideal temperature of twenty degrees Celsius. So the garden salad sits at room temperature for seven minutes — you're not cooking the salad, you're letting it settle after dressing. The recipe `salad recipe` is the seven-minute, twenty-degree garden-salad recipe.

The recipe `steak recipe` specifies the dish "ribeye steak", a recommended cook time of eleven minutes, and an ideal temperature of two hundred and thirty degrees Celsius. So the ribeye cooks at two-thirty for eleven minutes — hot and fast. The recipe `steak recipe` is the eleven-minute, two-thirty ribeye-steak recipe.

The recipe `soup recipe` specifies the dish "vegetable soup", a recommended cook time of twenty-five minutes, and an ideal temperature of one hundred degrees Celsius. So the vegetable soup simmers at one hundred for twenty-five minutes. The recipe `soup recipe` is the twenty-five-minute, one-hundred-degree vegetable-soup recipe.

The recipe `flan recipe` specifies the dish "caramel flan", a recommended cook time of fifty minutes, and an ideal temperature of one hundred and seventy degrees Celsius. So the caramel flan bakes at one-seventy for fifty minutes. The recipe `flan recipe` is the fifty-minute, one-seventy caramel-flan recipe.

That accounts for all nineteen recipes in the book: `soft-boiled egg recipe`, `spaghetti recipe`, `rib roast recipe`, `broccoli recipe`, `tea recipe`, `cake recipe`, `pizza recipe`, `yogurt recipe`, `instant coffee recipe`, `croissant recipe`, `risotto recipe`, `sourdough recipe`, `creme brulee recipe`, `paella recipe`, `ramen recipe`, `salad recipe`, `steak recipe`, `soup recipe`, `flan recipe`.

## The eighteen cooks of the morning

Now the cooks — eighteen of them, one paragraph per cook. Each cook is a specific instance of one specific person preparing one specific dish on one specific timer following one specific recipe at one specific start time.

The cook `morning coffee` was prepared by Marcus, on Bessie, following the `instant coffee recipe`, with the dish "instant coffee", starting at 2026-05-21T02:40:00-05:00 — two-forty in the morning. So at two-forty AM Marcus made himself a coffee using Bessie and the two-minute, ninety-degree instant-coffee recipe. That was the earliest cook of the morning.

The cook `soft-boiled egg` was prepared by Marcus, on Bessie, following the `soft-boiled egg recipe`, with the dish "soft-boiled egg", starting at 2026-05-21T02:45:00-05:00 — two forty-five. So at two forty-five AM Marcus put a soft-boiled egg on Bessie, using the ten-minute, one-hundred-degree soft-boiled-egg recipe. This is the cook that lines up with Marcus's first wind action of the morning.

The cook `salad starter` was prepared by Carmen, on Bessie, following the `salad recipe`, with the dish "garden salad", starting at 2026-05-21T02:46:00-05:00 — two forty-six. So at two forty-six AM Carmen put up a garden salad on Bessie, riding on the wind state Marcus had just set, using the seven-minute, twenty-degree salad recipe.

The cook `baker's espresso` was prepared by Lena, on Olive, following the `instant coffee recipe`, with the dish "instant coffee", starting at 2026-05-21T04:25:00-05:00 — four twenty-five. So at four twenty-five AM Lena made herself an espresso using Olive and the instant coffee recipe. Same dish as Marcus's morning coffee (it's the same recipe) but a separate cook record because it's a different person on a different timer at a different time.

The cook `spaghetti al pomodoro` was prepared by Lena, on Olive, following the `spaghetti recipe`, with the dish "spaghetti al pomodoro", starting at 2026-05-21T04:30:00-05:00 — four-thirty. Note that the cook identifier here happens to be the same string as the dish name. At four-thirty AM Lena started the actual pasta on Olive, using the twelve-minute spaghetti recipe.

The cook `steamed broccoli side` was prepared by Lena, on Olive, following the `broccoli recipe`, with the dish "steamed broccoli", starting at 2026-05-21T04:32:00-05:00 — four thirty-two. So at four thirty-two Lena added the broccoli side to Olive, using the twenty-five-minute broccoli recipe.

The cook `overnight yogurt` was prepared by Lena, on Olive, following the `yogurt recipe`, with the dish "homemade yogurt", starting at 2026-05-21T04:35:00-05:00 — four thirty-five. So at four thirty-five Lena got the homemade yogurt going on Olive, using the six-hour, forty-degree yogurt recipe.

The cook `risotto dinner` was prepared by Carmen, on Olive, following the `risotto recipe`, with the dish "mushroom risotto", starting at 2026-05-21T04:40:00-05:00 — four-forty. So at four-forty Carmen put a mushroom risotto on Olive, using the thirty-minute risotto recipe — joining Lena's four concurrent cooks on Olive.

The cook `soup of the day` was prepared by Diego, on Olive, following the `soup recipe`, with the dish "vegetable soup", starting at 2026-05-21T04:45:00-05:00 — four forty-five. So at four forty-five Diego put a vegetable soup on Olive, using the twenty-five-minute soup recipe — bringing Olive's concurrent cook count to six.

The cook `rib roast` was prepared by Marcus, on Rufus, following the `rib roast recipe`, with the dish "rib roast", starting at 2026-05-21T05:00:00-05:00 — five AM sharp. So at five AM Marcus put a rib roast on Rufus, using the ninety-minute rib-roast recipe.

The cook `preheat for pizza` was prepared by Marcus, on Rufus, following the `pizza recipe`, with the dish "pizza margherita", starting at 2026-05-21T05:02:00-05:00 — five oh two. So at five oh two Marcus also started a pizza margherita on Rufus, using the eight-minute, two-hundred-forty pizza recipe.

The cook `croissant batch` was prepared by Iris, on Rufus, following the `croissant recipe`, with the dish "butter croissant", starting at 2026-05-21T05:05:00-05:00 — five oh five. So at five oh five Iris put a batch of butter croissants on Rufus, using the eighteen-minute croissant recipe.

The cook `paella special` was prepared by Carmen, on Rufus, following the `paella recipe`, with the dish "paella valenciana", starting at 2026-05-21T05:10:00-05:00 — five-ten. So at five-ten Carmen put a paella on Rufus, using the forty-minute paella recipe.

The cook `late cake bake` was prepared by Marcus, on Rufus, following the `cake recipe`, with the dish "vanilla cake", starting at 2026-05-21T05:25:00-05:00 — five twenty-five. So at five twenty-five Marcus put a vanilla cake on Rufus, using the thirty-five-minute cake recipe. Note the "late" qualifier in the cook identifier.

The cook `steak dinner` was prepared by Marcus, on Rufus, following the `steak recipe`, with the dish "ribeye steak", starting at 2026-05-21T05:30:00-05:00 — five-thirty. So at five-thirty Marcus put a ribeye steak on Rufus, using the eleven-minute steak recipe.

The cook `flan dessert` was prepared by Iris, on Rufus, following the `flan recipe`, with the dish "caramel flan", starting at 2026-05-21T05:35:00-05:00 — five thirty-five. So at five thirty-five Iris put a caramel flan on Rufus, using the fifty-minute flan recipe. Rufus is now hosting seven concurrent cooks.

The cook `afternoon tea` was prepared by Marcus, on Twigs, following the `tea recipe`, with the dish "Earl Grey tea", starting at 2026-05-21T06:25:00-05:00 — six twenty-five. So at six twenty-five Marcus brewed Earl Grey on Twigs, using the four-minute tea recipe.

The cook `ramen lunch` was prepared by Diego, on Twigs, following the `ramen recipe`, with the dish "tonkotsu ramen", starting at 2026-05-21T06:26:00-05:00 — six twenty-six. So at six twenty-six Diego put a tonkotsu ramen on Twigs, using the five-minute ramen recipe.

That covers all eighteen cooks. To summarize the preparer-attribution: Marcus prepared seven (`morning coffee`, `soft-boiled egg`, `rib roast`, `preheat for pizza`, `late cake bake`, `steak dinner`, `afternoon tea`); Lena prepared four (`baker's espresso`, `spaghetti al pomodoro`, `steamed broccoli side`, `overnight yogurt`); Carmen prepared three (`salad starter`, `risotto dinner`, `paella special`); Diego prepared two (`soup of the day`, `ramen lunch`); Iris prepared two (`croissant batch`, `flan dessert`); Eli, Tomas, and Wendell prepared zero each. The recipe references span seventeen of the nineteen recipes; the two unused-today recipes are `sourdough recipe` (nobody making sourdough loaf this morning) and `creme brulee recipe` (nobody making creme brulee this morning). The `instant coffee recipe` is the only recipe followed by two distinct cooks today (Marcus's `morning coffee` and Lena's `baker's espresso`); the other sixteen used recipes are each followed by exactly one cook.

## The five wind actions

Now the wind actions — five of them, one paragraph each.

The wind action `Marcus wound Bessie` was performed by Marcus on the knob `Bessie's knob`, with a target angle of sixty degrees, at the timestamp 2026-05-21T02:45:00-05:00 — two forty-five AM. So at two forty-five Marcus turned Bessie's knob to sixty degrees (which is ten minutes of run-time given the six-degrees-per-minute geometry). That wind coincides with the start of his `soft-boiled egg` cook.

The wind action `Lena wound Olive` was performed by Lena on the knob `Olive's knob`, with a target angle of one hundred and twenty degrees, at the timestamp 2026-05-21T04:30:00-05:00 — four-thirty AM. So at four-thirty Lena turned Olive's knob to one hundred and twenty degrees (twenty minutes of run-time). That coincides with the start of her `spaghetti al pomodoro` cook.

The wind action `Marcus wound Rufus` was performed by Marcus on the knob `Rufus's knob`, with a target angle of three hundred and sixty degrees, at the timestamp 2026-05-21T05:00:00-05:00 — five AM. So at five AM Marcus turned Rufus's knob all the way around to three hundred and sixty (full wind, sixty minutes). That coincides with the start of his `rib roast` cook.

The wind action `Marcus wound Twigs` was performed by Marcus on the knob `Twigs's knob`, with a target angle of thirty degrees, at the timestamp 2026-05-21T06:25:00-05:00 — six twenty-five AM. So at six twenty-five Marcus turned Twigs's knob to thirty degrees (five minutes of run-time). That coincides with the start of his `afternoon tea` cook.

The wind action `Marcus readjusted Bessie` was performed by Marcus on the knob `Bessie's knob`, with a target angle of sixty degrees, at the timestamp 2026-05-21T06:30:00-05:00 — six-thirty AM. So at six-thirty Marcus returned to Bessie's knob — the same knob he had wound at two forty-five — and turned it back to sixty degrees again. Note the verb in the identifier is "readjusted" rather than "wound"; the operator who logged this chose that wording specifically to mark this as a second touch on the same knob rather than a fresh initial wind. With this fifth wind action Marcus accounts for four of today's five wind actions (Bessie at two forty-five, Rufus at five AM, Twigs at six twenty-five, Bessie again at six-thirty); Lena accounts for the one remaining (Olive at four-thirty). Eli, Carmen, Tomas, Diego, Iris, and Wendell did not appear in the wind log today.

## The twenty-three tick events

Now the tick events. There are twenty-three of them. Each tick records one tooth release at one escapement at one moment in time. They're sampled — the timers actually tick four times per second so a complete log would have many thousands of entries; the database stores representative samples.

The tick event `Bessie's tick 1` happened on Bessie at `Bessie's escapement` at 2026-05-21T02:46:15-05:00 — about a minute and a quarter after Marcus wound Bessie. The tick event `Bessie's tick 2` happened on Bessie at `Bessie's escapement` at 2026-05-21T02:47:30-05:00. The tick event `Bessie's tick 3` happened on Bessie at `Bessie's escapement` at 2026-05-21T02:48:45-05:00. The tick event `Bessie's tick 4` happened on Bessie at `Bessie's escapement` at 2026-05-21T02:50:00-05:00 — five minutes after the wind, halfway through. The tick event `Bessie's tick 5` happened on Bessie at `Bessie's escapement` at 2026-05-21T02:51:15-05:00. The tick event `Bessie's tick 6` happened on Bessie at `Bessie's escapement` at 2026-05-21T02:52:30-05:00. The tick event `Bessie's tick 7` happened on Bessie at `Bessie's escapement` at 2026-05-21T02:53:45-05:00. The tick event `Bessie's tick 8` happened on Bessie at `Bessie's escapement` at 2026-05-21T02:55:00-05:00 — exactly ten minutes after the wind, the same moment Bessie's bell rang.

The tick event `Olive's tick 1` happened on Olive at `Olive's escapement` at 2026-05-21T04:35:00-05:00. The tick event `Olive's tick 2` happened on Olive at `Olive's escapement` at 2026-05-21T04:40:00-05:00. The tick event `Olive's tick 3` happened on Olive at `Olive's escapement` at 2026-05-21T04:45:00-05:00. The tick event `Olive's tick 4` happened on Olive at `Olive's escapement` at 2026-05-21T04:50:00-05:00. The tick event `Olive's tick 5` happened on Olive at `Olive's escapement` at 2026-05-21T04:55:00-05:00. The tick event `Olive's tick 6` happened on Olive at `Olive's escapement` at 2026-05-21T05:00:00-05:00. The tick event `Olive's tick 7` happened on Olive at `Olive's escapement` at 2026-05-21T05:05:00-05:00. The tick event `Olive's tick 8` happened on Olive at `Olive's escapement` at 2026-05-21T05:10:00-05:00. So eight pasta ticks at five-minute spacing.

The tick event `Rufus's tick 1` happened on Rufus at `Rufus's escapement` at 2026-05-21T05:00:30-05:00 — thirty seconds after Marcus wound Rufus. The tick event `Rufus's tick 2` happened on Rufus at `Rufus's escapement` at 2026-05-21T05:01:00-05:00. The tick event `Rufus's tick 3` happened on Rufus at `Rufus's escapement` at 2026-05-21T05:01:30-05:00. The tick event `Rufus's tick 4` happened on Rufus at `Rufus's escapement` at 2026-05-21T05:02:00-05:00. So four roast ticks logged at thirty-second spacing, all very close to Rufus's five-AM wind.

The tick event `Twigs's tick 1` happened on Twigs at `Twigs's escapement` at 2026-05-21T06:26:00-05:00. The tick event `Twigs's tick 2` happened on Twigs at `Twigs's escapement` at 2026-05-21T06:27:00-05:00. The tick event `Twigs's tick 3` happened on Twigs at `Twigs's escapement` at 2026-05-21T06:28:00-05:00. So three tea ticks at one-minute spacing.

That accounts for the twenty-three tick events: eight on Bessie's escapement, eight on Olive's escapement, four on Rufus's escapement, three on Twigs's escapement. Every tick event names a parent timer through its HappenedOnTimer field, and the escapement named in its AtEscapement field always belongs to that same parent timer — no cross-timer ticks.

## The one ring event

The ring event `Bessie's ding` happened on Bessie, struck `Bessie's bell`, at 2026-05-21T02:55:00-05:00 — two fifty-five AM. So at two fifty-five Bessie's cam notch reached `Bessie's hammer catch`, the catch dropped into the notch, `Bessie's hammer arm` snapped forward, `Bessie's striker` hit `Bessie's bell` — the forty-millimeter, eighteen-hundred-hertz bell mounted on `Bessie's case` which houses Bessie — and the bell went off. The ring event `Bessie's ding` is the only ring event in the entire database today. No other bell has rung. `Olive's bell` has not been struck. `Rufus's bell` has not been struck. `Twigs's bell` has not been struck. Just the one Bessie ding at two fifty-five.

## What I have not told you

Here's what I deliberately have not told you. Every fact below is a function of the raw observations above plus the formulas in the rulebook, and a reader equipped with the rulebook should be able to recompute every one of them without ever consulting the database export.

I have not told you, for any of the four timers, what fraction of its full wind it currently holds. The wind fractions are computed as current-wind-angle divided by max-wind-angle. The four timers have four different fractions: Bessie at zero divided by three hundred sixty, Olive at one hundred twenty divided by three hundred sixty, Rufus at three hundred sixty divided by three hundred sixty, Twigs at thirty divided by three hundred sixty. I haven't stated any of the resulting fraction values. I haven't told you, for any timer, how much torque the mainspring is currently storing — that's wind-fraction times max-spring-torque. I haven't told you how many remaining minutes any timer has — that's wind-angle divided by six. I haven't told you whether any timer is currently armed or whether it has rung — those derive from whether remaining-minutes is greater than zero. I haven't told you any timer's wind-state classification (which would be one of "Unwound", "Partial", or "Full") or mechanical-phase classification (one of "freshly-wound", "mid-run", "nearly-done", or "rung"). I haven't told you any timer's expected ticks per minute, expected total ticks across a full wind, ticks elapsed expected so far, tick-efficiency percent, or whether it has a sparse tick record. I haven't told you any timer's tick count, ring count, cook count, total recipe minutes used, or average recipe minutes used.

I haven't told you, for any of the four bells, how many times it has rung or whether it has ever rung. I haven't told you, for any of the four bell hammers, whether its release state is currently "Held" or "Released". I haven't told you, for any of the four hammer catches, whether it is currently engaged.

I haven't told you, for any of the four gear trains, the total tooth count or average tooth count or gear count or output shaft rpm. I haven't told you, for any of the twelve individual gears, its size-class classification (which would be one of "small", "medium", or "large") or its is-first-in-chain or is-last-in-chain flag. I haven't told you any output shaft's current rpm or any winding knob's current scale reading or any escapement's tick count or has-ticked flag.

I haven't told you, for any of the eight people, their wind count, cook count, completed cook count, total recipe minutes attempted, average recipe minutes attempted, activity-level classification (one of "idle", "casual", or "busy"), or cooking-style classification (one of "snacking", "short-order", "slow-cooking", or "not cooking").

I haven't told you, for any of the nineteen recipes, how many times it was used today, its duration-category classification (one of "quick", "short", "medium", or "long"), its cooking-temp profile classification (one of "low", "boiling", "warm oven", or "hot oven"), its assignment-verdict classification (one of "unused", "well-matched", "under-provisioned", or "over-provisioned"), or its total/average timer-capacity allocation.

I haven't told you, for any of the eighteen cooks, its status classification (one of "Done", "Cooking", or "Pending"), its suitability verdict (one of "insufficient", "appropriate", or "overkill"), or any of the lookups it inherits from its parent timer, user, or recipe — TimerHasRung, TimerIsArmed, TimerRemainingMin, TimerMechanicalPhase, PreparedByName, PreparedByActivityLevel, RecipeDurationCategory, RecipeTempProfile, TimerCanCoverRecipe.

I haven't told you, for any of the five wind actions, which timer it was applied to (the rulebook computes this by chasing the turned-knob foreign key through the winding-knobs table to find what timer the knob is fixed to) or whether it resulted in a ring. I haven't told you, for any of the twenty-three tick events, the parent timer's label or has-rung state or mechanical phase.

And I haven't told you any kitchen-level aggregation — not the count of timers, cases, bells, users, cooks, recipes, wind actions, tick events, or ring events at the kitchen level; not the count of timers in each mechanical-phase bucket; not the count of cooks in each status bucket; not the count of recipes in each assignment-verdict bucket; not the count of users in each activity-level bucket; not the sum, mean, minimum, or maximum of recipe minutes across the book; not whether any timer has rung today; not whether there's any misprovisioned recipe; not the kitchen's overall operational-status classification (one of "quiet", "active", or "busy").

Every one of those withheld facts is a function of the raw observations above plus the formulas in the rulebook. The substrate-equivalence test is: a reader equipped with the rulebook and this narrative should be able to recompute every one of those values without ever looking at the database export. The rulebook is one substrate — formulas in machine-readable form. This narrative is another substrate — observations in human-readable prose, decompressed so that every relationship between entities is stated as its own English sentence rather than implied by row co-location. Both substrates project the same domain. Both should answer the same questions about it.

"""Content spec for "Effortless ACME Corp: PKO" — first-pass scripts.

Every finding referenced is a real witnessed value from the
procedural-knowledge-ontology rulebook (see that project's README and
WITNESS-LOOPS.md). These are draft scripts capturing the arc, expected to be
revised once assembly starts.
"""

VIDEOS = {}

# ---------------------------------------------------------------------------
# Episode 1 — The Book of Record (trailer / cold open)
# ---------------------------------------------------------------------------
VIDEOS["01-the-book-of-record"] = {
    "title": "The Book of Record",
    "description": "Episode 1 of Effortless ACME Corp: PKO. Cold open on an unexplained "
                    "finding, then a two-minute promise: this season follows two of ACME's "
                    "real procedures and the one book that watches both.",
    "logline": "Cold open on an unexplained finding at ACME Corporation, then a two-minute "
               "promise: this season follows two of ACME's real procedures and the one book "
               "that watches both.",
    "working_title": "Effortless ACME Corp: PKO, Episode 1, The Book of Record",
    "target_length": "~2:00",
    "voice": "Cold-open energy, plain-spoken, a little true-crime. Never technical yet.",
    "running_example": "The quarter-end close's cutoff-bypass finding (ProcessMiningRuns.IsDriftOnLiveVersion), used as the unexplained hook.",
    "how_to_use": "Cold open on the finding with zero context, pull back to ACME Corp, define the Procedure Register in one beat, preview both season arcs, title card.",
    "output_file": "renders/acme-pko-01-the-book-of-record.final.mp4",
    "status": "Assembling",
    "structure": "Cold open (unexplained finding) -> pull back to ACME Corp -> define the Register -> preview two arcs -> title card.",
    "acts": [
        {"id": "act-1-finding", "order": 1, "numeral": "I", "act_title": "THE FINDING",
         "timecode": "0:00-0:35", "tagline": "one finding, no explanation yet"},
        {"id": "act-2-book", "order": 2, "numeral": "II", "act_title": "THE BOOK",
         "timecode": "0:35-2:00", "tagline": "this is ACME, and this is what watches it"},
    ],
    "scenes": [
        {"order": 1, "title": "Cutoff, Bypassed", "act": "act-1-finding", "timecode": "0:00-0:20",
         "purpose": "Cold open. On screen: a single fired finding, no labels explained yet.",
         "script": "Every quarter, ACME Corporation closes its books. This quarter, ledger entries were posted after the cutoff date. Nobody approved a reopening."},
        {"order": 2, "title": "Caught, Three Weeks Later", "act": "act-1-finding", "timecode": "0:20-0:35",
         "purpose": "Reveal it wasn't caught by a person in the moment.",
         "script": "No auditor caught it that day. It was found three weeks later, by comparing what the procedure said should happen to what the system actually recorded."},
        {"order": 3, "title": "This Is ACME", "act": "act-2-book", "timecode": "0:35-1:05",
         "purpose": "Pull back to establish the setting.",
         "script": "This is ACME Corporation. Every quarter, the books close. Every policy change reaches every employee. Both are procedures, and both have a plan, and a record of what really happened."},
        {"order": 4, "title": "The Procedure Register", "act": "act-2-book", "timecode": "1:05-1:30",
         "purpose": "Define the app in one beat.",
         "script": "ACME keeps one book for this: the Procedure Register. One side says what a procedure is supposed to do. The other side says what actually happened. When the two disagree, the book knows, and it says so."},
        {"order": 5, "title": "Two Stories, One Season", "act": "act-2-book", "timecode": "1:30-1:50",
         "purpose": "Preview both arcs.",
         "script": "This season follows two of ACME's procedures from the inside. Closing the books at quarter end, and rolling a policy change out to every employee. Both get watched the same way, by the same book."},
        {"order": 6, "title": "Effortless ACME Corp, PKO", "act": "act-2-book", "timecode": "1:50-2:00",
         "purpose": "Title card.",
         "script": "Effortless ACME Corp. PKO."},
    ],
}

# ---------------------------------------------------------------------------
# Episode 2 — Close the Books (Arc 1: Quarter-End Financial Close)
# ---------------------------------------------------------------------------
VIDEOS["02-close-the-books"] = {
    "title": "Close the Books",
    "description": "Episode 2 of Effortless ACME Corp: PKO. ACME's quarter-end close, "
                    "followed role by role.",
    "logline": "ACME's quarter-end close, followed role by role: the plan, who signs off, "
               "the workpaper nobody attached, the AI decisions nobody reviewed, and the "
               "drift a mined event log caught that no human ever wrote down.",
    "working_title": "Effortless ACME Corp: PKO, Episode 2, Close the Books",
    "target_length": "~8:00",
    "voice": "Plain-spoken workplace drama. Each role gets its own short scene in its own "
             "voice; process-steward is the throughline, appearing in every episode.",
    "running_example": "ACME's Quarter-End Financial Close, procedure version close-v1.1.0.",
    "how_to_use": "A previously-on bridge into six role-driven beats grouped into three acts, "
                  "closing on the informal knowledge broker who actually knows how "
                  "reconciliation works.",
    "output_file": "renders/acme-pko-02-close-the-books.final.mp4",
    "status": "Assembling",
    "structure": "Previously-on bridge -> 6 role beats across 3 acts (plan, evidence, what "
                 "actually happened) -> next-time tease into Episode 3.",
    "acts": [
        {"id": "act-1-plan", "order": 1, "numeral": "I", "act_title": "THE PLAN",
         "timecode": "0:00-2:30", "tagline": "who's supposed to do what, and who checks it"},
        {"id": "act-2-evidence", "order": 2, "numeral": "II", "act_title": "THE EVIDENCE",
         "timecode": "2:30-5:00", "tagline": "what got signed off, and what never got looked at"},
        {"id": "act-3-what-happened", "order": 3, "numeral": "III", "act_title": "WHAT ACTUALLY HAPPENED",
         "timecode": "5:00-8:00", "tagline": "the system that watches the system, and the person everyone actually asks"},
    ],
    "scenes": [
        {"order": 1, "title": "Previously, at ACME", "act": "act-1-plan", "timecode": "0:00-0:20",
         "purpose": "Bridge from Episode 1's hook.",
         "script": "Previously on ACME Corp. PKO. Every quarter, the books close, and something was watching. This time, we start from the beginning."},
        {"order": 2, "title": "The Plan", "act": "act-1-plan", "timecode": "0:20-1:30",
         "purpose": "finance-analyst opens the close. Introduce Procedures/Steps. Introduce process-steward as recurring witness-keeper.",
         "script": "The quarter-end close is a procedure, written down step by step. Someone owns making sure it's followed. That's the process steward's job, and the process steward asks the first question of the season. Is anyone actually watching whether this plan gets followed?"},
        {"order": 3, "title": "Who Signs Off", "act": "act-1-plan", "timecode": "1:30-2:30",
         "purpose": "controller; segregation of duties (req-close-separation).",
         "script": "The controller's rule is simple. Whoever prepares an entry cannot be the one who approves it. It sounds obvious. ACME's model checks it on every single entry, not just the ones somebody remembers to double check."},
        {"order": 4, "title": "No Workpaper", "act": "act-2-evidence", "timecode": "2:30-3:45",
         "purpose": "cfo; unbacked reconciliation (VerificationOutcomes.IsUnbackedObservation) + evidence retention.",
         "script": "One reconciliation this quarter was marked passed. No workpaper was ever attached to it. The model calls this an unbacked observation, and it also enforces something else. Any evidence behind a close has to stay retrievable for seven years."},
        {"order": 5, "title": "Who's Actually Deciding", "act": "act-2-evidence", "timecode": "3:45-5:00",
         "purpose": "variance-review-agent; AgentDecisionRecords, AuthorityBoundaries.",
         "script": "Part of this close is reviewed by an AI agent, not a person. Every decision it makes is recorded against a named authority boundary, so when an automated call turns out to be wrong, there's a record of exactly what it was allowed to decide in the first place."},
        {"order": 6, "title": "What Actually Happened", "act": "act-3-what-happened", "timecode": "5:00-6:30",
         "purpose": "close-automation; ProcessMiningRuns.IsDriftOnLiveVersion.",
         "script": "The plan says entries stop at cutoff. This quarter, ACME mined the actual event log from the ledger, and it showed entries posted after cutoff, on the live version of the procedure, with no reopening on record. That's not a guess. That's what the system of record actually did."},
        {"order": 7, "title": "Ask Priya", "act": "act-3-what-happened", "timecode": "6:30-7:45",
         "purpose": "knowledge-authority; KnowledgeBrokerLinks (Priya Raman) + shared controlled vocabulary term.",
         "script": "Officially, reconciliation questions go to whoever holds the role. Unofficially, three different people all go to the same coworker, Priya, because she actually knows how it works. ACME's model tracks that too, and it also caught something smaller. Two different requirements were both describing evidence retention in their own words, until they were pointed at the same defined term."},
        {"order": 8, "title": "Next Time", "act": "act-3-what-happened", "timecode": "7:45-8:00",
         "purpose": "Tease Arc 2.",
         "script": "Next time on ACME Corp. PKO. A policy change goes out to every employee, and not everyone agrees it should."},
    ],
}

# ---------------------------------------------------------------------------
# Episode 3 — The Policy Goes Out (Arc 2: Workforce Policy Change & Notification)
# ---------------------------------------------------------------------------
VIDEOS["03-the-policy-goes-out"] = {
    "title": "The Policy Goes Out",
    "description": "Episode 3 of Effortless ACME Corp: PKO. A workforce policy change moves "
                    "from draft to every employee's inbox.",
    "logline": "A workforce policy change moves from draft to every employee's inbox, and the "
               "model catches the one message that should never have sent, right next to the "
               "one that was correctly held back.",
    "working_title": "Effortless ACME Corp: PKO, Episode 3, The Policy Goes Out",
    "target_length": "~10:00",
    "voice": "Same plain-spoken workplace drama as Episode 2, raising the stakes: the audience "
             "is every employee, not an internal ledger.",
    "running_example": "ACME's Workforce Policy Change and Employee Notification procedure, policy-v1.0.0.",
    "how_to_use": "Previously-on bridge, six role-driven beats across three acts, closing on "
                  "the at-risk knowledge broker that ties back to Episode 2.",
    "output_file": "renders/acme-pko-03-the-policy-goes-out.final.mp4",
    "status": "Draft",
    "structure": "Previously-on bridge -> 6 role beats across 3 acts (draft, review, send) -> "
                 "next-time tease into the finale.",
    "acts": [
        {"id": "act-1-draft", "order": 1, "numeral": "I", "act_title": "DRAFTING THE CHANGE",
         "timecode": "0:00-3:00", "tagline": "where a policy comes from, and who has to sign off"},
        {"id": "act-2-review", "order": 2, "numeral": "II", "act_title": "HUMANS AND MACHINES",
         "timecode": "3:00-6:00", "tagline": "who wrote it, who's accountable, and who decides"},
        {"id": "act-3-send", "order": 3, "numeral": "III", "act_title": "THE SEND",
         "timecode": "6:00-10:00", "tagline": "what actually reached every employee's inbox"},
    ],
    "scenes": [
        {"order": 1, "title": "Previously, at ACME", "act": "act-1-draft", "timecode": "0:00-0:20",
         "purpose": "Bridge from Episode 2.",
         "script": "Previously on ACME Corp. PKO. Last time, a quarter closed, and the model caught what the paperwork missed. This time, a different kind of paperwork. A policy, headed for every employee."},
        {"order": 2, "title": "Drafting the Change", "act": "act-1-draft", "timecode": "0:20-1:45",
         "purpose": "hr-policy-owner; ChangeRequests, fragile single-witness tacit claim.",
         "script": "A workforce policy change starts as a change request, owned by ACME's people team. Some of what goes into it is written down. Some of it lives in one person's head, and the model has a name for that kind of knowledge, and a way of flagging when it's the only copy that exists."},
        {"order": 3, "title": "Counsel's Gate", "act": "act-1-draft", "timecode": "1:45-3:00",
         "purpose": "employment-counsel; req-policy-legal blocking requirement.",
         "script": "Before anything ships, employment counsel has to approve any legal or privacy commitment in it. It's a blocking requirement. The model can tell the difference between a rule that's just written down and a rule that's actually been checked, every time."},
        {"order": 4, "title": "The AI Drafted It", "act": "act-2-review", "timecode": "3:00-4:30",
         "purpose": "policy-drafting-agent; RoleAssignments.IsHumanToNonHumanHandover.",
         "script": "Part of this draft was written by an AI drafting agent, not a person. ACME's model records the handover explicitly. Which control used to be a human's job, when an AI took it over, and who approved that change."},
        {"order": 5, "title": "Quiet Hours", "act": "act-2-review", "timecode": "4:30-6:00",
         "purpose": "communications-manager; consent, quiet hours, opt-out requirements.",
         "script": "The communications manager's rules are strict. No message without consent, nothing sent between eight PM and eight AM in the recipient's own time zone, and every message needs a working opt-out. None of that is a suggestion. All of it is enforced."},
        {"order": 6, "title": "The Send", "act": "act-3-send", "timecode": "6:00-7:45",
         "purpose": "notification-publisher; MessageDeliveries.IsConsentViolation next to a correctly-suppressed send.",
         "script": "One message went out to a recipient who had never consented. Right next to it, in the same batch, a different message was correctly held back for the exact same reason. The model doesn't just flag the mistake. It proves the rule was actually capable of stopping one, because it just did, right there."},
        {"order": 7, "title": "The Person Who Left", "act": "act-3-send", "timecode": "7:45-9:30",
         "purpose": "knowledge-authority; KnowledgeBrokerLinks (Jordan Park at-risk broker), ties both arcs together.",
         "script": "One more thing the model caught, and it's not about this policy at all. Someone at ACME is still actively relying on a coworker for segregation-of-duties questions. That coworker doesn't hold any role at ACME anymore. Nobody's org chart shows that. The model does."},
        {"order": 8, "title": "Next Time", "act": "act-3-send", "timecode": "9:30-10:00",
         "purpose": "Tease the finale.",
         "script": "Next time on ACME Corp. PKO. How does any of this actually work? One file, and everything you've just watched, comes out of it."},
    ],
}

# ---------------------------------------------------------------------------
# Episode 4 — How the Book Writes Itself, Part One: The Model (finale, draft)
# ---------------------------------------------------------------------------
VIDEOS["04-how-the-book-writes-itself-the-model"] = {
    "title": "How the Book Writes Itself, The Model",
    "description": "Episode 4 of Effortless ACME Corp: PKO. Finale, part one. How a role's "
                    "question becomes a database column that can fire.",
    "logline": "Every finding from the last two episodes traces back to one file: the "
               "rulebook. This is how a role's question becomes a database column that can "
               "actually fire.",
    "working_title": "Effortless ACME Corp: PKO, Episode 4, How the Book Writes Itself, Part One, The Model",
    "target_length": "~9:00",
    "voice": "Fourth-wall break. Still plain-spoken, now technical enough for a builder to "
             "follow, without losing the general viewer.",
    "running_example": "The witness chain behind Episode 2's process-mining finding and "
                        "Episode 3's at-risk broker (WitnessLoops -> RoleQuestions -> RulebookFields).",
    "how_to_use": "ROUGH DRAFT. Covers the rulebook JSON, the witness chain, and effortless "
                  "build. Scenes to be refined after Episodes 1-3 are locked.",
    "output_file": "renders/acme-pko-04-how-the-book-writes-itself-the-model.final.mp4",
    "status": "Draft",
    "structure": "One file -> a role asks a question -> the build. Rough draft, all scenes subject to revision.",
    "acts": [
        {"id": "act-1-onefile", "order": 1, "numeral": "I", "act_title": "ONE FILE",
         "timecode": "0:00-3:00", "tagline": "everything you watched came from here"},
        {"id": "act-2-question", "order": 2, "numeral": "II", "act_title": "A ROLE ASKS A QUESTION",
         "timecode": "3:00-6:00", "tagline": "how a question becomes a column that can fire"},
        {"id": "act-3-build", "order": 3, "numeral": "III", "act_title": "THE BUILD",
         "timecode": "6:00-9:00", "tagline": "one command, and the book updates itself"},
    ],
    "scenes": [
        {"order": 1, "title": "One File", "act": "act-1-onefile", "timecode": "0:00-1:30",
         "purpose": "ROUGH. Reveal the rulebook JSON as the single source behind everything shown so far.",
         "script": "Everything in the last two episodes. The cutoff finding, Priya, Jordan Park, all of it, came from one file. A rulebook. Not a diagram of one. The actual thing."},
        {"order": 2, "title": "Not Code, a Model", "act": "act-1-onefile", "timecode": "1:30-3:00",
         "purpose": "ROUGH. Distinguish declarative model from imperative code.",
         "script": "The rulebook doesn't say how to check anything. It says what a procedure is, what a role is, what counts as evidence. Everything that checks itself is generated from that."},
        {"order": 3, "title": "process-steward Asks", "act": "act-2-question", "timecode": "3:00-4:15",
         "purpose": "ROUGH. Show the actual RoleQuestions row behind Episode 2's mining finding.",
         "script": "Here's the process steward's actual question, in its own voice, sitting in the rulebook. Does the live procedure actually match what we mined from the real system. That's a roe in a table called RoleQuestions."},
        {"order": 4, "title": "The Question Becomes a Field", "act": "act-2-question", "timecode": "4:15-5:15",
         "purpose": "ROUGH. Show RulebookFields.InventedForQuestion provenance.",
         "script": "Every field that can answer a question like that traces back to it, by name, in a table called RulebookFields. Nothing in this model exists without a reason somebody can point to."},
        {"order": 5, "title": "Loop Three", "act": "act-2-question", "timecode": "5:15-6:00",
         "purpose": "ROUGH. Name this season's witness loop explicitly.",
         "script": "This season's newest findings came from the third round of that process. Two roles, three questions, and every one of them was checked against real data before it was allowed to ship."},
        {"order": 6, "title": "effortless build", "act": "act-3-build", "timecode": "6:00-7:30",
         "purpose": "ROUGH. Run the real build command on screen.",
         "script": "One command turns that file into a real database, real screens, and a real security layer. Watch it happen."},
        {"order": 7, "title": "The Database Updates Itself", "act": "act-3-build", "timecode": "7:30-8:30",
         "purpose": "ROUGH. Show generated views/tables appearing.",
         "script": "Every table, every view, every rule you've seen enforced, regenerated from the same file, every time."},
        {"order": 8, "title": "Next", "act": "act-3-build", "timecode": "8:30-9:00",
         "purpose": "ROUGH. Tease part two.",
         "script": "Part two. What happens after the file becomes a database, in two different substrates that have to agree with each other."},
    ],
}

# ---------------------------------------------------------------------------
# Episode 5 — How the Book Writes Itself, Part Two: The Machine (finale, draft)
# ---------------------------------------------------------------------------
VIDEOS["05-how-the-book-writes-itself-the-machine"] = {
    "title": "How the Book Writes Itself, The Machine",
    "description": "Episode 5 of Effortless ACME Corp: PKO. Finale, part two. Dual "
                    "substrates, access control, and the Explorer.",
    "logline": "The same rulebook becomes two independent substrates that have to agree, a "
               "security layer with one schema per role, and an explorer that never needed a "
               "screen built for it.",
    "working_title": "Effortless ACME Corp: PKO, Episode 5, How the Book Writes Itself, Part Two, The Machine",
    "target_length": "~6:00",
    "voice": "Continuation of Episode 4's fourth-wall break. Closes with a direct call to action.",
    "running_example": "The Postgres and OWL substrates, the twelve role-scoped schemas, and the generic Explorer.",
    "how_to_use": "ROUGH DRAFT. Covers dual substrates, access control, the Explorer, and a closing CTA.",
    "output_file": "renders/acme-pko-05-how-the-book-writes-itself-the-machine.final.mp4",
    "status": "Draft",
    "structure": "Two engines, one answer -> twelve copies of the truth -> the screen nobody built. Rough draft.",
    "acts": [
        {"id": "act-1-substrates", "order": 1, "numeral": "I", "act_title": "TWO ENGINES, ONE ANSWER",
         "timecode": "0:00-2:00", "tagline": "Postgres and OWL, built from the same file, checked against each other"},
        {"id": "act-2-access", "order": 2, "numeral": "II", "act_title": "TWELVE COPIES OF THE TRUTH",
         "timecode": "2:00-4:00", "tagline": "one schema per role, and nothing outside it exists"},
        {"id": "act-3-explorer", "order": 3, "numeral": "III", "act_title": "THE SCREEN NOBODY BUILT",
         "timecode": "4:00-6:00", "tagline": "every table, every field, and a call to action"},
    ],
    "scenes": [
        {"order": 1, "title": "Two Engines", "act": "act-1-substrates", "timecode": "0:00-1:00",
         "purpose": "ROUGH. Show OWL ontology + Postgres both generated from the same rulebook.",
         "script": "The same rulebook that became a Postgres database also became a real OWL ontology, with its own reasoning rules. Neither one is the real one. Both are built from the same file."},
        {"order": 2, "title": "Checked Against Each Other", "act": "act-1-substrates", "timecode": "1:00-2:00",
         "purpose": "ROUGH. Show a conformance check where both substrates agree.",
         "script": "When two independently built engines answer the same question the same way, that's not a coincidence. That's the model actually holding."},
        {"order": 3, "title": "Twelve Roles, Twelve Schemas", "act": "act-2-access", "timecode": "2:00-3:00",
         "purpose": "ROUGH. Show role-scoped Postgres schemas, a missing column raising an error for the wrong role.",
         "script": "Every role you met this season, the controller, the cfo, the communications manager, reads through its own narrow slice of the database. A field with no grant doesn't just look empty. It doesn't exist for that role."},
        {"order": 4, "title": "The Access Rules Are Data Too", "act": "act-2-access", "timecode": "3:00-4:00",
         "purpose": "ROUGH. Show AccessPolicies/FieldGrants as rulebook rows.",
         "script": "Who can see what isn't a setting hidden in application code. It's rows in the same rulebook, checked the same way everything else in this season was checked."},
        {"order": 5, "title": "The Screen Nobody Built", "act": "act-3-explorer", "timecode": "4:00-5:15",
         "purpose": "ROUGH. Show the generic Explorer rendering ProcessMiningRuns/KnowledgeBrokerLinks with no bespoke code.",
         "script": "Every table in this season, including the ones added for this season, showed up in the Explorer automatically. Nobody wrote a screen for process mining runs or knowledge broker links. The catalog did."},
        {"order": 6, "title": "Try It", "act": "act-3-explorer", "timecode": "5:15-6:00",
         "purpose": "ROUGH. Closing CTA.",
         "script": "The whole model, the whole season, and the tools that built it, are one file and three commands away. Effortless ACME Corp. PKO."},
    ],
}

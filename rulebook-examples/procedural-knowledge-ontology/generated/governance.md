# Procedural Knowledge Governance

Canonical PKO rulebook: **PKO-Native Procedural Knowledge Rulebook**

## Stewardship and authority

- **Quarter-End Financial Close v1.1.0** — steward: Procedural Knowledge Steward; authority: Procedural Knowledge Authority; cadence: 90 days.
- **Workforce Policy Change and Employee Notification v1.0.0** — steward: Procedural Knowledge Steward; authority: People Policy Owner; cadence: 60 days.

## Change requests

- **Add source timestamp as blocking verification** (Defect, Approved) — Adds requirement and verification; no breaking class change.
- **Add secondary delivery receipt reconciliation** (Enhancement, UnderReview) — May add a step or fallback transition and operational binding.

## Knowledge gaps

- **High / Resolved:** No validated fallback exists for simultaneous ERP and signed-snapshot repository outage. Owner: Corporate Controller. Plan: Resolved by adding offline signed export escrow.
- **Medium / Open:** SMS provider delivery receipts are not always final for ported numbers. Owner: Employee Communications Manager. Plan: Pilot secondary delivery-status reconciliation.

## Elicitation and learning

- **Shadowing** — practitioner Devon Okafor; facilitator Maria Chen. Observed actual variance investigation and captured timestamp-related failure mode.
- **FacilitatedWorkshop** — practitioner Elena Garcia; facilitator Amina Yusuf. Mapped policy drafting, legal review, channel constraints, and acknowledgement exceptions.
- **PractitionerInterview** — practitioner Noah Williams; facilitator Elena Garcia. Captured legal rationale and the boundary between AI drafting and human commitment.
- **Retrospective:** Captured feed timestamp failure mode and updated the rulebook.
- **TabletopExercise:** Tested consent, quiet-hours, and unreachable-recipient exceptions.

## Role history

- Finance Analyst ← Maria Chen; valid 2026-01-01T00:00:00-06:00 to —; Active. Assigned for fiscal 2026
- Corporate Controller ← Devon Okafor; valid 2026-01-01T00:00:00-06:00 to —; Active. Controller appointment
- Chief Financial Officer ← Priya Raman; valid 2026-01-01T00:00:00-06:00 to —; Active. CFO appointment
- People Policy Owner ← Elena Garcia; valid 2026-02-01T00:00:00-06:00 to —; Active. Policy stewardship assignment
- Employment Counsel ← Noah Williams; valid 2026-01-15T00:00:00-06:00 to —; Active. Employment counsel assignment
- Notification Publisher ← Employee Notification Pipeline; valid 2026-03-01T00:00:00-06:00 to —; Active. Notification platform release 4.4
- Variance Review Agent ← Maria Chen; valid 2025-01-01T00:00:00-06:00 to 2026-03-31T23:59:59-05:00; Superseded. Human pilot before AI deployment
- Variance Review Agent ← Variance Analysis AI; valid 2026-04-01T00:00:00-05:00 to —; Active. Approved AI-assisted variance triage

## Operational bindings

- **GeneralLedger.TrialBalance** (Read) — ERP Ledger API; authoritative: Yes; freshness SLA: 15 minutes.
- **HR.ChangeTicket** (ReadWrite) — Policy Change Ticket HR-4821; authoritative: Yes; freshness SLA: 30 minutes.
- **HR.CommunicationConsent** (Read) — Employee Communication Consent Registry; authoritative: Yes; freshness SLA: 15 minutes.
- **People.EmailTemplate** (Read) — Employee Policy Email Template; authoritative: Yes; freshness SLA: 1440 minutes.
- **People.SmsTemplate** (Read) — Employee Policy SMS Template; authoritative: Yes; freshness SLA: 1440 minutes.

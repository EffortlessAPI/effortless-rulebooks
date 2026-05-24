# Shape note: README-only — never initialized as an ERB project

This folder lives inside `rulebook-examples/` but is **not** an Effortless Rulebook (ERB) project. It contains only a `README.md` sketching the domain — the project was never initialized.

## Why keep it here

The README describes a generic expense-approval domain that is a good future candidate for porting to canonical ERB shape. Keeping the spec colocated with the other examples makes it easier to pick up later.

## To turn this into a real ERB example

1. `cd rulebook-examples/expense-approval-demo`
2. `effortless -init`
3. Author the rulebook from the existing `README.md` spec
4. Save it as `effortless-rulebook/expense-approval-demo-rulebook.json`
5. Update `effortless.json` so `-o expense-approval-demo-rulebook.json` and `airtabletorulebook` is `IsDisabled: true`
6. Generate a `CLAUDE.md` from `../TEMPLATE-CLAUDE.md`
7. `effortless build`

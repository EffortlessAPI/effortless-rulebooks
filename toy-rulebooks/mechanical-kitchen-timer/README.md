A Mechanical Kitchen Timer — 5 Moving Parts
1. The Winding Knob (input)
The dial on the front. The user rotates it clockwise (say, to "20 minutes"). Its shaft is fixed to the mainspring arbor, so turning the knob winds the spring. A printed scale 0–60 around its rim shows the set time against a fixed pointer on the case.

2. The Mainspring (energy store)
A flat coiled steel spring inside a drum. One end anchors to the arbor (driven by §1), the other to the case. Winding the knob stores torque; the spring then continuously tries to unwind, rotating the arbor counter-clockwise back toward zero. This unwinding torque is what drives everything else.

3. The Gear Train (speed reduction)
A short stack of three meshed gears between the mainspring arbor and the escapement. It steps the arbor's slow rotation down — or rather, steps the escapement's fast oscillation up — so that one full unwind of §2 takes exactly 60 minutes at the output shaft. The dial pointer is mounted on this output shaft, so the printed scale moves visibly back toward 0.

4. The Escapement Wheel + Pallet (the regulator)
A toothed wheel driven by §3, with a pivoting two-pronged pallet sitting across its teeth. The pallet rocks back and forth; each rock releases exactly one tooth of the wheel — tick … tick … tick. Without this, the mainspring would dump all its energy in a fraction of a second. The pallet's rocking rate (set by its mass and a small hairspring) is what defines "one minute equals one minute."

5. The Bell Hammer (output)
A spring-loaded arm with a small steel striker, held back by a cam on the arbor of §2. When the arbor rotates back to the 0 position, the cam's notch aligns with the hammer's catch, the catch releases, and the hammer's own little spring snaps it against the bell on top of the case — ding. The timer is now fully unwound; everything sits still until §1 is turned again.

How they couple: §1 winds §2 → §2 drives §3 → §3 drives §4, which paces the unwinding back through §3 to §2 → when §2 reaches zero it trips §5.

One input, one output, three parts in between doing the actual work. Want me to go deeper on any one of them (e.g. the escapement geometry, or the cam profile that releases the hammer)?
---

## Local transpiler bus (`127.0.0.1:4242`)

> **All 11 local transpilers are hosted by the effortless CLI itself.** Start
> the bus with `effortless serve -port 4242` from the repo root; it serves every
> tool under `effortless-tools/<name>/` — `oss-postgres-calculated-to-rulebook`,
> `oss-rulebook-to-python`, `oss-rulebook-to-golang`, `oss-rulebook-to-cobol`,
> `oss-rulebook-to-owl`, and more — as a first-class route any `effortless
> build` can call. `GET /` lists them.
>
> **Address it as `127.0.0.1`, never `localhost`.** The host binds the literal
> prefix `http://127.0.0.1:<port>/`, so a request carrying a `localhost` Host
> header gets a bare 404 with no explanation.
>
> **Every repo-local route carries the `oss-` prefix.** The bare names
> (`rulebook-to-python`, `rulebook-to-xlsx`, `rulebook-to-owl`,
> `rulebook-to-airtable`, `airtable-to-rulebook`) belong to the commercial
> catalog as `effortless/effortless/<tool>`; the prefix is the only thing
> keeping a repo-local route from shadowing one.
> `orchestration/local_tool_shim.py` refuses to run if any tool is missing it.

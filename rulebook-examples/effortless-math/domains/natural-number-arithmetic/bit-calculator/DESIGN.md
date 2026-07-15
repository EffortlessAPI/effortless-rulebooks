# Bit Calculator — a gate-level arithmetic provider (4-bit, extends to N-bit as data)

This folder is a **federated provider theorem** for the `natural-number-arithmetic`
domain. It proves the four flagship arithmetic facts a **second, independent way**:
not through Peano `Zero`/`Successor` chains (that is the sibling
`theorem-contract.json`), but by **logic gates wired into circuits** — the way a CPU
actually adds.

Everything below the input bits is **data**. There is no arithmetic in any formula,
in any injector, or in any hand-written SQL. "Computing `2 + 2`" is a recursive walk
of a netlist that bottoms out at 4-row gate truth tables. This is the base-level
answer to *"but how does arithmetic actually work?"* — and, by extension, the ground
under every heavier claim the network makes.

## Why it is federated, not inlined

It mirrors how Fermat's Last Theorem already works in this repo:

- The **main** `effortless-math-rulebook.json` imports only a **provider certificate**
  — theorem ID + version, bound hypotheses (operands and their bit encodings),
  conclusions (result bits), proof status, remaining imports (none), artifact hashes.
  Exactly the certificate shape the domain `CLAUDE.md` mandates for providers.
- The full gate-level proof model lives **here**, as its own effortless project with
  its own rulebook, its own `effortless-postgres/` output, and its own test DB.
- The main rulebook consumes the **versioned contract only**, never the calculator's
  private gate/wire tables. (Provider theorem doctrine.)

It is a **different claim** from the Peano domain and does not touch the v21 witness
or the four `FULLY_INTERNALIZED_FOR_SCOPE` Successor theorems. Two disjoint
zero-import substrates independently agree that `2 + 2 = 4`.

## The model (Rosetta Stone netlist, as rulebook tables)

Borrowed directly from `computational-rosetta-stone.json`: components are **netlists**
of typed `instances` plus `from -> to` `connections`, bottoming out at primitive gate
types whose semantics are truth tables.

| Table | Holds | Example key |
|---|---|---|
| `GateTypes` | the 4 primitive gates | `gate-xor` |
| `GateTruthRows` | truth table as data (4 rows/binary gate, 2/NOT) | `gate-and--i0-1--i1-1` |
| `Components` | half_adder -> full_adder -> adder4 -> sub4 -> mul4 -> div4 | `comp-adder4` |
| `ComponentPins` | named component inputs/outputs | `comp-adder4--out--s2` |
| `ComponentInstances` | typed sub-parts wired inside a component | `comp-full-adder--inst--ha0` |
| `ComponentConnections` | the netlist: `from` port -> `to` port | `comp-adder4--conn--fa0.carry--to--fa1.cin` |
| `Computations` | one row per evaluation: op + operands **+ their bit decomposition** | `compute--add--2--2` |
| `InputWires` | **only the input wires**, materialized by the width-agnostic engine | `compute--add--2--2--wire--a1` |

### Port grammar

A connection endpoint is either:

- a **bare pin name** (`a1`, `sum`, `cout`) — a pin on the *enclosing* component, or
- **`instance.port`** (`ha0.a`, `fa0.carry`) — a pin on a named sub-instance.

A connection asserts *"the signal at `from` equals the signal at `to`."* An input pin
of a gate instance is driven by whatever connects into it; the gate's output pin is
computed from its truth table; that output then drives downstream connections.
Ripple carry is just `fa0.carry -> fa1.cin`.

## The Chinese-room contract (the important part)

1. The rulebook expresses `2 + 2` as a `Computations` row: `Op = add`,
   `Component = comp-adder4`, `A_Bits = "0010"`, `B_Bits = "0010"`. **The rulebook is
   the only thing that knows "2 is these bits."**
2. A **general-purpose, width-agnostic engine** reads that row and inserts `InputWires`
   rows — one per input pin — copying the declared bits onto the wires. It has *zero*
   understanding of arithmetic. It reads the width off `length(A_Bits)`; it works
   unchanged for 2, 4, 8, or 200 bits. Chinese room.
3. A recursive Postgres view (`WITH RECURSIVE` over `ComponentConnections` +
   `GateTruthRows`) **settles every internal and output wire live**. Nothing arithmetic
   is ever stored — only the input bits are data; every downstream bit is a computation
   over rows.
4. The **answer is an invariant over those rows**: an `InvariantCheck` asserts the
   settled output pins spell the expected numeral (`0100` = 4). The engine never learns
   it did addition. The gates did.

## Operation coverage (4-bit; the pattern is width-free)

| Op | Component | Built from |
|---|---|---|
| add | `comp-adder4` | 4 x full_adder ripple carry |
| sub | `comp-sub4` | adder4 + NOT on B + `cin = 1` (two's complement) |
| mul | `comp-mul4` | shift-and-add array of adder4 |
| div | `comp-div4` | restoring division: array of sub4 + selects |

Widening to 8 (or 200) bits adds `Components`/`Instances`/`Connections`/`GateTruthRows`
**rows** — never code. The engine and the recursive view are width-agnostic.

## What the main rulebook imports

Exactly the four flagship facts, matching the Peano domain for clean parity — the
gate-substrate twin of the four Successor theorems:

- `bitcalc-add-2-2 = 4`
- `bitcalc-sub-4-2 = 2`
- `bitcalc-mul-2-2 = 4`
- `bitcalc-div-4-2 = 2`

The engine still *computes* arbitrary 4-bit operands (exercised exhaustively by the
test DB), but the formal import is these four. `ProofStatus = FULLY_INTERNALIZED_FOR_SCOPE`,
`ActiveImportCount = 0`.

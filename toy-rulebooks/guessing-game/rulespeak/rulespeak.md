# 📘 Guessing Game — RuleSpeak®

_Number-guessing game tracking guesses, hints, and best-score records per player._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **形状** | A 形状 is identified by its 名称 and is related to optionally a 边 (its edges). | — |
| 名称 | A defined attribute. | — |
| Edges | A defined attribute. | — |
| 边的类型 | Computed as the value matched against the 边的数量: “空” when it is 0; “点” when it is 1; “线” when it is 2; “三角形” when it is 3; “正方形” when it is 4; otherwise “--”. | — |
| 边标签 | An empty string. | _Multi-value display string of the child edges' values (seeded; one entry per edge, ordered by label)._ |
| 边长 | An empty string. | _Multi-value display string of the child edges' values (seeded; one entry per edge, ordered by label)._ |
| 边的数量 | The number of 边 related to the 形状. | — |
| 边角 | An empty string. | _Multi-value display string of the child edges' values (seeded; one entry per edge, ordered by label)._ |
| 角度总和 | The total 角度 across the 边 related to the 形状. | — |
| 最大边长 | The total 长度 across the 边 related to the 形状. | — |
| 非斜边角度总和 | The total 长度平方 across the 边 related to the 形状. | — |
| 勾股定理成立 | Determined by priority: the 非斜边角度总和 is the 最大边长 times the 最大边长 if the 边的数量 is 3; in all other cases, no value. | — |
| **边** | A 边 is identified by its 标签. | — |
| 名称 | Computed as the 形状, followed by a hyphen, followed by the 标签. | — |
| 形状 | A defined attribute. | — |
| 形状的最大边长 | Taken from the linked 形状. | — |
| 边的数量 | Taken from the linked 形状. | — |
| 是否为斜边 | Computed as the 长度 is the 形状的最大边长. | — |
| 形状类型 | The 边的类型 of the 边's 形状. | — |
| 标签 | A defined attribute. | — |
| 长度 | A defined attribute. | — |
| 长度平方 | Computed as the 长度 times the 长度. | — |
| 角度 | A defined attribute. | — |

## 2 Fact Types

- a **形状** may reference one **边**

## 3 Operative Rules

_No operative rules yet. Required fields and foreign keys imply structural
`must`-rules automatically; to declare semantic obligations (`must` / `must not` / `should`), add a **Constraints** table whose rows point at
boolean calculated fields. See the tool README for the column contract._

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 边的类型** | A 形状's 边的类型 is computed as the value matched against the 边的数量: “空” when it is 0; “点” when it is 1; “线” when it is 2; “三角形” when it is 3; “正方形” when it is 4; otherwise “--”. |
| **DR-2 边标签** | A 形状's 边标签 is an empty string. |
| **DR-3 边长** | A 形状's 边长 is an empty string. |
| **DR-4 边的数量** | A 形状's 边的数量 is the number of 边 related to the 形状. |
| **DR-5 边角** | A 形状's 边角 is an empty string. |
| **DR-6 角度总和** | A 形状's 角度总和 is the total 角度 across the 边 related to the 形状. |
| **DR-7 最大边长** | A 形状's 最大边长 is the total 长度 across the 边 related to the 形状. |
| **DR-8 非斜边角度总和** | A 形状's 非斜边角度总和 is the total 长度平方 across the 边 related to the 形状. |
| **DR-9 勾股定理成立** | The 形状's 勾股定理成立 is determined by the following priority:<br>1. the 非斜边角度总和 is the 最大边长 times the 最大边长, if the 边的数量 is 3;<br>2. in all other cases, no value. |
| **DR-10 名称** | A 边's 名称 is computed as the 形状, followed by a hyphen, followed by the 标签. |
| **DR-11 形状的最大边长** | A 边's 形状的最大边长 — taken from the linked 形状. |
| **DR-12 边的数量** | A 边's 边的数量 — taken from the linked 形状. |
| **DR-13 是否为斜边** | A 边's 是否为斜边 is computed as the 长度 is the 形状的最大边长. |
| **DR-14 形状类型** | A 边's 形状类型 is the 边的类型 of the 边's 形状. |
| **DR-15 长度平方** | A 边's 长度平方 is computed as the 长度 times the 长度. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **形状.边的类型** | formula | `Switch(边的数量, 0, "空", 1, "点", 2, "线", 3, "三角形", 4, "正方形", "--")` |
| **形状.边标签** | lookup | — |
| **形状.边长** | lookup | — |
| **形状.边的数量** | rollup | `Count(边 via 形状)` |
| **形状.边角** | lookup | — |
| **形状.角度总和** | rollup | `Sum(边.角度 via 形状)` |
| **形状.最大边长** | rollup | `Sum(边.长度 via 形状)` |
| **形状.非斜边角度总和** | rollup | `Sum(边.长度平方 via 形状)` |
| **形状.勾股定理成立** | formula | `If(边的数量 = 3, 非斜边角度总和 = 最大边长 * 最大边长, Blank())` |
| **边.名称** | formula | `形状 & "-" & 标签` |
| **边.形状的最大边长** | lookup | `Lookup(形状.最大边长 via 形状)` |
| **边.边的数量** | lookup | `Lookup(形状.边的数量 via 形状)` |
| **边.是否为斜边** | formula | `长度 = 形状的最大边长` |
| **边.形状类型** | lookup | `Lookup(形状.边的类型 via 形状)` |
| **边.长度平方** | formula | `长度 * 长度` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._

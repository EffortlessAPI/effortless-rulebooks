# 📘 Is Everything a Language? — RuleSpeak

_Semiotic candidates evaluated by formula — shows substrate-equality holds for non-CRUD ontologies too._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Language Candidate** | A language candidate tracked by the business. |
| Has Grammar | Does this candidate have a Grammar?  Generally follows candidates that have syntax also have grammar. |
| Question | Question that 100 random people could be asked, family feud style. |
| Predicted Answer | The predicted answer as the top most popular answer among those in the family feud polling pool. |
| Prediction Predicates | Computed as `If(HasSyntax, "Has Syntax", "No Syntax")`, followed by the literal “ & ”, followed by `If(IsParsed, "Requires Parsing", "No Parsing Neede")`, followed by the literal “ & ”, followed by `If(IsDescriptionOf, "Describes the thing", "Is the Thing")`, followed by the literal “ & ”, followed by `If(HasLinearDecodingPressure, "Has Linear Decoding Pressure", "No Decoding Pressure")`, followed by the literal “ & ”, followed by `If(ResolvesToAnAST, "Resolves to AST", "No AST")`, followed by a comma followed by a space, followed by `If(IsStableOntologyReference, "Is Stable Ontology", "Not 'Ontology'")`, followed by the literal “ AND ”, followed by `If(CanBeHeld, "Can Be Held", "Can't Be Held")`, followed by a comma followed by a space, followed by `If(HasIdentity, "Has Identity", "Has no Identity")`. |
| Prediction Fail | If the family feud answer does not match the chosen language candidates status then this explains what did not match. t also flags (in english) mismatch where a candidate is marked as BOTH open AND closed world which does not make sense. |
| Is Description of | True when the distance from concept is greater than 1. |
| Is Open Closed World Conflicted | True when all of the following hold: the is open world flag is set and the is closed world flag is set. |
| Relationship to Concept | Computed: `If(DistanceFromConcept = 1, "IsMirrorOf", "IsDescriptionOf")`. |
| **Is Everything a Language** | An is everything a language tracked by the business. |
| **ERB Customization** | An ERB customization tracked by the business. |

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Has Grammar** | A language candidate is considered to have a grammar if the has syntax is `True()`. |
| **DR-2 Question** | A language candidate's question is computed as the literal “Is " & {{Name}} & " a language?”. |
| **DR-3 Predicted Answer** | A language candidate is flagged predicted answer if all of the following hold: the has syntax flag is set; the is parsed flag is set; the is description of flag is set; the has linear decoding pressure flag is set; the resolves to an AST flag is set; the is stable ontology reference flag is set; it is not the case that the can be held flag is set; and it is not the case that the has identity flag is set. |
| **DR-4 Prediction Predicates** | A language candidate's prediction predicates is computed as `If(HasSyntax, "Has Syntax", "No Syntax")`, followed by the literal “ & ”, followed by `If(IsParsed, "Requires Parsing", "No Parsing Neede")`, followed by the literal “ & ”, followed by `If(IsDescriptionOf, "Describes the thing", "Is the Thing")`, followed by the literal “ & ”, followed by `If(HasLinearDecodingPressure, "Has Linear Decoding Pressure", "No Decoding Pressure")`, followed by the literal “ & ”, followed by `If(ResolvesToAnAST, "Resolves to AST", "No AST")`, followed by a comma followed by a space, followed by `If(IsStableOntologyReference, "Is Stable Ontology", "Not 'Ontology'")`, followed by the literal “ AND ”, followed by `If(CanBeHeld, "Can Be Held", "Can't Be Held")`, followed by a comma followed by a space, followed by `If(HasIdentity, "Has Identity", "Has no Identity")`. |
| **DR-5 Prediction Fail** | A language candidate's prediction fail is computed as `If(Not(PredictedAnswer = IsLanguage),<br> Name & " " & If(PredictedAnswer, "Is", "Isn't") & " a Family Feud Language, but " & <br> If(IsLanguage, "Is", "Is Not") & " marked as a 'Language Candidate.'", "")`, followed by `If(IsOpenClosedWorldConflicted, " - Open World vs. Closed World Conflict.", "")`. |
| **DR-6 Is Description of** | A language candidate is considered a description of if the distance from concept is greater than 1. |
| **DR-7 Is Open Closed World Conflicted** | A language candidate is considered open closed world conflicted if all of the following hold: the is open world flag is set and the is closed world flag is set. |
| **DR-8 Relationship to Concept** | The language candidate's relationship to concept is determined by the following priority:<br>1. the literal “IsMirrorOf”, if the distance from concept is 1;<br>2. otherwise the literal “IsDescriptionOf”. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **LanguageCandidates.HasGrammar** | formula | `HasSyntax = True()` |
| **LanguageCandidates.Question** | formula | `"Is " & Name & " a language?"` |
| **LanguageCandidates.PredictedAnswer** | formula | `And(
  HasSyntax,
  IsParsed,
  IsDescriptionOf,
  HasLinearDecodingPressure,
  ResolvesToAnAST,
  IsStableOntologyReference,
  Not(CanBeHeld),
  Not(HasIdentity)
)` |
| **LanguageCandidates.PredictionPredicates** | formula | `If(HasSyntax, "Has Syntax", "No Syntax") & " & " & If(IsParsed, "Requires Parsing", "No Parsing Neede") & " & " & If(IsDescriptionOf, "Describes the thing", "Is the Thing") & " & " & If(HasLinearDecodingPressure, "Has Linear Decoding Pressure", "No Decoding Pressure") & " & " & If(ResolvesToAnAST, "Resolves to AST", "No AST") & ", " & If(IsStableOntologyReference, "Is Stable Ontology", "Not 'Ontology'") & " AND " & If(CanBeHeld, "Can Be Held", "Can't Be Held") & ", " &If(HasIdentity, "Has Identity", "Has no Identity")` |
| **LanguageCandidates.PredictionFail** | formula | `If(Not(PredictedAnswer = IsLanguage),
  Name & " " & If(PredictedAnswer, "Is", "Isn't") & " a Family Feud Language, but " & 
  If(IsLanguage, "Is", "Is Not") & " marked as a 'Language Candidate.'", "") & If(IsOpenClosedWorldConflicted, " - Open World vs. Closed World Conflict.", "")` |
| **LanguageCandidates.IsDescriptionOf** | formula | `DistanceFromConcept > 1` |
| **LanguageCandidates.IsOpenClosedWorldConflicted** | formula | `And(IsOpenWorld, IsClosedWorld)` |
| **LanguageCandidates.RelationshipToConcept** | formula | `If(DistanceFromConcept = 1, "IsMirrorOf", "IsDescriptionOf")` |

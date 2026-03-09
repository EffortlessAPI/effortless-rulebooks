"""
ERB SDK - Python Implementation
================================
Mirrors the PostgreSQL functions from postgres/02-create-functions.sql
Source: effortless-rulebook/effortless-rulebook.json

DAG Execution Order:
  Level 0: Raw fields
  Level 1: category_contains_language, has_grammar, relationship_to_concept, family_fued_question
  Level 2: is_a_family_feud_top_answer (depends on category_contains_language)
  Level 3: family_feud_mismatch (depends on is_a_family_feud_top_answer)
"""

from dataclasses import dataclass
from typing import Optional


@dataclass
class LanguageCandidate:
    """A candidate item to evaluate whether it qualifies as a 'language'."""

    # Primary Key
    language_candidate_id: str

    # Raw Fields
    name: Optional[str] = None
    category: Optional[str] = None
    can_be_held: Optional[bool] = None
    meaning_is_serialized: Optional[bool] = None
    requires_parsing: Optional[bool] = None
    is_ongology_descriptor: Optional[bool] = None
    has_syntax: Optional[bool] = None
    chosen_language_candidate: Optional[bool] = None
    sort_order: Optional[int] = None
    has_identity: Optional[bool] = None
    distance_from_concept: Optional[int] = None

    # =========================================================================
    # CALCULATED FIELDS - Mirrors PostgreSQL functions exactly
    # =========================================================================

    # Level 1: Simple calculations on raw fields only
    # ------------------------------------------------

    def calc_category_contains_language(self) -> bool:
        """
        Mirrors: calc_{entity}_category_contains_language() in PostgreSQL
        Formula: FIND("language", LOWER(category)) > 0
        """
        if self.category is None:
            return False
        return "language" in self.category.lower()

    def calc_has_grammar(self) -> str:
        """
        Mirrors: calc_{entity}_has_grammar() in PostgreSQL
        Formula: CAST(has_syntax AS TEXT)
        """
        if self.has_syntax is None:
            return ""
        return "true" if self.has_syntax else ""

    def calc_relationship_to_concept(self) -> str:
        """
        Mirrors: calc_{entity}_relationship_to_concept() in PostgreSQL
        Formula: IF(distance_from_concept = 1, "IsMirrorOf", "IsDescriptionOf")
        """
        if self.distance_from_concept == 1:
            return "IsMirrorOf"
        return "IsDescriptionOf"

    def calc_family_fued_question(self) -> str:
        """
        Mirrors: calc_{entity}_family_fued_question() in PostgreSQL
        Formula: "Is " & name & " a language?"
        """
        name = self.name or ""
        return f"Is {name} a language?"

    # Level 2: Depends on Level 1 calculations
    # ----------------------------------------

    def calc_is_a_family_feud_top_answer(self) -> bool:
        """
        Mirrors: calc_{entity}_is_a_family_feud_top_answer() in PostgreSQL
        Formula: AND(
            category_contains_language,
            has_syntax,
            NOT(can_be_held),
            meaning_is_serialized,
            requires_parsing,
            is_ongology_descriptor,
            NOT(has_identity),
            distance_from_concept = 2
        )
        """
        # Depends on Level 1 calc
        category_contains_language = self.calc_category_contains_language()

        # All conditions must be true
        return (
            category_contains_language
            and (self.has_syntax or False)
            and not (self.can_be_held or False)
            and (self.meaning_is_serialized or False)
            and (self.requires_parsing or False)
            and (self.is_ongology_descriptor or False)
            and not (self.has_identity or False)
            and self.distance_from_concept == 2
        )

    # Level 3: Depends on Level 2 calculations
    # ----------------------------------------

    def calc_family_feud_mismatch(self) -> Optional[str]:
        """
        Mirrors: calc_{entity}_family_feud_mismatch() in PostgreSQL
        Formula: IF(is_a_family_feud_top_answer != chosen_language_candidate,
            name & " " & IF(is_a_family_feud_top_answer, "Is", "Isn't") &
            " a Family Feud Language, but " &
            IF(chosen_language_candidate, "Is", "Is Not") &
            " marked as a 'Language Candidate.'",
            NULL
        )
        """
        # Depends on Level 2 calc
        is_top_answer = self.calc_is_a_family_feud_top_answer()
        chosen = self.chosen_language_candidate or False

        if is_top_answer != chosen:
            is_word = "Is" if is_top_answer else "Isn't"
            marked_word = "Is" if chosen else "Is Not"
            return (
                f"{self.name or ''} {is_word} a Family Feud Language, but "
                f"{marked_word} marked as a 'Language Candidate.'"
            )
        return None

    # =========================================================================
    # VIEW PROPERTIES - Computed view with all calculated fields
    # =========================================================================

    def to_view(self) -> dict:
        """Returns all raw + calculated fields as a dict (mirrors vw_{entity})."""
        return {
            # Primary Key
            "language_candidate_id": self.language_candidate_id,
            # Raw Fields
            "name": self.name,
            "category": self.category,
            "can_be_held": self.can_be_held,
            "meaning_is_serialized": self.meaning_is_serialized,
            "requires_parsing": self.requires_parsing,
            "is_ongology_descriptor": self.is_ongology_descriptor,
            "has_syntax": self.has_syntax,
            "chosen_language_candidate": self.chosen_language_candidate,
            "sort_order": self.sort_order,
            "has_identity": self.has_identity,
            "distance_from_concept": self.distance_from_concept,
            # Calculated Fields (DAG order)
            "category_contains_language": self.calc_category_contains_language(),
            "has_grammar": self.calc_has_grammar(),
            "relationship_to_concept": self.calc_relationship_to_concept(),
            "family_fued_question": self.calc_family_fued_question(),
            "is_a_family_feud_top_answer": self.calc_is_a_family_feud_top_answer(),
            "family_feud_mismatch": self.calc_family_feud_mismatch(),
        }


@dataclass
class IsEverythingALanguage:
    """Argument steps in the philosophical debate about language definition."""

    # Primary Key
    is_everything_a_language_id: str

    # Raw Fields
    name: Optional[str] = None
    argument_name: Optional[str] = None
    argument_category: Optional[str] = None
    step_type: Optional[str] = None
    statement: Optional[str] = None
    formalization: Optional[str] = None
    related_candidate_name: Optional[str] = None
    related_candidate_id: Optional[str] = None
    evidence_from_rulebook: Optional[str] = None
    notes: Optional[str] = None

    # No calculated fields on this entity

    def to_view(self) -> dict:
        """Returns all fields as a dict (mirrors vw_is_everything_a_language)."""
        return {
            "is_everything_a_language_id": self.is_everything_a_language_id,
            "name": self.name,
            "argument_name": self.argument_name,
            "argument_category": self.argument_category,
            "step_type": self.step_type,
            "statement": self.statement,
            "formalization": self.formalization,
            "related_candidate_name": self.related_candidate_name,
            "related_candidate_id": self.related_candidate_id,
            "evidence_from_rulebook": self.evidence_from_rulebook,
            "notes": self.notes,
        }


# =============================================================================
# CORE LANGUAGE DEFINITION (from the rulebook)
# =============================================================================

def is_language(candidate: LanguageCandidate) -> bool:
    """
    Core language definition from the rulebook:
    Language(x) := HasSyntax(x)
                   AND RequiresParsing(x)
                   AND Meaning_Is_Serialized(x)
                   AND IsOngologyDescriptor(x)
    """
    return (
        (candidate.has_syntax or False)
        and (candidate.requires_parsing or False)
        and (candidate.meaning_is_serialized or False)
        and (candidate.is_ongology_descriptor or False)
    )


# =============================================================================
# LOADER - Load from JSON rulebook
# =============================================================================

def load_from_rulebook(rulebook_path: str) -> tuple[list[LanguageCandidate], list[IsEverythingALanguage]]:
    """Load entities from the effortless-rulebook.json file."""
    import json

    with open(rulebook_path, 'r') as f:
        data = json.load(f)

    candidates = []
    for item in data.get("LanguageCandidates", {}).get("data", []):
        candidates.append(LanguageCandidate(
            language_candidate_id=item.get("LanguageCandidateId", ""),
            name=item.get("Name"),
            category=item.get("Category"),
            can_be_held=item.get("CanBeHeld"),
            meaning_is_serialized=item.get("Meaning_Is_Serialized"),
            requires_parsing=item.get("RequiresParsing"),
            is_ongology_descriptor=item.get("IsOngologyDescriptor"),
            has_syntax=item.get("HasSyntax"),
            chosen_language_candidate=item.get("ChosenLanguageCandidate"),
            sort_order=item.get("SortOrder"),
            has_identity=item.get("HasIdentity"),
            distance_from_concept=item.get("DistanceFromConcept"),
        ))

    arguments = []
    for item in data.get("IsEverythingALanguage", {}).get("data", []):
        arguments.append(IsEverythingALanguage(
            is_everything_a_language_id=item.get("IsEverythingALanguageId", ""),
            name=item.get("Name"),
            argument_name=item.get("ArgumentName"),
            argument_category=item.get("ArgumentCategory"),
            step_type=item.get("StepType"),
            statement=item.get("Statement"),
            formalization=item.get("Formalization"),
            related_candidate_name=item.get("RelatedCandidateName"),
            related_candidate_id=item.get("RelatedCandidateId"),
            evidence_from_rulebook=item.get("EvidenceFromRulebook"),
            notes=item.get("Notes"),
        ))

    return candidates, arguments


if __name__ == "__main__":
    # Example usage
    import os

    # Load from rulebook
    rulebook_path = os.path.join(
        os.path.dirname(__file__),
        "../../effortless-rulebook/effortless-rulebook.json"
    )

    if os.path.exists(rulebook_path):
        candidates, arguments = load_from_rulebook(rulebook_path)

        print(f"Loaded {len(candidates)} language candidates")
        print(f"Loaded {len(arguments)} argument steps")
        print()

        # Show first candidate with all calculated fields
        if candidates:
            c = sorted(candidates, key=lambda x: x.sort_order or 999)[0]
            print(f"First candidate: {c.name}")
            view = c.to_view()
            for k, v in view.items():
                print(f"  {k}: {v}")
    else:
        print(f"Rulebook not found at: {rulebook_path}")

#!/usr/bin/env python3
"""
Generate English documentation from ANY Effortless Rulebook.

ARCHITECTURE: Generic Rulebook → English Specification (LLM-Driven)
====================================================================
This substrate reads the effortless-rulebook.json and uses an LLM to generate:
1. specification.md - Plain English explanation of all calculated fields

The core insight: Let the LLM do the work. Instead of writing formula parsers,
just send the rulebook JSON to the LLM and ask it to write the specification.

Two LLM calls, zero formula parsing.
"""

import sys
import os
import argparse
import json
from pathlib import Path

# Add project root to path for shared imports
sys.path.insert(0, str(Path(__file__).resolve().parent.parent.parent))

from orchestration.shared import load_rulebook, get_candidate_name_from_cwd, handle_clean_arg

# =============================================================================
# MODEL TIER CONFIGURATION
# =============================================================================

MODEL_TIERS = {
    "smart": {
        "openai": "gpt-4o",
        "anthropic": "claude-sonnet-4-20250514",
        "description": "Most capable models - highest accuracy, slowest, most expensive"
    },
    "medium": {
        "openai": "gpt-4o-mini",
        "anthropic": "claude-3-5-haiku-20241022",
        "description": "Balanced models - good accuracy, moderate speed/cost"
    },
    "cheap": {
        "openai": "gpt-3.5-turbo",
        "anthropic": "claude-3-haiku-20240307",
        "description": "Budget models - faster/cheaper but less reliable"
    },
}

DEFAULT_TIER = os.environ.get("LLM_TIER", "medium")
DEFAULT_PROVIDER = os.environ.get("LLM_PROVIDER", "openai")


def get_model_for_tier(tier: str, provider: str) -> str:
    """Get the model name for a given tier and provider."""
    if tier not in MODEL_TIERS:
        print(f"Warning: Unknown tier '{tier}', using 'medium'")
        tier = "medium"
    return MODEL_TIERS[tier].get(provider, MODEL_TIERS[tier]["openai"])


def get_llm_response(prompt: str, provider: str = None, tier: str = None) -> str:
    """Get a response from the LLM."""
    provider = provider or DEFAULT_PROVIDER
    tier = tier or DEFAULT_TIER
    model = get_model_for_tier(tier, provider)

    print(f"  Calling {provider.upper()} ({model})...")
    sys.stdout.flush()

    try:
        if provider == "openai":
            import openai
            client = openai.OpenAI()
            response = client.chat.completions.create(
                model=model,
                messages=[{"role": "user", "content": prompt}],
                temperature=0.3,  # Lower temp for more consistent outputs
                max_tokens=4096,
            )
            return response.choices[0].message.content
        elif provider == "anthropic":
            import anthropic
            client = anthropic.Anthropic()
            response = client.messages.create(
                model=model,
                max_tokens=4096,
                temperature=0.3,
                messages=[{"role": "user", "content": prompt}],
            )
            return response.content[0].text
        else:
            raise ValueError(f"Unknown provider: {provider}")
    except Exception as e:
        print(f"Warning: LLM call failed: {e}")
        return f"[LLM generation failed: {e}]"


# =============================================================================
# LLM-DRIVEN SPECIFICATION GENERATION
# =============================================================================

def generate_specification(rulebook: dict, provider: str = None, tier: str = None) -> str:
    """Use LLM to generate plain English specification from rulebook."""

    provider = provider or DEFAULT_PROVIDER
    tier = tier or DEFAULT_TIER

    rulebook_name = rulebook.get('Name', 'Untitled Rulebook')

    prompt = f"""You are a technical writer creating a specification document.

Given this rulebook JSON, write a clear English specification document that explains
how to compute each calculated field.

RULEBOOK:
```json
{json.dumps(rulebook, indent=2)}
```

Write a specification document in Markdown format that includes:

1. A title and brief overview of what this rulebook does
2. For each entity that has calculated fields:
   - List the input fields (type="raw") with their names, types, and descriptions
   - For each calculated field (type="calculated"):
     - Explain in plain English exactly how to compute it from the inputs
     - Include the formula for reference
     - Provide a concrete example using data from the rulebook if available

The specification should be clear enough that someone could follow it to compute
the correct values without seeing the original formulas.

IMPORTANT: Focus on the actual content of this specific rulebook ("{rulebook_name}").
Do not include generic boilerplate about "language classification" or unrelated domains."""

    print(f"  Generating specification via LLM...")
    return get_llm_response(prompt, provider, tier)


# =============================================================================
# MAIN
# =============================================================================

def main():
    GENERATED_FILES = [
        'test-results.md',
        'specification.md',
    ]

    if '--clean' in sys.argv:
        if handle_clean_arg(GENERATED_FILES, "English substrate: Removes test-results.md and specification.md."):
            return 0

    parser = argparse.ArgumentParser(
        description="Generate English specification from any Effortless Rulebook (LLM-driven)"
    )
    parser.add_argument(
        "--tier", "-t",
        choices=["smart", "medium", "cheap"],
        default=DEFAULT_TIER,
        help=f"Model intelligence tier (default: {DEFAULT_TIER})"
    )
    parser.add_argument(
        "--provider", "-p",
        choices=["openai", "anthropic"],
        default=DEFAULT_PROVIDER,
        help=f"LLM provider (default: {DEFAULT_PROVIDER})"
    )
    parser.add_argument(
        "--regenerate", "-r",
        action="store_true",
        help="Force regeneration of all content without prompting"
    )
    parser.add_argument(
        "--no-prompt",
        action="store_true",
        help="Skip interactive prompts"
    )

    args = parser.parse_args()

    candidate_name = get_candidate_name_from_cwd()

    # Check if specification already exists
    spec_file = Path("specification.md")
    if spec_file.exists() and not args.regenerate:
        print(f"\nExisting specification.md found.")
        if args.no_prompt:
            print("Skipping regeneration (use --regenerate to force).")
            return 2

        if not sys.stdin.isatty():
            print("Non-interactive mode detected. Skipping regeneration (use --regenerate to force).")
            return 2

        print(f"\nRegenerate English specification?")
        try:
            response = input("Regenerate? [y/N]: ").strip().lower()
            if response not in ('y', 'yes'):
                print("Skipping regeneration.")
                return 2
        except (EOFError, KeyboardInterrupt):
            print("\nSkipping regeneration.")
            return 2

    print(f"Generating {candidate_name} substrate...")

    # Load the rulebook
    try:
        rulebook = load_rulebook()
        rulebook_name = rulebook.get('Name', 'Unknown')
        print(f"  Loaded rulebook: {rulebook_name}")
    except FileNotFoundError as e:
        print(f"Error: {e}")
        return 1

    # Generate specification via LLM
    print("\n=== Generating Specification (LLM-driven) ===")
    print(f"  Generating specification via LLM...")

    # ALWAYS ask before making LLM call (unless --no-prompt or non-interactive)
    if not args.no_prompt and sys.stdin.isatty():
        model = get_model_for_tier(args.tier, args.provider)
        print(f"\n  This will call {args.provider.upper()} ({model}) to generate the specification.")
        try:
            response = input("  Proceed with LLM call? [y/N]: ").strip().lower()
            if response not in ('y', 'yes'):
                print("  Skipping LLM call.")
                return 2
        except (EOFError, KeyboardInterrupt):
            print("\n  Skipping LLM call.")
            return 2

    spec_content = generate_specification(rulebook, args.provider, args.tier)
    with open("specification.md", 'w', encoding='utf-8') as f:
        f.write(spec_content)
    print("  Created specification.md")

    print(f"\nDone generating {candidate_name} substrate.")
    return 0


if __name__ == "__main__":
    sys.exit(main())

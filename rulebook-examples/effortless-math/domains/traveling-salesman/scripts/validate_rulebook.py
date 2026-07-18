#!/usr/bin/env python3
from validate_rulebook_v4 import validate_repository_state
from validate_summary_alignment_v4 import validate_summary_alignment

if __name__ == "__main__":
    validate_repository_state()
    validate_summary_alignment()

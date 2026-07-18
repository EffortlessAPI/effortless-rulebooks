#!/usr/bin/env python3
from validate_rulebook_v3_temporal import main as validate_v3
from validate_rulebook_v5 import validate_repository_state
from validate_summary_alignment_v5 import validate_summary_alignment

if __name__ == "__main__":
    validate_v3()
    validate_repository_state()
    validate_summary_alignment()

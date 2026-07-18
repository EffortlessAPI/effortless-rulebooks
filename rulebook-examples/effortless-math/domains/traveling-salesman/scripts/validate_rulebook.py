#!/usr/bin/env python3
from validate_rulebook_v3_temporal import main as validate_v3
from validate_rulebook_v5_temporal import main as validate_v5
from validate_rulebook_v6 import validate_repository_state
from validate_summary_alignment_v6 import validate_summary_alignment

if __name__ == "__main__":
    validate_v3()
    validate_v5()
    validate_repository_state()
    validate_summary_alignment()

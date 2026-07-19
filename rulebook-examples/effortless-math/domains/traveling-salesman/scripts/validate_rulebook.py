#!/usr/bin/env python3
from validate_rulebook_v3_temporal import main as validate_v3
from validate_rulebook_v5_temporal import main as validate_v5
from validate_rulebook_v6 import validate_repository_state as validate_v6
from validate_summary_alignment_v6 import validate_summary_alignment as validate_summary_v6
from validate_rulebook_v7 import validate_repository_state as validate_v7
from validate_summary_alignment_v7 import validate_summary_alignment as validate_summary_v7

if __name__ == "__main__":
    validate_v3()
    validate_v5()
    validate_v6()
    validate_summary_v6()
    validate_v7()
    validate_summary_v7()

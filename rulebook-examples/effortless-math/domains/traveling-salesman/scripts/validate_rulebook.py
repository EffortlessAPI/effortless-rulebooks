#!/usr/bin/env python3
from validate_rulebook_v3 import main as validate_rulebook
from validate_summary_alignment import main as validate_summary


if __name__ == "__main__":
    validate_rulebook()
    validate_summary()

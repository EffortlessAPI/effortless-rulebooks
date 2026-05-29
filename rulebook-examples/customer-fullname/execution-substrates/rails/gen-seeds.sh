#!/bin/bash
cd "$(dirname "$0")"
export ERB_RULEBOOK_PATH="../../effortless-rulebook/customer-fullname-rulebook.json"
export ERB_OUTPUT_DIR="."
python3 rulebook-to-rails-seeds.py

#!/usr/bin/env python3
from __future__ import annotations

import argparse
import shutil
from pathlib import Path

parser = argparse.ArgumentParser(
    description='Stage the effortless-math payload as an effortless-rulebooks example domain.'
)
parser.add_argument('repo_root', type=Path, help='Path to the effortless-rulebooks repository root')
args = parser.parse_args()

source = Path(__file__).resolve().parents[1]
repo_root = args.repo_root.resolve()

required = [repo_root / 'CLAUDE.md', repo_root / 'rulebook-examples']
for path in required:
    if not path.exists():
        raise FileNotFoundError(f'Expected effortless-rulebooks path is missing: {path}')

destination = repo_root / 'rulebook-examples' / 'effortless-math'
if destination.exists():
    raise FileExistsError(f'Destination already exists; refusing to merge or overwrite: {destination}')

shutil.copytree(source, destination)
print(f'Staged payload at: {destination}')
print('Next: read the repository root CLAUDE.md, then run:')
print('  cd ' + str(repo_root))
print('  ERB_DOMAIN=effortless-math python rulebook-examples/effortless-math/scripts/validate_starter.py')
print('  ERB_DOMAIN=effortless-math effortless build')

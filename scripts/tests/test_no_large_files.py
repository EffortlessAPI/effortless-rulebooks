#!/usr/bin/env python3
"""No file kept in this repository is larger than 50 MB.

GitHub warns at 50 MB and refuses a push holding a file over 100 MB, and a refused push takes
every other commit in it down with it. Two raw datasets of 94 and 95 MB sat in
rulebook-examples/simpsons-paradox/data/loop-88-candidates until 2026-11-22. Raw data that large
is downloaded when needed (its candidate.json says from where) and ignored, not committed.

    python3 scripts/tests/test_no_large_files.py        (or pytest)
"""
import subprocess
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
LIMIT_MB = 50


class NoLargeFiles(unittest.TestCase):
    def test_no_tracked_file_is_over_the_limit(self):
        listed = subprocess.run(["git", "ls-files", "-z"], cwd=REPO, capture_output=True, check=True).stdout
        large = []
        for name in listed.decode().split("\0"):
            path = REPO / name
            if name and path.is_file() and not path.is_symlink():
                size = path.stat().st_size
                if size > LIMIT_MB * 1024 * 1024:
                    large.append(f"{size / 1048576:.0f} MB  {name}")
        self.assertEqual(large, [], f"files over {LIMIT_MB} MB are kept in the repository")


if __name__ == "__main__":
    unittest.main()

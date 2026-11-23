#!/usr/bin/env python3
"""scripts/scan-project-slots.py refuses to run when the folders under rulebook-examples/ and
toy-rulebooks/ and the root rulebook's rows disagree. It stopped running for two months because
three things were on disk and not in the rulebook, and nothing said so until someone ran it.

This holds, on the repository as it is: every project folder has its RulebookDomains row, every
row has its one launch profile with a primary service (the three rows a new project needs), and
`assets/` (the images the areas' READMEs show) is not taken for a project.

    python3 scripts/tests/test_scan_project_slots.py        (or pytest)
"""
import importlib.util
import json
import shutil
import sys
import tempfile
import unittest
from collections import OrderedDict
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
spec = importlib.util.spec_from_file_location("scan_project_slots", REPO / "scripts" / "scan-project-slots.py")
scan = importlib.util.module_from_spec(spec)
sys.modules["scan_project_slots"] = scan
spec.loader.exec_module(scan)


class ThisRepository(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.rulebook = json.loads((REPO / "effortless-rulebook" / "effortless-rulebook.json").read_text(encoding="utf-8"),
                                  object_pairs_hook=OrderedDict)

    def test_every_project_folder_has_a_row_and_every_row_a_folder(self):
        scan.modeled_project_directories(self.rulebook, REPO)      # raises ScanError naming the difference

    def test_every_row_has_one_launch_profile_with_a_primary_service(self):
        domains = {row["DomainId"] for row in self.rulebook["RulebookDomains"]["data"]}
        profiles = [row["Domain"] for row in self.rulebook["ProjectLaunchProfiles"]["data"]]
        self.assertEqual(sorted(domains - set(profiles)), [], "rows with no launch profile")
        self.assertEqual(sorted(set(profiles) - domains), [], "launch profiles of no row")
        self.assertEqual(len(profiles), len(set(profiles)), "a row has two launch profiles")
        with_primary = {row["LaunchProfile"] for row in self.rulebook["ProjectLocalServices"]["data"]
                        if row["ServiceRole"] == "primary"}
        need_one = {row["ProjectLaunchProfileId"] for row in self.rulebook["ProjectLaunchProfiles"]["data"]
                    if row["RequiresLocalUrl"]}
        self.assertEqual(sorted(need_one - with_primary), [], "launch profiles that need a URL and have no primary service")


class WhatCountsAsAProject(unittest.TestCase):
    def setUp(self):
        self.root = Path(tempfile.mkdtemp(prefix="erb-scan-")).resolve()
        self.addCleanup(shutil.rmtree, self.root, ignore_errors=True)
        for folder in ("rulebook-examples/assets", "toy-rulebooks/assets", "toy-rulebooks/demo", "toy-rulebooks/.hidden"):
            (self.root / folder).mkdir(parents=True)
        self.rulebook = {"RulebookDomains": {"data": [
            {"DomainId": "domain-root", "RelativePath": "./"},
            {"DomainId": "domain-demo", "RelativePath": "toy-rulebooks/demo/"}]}}

    def test_assets_is_not_a_project(self):
        found = scan.modeled_project_directories(self.rulebook, self.root)
        self.assertEqual([row["DomainId"] for row, _, _ in found], ["domain-root", "domain-demo"])

    def test_any_other_folder_without_a_row_stops_the_scan_and_says_what_it_needs(self):
        (self.root / "toy-rulebooks" / "new-one").mkdir()
        with self.assertRaises(scan.ScanError) as stopped:
            scan.modeled_project_directories(self.rulebook, self.root)
        self.assertIn("domain-new-one", str(stopped.exception))
        self.assertIn("ProjectLaunchProfiles", str(stopped.exception))


if __name__ == "__main__":
    unittest.main()

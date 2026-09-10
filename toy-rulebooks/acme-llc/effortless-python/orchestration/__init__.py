# Marks `orchestration` as a Python package. Kept intentionally non-empty: a
# zero-byte file shipped through the tool's FileSet HTTP response has been
# observed to get dropped by the conformance harness's FileSet-XML parser
# (tool-client.ts's extractFiles(), shared by every rulebook-to-* conformance
# runner) — a self-closing `<FileContents />` tag for an empty file doesn't
# match its `<FileContents>...</FileContents>` regex, so the file silently
# never reaches the harness. A one-line comment sidesteps that without
# touching the shared transport code every other runner also depends on.

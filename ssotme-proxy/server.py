#!/usr/bin/env python3
"""
ssotme-proxy — local transpiler server on localhost:4242

Each transpiler is a route.  POST to the route, get a fileset back.

    POST /rulebook-to-python
    POST /rulebook-to-postgres
    POST /airtable-to-rulebook

Request body (JSON):
    {
        "inputFile":  "<path to effortless-rulebook.json>",
        "outputDir":  "<path to project output folder>",
        "clean":      false
    }
    Paths are resolved relative to X-Working-Dir header (or server CWD).

Response (JSON):
    {
        "success": true,
        "output":  "...(stdout/stderr)...",
        "files":   { "filename.py": "...content..." }
    }

The injector script IS the body of the route handler — the proxy sets
ERB_RULEBOOK_PATH + ERB_OUTPUT_DIR and runs it.  No separate mechanism.

Install into a project:
    effortless -install http://localhost:4242/rulebook-to-python \\
        -i effortless-rulebook/effortless-rulebook.json \\
        -o python/
"""

import json
import os
import subprocess
import sys
import traceback
from http.server import BaseHTTPRequestHandler, HTTPServer
from pathlib import Path
from urllib.parse import urlparse

PORT = 4242
SCRIPT_DIR = Path(__file__).resolve().parent
PROJECT_ROOT = SCRIPT_DIR.parent

# ---------------------------------------------------------------------------
# Transpiler registry  —  route → injector script
# ---------------------------------------------------------------------------
TRANSPILERS = {
    "rulebook-to-python": {
        "script": PROJECT_ROOT / "execution-substrates" / "python" / "inject-into-python.py",
        "description": "Generate Python calculation library from rulebook",
    },
    "rulebook-to-postgres": {
        "script": PROJECT_ROOT / "execution-substrates" / "effortless-postgres" / "inject-into-postgres.py",
        "description": "Generate PostgreSQL schema/functions/views from rulebook",
    },
    "airtable-to-rulebook": {
        "tool": "airtable-to-rulebook",
        "description": "Pull rulebook from Airtable base",
    },
}


def resolve_path(s: str, working_dir: str) -> Path:
    p = Path(s)
    return p if p.is_absolute() else (Path(working_dir) / p).resolve()


def run_injector(cfg: dict, input_file: Path, output_dir: Path, clean: bool) -> dict:
    script = cfg.get("script")
    tool = cfg.get("tool")

    if script:
        if not script.exists():
            return {"success": False, "output": f"Injector not found: {script}", "files": {}}
        cmd = [sys.executable, str(script)]
    elif tool:
        cmd = [tool, "-o", str(output_dir / "effortless-rulebook.json"),
               "-account", "airtable", "-p", "view=Grid view"]
    else:
        return {"success": False, "output": "No script or tool configured", "files": {}}

    if clean:
        cmd.append("--clean")

    output_dir.mkdir(parents=True, exist_ok=True)
    env = os.environ.copy()
    env["ERB_RULEBOOK_PATH"] = str(input_file)
    env["ERB_OUTPUT_DIR"] = str(output_dir)

    result = subprocess.run(cmd, cwd=str(output_dir), env=env,
                            capture_output=True, text=True)

    # Collect written files as fileset
    files = {}
    for f in output_dir.rglob("*"):
        if f.is_file():
            try:
                files[str(f.relative_to(output_dir))] = f.read_text(encoding="utf-8")
            except Exception:
                files[str(f.relative_to(output_dir))] = "<binary>"

    return {
        "success": result.returncode == 0,
        "output": result.stdout + result.stderr,
        "files": files,
    }


class ProxyHandler(BaseHTTPRequestHandler):
    def log_message(self, fmt, *args):
        print(f"[ssotme-proxy] {fmt % args}")

    def send_json(self, status: int, body: dict):
        data = json.dumps(body, indent=2).encode()
        self.send_response(status)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", len(data))
        self.end_headers()
        self.wfile.write(data)

    def do_GET(self):
        path = urlparse(self.path).path
        if path in ("/", "/ping"):
            self.send_json(200, {
                "status": "ok",
                "server": "ssotme-proxy",
                "port": PORT,
                "transpilers": {k: v["description"] for k, v in TRANSPILERS.items()},
            })
        else:
            self.send_json(404, {"error": "Not found"})

    def do_POST(self):
        route = urlparse(self.path).path.lstrip("/")
        working_dir = self.headers.get("X-Working-Dir", os.getcwd())

        if route not in TRANSPILERS:
            self.send_json(404, {
                "error": f"Unknown transpiler '{route}'",
                "available": list(TRANSPILERS.keys()),
            })
            return

        length = int(self.headers.get("Content-Length", 0))
        body_bytes = self.rfile.read(length) if length else b"{}"
        try:
            req = json.loads(body_bytes)
        except json.JSONDecodeError as e:
            self.send_json(400, {"error": f"Invalid JSON: {e}"})
            return

        input_file = resolve_path(req.get("inputFile", ""), working_dir)
        output_dir = resolve_path(req.get("outputDir", route), working_dir)
        clean = bool(req.get("clean", False))

        try:
            result = run_injector(TRANSPILERS[route], input_file, output_dir, clean)
        except Exception:
            result = {"success": False, "output": traceback.format_exc(), "files": {}}

        self.send_json(200 if result["success"] else 500, result)


def main():
    for name, cfg in TRANSPILERS.items():
        script = cfg.get("script")
        if script and not script.exists():
            print(f"[warn] {name}: injector not found at {script}")

    server = HTTPServer(("localhost", PORT), ProxyHandler)
    print(f"ssotme-proxy  http://localhost:{PORT}")
    for name, cfg in TRANSPILERS.items():
        print(f"  POST /{name}  —  {cfg['description']}")
    print("  GET  /ping")
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        print("\nssotme-proxy stopped.")


if __name__ == "__main__":
    main()

#!/bin/bash
# create-substrate-report.sh - Generate substrate-report.html for English substrate

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

python3 << 'PYTHON_SCRIPT'
import html
import os
import re

SUBSTRATE_NAME = "english"
SUBSTRATE_TITLE = "English (NLG) Execution Substrate"
SUBSTRATE_ICON = "📝"

def read_file(path, default=""):
    try:
        with open(path, 'r') as f:
            return f.read()
    except:
        return default

def markdown_to_html(md):
    """Simple markdown to HTML conversion"""
    lines = md.split('\n')
    html_out = []
    in_list = False
    in_code = False

    for line in lines:
        # Code blocks
        if line.startswith('```'):
            if in_code:
                html_out.append('</pre>')
                in_code = False
            else:
                html_out.append('<pre>')
                in_code = True
            continue
        if in_code:
            html_out.append(html.escape(line))
            continue

        # Headers
        if line.startswith('### '):
            html_out.append(f'<h4>{html.escape(line[4:])}</h4>')
        elif line.startswith('## '):
            html_out.append(f'<h3>{html.escape(line[3:])}</h3>')
        elif line.startswith('# '):
            html_out.append(f'<h2>{html.escape(line[2:])}</h2>')
        # Lists
        elif line.startswith('- '):
            if not in_list:
                html_out.append('<ul>')
                in_list = True
            html_out.append(f'<li>{html.escape(line[2:])}</li>')
        else:
            if in_list and line.strip() == '':
                html_out.append('</ul>')
                in_list = False
            elif line.strip():
                # Bold
                line = re.sub(r'\*\*(.+?)\*\*', r'<strong>\1</strong>', line)
                html_out.append(f'<p>{line}</p>')

    if in_list:
        html_out.append('</ul>')
    return '\n'.join(html_out)

log_content = read_file('.last-run.log', 'No log available')
test_results = read_file('test-results.md', 'No test results available')
specification = read_file('specification.md', '# No specification available')

score_match = re.search(r'(\d+\.?\d*)%', test_results)
score = score_match.group(0) if score_match else "N/A"
passed_match = re.search(r'\| Passed \| (\d+)', test_results)
passed = passed_match.group(1) if passed_match else "0"
failed_match = re.search(r'\| Failed \| (\d+)', test_results)
failed = failed_match.group(1) if failed_match else "0"
total_match = re.search(r'\| Total Fields Tested \| (\d+)', test_results)
total = total_match.group(1) if total_match else "0"

score_num = float(score.replace('%', '')) if '%' in score else 0
if score_num >= 100: score_class = "score-perfect"
elif score_num >= 80: score_class = "score-good"
elif score_num >= 60: score_class = "score-warning"
else: score_class = "score-danger"

log_escaped = html.escape(log_content)
spec_html = markdown_to_html(specification)

html_template = '''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Substrate Report: {substrate_name}</title>
    <style>
:root {{
    --bg-primary: #ffffff; --bg-secondary: #f8f9fa; --bg-tertiary: #e9ecef;
    --text-primary: #212529; --text-secondary: #6c757d; --border-color: #dee2e6;
    --accent-color: #0d6efd; --success-color: #198754; --warning-color: #ffc107;
    --danger-color: #dc3545; --code-bg: #f6f8fa; --shadow: 0 2px 8px rgba(0,0,0,0.1); --radius: 6px;
}}
[data-theme="dark"] {{
    --bg-primary: #1a1a2e; --bg-secondary: #16213e; --bg-tertiary: #0f3460;
    --text-primary: #eaeaea; --text-secondary: #b0b0b0; --border-color: #3a3a5c;
    --accent-color: #4dabf7; --success-color: #51cf66; --warning-color: #fcc419;
    --danger-color: #ff6b6b; --code-bg: #0d1117; --shadow: 0 2px 8px rgba(0,0,0,0.3);
}}
* {{ margin: 0; padding: 0; box-sizing: border-box; }}
body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; background: var(--bg-secondary); color: var(--text-primary); line-height: 1.5; min-height: 100vh; }}
header {{ background: var(--bg-primary); border-bottom: 1px solid var(--border-color); padding: 0.75rem 1rem; display: flex; justify-content: space-between; align-items: center; }}
.header-left {{ display: flex; align-items: center; gap: 0.75rem; }}
.substrate-icon {{ font-size: 1.5rem; }}
h1 {{ font-size: 1.1rem; font-weight: 600; }}
.header-stats {{ display: flex; gap: 1rem; font-size: 0.8rem; }}
.stat {{ display: flex; align-items: center; gap: 0.25rem; }}
.stat-value {{ font-weight: 600; }}
.score-perfect, .score-good {{ color: var(--success-color); }}
.score-warning {{ color: var(--warning-color); }}
.score-danger {{ color: var(--danger-color); }}
#theme-toggle {{ background: var(--bg-tertiary); border: 1px solid var(--border-color); border-radius: 50%; width: 28px; height: 28px; cursor: pointer; display: flex; align-items: center; justify-content: center; font-size: 0.9rem; }}
#theme-toggle:hover {{ background: var(--accent-color); color: white; }}
#theme-toggle .moon {{ display: none; }}
[data-theme="dark"] #theme-toggle .sun {{ display: none; }}
[data-theme="dark"] #theme-toggle .moon {{ display: inline; }}
.tabs {{ display: flex; background: var(--bg-primary); border-bottom: 1px solid var(--border-color); overflow-x: auto; padding: 0 0.5rem; }}
.tab {{ background: none; border: none; padding: 0.5rem 1rem; cursor: pointer; font-size: 0.8rem; color: var(--text-secondary); border-bottom: 2px solid transparent; white-space: nowrap; }}
.tab:hover {{ color: var(--text-primary); }}
.tab.active {{ color: var(--accent-color); border-bottom-color: var(--accent-color); font-weight: 500; }}
main {{ padding: 1rem; }}
.tab-content {{ display: none; }}
.tab-content.active {{ display: block; animation: fadeIn 0.2s ease; }}
@keyframes fadeIn {{ from {{ opacity: 0; }} to {{ opacity: 1; }} }}
.card {{ background: var(--bg-primary); border: 1px solid var(--border-color); border-radius: var(--radius); padding: 1rem; margin-bottom: 1rem; box-shadow: var(--shadow); }}
.card h2 {{ font-size: 0.9rem; font-weight: 600; margin-bottom: 0.75rem; }}
pre {{ background: var(--code-bg); border: 1px solid var(--border-color); border-radius: var(--radius); padding: 0.75rem; overflow-x: auto; font-family: 'SF Mono', Monaco, monospace; font-size: 0.75rem; line-height: 1.4; max-height: 500px; overflow-y: auto; }}
.code-info {{ font-size: 0.75rem; color: var(--text-secondary); margin-bottom: 0.5rem; }}
.markdown-content {{ line-height: 1.7; }}
.markdown-content h2 {{ font-size: 1.1rem; font-weight: 600; margin: 1.5rem 0 0.75rem 0; border-bottom: 1px solid var(--border-color); padding-bottom: 0.5rem; }}
.markdown-content h3 {{ font-size: 1rem; font-weight: 600; margin: 1.25rem 0 0.5rem 0; }}
.markdown-content h4 {{ font-size: 0.9rem; font-weight: 600; margin: 1rem 0 0.5rem 0; color: var(--text-secondary); }}
.markdown-content p {{ margin-bottom: 0.75rem; }}
.markdown-content ul {{ margin-left: 1.5rem; margin-bottom: 0.75rem; }}
.markdown-content li {{ margin-bottom: 0.35rem; }}
.markdown-content pre {{ margin: 0.75rem 0; }}
.markdown-content strong {{ font-weight: 600; }}
.results-summary {{ display: flex; gap: 1.5rem; flex-wrap: wrap; margin-bottom: 1rem; }}
.result-item {{ text-align: center; }}
.result-value {{ font-size: 1.5rem; font-weight: 700; }}
.result-label {{ font-size: 0.7rem; color: var(--text-secondary); text-transform: uppercase; }}
.prose-container {{ max-height: 600px; overflow-y: auto; padding-right: 0.5rem; }}
    </style>
</head>
<body>
    <header>
        <div class="header-left">
            <span class="substrate-icon">{icon}</span>
            <h1>{title}</h1>
        </div>
        <div class="header-stats">
            <div class="stat"><span>Score:</span><span class="stat-value {score_class}">{score}</span></div>
            <div class="stat"><span>{passed}/{total} passed</span></div>
        </div>
        <button id="theme-toggle" title="Toggle theme"><span class="sun">☀️</span><span class="moon">🌙</span></button>
    </header>
    <nav class="tabs">
        <button class="tab active" data-tab="description">Description</button>
        <button class="tab" data-tab="log">Run Log</button>
        <button class="tab" data-tab="results">Test Results</button>
        <button class="tab" data-tab="specification">Specification</button>
    </nav>
    <main>
        <div id="description" class="tab-content active">
            <div class="card">
                <h2>What This Substrate Does</h2>
                <div class="markdown-content">
                    <p>The English substrate tests whether <strong>plain English can serve as an execution substrate</strong>. It uses a two-step LLM flow:</p>
                    <h3>Two-Step LLM Architecture</h3>
                    <ol>
                        <li><strong>Inject</strong>: <code>inject-into-english.py</code> sends the rulebook JSON to an LLM, which writes a plain English specification explaining how to compute each calculated field</li>
                        <li><strong>Take Test</strong>: <code>take-test.py</code> sends the English specification + test data to an LLM, which computes the calculated field values by following the English instructions</li>
                    </ol>
                    <p><strong>Zero formula parsing.</strong> The LLM reads formulas natively and explains them better than regex could.</p>
                </div>
            </div>
            <div class="card">
                <h2>Key Features</h2>
                <ul class="markdown-content">
                    <li><strong>Generic</strong>: Works with ANY rulebook - no domain-specific hardcoding</li>
                    <li><strong>LLM-Driven</strong>: Specification generated by LLM, not template strings</li>
                    <li><strong>Round-Trip Testing</strong>: Verifies the English spec is clear enough to compute correct values</li>
                    <li><strong>Minimal Code</strong>: ~130 lines total, down from ~425 lines</li>
                </ul>
            </div>
        </div>
        <div id="log" class="tab-content">
            <div class="card">
                <h2>Execution Log</h2>
                <pre>{log_content}</pre>
            </div>
        </div>
        <div id="results" class="tab-content">
            <div class="card">
                <h2>Test Summary</h2>
                <div class="results-summary">
                    <div class="result-item"><div class="result-value {score_class}">{score}</div><div class="result-label">Score</div></div>
                    <div class="result-item"><div class="result-value score-perfect">{passed}</div><div class="result-label">Passed</div></div>
                    <div class="result-item"><div class="result-value">{failed}</div><div class="result-label">Failed</div></div>
                    <div class="result-item"><div class="result-value">{total}</div><div class="result-label">Total</div></div>
                </div>
            </div>
        </div>
        <div id="specification" class="tab-content">
            <div class="card">
                <h2>specification.md (LLM-Generated)</h2>
                <div class="prose-container markdown-content">{specification}</div>
            </div>
        </div>
    </main>
    <script>
const themeToggle = document.getElementById('theme-toggle');
const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
function getInitialTheme() {{ try {{ if (window.parent && window.parent.document.documentElement.dataset.theme) return window.parent.document.documentElement.dataset.theme; }} catch (e) {{}} return prefersDark ? 'dark' : 'light'; }}
document.documentElement.dataset.theme = getInitialTheme();
themeToggle.addEventListener('click', () => {{ document.documentElement.dataset.theme = document.documentElement.dataset.theme === 'dark' ? 'light' : 'dark'; }});
window.addEventListener('message', (event) => {{ if (event.data.type === 'theme-change') document.documentElement.dataset.theme = event.data.theme; }});
document.querySelectorAll('.tab').forEach(tab => {{ tab.addEventListener('click', () => {{ document.querySelectorAll('.tab').forEach(t => t.classList.remove('active')); document.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active')); tab.classList.add('active'); document.getElementById(tab.dataset.tab).classList.add('active'); }}); }});
    </script>
</body>
</html>'''

output = html_template.format(
    substrate_name=SUBSTRATE_NAME, title=SUBSTRATE_TITLE, icon=SUBSTRATE_ICON,
    score=score, score_class=score_class, passed=passed, failed=failed, total=total,
    log_content=log_escaped,
    specification=spec_html
)

with open('substrate-report.html', 'w') as f:
    f.write(output)
print(f"Generated substrate-report.html for {SUBSTRATE_NAME}")
PYTHON_SCRIPT

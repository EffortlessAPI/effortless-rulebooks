// A page of the app in a real browser, for what only a page does: headless Chrome driven over its
// DevTools pipe (fd 3 in, fd 4 out, one JSON message per NUL), so no package is needed.
//
//   const page = await openPage('http://127.0.0.1:43284/admin.html');
//   await page.until('document.querySelector("#grid tbody tr")');
//   await page.run('document.querySelector("#add-row").click()');
//   await page.close();
import { spawn } from 'node:child_process';
import { once } from 'node:events';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';

const CHROME = process.env.CHROME_PATH || '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome';

export async function openPage(url, { timeoutMs = 20000 } = {}) {
  if (!fs.existsSync(CHROME)) throw new Error(`No Chrome at ${CHROME}: set CHROME_PATH to run the page tests`);
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-chrome-'));
  const chrome = spawn(CHROME, ['--headless=new', '--remote-debugging-pipe', '--no-first-run', '--no-default-browser-check',
    `--user-data-dir=${profile}`, 'about:blank'], { stdio: ['ignore', 'ignore', 'pipe', 'pipe', 'pipe'] });
  let log = '';
  chrome.stderr.on('data', (d) => { log += d; });
  const [, , , input, output] = chrome.stdio;
  let nextId = 1;
  const waiting = new Map();
  let buffer = '';
  output.on('data', (chunk) => {
    buffer += chunk;
    let end;
    while ((end = buffer.indexOf('\0')) >= 0) {
      const message = JSON.parse(buffer.slice(0, end));
      buffer = buffer.slice(end + 1);
      const w = message.id && waiting.get(message.id);
      if (!w) continue;
      waiting.delete(message.id);
      message.error ? w.reject(new Error(`${w.method}: ${message.error.message}`)) : w.resolve(message.result);
    }
  });
  const send = (method, params = {}, sessionId) => new Promise((resolve, reject) => {
    const id = nextId++;
    const timer = setTimeout(() => { waiting.delete(id); reject(new Error(`${method} got no answer in ${timeoutMs} ms\n${log}`)); }, timeoutMs);
    waiting.set(id, { method, resolve: (r) => { clearTimeout(timer); resolve(r); }, reject: (e) => { clearTimeout(timer); reject(e); } });
    input.write(JSON.stringify({ id, method, params, ...(sessionId ? { sessionId } : {}) }) + '\0');
  });
  // Chrome writes its profile until it has exited, so the profile goes after that.
  const close = async () => {
    if (chrome.exitCode === null && chrome.signalCode === null) { const exited = once(chrome, 'exit'); chrome.kill(); await exited; }
    fs.rmSync(profile, { recursive: true, force: true, maxRetries: 5 });
  };
  try {
    const { targetId } = await send('Target.createTarget', { url });
    const { sessionId } = await send('Target.attachToTarget', { targetId, flatten: true });
    // An expression's value in the page (awaited when it is a promise); a thrown error is thrown here.
    const run = async (expression) => {
      const r = await send('Runtime.evaluate', { expression, awaitPromise: true, returnByValue: true }, sessionId);
      if (r.exceptionDetails) throw new Error(`In the page: ${r.exceptionDetails.exception?.description || r.exceptionDetails.text}`);
      return r.result.value;
    };
    // Waits until an expression is truthy, and gives its value.
    const until = async (expression, what = expression) => {
      const start = Date.now();
      for (;;) {
        const value = await run(expression).catch(() => undefined);
        if (value) return value;
        if (Date.now() - start > timeoutMs) throw new Error(`The page never showed: ${what}\nIts message: ${await run("document.getElementById('message')?.textContent").catch(() => '?')}`);
        await new Promise((r) => setTimeout(r, 100));
      }
    };
    return { run, until, close };
  } catch (e) { await close(); throw e; }
}

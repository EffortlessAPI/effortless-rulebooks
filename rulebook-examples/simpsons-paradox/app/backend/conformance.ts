import { spawn } from 'node:child_process';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const PROJECT_ROOT = path.resolve(__dirname, '../..');
const CONFORMANCE_SCRIPT = path.join(PROJECT_ROOT, 'owl/run-conformance.sh');

export type ConformanceStatus = 'idle' | 'running' | 'passed' | 'failed';

export interface ConformanceRunState {
  status: ConformanceStatus;
  started_at: string | null;
  finished_at: string | null;
  exit_code: number | null;
  output: string;
}

let state: ConformanceRunState = {
  status: 'idle',
  started_at: null,
  finished_at: null,
  exit_code: null,
  output: '',
};

export function getConformanceState(): ConformanceRunState {
  return { ...state };
}

export function startConformanceRun(): ConformanceRunState {
  if (state.status === 'running') {
    throw new Error('Conformance test already running');
  }

  state = {
    status: 'running',
    started_at: new Date().toISOString(),
    finished_at: null,
    exit_code: null,
    output: '[conformance] starting OWL-SHACL vs Postgres receipt…\n',
  };

  const child = spawn('bash', [CONFORMANCE_SCRIPT], {
    cwd: PROJECT_ROOT,
    env: { ...process.env },
  });

  child.stdout.on('data', (chunk: Buffer) => {
    state.output += chunk.toString();
  });
  child.stderr.on('data', (chunk: Buffer) => {
    state.output += chunk.toString();
  });
  child.on('error', (err) => {
    state.output += `\n[conformance] spawn error: ${err.message}\n`;
    state.status = 'failed';
    state.exit_code = 1;
    state.finished_at = new Date().toISOString();
  });
  child.on('close', (code) => {
    state.exit_code = code ?? 1;
    state.status = code === 0 ? 'passed' : 'failed';
    state.finished_at = new Date().toISOString();
    if (!state.output.endsWith('\n')) {
      state.output += '\n';
    }
    state.output += `[conformance] finished exit ${state.exit_code}\n`;
  });

  return getConformanceState();
}

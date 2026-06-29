const DEFAULT_REPO = 'https://github.com/EffortlessAPI/effortless-rulebooks';
const DEFAULT_BRANCH = 'main';
const DEFAULT_PROJECT = 'rulebook-examples/simpsons-paradox';

function repoBase(): string {
  return (process.env.GITHUB_REPO_BASE ?? DEFAULT_REPO).replace(/\/$/, '');
}

function branch(): string {
  return process.env.GITHUB_BRANCH ?? DEFAULT_BRANCH;
}

function projectPath(): string {
  return process.env.GITHUB_PROJECT_PATH ?? DEFAULT_PROJECT;
}

/** Tree URL for the simpsons-paradox project folder on GitHub. */
export function projectTreeUrl(): string {
  return `${repoBase()}/tree/${branch()}/${projectPath()}`;
}

/** Blob URL for a file under the project folder. */
export function projectFileUrl(relativePath: string): string {
  const clean = relativePath.replace(/^\/+/, '');
  return `${repoBase()}/blob/${branch()}/${projectPath()}/${clean}`;
}

/** Commit URL (full or short hash). */
export function commitUrl(hash: string): string {
  return `${repoBase()}/commit/${hash}`;
}

/** Tag URL (points at tree for that tag under the project). */
export function tagUrl(tag: string): string {
  return `${repoBase()}/tree/${tag}/${projectPath()}`;
}

export const PROJECT_LINKS = {
  readme: () => projectFileUrl('README.md'),
  rulebook: () => projectFileUrl('effortless-rulebook/simpsons-paradox-rulebook.json'),
  rulespeak: () => projectFileUrl('rulespeak/rulespeak.md'),
  postgres: () => projectFileUrl('effortless-postgres/README.md'),
  explorerHtml: () => projectFileUrl('simpsons-paradox-explorer.html'),
  loops: () => projectFileUrl('effortless-rulebook/simpsons-paradox-rulebook.json'),
  conclusionsTable: () => projectFileUrl('effortless-rulebook/simpsons-paradox-rulebook.json'),
  invariantChecks: () => projectFileUrl('effortless-rulebook/simpsons-paradox-rulebook.json'),
} as const;

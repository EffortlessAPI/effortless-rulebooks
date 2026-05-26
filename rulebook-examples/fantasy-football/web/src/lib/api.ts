export async function fetchApi(path: string, options: RequestInit = {}) {
  const email = localStorage.getItem('userEmail')
  const headers = {
    'Content-Type': 'application/json',
    ...(email && { 'X-User-Email': email }),
    ...options.headers,
  }

  const response = await fetch(path, { ...options, headers })
  if (response.status === 401) {
    localStorage.removeItem('userEmail')
    if (email) {
      window.location.href = '/'
    }
    throw new Error('Not authenticated')
  }
  if (!response.ok) {
    throw new Error(`${response.status}: ${response.statusText}`)
  }
  return response.json()
}

export async function fetchMe() {
  try {
    return await fetchApi('/api/me')
  } catch {
    return null
  }
}

export async function fetchDevUsers() {
  return fetchApi('/api/dev-users')
}

export async function fetchRulebook() {
  return fetchApi('/api/rulebook')
}

export async function fetchPlayers() {
  return fetchApi('/api/players')
}

export async function fetchRosters() {
  return fetchApi('/api/rosters')
}

export async function fetchMatchups() {
  return fetchApi('/api/matchups')
}

export async function fetchStandings() {
  return fetchApi('/api/standings')
}

export async function updatePlayer(id: string, data: Record<string, unknown>) {
  return fetchApi(`/api/players/${id}`, {
    method: 'PATCH',
    body: JSON.stringify(data),
  })
}

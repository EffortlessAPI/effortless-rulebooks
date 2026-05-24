import { DagCell as DagCellWrapper } from '../explainer-dag'
import { DagCell } from '../components/DagCell'
import { useApi } from '../lib/useApi'
import { fetchMatchups } from '../lib/api'

interface Matchup {
  matchup_id: string
  name: string
  week: number | string
  team1: string
  team2: string
  team1_score: number | string
  team2_score: number | string
  margin: number | string
  winner: string
  loser: string
  is_tie_game: boolean
}

function parseNumber(val: number | string | null): number | null {
  if (val === null || val === undefined) return null
  return typeof val === 'string' ? parseFloat(val) : val
}

export function Matchups() {
  const { data: rawMatchups, loading, error } = useApi<Matchup[]>(fetchMatchups)
  const matchups = rawMatchups?.map(m => ({
    ...m,
    week: parseNumber(m.week) as number,
    team1_score: parseNumber(m.team1_score),
    team2_score: parseNumber(m.team2_score),
    margin: parseNumber(m.margin),
  })) ?? null

  if (loading) return <div style={{ padding: '2rem' }}>Loading matchups...</div>
  if (error) return <div style={{ padding: '2rem', color: 'red' }}>Error: {error}</div>
  if (!matchups || matchups.length === 0) return <div style={{ padding: '2rem' }}>No matchups</div>

  const grouped = matchups.reduce(
    (acc, m) => {
      if (!acc[m.week as unknown as number]) acc[m.week as unknown as number] = []
      acc[m.week as unknown as number].push(m)
      return acc
    },
    {} as Record<number, Matchup[]>
  )

  return (
    <div style={{ padding: '2rem' }}>
      <h1>Matchups</h1>

      {Object.entries(grouped)
        .sort(([a], [b]) => +a - +b)
        .map(([week, weekMatchups]) => (
          <div key={week} style={{ marginBottom: '2rem' }}>
            <h2>Week {week}</h2>
            <div style={{ display: 'grid', gap: '1rem' }}>
              {weekMatchups.map((m) => (
                <div
                  key={m.matchup_id}
                  style={{
                    background: 'white',
                    padding: '1.5rem',
                    border: '1px solid #ddd',
                    borderRadius: '8px',
                  }}
                >
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr auto 1fr', gap: '2rem', alignItems: 'center' }}>
                    <div>
                      <DagCellWrapper table="Matchups" field="Team1Score">
                        <DagCell label={m.team1} value={m.team1_score ? (m.team1_score as number).toFixed(2) : 'N/A'} />
                      </DagCellWrapper>
                    </div>

                    <div style={{ textAlign: 'center' }}>
                      {m.is_tie_game ? (
                        <div style={{ color: '#ff9800', fontSize: '1.2rem', fontWeight: 'bold' }}>TIED</div>
                      ) : (
                        <div>
                          <div style={{ fontSize: '0.85rem', color: '#666' }}>Margin</div>
                          <DagCellWrapper table="Matchups" field="Margin">
                            <DagCell
                              label="Margin"
                              value={m.margin ? Math.abs(m.margin as number).toFixed(2) : 'N/A'}
                              formula="Team1Score - Team2Score"
                            />
                          </DagCellWrapper>
                          <div style={{ marginTop: '0.5rem', fontSize: '0.9rem', fontWeight: 'bold' }}>
                            {m.winner === m.team1 ? `${m.team1} wins` : `${m.team2} wins`}
                          </div>
                        </div>
                      )}
                    </div>

                    <div>
                      <DagCellWrapper table="Matchups" field="Team2Score">
                        <DagCell label={m.team2} value={m.team2_score ? (m.team2_score as number).toFixed(2) : 'N/A'} />
                      </DagCellWrapper>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
    </div>
  )
}

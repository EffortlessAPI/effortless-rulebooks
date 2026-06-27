import { DagCell as DagCellWrapper } from '../explainer-dag'
import { DagCell } from '../components/DagCell'
import { useApi } from '../lib/useApi'
import { fetchStandings } from '../lib/api'

interface Standing {
  standing_id: string
  name: string
  owner: string
  wins: number | string
  losses: number | string
  ties: number | string
  win_pct: number | string
  points_for: number | string
  points_against: number | string
  seed_rank: number | string | null
  playoff_bound: boolean
  champs_path: string | null
}

function parseNumber(val: number | string | null): number | null {
  if (val === null || val === undefined) return null
  return typeof val === 'string' ? parseFloat(val) : val
}

export function Standings() {
  const { data: rawStandings, loading, error } = useApi<Standing[]>(fetchStandings)
  const standings = rawStandings?.map(s => ({
    ...s,
    win_pct: parseNumber(s.win_pct),
    points_for: parseNumber(s.points_for),
    points_against: parseNumber(s.points_against),
    seed_rank: parseNumber(s.seed_rank),
  })) ?? null

  if (loading) return <div style={{ padding: '2rem' }}>Loading standings...</div>
  if (error) return <div style={{ padding: '2rem', color: 'red' }}>Error: {error}</div>
  if (!standings || standings.length === 0) return <div style={{ padding: '2rem' }}>No standings data</div>

  return (
    <div style={{ padding: '2rem' }}>
      <h1>League Standings</h1>
      <div style={{ display: 'grid', gap: '2rem', marginTop: '2rem' }}>
        {standings.map((s) => (
          <div
            key={s.standing_id}
            style={{
              background: 'white',
              padding: '1.5rem',
              border: '1px solid #ddd',
              borderRadius: '8px',
            }}
          >
            <h2>{s.name}</h2>
            <p style={{ color: '#666', marginBottom: '1.5rem' }}>Owner: {s.owner}</p>

            <div
              style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
                gap: '1rem',
                marginBottom: '1.5rem',
              }}
            >
              <DagCell label="Wins" value={s.wins} formula="COUNT(Matchups WHERE Winner = ThisRoster)" />
              <DagCell label="Losses" value={s.losses} formula="COUNT(Matchups WHERE Loser = ThisRoster)" />
              <DagCellWrapper table="Standings" field="WinPct">
                <DagCell label="Win %" value={s.win_pct ? (s.win_pct as number).toFixed(2) : 'N/A'} formula="Wins / (Wins + Losses)" />
              </DagCellWrapper>
              <DagCellWrapper table="Standings" field="PointsFor">
                <DagCell
                  label="Points For"
                  value={s.points_for ? (s.points_for as number).toFixed(2) : 'N/A'}
                  formula="SUM(Matchup scores for this roster)"
                />
              </DagCellWrapper>
              <DagCellWrapper table="Standings" field="PointsAgainst">
                <DagCell
                  label="Points Against"
                  value={s.points_against ? (s.points_against as number).toFixed(2) : 'N/A'}
                  formula="SUM(opponent scores against this roster)"
                />
              </DagCellWrapper>
              <DagCellWrapper table="Standings" field="SeedRank">
                <DagCell
                  label="Playoff Seed"
                  value={s.seed_rank ? `#${s.seed_rank}` : 'N/A'}
                  formula="RANK() OVER (ORDER BY WinPct DESC, PointsFor DESC)"
                />
              </DagCellWrapper>
            </div>

            <div
              style={{
                padding: '1rem',
                background: s.playoff_bound ? '#d4edda' : '#f8d7da',
                border: `1px solid ${s.playoff_bound ? '#28a745' : '#f5c6cb'}`,
                borderRadius: '4px',
                color: s.playoff_bound ? '#155724' : '#721c24',
              }}
            >
              <strong>{s.playoff_bound ? '✓ Playoff Bound' : '✗ Eliminated'}</strong>
              {s.champs_path && <div style={{ marginTop: '0.5rem' }}>{s.champs_path}</div>}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

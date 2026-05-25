import { DagCell as DagCellWrapper } from '../explainer-dag'
import { DagCell } from '../components/DagCell'
import { useApi } from '../lib/useApi'
import { fetchRosters } from '../lib/api'

interface Roster {
  roster_id: string
  name: string
  owner: string
  total_passing_yards: number | string
  total_rushing_yards: number | string
  total_receiving_yards: number | string
  total_touchdowns: number | string
  total_interceptions: number | string
  projected_score: number | string
}

function parseNumber(val: number | string | null): number | null {
  if (val === null || val === undefined) return null
  return typeof val === 'string' ? parseFloat(val) : val
}

export function Rosters() {
  const { data: rawRosters, loading, error } = useApi<Roster[]>(fetchRosters)
  const rosters = rawRosters?.map(r => ({
    ...r,
    total_passing_yards: parseNumber(r.total_passing_yards),
    total_rushing_yards: parseNumber(r.total_rushing_yards),
    total_receiving_yards: parseNumber(r.total_receiving_yards),
    total_touchdowns: parseNumber(r.total_touchdowns),
    total_interceptions: parseNumber(r.total_interceptions),
    projected_score: parseNumber(r.projected_score),
  })) ?? null

  if (loading) return <div style={{ padding: '2rem' }}>Loading rosters...</div>
  if (error) return <div style={{ padding: '2rem', color: 'red' }}>Error: {error}</div>
  if (!rosters || rosters.length === 0) return <div style={{ padding: '2rem' }}>No rosters</div>

  return (
    <div style={{ padding: '2rem' }}>
      <h1>Rosters</h1>
      <div style={{ display: 'grid', gap: '2rem', marginTop: '2rem' }}>
        {rosters.map((r) => (
          <div
            key={r.roster_id}
            style={{
              background: 'white',
              padding: '1.5rem',
              border: '1px solid #ddd',
              borderRadius: '8px',
            }}
          >
            <h2>{r.name}</h2>
            <p style={{ color: '#666', marginBottom: '1.5rem' }}>Owner: {r.owner}</p>

            <div
              style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))',
                gap: '1rem',
              }}
            >
              <DagCellWrapper table="Rosters" field="TotalPassingYards">
                <DagCell
                  label="Pass Yards"
                  value={r.total_passing_yards}
                  formula="SUM(Players.PassingYards WHERE Position=QB)"
                />
              </DagCellWrapper>
              <DagCellWrapper table="Rosters" field="TotalRushingYards">
                <DagCell
                  label="Rush Yards"
                  value={r.total_rushing_yards}
                  formula="SUM(Players.RushingYards WHERE Position in RB,WR,TE)"
                />
              </DagCellWrapper>
              <DagCellWrapper table="Rosters" field="TotalReceivingYards">
                <DagCell
                  label="Rec Yards"
                  value={r.total_receiving_yards}
                  formula="SUM(Players.ReceivedYards WHERE Position in WR,TE,RB)"
                />
              </DagCellWrapper>
              <DagCellWrapper table="Rosters" field="TotalTouchdowns">
                <DagCell label="Total TDs" value={r.total_touchdowns} formula="SUM(Players.TotalTouchdowns)" />
              </DagCellWrapper>
              <DagCellWrapper table="Rosters" field="TotalInterceptions">
                <DagCell label="INTs" value={r.total_interceptions} formula="SUM(Players.Interceptions WHERE QB)" />
              </DagCellWrapper>
              <DagCellWrapper table="Rosters" field="ProjectedScore">
                <DagCell
                  label="Projected Score"
                  value={r.projected_score}
                  formula="(PassYds/25) + (RushYds/10) + (RecYds/10) + (TDs*6) - (INTs*2)"
                />
              </DagCellWrapper>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

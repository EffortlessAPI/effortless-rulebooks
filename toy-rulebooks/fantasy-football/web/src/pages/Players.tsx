import { useState } from 'react'
import { DagCell as DagCellWrapper } from '../explainer-dag'
import { DagCell } from '../components/DagCell'
import { useApi } from '../lib/useApi'
import { fetchPlayers, updatePlayer } from '../lib/api'

interface Player {
  player_id: string
  name: string
  position: string
  passing_yards: number | string
  passing_touchdowns: number | string
  interceptions: number | string
  rushing_yards: number | string
  rushing_touchdowns: number | string
  receptions: number | string
  received_yards: number | string
  received_touchdowns: number | string
  total_touchdowns: number | string
  projected_score: number | string
}

function parseNumber(val: number | string | null): number | null {
  if (val === null || val === undefined) return null
  return typeof val === 'string' ? parseFloat(val) : val
}

export function Players() {
  const { data: rawPlayers, loading, error, reload } = useApi<Player[]>(fetchPlayers)
  const players = rawPlayers?.map(p => ({
    ...p,
    passing_yards: parseNumber(p.passing_yards),
    passing_touchdowns: parseNumber(p.passing_touchdowns),
    interceptions: parseNumber(p.interceptions),
    rushing_yards: parseNumber(p.rushing_yards),
    rushing_touchdowns: parseNumber(p.rushing_touchdowns),
    receptions: parseNumber(p.receptions),
    received_yards: parseNumber(p.received_yards),
    received_touchdowns: parseNumber(p.received_touchdowns),
    total_touchdowns: parseNumber(p.total_touchdowns),
    projected_score: parseNumber(p.projected_score),
  })) ?? null
  const [editingId, setEditingId] = useState<string | null>(null)
  const [editValues, setEditValues] = useState<Record<string, number>>({})

  const handleEdit = (player: Player) => {
    setEditingId(player.player_id)
    setEditValues({
      passing_yards: (player.passing_yards as number) ?? 0,
      passing_touchdowns: (player.passing_touchdowns as number) ?? 0,
      interceptions: (player.interceptions as number) ?? 0,
      rushing_yards: (player.rushing_yards as number) ?? 0,
      rushing_touchdowns: (player.rushing_touchdowns as number) ?? 0,
      receptions: (player.receptions as number) ?? 0,
      received_yards: (player.received_yards as number) ?? 0,
      received_touchdowns: (player.received_touchdowns as number) ?? 0,
    })
  }

  const handleSave = async () => {
    try {
      await updatePlayer(editingId!, editValues)
      setEditingId(null)
      await reload()
    } catch (err) {
      console.error(err)
      alert('Failed to update player')
    }
  }

  if (loading) return <div style={{ padding: '2rem' }}>Loading players...</div>
  if (error) return <div style={{ padding: '2rem', color: 'red' }}>Error: {error}</div>
  if (!players || players.length === 0) return <div style={{ padding: '2rem' }}>No players</div>

  return (
    <div style={{ padding: '2rem' }}>
      <h1>Players</h1>
      <div style={{ display: 'grid', gap: '1.5rem', marginTop: '2rem' }}>
        {players.map((p) => (
          <div
            key={p.player_id}
            style={{
              background: 'white',
              padding: '1.5rem',
              border: '1px solid #ddd',
              borderRadius: '8px',
            }}
          >
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
              <div>
                <h3>{p.name}</h3>
                <p style={{ color: '#666' }}>{p.position}</p>
              </div>
              {editingId !== p.player_id && (
                <button
                  onClick={() => handleEdit(p)}
                  style={{
                    padding: '0.5rem 1rem',
                    background: '#007bff',
                    color: 'white',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: 'pointer',
                  }}
                >
                  Edit Stats
                </button>
              )}
            </div>

            {editingId === p.player_id ? (
              <div style={{ marginTop: '1rem', display: 'grid', gap: '0.5rem' }}>
                <label>
                  Passing Yards:
                  <input
                    type="number"
                    value={editValues.passing_yards}
                    onChange={(e) => setEditValues({ ...editValues, passing_yards: +e.target.value })}
                    style={{ marginLeft: '0.5rem', width: '100px' }}
                  />
                </label>
                <label>
                  Passing TDs:
                  <input
                    type="number"
                    value={editValues.passing_touchdowns}
                    onChange={(e) => setEditValues({ ...editValues, passing_touchdowns: +e.target.value })}
                    style={{ marginLeft: '0.5rem', width: '100px' }}
                  />
                </label>
                <label>
                  Interceptions:
                  <input
                    type="number"
                    value={editValues.interceptions}
                    onChange={(e) => setEditValues({ ...editValues, interceptions: +e.target.value })}
                    style={{ marginLeft: '0.5rem', width: '100px' }}
                  />
                </label>
                <label>
                  Rushing Yards:
                  <input
                    type="number"
                    value={editValues.rushing_yards}
                    onChange={(e) => setEditValues({ ...editValues, rushing_yards: +e.target.value })}
                    style={{ marginLeft: '0.5rem', width: '100px' }}
                  />
                </label>
                <label>
                  Rushing TDs:
                  <input
                    type="number"
                    value={editValues.rushing_touchdowns}
                    onChange={(e) => setEditValues({ ...editValues, rushing_touchdowns: +e.target.value })}
                    style={{ marginLeft: '0.5rem', width: '100px' }}
                  />
                </label>
                <label>
                  Receptions:
                  <input
                    type="number"
                    value={editValues.receptions}
                    onChange={(e) => setEditValues({ ...editValues, receptions: +e.target.value })}
                    style={{ marginLeft: '0.5rem', width: '100px' }}
                  />
                </label>
                <label>
                  Receiving Yards:
                  <input
                    type="number"
                    value={editValues.received_yards}
                    onChange={(e) => setEditValues({ ...editValues, received_yards: +e.target.value })}
                    style={{ marginLeft: '0.5rem', width: '100px' }}
                  />
                </label>
                <label>
                  Receiving TDs:
                  <input
                    type="number"
                    value={editValues.received_touchdowns}
                    onChange={(e) => setEditValues({ ...editValues, received_touchdowns: +e.target.value })}
                    style={{ marginLeft: '0.5rem', width: '100px' }}
                  />
                </label>
                <div style={{ display: 'flex', gap: '0.5rem', marginTop: '0.5rem' }}>
                  <button
                    onClick={handleSave}
                    style={{
                      padding: '0.5rem 1rem',
                      background: '#28a745',
                      color: 'white',
                      border: 'none',
                      borderRadius: '4px',
                      cursor: 'pointer',
                    }}
                  >
                    Save
                  </button>
                  <button
                    onClick={() => setEditingId(null)}
                    style={{
                      padding: '0.5rem 1rem',
                      background: '#6c757d',
                      color: 'white',
                      border: 'none',
                      borderRadius: '4px',
                      cursor: 'pointer',
                    }}
                  >
                    Cancel
                  </button>
                </div>
              </div>
            ) : (
              <div
                style={{
                  display: 'grid',
                  gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))',
                  gap: '0.5rem',
                  marginTop: '1rem',
                }}
              >
                <DagCell label="Pass Yards" value={p.passing_yards} />
                <DagCell label="Pass TDs" value={p.passing_touchdowns} />
                <DagCell label="INTs" value={p.interceptions} />
                <DagCell label="Rush Yards" value={p.rushing_yards} />
                <DagCell label="Rush TDs" value={p.rushing_touchdowns} />
                <DagCell label="Receptions" value={p.receptions} />
                <DagCell label="Rec Yards" value={p.received_yards} />
                <DagCell label="Rec TDs" value={p.received_touchdowns} />
                <DagCellWrapper table="Players" field="TotalTouchdowns">
                  <DagCell label="Total TDs" value={p.total_touchdowns} formula="PassingTDs + RushingTDs + RecTDs" />
                </DagCellWrapper>
                <DagCellWrapper table="Players" field="ProjectedScore">
                  <DagCell
                    label="Projected Score"
                    value={p.projected_score}
                    formula="(PassYds/25) + (TDs*6) - (INTs*2)"
                  />
                </DagCellWrapper>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  )
}

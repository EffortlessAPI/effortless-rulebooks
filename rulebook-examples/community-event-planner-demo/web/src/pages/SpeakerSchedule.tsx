import { useEffect, useState } from 'react'
import { DagCell } from '../explainer-dag'

interface Speaker {
  speaker_id: string
  name: string
  availability_start: string
  availability_end: string
  assignment_count: number
  is_overbooked: boolean
}

export default function SpeakerSchedule() {
  const [speakers, setSpeakers] = useState<Speaker[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetch('/api/speakers')
      .then(r => r.json())
      .then(setSpeakers)
      .finally(() => setLoading(false))
      .catch(console.error)
  }, [])

  if (loading) {
    return <div className="card">Loading speakers...</div>
  }

  const overbookedSpeakers = speakers.filter(s => s.is_overbooked)
  const availableSpeakers = speakers.filter(s => !s.is_overbooked)

  return (
    <div>
      <h2 style={{ marginBottom: '1.5rem' }}>Speaker Schedule & Workload</h2>

      {overbookedSpeakers.length > 0 && (
        <div className="card" style={{ borderLeft: '4px solid #ef4444', marginBottom: '1.5rem' }}>
          <h3 style={{ color: '#ef4444' }}>⚠️ <DagCell table="Speakers" field="IsOverbooked">Overbooked Speakers</DagCell></h3>
          <p>The following speakers are assigned to more than 3 events:</p>
          <ul style={{ marginLeft: '1rem' }}>
            {overbookedSpeakers.map(s => (
              <li key={s.speaker_id}>
                <strong>{s.name}</strong> —{' '}
                <DagCell table="Speakers" field="AssignmentCount">
                  {s.assignment_count} assignments
                </DagCell>
                <br />
                <span style={{ fontSize: '0.9rem', color: '#666' }}>
                  Available: {new Date(s.availability_start).toLocaleString()} to{' '}
                  {new Date(s.availability_end).toLocaleString()}
                </span>
              </li>
            ))}
          </ul>
        </div>
      )}

      <div className="card">
        <h3>Available Speakers</h3>
        <ul style={{ marginLeft: '1rem' }}>
          {availableSpeakers.map(s => (
            <li key={s.speaker_id} style={{ marginBottom: '0.5rem' }}>
              <strong>{s.name}</strong> —{' '}
              <DagCell table="Speakers" field="AssignmentCount">
                {s.assignment_count} assignment{s.assignment_count !== 1 ? 's' : ''}
              </DagCell>
            </li>
          ))}
        </ul>
      </div>
    </div>
  )
}

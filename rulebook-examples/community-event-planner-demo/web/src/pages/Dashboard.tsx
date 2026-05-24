import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { DagCell } from '../explainer-dag'

interface Event {
  event_id: string
  name: string
  event_date: string
  total_speakers_assigned: number
  booked_capacity: number
  available_capacity: number
  event_status: string
  venue_conflict_count: number
}

export default function Dashboard() {
  const [events, setEvents] = useState<Event[]>([])
  const [loading, setLoading] = useState(true)
  const navigate = useNavigate()

  useEffect(() => {
    fetch('/api/events')
      .then(r => r.json())
      .then(setEvents)
      .finally(() => setLoading(false))
      .catch(console.error)
  }, [])

  if (loading) {
    return <div className="card">Loading events...</div>
  }

  const readyCount = events.filter(e => e.event_status === 'ready').length
  const totalAttendees = events.reduce((sum, e) => sum + e.booked_capacity, 0)

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h2>Dashboard</h2>
        <button onClick={() => navigate('/speakers')} style={{ padding: '0.5rem 1rem' }}>View Speaker Schedule</button>
      </div>

      <div className="grid" style={{ marginBottom: '2rem' }}>
        <div className="stat-box">
          <div className="label">Total Events</div>
          <DagCell table="Events" field="Name">
            <div className="value">{events.length}</div>
          </DagCell>
        </div>
        <div className="stat-box">
          <div className="label">Ready to Publish</div>
          <DagCell table="Events" field="EventStatus">
            <div className="value">{readyCount}</div>
          </DagCell>
        </div>
        <div className="stat-box">
          <div className="label">Total Attendees</div>
          <DagCell table="Events" field="BookedCapacity">
            <div className="value">{totalAttendees}</div>
          </DagCell>
        </div>
        <div className="stat-box">
          <div className="label">With Issues</div>
          <DagCell table="Events" field="EventStatus">
            <div className="value" style={{ color: '#ef4444' }}>
              {events.length - readyCount}
            </div>
          </DagCell>
        </div>
      </div>

      <div className="card">
        <h2>Upcoming Events</h2>
        <ul className="event-list">
          {events.map(event => (
            <li
              key={event.event_id}
              className="event-item"
              onClick={() => navigate(`/events/${event.event_id}`)}
            >
              <div className="event-item-title">{event.name}</div>
              <div className="event-item-meta">
                <div>
                  📅 {new Date(event.event_date).toLocaleDateString()} at{' '}
                  {new Date(event.event_date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </div>
                <div>
                  👥 <DagCell table="Events" field="TotalSpeakersAssigned">{event.total_speakers_assigned}</DagCell> speaker
                  {event.total_speakers_assigned !== 1 ? 's' : ''} |{' '}
                  <DagCell table="Events" field="BookedCapacity">{event.booked_capacity}</DagCell> attendees |{' '}
                  <DagCell table="Events" field="AvailableCapacity">{event.available_capacity}</DagCell> seats left
                </div>
                <div>
                  Status: <DagCell table="Events" field="EventStatus">
                    <span className={`status-${event.event_status}`}>{event.event_status.toUpperCase()}</span>
                  </DagCell>
                  {event.venue_conflict_count > 1 && ' ⚠️ Venue Conflict'}
                </div>
              </div>
            </li>
          ))}
        </ul>
      </div>
    </div>
  )
}

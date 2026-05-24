import { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { DagCell } from '../explainer-dag'

interface Event {
  event_id: string
  name: string
  venue: string
  venue_capacity: number
  event_date: string
  duration_minutes: number
  total_speakers_assigned: number
  has_speakers: boolean
  booked_capacity: number
  available_capacity: number
  at_capacity: boolean
  has_venue_conflict: boolean
  event_status: string
  registration_deadline?: string
  registration_close_days_before_event?: number
  is_registration_open?: boolean
  category?: string
}

interface Assignment {
  assignment_id: string
  speaker_ref: string
  is_available: boolean
}

interface User {
  email: string
  role: string
}

export default function EventDetail() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [event, setEvent] = useState<Event | null>(null)
  const [assignments, setAssignments] = useState<Assignment[]>([])
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)
  const [showEditModal, setShowEditModal] = useState(false)
  const [editName, setEditName] = useState('')
  const [editVenue, setEditVenue] = useState('')
  const [editDate, setEditDate] = useState('')
  const [editDuration, setEditDuration] = useState(0)
  const [editCloseDaysBeforeEvent, setEditCloseDaysBeforeEvent] = useState<number>(1)
  const [editCategory, setEditCategory] = useState('')
  const [editError, setEditError] = useState('')
  const [isSaving, setIsSaving] = useState(false)
  const isAdmin = user?.role === 'Admin'

  const REGISTRATION_CLOSE_OPTIONS = [
    ...Array.from({ length: 30 }, (_, i) => i + 1),
    60, 90, 120, 150, 180, 365
  ]

  const formatOptionLabel = (days: number): string => {
    if (days <= 30) return `${days} day${days === 1 ? '' : 's'}`
    if (days === 60) return '2 months'
    if (days === 90) return '3 months'
    if (days === 120) return '4 months'
    if (days === 150) return '5 months'
    if (days === 180) return '6 months'
    if (days === 365) return '1 year'
    return `${days} days`
  }

  useEffect(() => {
    Promise.all([
      fetch(`/api/events/${id}`).then(r => r.json()),
      fetch(`/api/assignments/${id}`).then(r => r.json()),
      fetch(`/api/me`).then(r => r.json())
    ])
      .then(([e, a, u]) => {
        setEvent(e)
        setAssignments(a)
        setUser(u)
      })
      .finally(() => setLoading(false))
      .catch(console.error)
  }, [id])

  const openEditModal = () => {
    if (!event) return
    setEditError('')
    setEditName(event.name)
    setEditVenue(event.venue)
    setEditDate(event.event_date.split('T')[0])
    setEditDuration(event.duration_minutes)
    setEditCloseDaysBeforeEvent(event.registration_close_days_before_event || 1)
    setEditCategory(event.category || '')
    setShowEditModal(true)
  }

  const handleSave = async () => {
    if (!event) return
    setEditError('')
    setIsSaving(true)
    try {
      const response = await fetch(`/api/events/${id}`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          name: editName,
          venue: editVenue,
          event_date: editDate,
          duration_minutes: editDuration,
          registration_close_days_before_event: editCloseDaysBeforeEvent,
          category: editCategory || null
        })
      })
      if (!response.ok) {
        let errorMessage = 'Failed to save event'
        try {
          const errorData = await response.json()
          errorMessage = errorData.error || errorMessage
        } catch (e) {
          errorMessage = `Server error (${response.status})`
        }
        setEditError(errorMessage)
        setIsSaving(false)
        return
      }
      const refreshed = await fetch(`/api/events/${id}`).then(r => r.json())
      setEvent(refreshed)
      setShowEditModal(false)
      setIsSaving(false)
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Network error'
      setEditError(`Failed to save: ${message}`)
      setIsSaving(false)
    }
  }

  if (loading) {
    return <div className="card">Loading...</div>
  }

  if (!event) {
    return <div className="card">Event not found</div>
  }

  return (
    <div>
      <button onClick={() => navigate('/')} style={{ marginBottom: '1rem' }}>← Back</button>

      <div className="card">
        <h2>{event.name}</h2>

        <div className="grid" style={{ marginBottom: '2rem', gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))' }}>
          <div>
            <strong>Venue:</strong> {event.venue}
          </div>
          <div>
            <strong>Date:</strong> {new Date(event.event_date).toLocaleDateString()}
          </div>
          <div>
            <strong>Time:</strong> {new Date(event.event_date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
          </div>
          <div>
            <strong>Duration:</strong> {event.duration_minutes} mins
          </div>
        </div>

        <h3>Event Status</h3>
        <div style={{ marginBottom: '2rem' }}>
          <div style={{ marginBottom: '0.5rem' }}>
            <DagCell table="Events" field="EventStatus">
              <span className={`status-${event.event_status}`}>{event.event_status.toUpperCase()}</span>
            </DagCell>
          </div>
          <ul style={{ marginLeft: '1rem' }}>
            <li><DagCell table="Events" field="HasSpeakers">{event.has_speakers ? '✓' : '✗'}</DagCell> Speakers assigned: <DagCell table="Events" field="TotalSpeakersAssigned">{event.total_speakers_assigned}</DagCell></li>
            <li><DagCell table="Events" field="AtCapacity">{!event.at_capacity ? '✓' : '✗'}</DagCell> Capacity available: <DagCell table="Events" field="AvailableCapacity">{event.available_capacity}</DagCell> / <DagCell table="Events" field="BookedCapacity">{event.booked_capacity}</DagCell></li>
            <li><DagCell table="Events" field="HasVenueConflict">{!event.has_venue_conflict ? '✓' : '✗'}</DagCell> No venue conflicts</li>
          </ul>
        </div>

        <h3>Speakers</h3>
        <ul style={{ marginBottom: '2rem', marginLeft: '1rem' }}>
          {assignments.map(a => (
            <li key={a.assignment_id}>
              <DagCell table="Assignments" field="IsAvailable">{a.is_available ? '✓' : '✗'}</DagCell> {a.speaker_ref}
            </li>
          ))}
        </ul>

        <h3>Attendees</h3>
        <div style={{ marginBottom: '2rem' }}>
          <div>Booked: <strong><DagCell table="Events" field="BookedCapacity">{event.booked_capacity}</DagCell></strong></div>
          <div>Available: <strong><DagCell table="Events" field="AvailableCapacity">{event.available_capacity}</DagCell></strong></div>
        </div>

        <h3>Details</h3>
        <div style={{ marginBottom: '2rem' }}>
          <div>Category: <strong><DagCell table="Events" field="Category">{event.category || 'Uncategorized'}</DagCell></strong></div>
        </div>

        <h3>Registration</h3>
        <div style={{ marginBottom: '2rem' }}>
          <div>
            Deadline: <strong>{event.registration_deadline ? new Date(event.registration_deadline).toLocaleString() : 'Not set'}</strong>
          </div>
          <div>
            Status: <DagCell table="Events" field="IsRegistrationOpen">
              <strong style={{ color: event.is_registration_open ? '#22c55e' : '#ef4444' }}>
                {event.is_registration_open ? 'Open' : 'Closed'}
              </strong>
            </DagCell>
          </div>
        </div>

        {isAdmin && (
          <button
            onClick={openEditModal}
            title="Edit event"
            style={{
              background: 'none',
              border: 'none',
              color: '#3b82f6',
              cursor: 'pointer',
              fontSize: '1.5rem',
              padding: '0.5rem',
              marginLeft: '-0.5rem'
            }}
          >
            ✏️
          </button>
        )}
      </div>

      {showEditModal && (
        <div
          style={{
            position: 'fixed',
            inset: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            zIndex: 1000
          }}
          onClick={() => setShowEditModal(false)}
        >
          <div
            className="card"
            style={{
              maxWidth: '500px',
              width: '90%',
              maxHeight: '90vh',
              overflowY: 'auto',
              backgroundColor: 'white'
            }}
            onClick={e => e.stopPropagation()}
          >
            <h2 style={{ marginBottom: '1.5rem' }}>Edit Event</h2>

            {editError && (
              <div
                style={{
                  backgroundColor: '#fee2e2',
                  border: '1px solid #fecaca',
                  color: '#991b1b',
                  padding: '0.75rem',
                  borderRadius: '4px',
                  marginBottom: '1rem',
                  fontSize: '0.9rem'
                }}
              >
                ⚠️ {editError}
              </div>
            )}

            <div className="form-group">
              <label>Event Name:</label>
              <input
                type="text"
                value={editName}
                onChange={e => setEditName(e.target.value)}
                style={{ width: '100%', marginBottom: '1rem' }}
              />
            </div>

            <div className="form-group">
              <label>Venue:</label>
              <input
                type="text"
                value={editVenue}
                onChange={e => setEditVenue(e.target.value)}
                style={{ width: '100%', marginBottom: '1rem' }}
              />
            </div>

            <div className="form-group">
              <label>Event Date & Time:</label>
              <input
                type="datetime-local"
                value={editDate}
                onChange={e => setEditDate(e.target.value)}
                style={{ width: '100%', marginBottom: '1rem' }}
              />
            </div>

            <div className="form-group">
              <label>Duration (minutes):</label>
              <input
                type="number"
                value={editDuration}
                onChange={e => setEditDuration(parseInt(e.target.value) || 0)}
                style={{ width: '100%', marginBottom: '1rem' }}
              />
            </div>

            <div className="form-group">
              <label>Registration Closes:</label>
              <select
                value={editCloseDaysBeforeEvent}
                onChange={e => setEditCloseDaysBeforeEvent(parseInt(e.target.value))}
                style={{ width: '100%', marginBottom: '1rem' }}
              >
                {REGISTRATION_CLOSE_OPTIONS.map(days => (
                  <option key={days} value={days}>
                    {formatOptionLabel(days)} before event
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label>Category:</label>
              <select
                value={editCategory}
                onChange={e => setEditCategory(e.target.value)}
                style={{ width: '100%', marginBottom: '1rem' }}
              >
                <option value="">Uncategorized</option>
                <option value="Tech">Tech</option>
                <option value="Learning">Learning</option>
                <option value="Community">Community</option>
              </select>
            </div>

            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <button
                onClick={handleSave}
                disabled={isSaving}
                style={{
                  background: isSaving ? '#9ca3af' : '#22c55e',
                  color: 'white',
                  padding: '0.5rem 1rem',
                  flex: 1,
                  cursor: isSaving ? 'not-allowed' : 'pointer',
                  opacity: isSaving ? 0.7 : 1
                }}
              >
                {isSaving ? 'Saving...' : 'Save'}
              </button>
              <button
                onClick={() => setShowEditModal(false)}
                disabled={isSaving}
                style={{
                  background: isSaving ? '#9ca3af' : '#666',
                  color: 'white',
                  padding: '0.5rem 1rem',
                  flex: 1,
                  cursor: isSaving ? 'not-allowed' : 'pointer',
                  opacity: isSaving ? 0.7 : 1
                }}
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

import { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { DagCell } from '../explainer-dag'

interface Venue {
  venue_id: string
  name: string
  location: string
  capacity: number
}

export default function VenueDetail() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [venue, setVenue] = useState<Venue | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetch(`/api/venues/${id}`)
      .then(r => r.json())
      .then(setVenue)
      .finally(() => setLoading(false))
      .catch(console.error)
  }, [id])

  if (loading) {
    return <div className="card">Loading...</div>
  }

  if (!venue) {
    return <div className="card">Venue not found</div>
  }

  return (
    <div>
      <button onClick={() => navigate('/')} style={{ marginBottom: '1rem' }}>← Back</button>

      <div className="card">
        <h2><DagCell table="Venues" field="Name">{venue.name}</DagCell></h2>
        <div className="grid" style={{ marginBottom: '2rem' }}>
          <div>
            <strong>Location:</strong> <DagCell table="Venues" field="Location">{venue.location}</DagCell>
          </div>
          <div>
            <strong>Capacity:</strong> <DagCell table="Venues" field="Capacity">{venue.capacity}</DagCell> seats
          </div>
        </div>
      </div>
    </div>
  )
}

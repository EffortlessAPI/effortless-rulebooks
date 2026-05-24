async function req(method, path, body) {
  const res = await fetch(path, {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: body ? JSON.stringify(body) : undefined,
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`${method} ${path} failed: ${res.status} ${text}`);
  }
  return res.json();
}

export const api = {
  identities: () => req('GET', '/api/identities'),
  schema: () => req('GET', '/api/schema'),
  events: () => req('GET', '/api/events'),
  event: (id) => req('GET', `/api/events/${id}`),
  createEvent: (data) => req('POST', '/api/events', data),
  updateEvent: (id, data) => req('PATCH', `/api/events/${id}`, data),
  deleteEvent: (id) => req('DELETE', `/api/events/${id}`),
  shift: (id) => req('GET', `/api/shifts/${id}`),
  createShift: (eventId, data) => req('POST', `/api/events/${eventId}/shifts`, data),
  updateShift: (id, data) => req('PATCH', `/api/shifts/${id}`, data),
  deleteShift: (id) => req('DELETE', `/api/shifts/${id}`),
  assign: (shiftId, volunteerId) =>
    req('POST', `/api/shifts/${shiftId}/assignments`, { volunteer_id: volunteerId }),
  unassign: (assignmentId) => req('DELETE', `/api/assignments/${assignmentId}`),
  volunteers: () => req('GET', '/api/volunteers'),
  createVolunteer: (data) => req('POST', '/api/volunteers', data),
  updateVolunteer: (id, data) => req('PATCH', `/api/volunteers/${id}`, data),
  deleteVolunteer: (id) => req('DELETE', `/api/volunteers/${id}`),
  skills: () => req('GET', '/api/skills'),
  volunteerShifts: (id) => req('GET', `/api/volunteers/${id}/shifts`),
};

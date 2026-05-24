-- Demo seed: one festival + one food bank event so the dashboard has variety.

INSERT INTO skills (name) VALUES
  ('General'),
  ('Food Handling'),
  ('First Aid'),
  ('Driving'),
  ('Setup / Teardown'),
  ('Registration');

INSERT INTO volunteers (name, email, reliability_score, max_hours) VALUES
  ('Ava Chen',        'ava@example.org',     0.97, 8),
  ('Ben Ortiz',       'ben@example.org',     0.92, 6),
  ('Cara Singh',      'cara@example.org',    0.88, 8),
  ('Diego Park',      'diego@example.org',   0.75, 4),
  ('Elena Müller',    'elena@example.org',   0.99, 10),
  ('Frank O''Neil',   'frank@example.org',   0.65, 6),
  ('Grace Liu',       'grace@example.org',   0.90, 8),
  ('Hassan Reyes',    'hassan@example.org',  0.81, 6),
  ('Ines Watanabe',   'ines@example.org',    0.94, 8),
  ('Jonah Bell',      'jonah@example.org',   0.70, 4);

-- Skill mappings
INSERT INTO volunteer_skills (volunteer_id, skill_id)
SELECT v.id, s.id FROM volunteers v, skills s
WHERE
  (v.name = 'Ava Chen'      AND s.name IN ('General','First Aid','Registration'))
  OR (v.name = 'Ben Ortiz'  AND s.name IN ('General','Setup / Teardown','Driving'))
  OR (v.name = 'Cara Singh' AND s.name IN ('General','Food Handling','Registration'))
  OR (v.name = 'Diego Park' AND s.name IN ('General','Setup / Teardown'))
  OR (v.name = 'Elena Müller' AND s.name IN ('General','First Aid','Food Handling','Driving'))
  OR (v.name = 'Frank O''Neil' AND s.name IN ('General'))
  OR (v.name = 'Grace Liu'  AND s.name IN ('General','Food Handling','Registration'))
  OR (v.name = 'Hassan Reyes' AND s.name IN ('General','Driving','Setup / Teardown'))
  OR (v.name = 'Ines Watanabe' AND s.name IN ('General','First Aid','Registration'))
  OR (v.name = 'Jonah Bell' AND s.name IN ('General','Setup / Teardown'));

-- Events
INSERT INTO events (name, location, event_date, target_hours_per_volunteer) VALUES
  ('Riverside Music Festival', 'Riverside Park', '2026-06-13', 5),
  ('Saturday Food Bank',       'Community Hall', '2026-05-30', 3);

-- Shifts for Festival (event id 1)
INSERT INTO shifts (event_id, name, starts_at, ends_at, required_count, required_skill_id)
SELECT 1, t.name, t.starts_at, t.ends_at, t.req, s.id FROM (
  VALUES
    ('Morning Setup',        TIMESTAMP '2026-06-13 07:00', TIMESTAMP '2026-06-13 10:00', 3, 'Setup / Teardown'),
    ('Gate Registration',    TIMESTAMP '2026-06-13 10:00', TIMESTAMP '2026-06-13 14:00', 4, 'Registration'),
    ('First Aid Station',    TIMESTAMP '2026-06-13 10:00', TIMESTAMP '2026-06-13 20:00', 2, 'First Aid'),
    ('Afternoon Floaters',   TIMESTAMP '2026-06-13 13:00', TIMESTAMP '2026-06-13 17:00', 3, 'General'),
    ('Evening Teardown',     TIMESTAMP '2026-06-13 19:00', TIMESTAMP '2026-06-13 22:00', 4, 'Setup / Teardown')
) AS t(name, starts_at, ends_at, req, skill_name)
LEFT JOIN skills s ON s.name = t.skill_name;

-- Shifts for Food Bank (event id 2)
INSERT INTO shifts (event_id, name, starts_at, ends_at, required_count, required_skill_id)
SELECT 2, t.name, t.starts_at, t.ends_at, t.req, s.id FROM (
  VALUES
    ('Truck Unload',         TIMESTAMP '2026-05-30 08:00', TIMESTAMP '2026-05-30 10:00', 3, 'General'),
    ('Sort & Pack',          TIMESTAMP '2026-05-30 09:00', TIMESTAMP '2026-05-30 12:00', 4, 'Food Handling'),
    ('Driver Runs',          TIMESTAMP '2026-05-30 10:00', TIMESTAMP '2026-05-30 13:00', 2, 'Driving'),
    ('Front Desk',           TIMESTAMP '2026-05-30 09:00', TIMESTAMP '2026-05-30 12:00', 2, 'Registration')
) AS t(name, starts_at, ends_at, req, skill_name)
LEFT JOIN skills s ON s.name = t.skill_name;

-- Assignments — deliberately uneven to make the cascade interesting.
-- Festival assignments
INSERT INTO assignments (shift_id, volunteer_id)
SELECT sh.id, v.id FROM shifts sh, volunteers v WHERE sh.name='Morning Setup' AND v.name IN ('Ben Ortiz','Diego Park');
INSERT INTO assignments (shift_id, volunteer_id)
SELECT sh.id, v.id FROM shifts sh, volunteers v WHERE sh.name='Gate Registration' AND v.name IN ('Ava Chen','Cara Singh','Ines Watanabe','Grace Liu');
INSERT INTO assignments (shift_id, volunteer_id)
SELECT sh.id, v.id FROM shifts sh, volunteers v WHERE sh.name='First Aid Station' AND v.name IN ('Elena Müller');
INSERT INTO assignments (shift_id, volunteer_id)
SELECT sh.id, v.id FROM shifts sh, volunteers v WHERE sh.name='Afternoon Floaters' AND v.name IN ('Frank O''Neil','Jonah Bell','Diego Park','Hassan Reyes');
INSERT INTO assignments (shift_id, volunteer_id)
SELECT sh.id, v.id FROM shifts sh, volunteers v WHERE sh.name='Evening Teardown' AND v.name IN ('Ben Ortiz','Hassan Reyes','Jonah Bell');

-- Food Bank assignments
INSERT INTO assignments (shift_id, volunteer_id)
SELECT sh.id, v.id FROM shifts sh, volunteers v WHERE sh.name='Truck Unload' AND v.name IN ('Ben Ortiz','Hassan Reyes','Diego Park');
INSERT INTO assignments (shift_id, volunteer_id)
SELECT sh.id, v.id FROM shifts sh, volunteers v WHERE sh.name='Sort & Pack' AND v.name IN ('Cara Singh','Grace Liu','Elena Müller');
INSERT INTO assignments (shift_id, volunteer_id)
SELECT sh.id, v.id FROM shifts sh, volunteers v WHERE sh.name='Driver Runs' AND v.name IN ('Ben Ortiz');
INSERT INTO assignments (shift_id, volunteer_id)
SELECT sh.id, v.id FROM shifts sh, volunteers v WHERE sh.name='Front Desk' AND v.name IN ('Ava Chen','Ines Watanabe');

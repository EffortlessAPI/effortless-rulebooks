-- ============================================================================
-- 99-fk-constraints.sql — FK CONSTRAINTS (off by default)
-- ============================================================================
-- Demos must never fail on FK violations, so init-db.sh SKIPS this file
-- unless EFFORTLESS_ENFORCE_FKS=true is set in the environment.
--
--   EFFORTLESS_ENFORCE_FKS=true bash init-db.sh    # apply constraints
--   bash init-db.sh                                # leave them documented but unenforced
--
-- The rulebook always documents the FK relationships, and 01-drop-and-create-tables.sql
-- always installs the supporting indexes inline. This file just declares the actual
-- enforcement. Idempotent: every constraint is dropped if present, then added.
-- ============================================================================

-- Cases
ALTER TABLE cases DROP CONSTRAINT IF EXISTS fk_cases_houses_timer;
ALTER TABLE cases ADD CONSTRAINT fk_cases_houses_timer
  FOREIGN KEY (houses_timer) REFERENCES timers (timer_id);

-- Bells
ALTER TABLE bells DROP CONSTRAINT IF EXISTS fk_bells_mounted_on_case;
ALTER TABLE bells ADD CONSTRAINT fk_bells_mounted_on_case
  FOREIGN KEY (mounted_on_case) REFERENCES cases (case_id);

-- WindingKnobs
ALTER TABLE winding_knobs DROP CONSTRAINT IF EXISTS fk_winding_knobs_fixed_to_timer;
ALTER TABLE winding_knobs ADD CONSTRAINT fk_winding_knobs_fixed_to_timer
  FOREIGN KEY (fixed_to_timer) REFERENCES timers (timer_id);

-- Arbors
ALTER TABLE arbors DROP CONSTRAINT IF EXISTS fk_arbors_in_timer;
ALTER TABLE arbors ADD CONSTRAINT fk_arbors_in_timer
  FOREIGN KEY (in_timer) REFERENCES timers (timer_id);

-- Mainsprings
ALTER TABLE mainsprings DROP CONSTRAINT IF EXISTS fk_mainsprings_wrapped_on_arbor;
ALTER TABLE mainsprings ADD CONSTRAINT fk_mainsprings_wrapped_on_arbor
  FOREIGN KEY (wrapped_on_arbor) REFERENCES arbors (arbor_id);

-- GearTrains
ALTER TABLE gear_trains DROP CONSTRAINT IF EXISTS fk_gear_trains_in_timer;
ALTER TABLE gear_trains ADD CONSTRAINT fk_gear_trains_in_timer
  FOREIGN KEY (in_timer) REFERENCES timers (timer_id);

-- Gears
ALTER TABLE gears DROP CONSTRAINT IF EXISTS fk_gears_in_gear_train;
ALTER TABLE gears ADD CONSTRAINT fk_gears_in_gear_train
  FOREIGN KEY (in_gear_train) REFERENCES gear_trains (gear_train_id);

-- OutputShafts
ALTER TABLE output_shafts DROP CONSTRAINT IF EXISTS fk_output_shafts_in_gear_train;
ALTER TABLE output_shafts ADD CONSTRAINT fk_output_shafts_in_gear_train
  FOREIGN KEY (in_gear_train) REFERENCES gear_trains (gear_train_id);

-- DialPointers
ALTER TABLE dial_pointers DROP CONSTRAINT IF EXISTS fk_dial_pointers_on_output_shaft;
ALTER TABLE dial_pointers ADD CONSTRAINT fk_dial_pointers_on_output_shaft
  FOREIGN KEY (on_output_shaft) REFERENCES output_shafts (output_shaft_id);

-- Escapements
ALTER TABLE escapements DROP CONSTRAINT IF EXISTS fk_escapements_in_timer;
ALTER TABLE escapements ADD CONSTRAINT fk_escapements_in_timer
  FOREIGN KEY (in_timer) REFERENCES timers (timer_id);

-- EscapementWheels
ALTER TABLE escapement_wheels DROP CONSTRAINT IF EXISTS fk_escapement_wheels_in_escapement;
ALTER TABLE escapement_wheels ADD CONSTRAINT fk_escapement_wheels_in_escapement
  FOREIGN KEY (in_escapement) REFERENCES escapements (escapement_id);

-- Pallets
ALTER TABLE pallets DROP CONSTRAINT IF EXISTS fk_pallets_in_escapement;
ALTER TABLE pallets ADD CONSTRAINT fk_pallets_in_escapement
  FOREIGN KEY (in_escapement) REFERENCES escapements (escapement_id);

-- Hairsprings
ALTER TABLE hairsprings DROP CONSTRAINT IF EXISTS fk_hairsprings_attached_to_pallet;
ALTER TABLE hairsprings ADD CONSTRAINT fk_hairsprings_attached_to_pallet
  FOREIGN KEY (attached_to_pallet) REFERENCES pallets (pallet_id);

-- Cams
ALTER TABLE cams DROP CONSTRAINT IF EXISTS fk_cams_on_arbor;
ALTER TABLE cams ADD CONSTRAINT fk_cams_on_arbor
  FOREIGN KEY (on_arbor) REFERENCES arbors (arbor_id);

-- BellHammers
ALTER TABLE bell_hammers DROP CONSTRAINT IF EXISTS fk_bell_hammers_in_timer;
ALTER TABLE bell_hammers ADD CONSTRAINT fk_bell_hammers_in_timer
  FOREIGN KEY (in_timer) REFERENCES timers (timer_id);

-- HammerArms
ALTER TABLE hammer_arms DROP CONSTRAINT IF EXISTS fk_hammer_arms_of_bell_hammer;
ALTER TABLE hammer_arms ADD CONSTRAINT fk_hammer_arms_of_bell_hammer
  FOREIGN KEY (of_bell_hammer) REFERENCES bell_hammers (bell_hammer_id);

-- Strikers
ALTER TABLE strikers DROP CONSTRAINT IF EXISTS fk_strikers_on_arm;
ALTER TABLE strikers ADD CONSTRAINT fk_strikers_on_arm
  FOREIGN KEY (on_arm) REFERENCES hammer_arms (hammer_arm_id);

-- HammerCatches
ALTER TABLE hammer_catches DROP CONSTRAINT IF EXISTS fk_hammer_catches_of_bell_hammer;
ALTER TABLE hammer_catches ADD CONSTRAINT fk_hammer_catches_of_bell_hammer
  FOREIGN KEY (of_bell_hammer) REFERENCES bell_hammers (bell_hammer_id);
ALTER TABLE hammer_catches DROP CONSTRAINT IF EXISTS fk_hammer_catches_engaged_with_cam;
ALTER TABLE hammer_catches ADD CONSTRAINT fk_hammer_catches_engaged_with_cam
  FOREIGN KEY (engaged_with_cam) REFERENCES cams (cam_id);

-- WindActions
ALTER TABLE wind_actions DROP CONSTRAINT IF EXISTS fk_wind_actions_performed_by;
ALTER TABLE wind_actions ADD CONSTRAINT fk_wind_actions_performed_by
  FOREIGN KEY (performed_by) REFERENCES users (user_id);
ALTER TABLE wind_actions DROP CONSTRAINT IF EXISTS fk_wind_actions_turned_knob;
ALTER TABLE wind_actions ADD CONSTRAINT fk_wind_actions_turned_knob
  FOREIGN KEY (turned_knob) REFERENCES winding_knobs (winding_knob_id);

-- TickEvents
ALTER TABLE tick_events DROP CONSTRAINT IF EXISTS fk_tick_events_happened_on_timer;
ALTER TABLE tick_events ADD CONSTRAINT fk_tick_events_happened_on_timer
  FOREIGN KEY (happened_on_timer) REFERENCES timers (timer_id);
ALTER TABLE tick_events DROP CONSTRAINT IF EXISTS fk_tick_events_at_escapement;
ALTER TABLE tick_events ADD CONSTRAINT fk_tick_events_at_escapement
  FOREIGN KEY (at_escapement) REFERENCES escapements (escapement_id);

-- RingEvents
ALTER TABLE ring_events DROP CONSTRAINT IF EXISTS fk_ring_events_happened_on_timer;
ALTER TABLE ring_events ADD CONSTRAINT fk_ring_events_happened_on_timer
  FOREIGN KEY (happened_on_timer) REFERENCES timers (timer_id);
ALTER TABLE ring_events DROP CONSTRAINT IF EXISTS fk_ring_events_struck;
ALTER TABLE ring_events ADD CONSTRAINT fk_ring_events_struck
  FOREIGN KEY (struck) REFERENCES bells (bell_id);

-- Cooks
ALTER TABLE cooks DROP CONSTRAINT IF EXISTS fk_cooks_on_timer;
ALTER TABLE cooks ADD CONSTRAINT fk_cooks_on_timer
  FOREIGN KEY (on_timer) REFERENCES timers (timer_id);
ALTER TABLE cooks DROP CONSTRAINT IF EXISTS fk_cooks_prepared_by;
ALTER TABLE cooks ADD CONSTRAINT fk_cooks_prepared_by
  FOREIGN KEY (prepared_by) REFERENCES users (user_id);
ALTER TABLE cooks DROP CONSTRAINT IF EXISTS fk_cooks_follows_recipe;
ALTER TABLE cooks ADD CONSTRAINT fk_cooks_follows_recipe
  FOREIGN KEY (follows_recipe) REFERENCES recipes (recipe_id);

-- 28 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).

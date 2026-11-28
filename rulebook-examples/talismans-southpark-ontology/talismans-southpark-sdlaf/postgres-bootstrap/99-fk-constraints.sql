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

-- Individuals
ALTER TABLE individuals DROP CONSTRAINT IF EXISTS fk_individuals_reasoning;
ALTER TABLE individuals ADD CONSTRAINT fk_individuals_reasoning
  FOREIGN KEY (reasoning) REFERENCES reasoning (reasoning_id);
ALTER TABLE individuals DROP CONSTRAINT IF EXISTS fk_individuals_portrait;
ALTER TABLE individuals ADD CONSTRAINT fk_individuals_portrait
  FOREIGN KEY (portrait) REFERENCES portraits (portrait_id);
ALTER TABLE individuals DROP CONSTRAINT IF EXISTS fk_individuals_attends;
ALTER TABLE individuals ADD CONSTRAINT fk_individuals_attends
  FOREIGN KEY (attends) REFERENCES individuals (individual_id);

-- Parenthoods
ALTER TABLE parenthoods DROP CONSTRAINT IF EXISTS fk_parenthoods_reasoning;
ALTER TABLE parenthoods ADD CONSTRAINT fk_parenthoods_reasoning
  FOREIGN KEY (reasoning) REFERENCES reasoning (reasoning_id);
ALTER TABLE parenthoods DROP CONSTRAINT IF EXISTS fk_parenthoods_parent;
ALTER TABLE parenthoods ADD CONSTRAINT fk_parenthoods_parent
  FOREIGN KEY (parent) REFERENCES individuals (individual_id);
ALTER TABLE parenthoods DROP CONSTRAINT IF EXISTS fk_parenthoods_child;
ALTER TABLE parenthoods ADD CONSTRAINT fk_parenthoods_child
  FOREIGN KEY (child) REFERENCES individuals (individual_id);

-- Marriages
ALTER TABLE marriages DROP CONSTRAINT IF EXISTS fk_marriages_reasoning;
ALTER TABLE marriages ADD CONSTRAINT fk_marriages_reasoning
  FOREIGN KEY (reasoning) REFERENCES reasoning (reasoning_id);
ALTER TABLE marriages DROP CONSTRAINT IF EXISTS fk_marriages_partner_a;
ALTER TABLE marriages ADD CONSTRAINT fk_marriages_partner_a
  FOREIGN KEY (partner_a) REFERENCES individuals (individual_id);
ALTER TABLE marriages DROP CONSTRAINT IF EXISTS fk_marriages_partner_b;
ALTER TABLE marriages ADD CONSTRAINT fk_marriages_partner_b
  FOREIGN KEY (partner_b) REFERENCES individuals (individual_id);

-- Friendships
ALTER TABLE friendships DROP CONSTRAINT IF EXISTS fk_friendships_reasoning;
ALTER TABLE friendships ADD CONSTRAINT fk_friendships_reasoning
  FOREIGN KEY (reasoning) REFERENCES reasoning (reasoning_id);
ALTER TABLE friendships DROP CONSTRAINT IF EXISTS fk_friendships_friend_a;
ALTER TABLE friendships ADD CONSTRAINT fk_friendships_friend_a
  FOREIGN KEY (friend_a) REFERENCES individuals (individual_id);
ALTER TABLE friendships DROP CONSTRAINT IF EXISTS fk_friendships_friend_b;
ALTER TABLE friendships ADD CONSTRAINT fk_friendships_friend_b
  FOREIGN KEY (friend_b) REFERENCES individuals (individual_id);

-- Grandparenthoods
ALTER TABLE grandparenthoods DROP CONSTRAINT IF EXISTS fk_grandparenthoods_reasoning;
ALTER TABLE grandparenthoods ADD CONSTRAINT fk_grandparenthoods_reasoning
  FOREIGN KEY (reasoning) REFERENCES reasoning (reasoning_id);
ALTER TABLE grandparenthoods DROP CONSTRAINT IF EXISTS fk_grandparenthoods_upper_link;
ALTER TABLE grandparenthoods ADD CONSTRAINT fk_grandparenthoods_upper_link
  FOREIGN KEY (upper_link) REFERENCES parenthoods (parenthood_id);
ALTER TABLE grandparenthoods DROP CONSTRAINT IF EXISTS fk_grandparenthoods_lower_link;
ALTER TABLE grandparenthoods ADD CONSTRAINT fk_grandparenthoods_lower_link
  FOREIGN KEY (lower_link) REFERENCES parenthoods (parenthood_id);

-- GradeLevelRecords
ALTER TABLE grade_level_records DROP CONSTRAINT IF EXISTS fk_grade_level_records_reasoning;
ALTER TABLE grade_level_records ADD CONSTRAINT fk_grade_level_records_reasoning
  FOREIGN KEY (reasoning) REFERENCES reasoning (reasoning_id);
ALTER TABLE grade_level_records DROP CONSTRAINT IF EXISTS fk_grade_level_records_individual;
ALTER TABLE grade_level_records ADD CONSTRAINT fk_grade_level_records_individual
  FOREIGN KEY (individual) REFERENCES individuals (individual_id);

-- RoleRecords
ALTER TABLE role_records DROP CONSTRAINT IF EXISTS fk_role_records_reasoning;
ALTER TABLE role_records ADD CONSTRAINT fk_role_records_reasoning
  FOREIGN KEY (reasoning) REFERENCES reasoning (reasoning_id);
ALTER TABLE role_records DROP CONSTRAINT IF EXISTS fk_role_records_individual;
ALTER TABLE role_records ADD CONSTRAINT fk_role_records_individual
  FOREIGN KEY (individual) REFERENCES individuals (individual_id);

-- 19 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).

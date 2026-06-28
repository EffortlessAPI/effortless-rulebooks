export type Role = "coordinator" | "bride" | "groom";

export type Me = {
  user_id: string;
  email: string;
  role: Role;
  display_name: string;
};

export type DevUser = Me;

export type Table = {
  table_id: string;
  name: string;
  label: string;
  seats: number;
  head_count: number;
  open_seats: number;
  over_capacity: boolean;
  raw_happiness: string | number;
  violation_count: number;
  happiness: string | number;
  grade: "Conflict" | "Over Capacity" | "Great" | "OK" | "Cold";
  bride_side_count: number;
  groom_side_count: number;
  side_skew: number;
};

export type Guest = {
  guest_id: string;
  name: string;
  full_name: string;
  side: "bride" | "groom" | "both";
  dietary: string;
  language: string;
  age_band: string;
  assigned_table: string | null;
  table_label: string | null;
  table_seats: number | null;
  table_head_count: number | null;
  satisfaction: string | number;
  mood: "Happy" | "Neutral" | "Unhappy";
};

export type Relationship = {
  rel_id: string;
  name: string;
  guest_a: string;
  guest_b: string;
  guest_a_name: string;
  guest_b_name: string;
  guest_a_table: string | null;
  guest_b_table: string | null;
  kind: "loves" | "prefers" | "avoid" | "must-not";
  weight: string | number;
  same_table: boolean;
  seated_table: string | null;
  effective_score: string | number;
  is_must_not_violation: boolean;
  is_satisfied: boolean;
  violation_table: string | null;
  violation_flag: string | number;
};

export type PlanScore = {
  total_happiness: string | number;
  over_capacity_count: number;
  must_not_violations: number;
  satisfied_pairs: number;
  total_pairs: number;
  total_guests: number;
  total_tables: number;
};

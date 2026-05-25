export type Role = 'researcher' | 'reviewer' | 'public';

export type DevUser = {
  email: string;
  name: string;
  role: Role;
  blurb: string;
};

export type Capability = {
  capabilities_id: string;
  name: string;
  description: string | null;
  tier: 'foundational' | 'composite' | 'emergent' | string;
  weight: string | number;
};

export type Intelligence = {
  intelligences_id: string;
  name: string;
  description: string | null;
  substrate: string;
  assessment_count: number;
  total_weighted_score: string | number;
  taxonomy_class: 'Generalist' | 'Broad' | 'Narrow' | string;
};

export type Assessment = {
  assessments_id: string;
  name: string;
  intelligence: string;
  capability: string;
  raw_score: number;
  intelligence_name: string | null;
  capability_name: string | null;
  capability_tier: string | null;
  capability_weight: string | number | null;
  weighted_score: string | number;
};

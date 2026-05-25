// Mirrors the shape of the field DAG response.

export type FieldType = "raw" | "calculated" | "aggregation" | "lookup" | "relationship";

export interface FieldRef {
  table: string;
  field: string;
}

export interface FieldNode extends FieldRef {
  datatype: string;
  type: FieldType;
  nullable: boolean;
  formula: string | null;
  description: string;
  relatedTo: string | null;
}

export interface DagResponse extends FieldNode {
  depth: number;
  fanIn: number;
  fanOut: number;
  upstream: FieldNode[];
  downstream: FieldNode[];
  leaves: FieldNode[];
  consumerTransitive: FieldRef[];
}

// Raw field shape as it appears inside a rulebook's `schema` array.
export interface RawField {
  name: string;
  datatype?: string;
  type?: FieldType;
  nullable?: boolean;
  formula?: string;
  RelatedTo?: string;
  Description?: string;
}

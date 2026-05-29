// Public barrel for the explainer-dag module.

export { FieldDag } from "./pages/FieldDag.tsx";
export type {
  FieldDagProps,
  ExplainerDagRouting,
  FieldLinkProps,
} from "./pages/FieldDag.tsx";

export { resolveDag, listTablesAndFields } from "./lib/dagResolver.ts";
export type {
  DagResponse,
  FieldNode,
  FieldRef,
  FieldType,
  RawField,
} from "./lib/types.ts";

export { rulebook } from "./embedded-rulebook.ts";
export type { Rulebook } from "./embedded-rulebook.ts";

// Optional companion components — host apps can drop these in alongside FieldDag.
export { DagCell } from "./components/DagCell.tsx";
export { DagHoverCard } from "./components/DagHoverCard.tsx";
export { DagToggle } from "./components/DagToggle.tsx";
export { FieldChip } from "./components/FieldChip.tsx";
export { FormulaText } from "./components/FormulaText.tsx";
export { TypeBadge, typeGlyph, typeLabel, typeTone } from "./components/TypeBadge.tsx";

// Routing context — for advanced consumers who want to nest <FieldChip /> /
// <DagCell /> outside of a <FieldDag /> tree.
export {
  RoutingContext,
  useFieldLink,
  useOnBack,
  useNavigateField,
} from "./lib/routingContext.tsx";

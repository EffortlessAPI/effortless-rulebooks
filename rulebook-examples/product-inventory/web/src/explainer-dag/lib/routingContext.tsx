// Routing context for the explainer DAG. The host app supplies a FieldLink
// component (and optional onBack / navigate fns) so this package never depends
// on a specific router.

import { createContext, useContext } from "react";
import type { ReactNode } from "react";

export interface FieldLinkProps {
  table: string;
  field: string;
  className?: string;
  children: ReactNode;
}

export interface ExplainerDagRouting {
  /** Render a link to another field's DAG page. */
  FieldLink?: (props: FieldLinkProps) => JSX.Element;
  /** Called when the user clicks the "← back" button. */
  onBack?: () => void;
  /** Programmatic navigation (used by DagCell's double-click). */
  navigate?: (table: string, field: string) => void;
}

function defaultHref(table: string, field: string): string {
  return `#/dag/${encodeURIComponent(table)}/${encodeURIComponent(field)}`;
}

function DefaultFieldLink(props: FieldLinkProps): JSX.Element {
  return (
    <a
      href={defaultHref(props.table, props.field)}
      className={props.className}
    >
      {props.children}
    </a>
  );
}

function defaultNavigate(table: string, field: string): void {
  if (typeof window !== "undefined") {
    window.location.hash = `#/dag/${encodeURIComponent(table)}/${encodeURIComponent(field)}`;
  }
}

export const RoutingContext = createContext<Required<ExplainerDagRouting>>({
  FieldLink: DefaultFieldLink,
  onBack: () => {
    if (typeof window !== "undefined") window.history.back();
  },
  navigate: defaultNavigate,
});

export function useFieldLink(): (props: FieldLinkProps) => JSX.Element {
  return useContext(RoutingContext).FieldLink;
}

export function useOnBack(): () => void {
  return useContext(RoutingContext).onBack;
}

export function useNavigateField(): (table: string, field: string) => void {
  return useContext(RoutingContext).navigate;
}

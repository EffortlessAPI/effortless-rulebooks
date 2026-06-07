// Routing context for the explainer DAG. The host app supplies a FieldLink /
// TableLink component (and optional onBack / onHome / navigate fns) so this
// package never depends on a specific router.

import { createContext, useContext } from "react";
import type { ReactNode } from "react";

export interface FieldLinkProps {
  table: string;
  field: string;
  className?: string;
  children: ReactNode;
}

export interface TableLinkProps {
  table: string;
  className?: string;
  children: ReactNode;
}

export interface ExplainerDagRouting {
  /** Render a link to another field's DAG page (/dag/:table/:field). */
  FieldLink?: (props: FieldLinkProps) => JSX.Element;
  /** Render a link to a table's page (/dag/:table). */
  TableLink?: (props: TableLinkProps) => JSX.Element;
  /** Called when the user clicks the "← back" button. */
  onBack?: () => void;
  /** Called when the user clicks the "🏠 Home" button — return to the host
   *  app's original page (the point the exploration started from). */
  onHome?: () => void;
  /** Programmatic navigation to a field page (used by DagCell's double-click). */
  navigate?: (table: string, field: string) => void;
  /** Programmatic navigation to a table page. */
  navigateTable?: (table: string) => void;
}

function fieldHref(table: string, field: string): string {
  return `#/dag/${encodeURIComponent(table)}/${encodeURIComponent(field)}`;
}
function tableHref(table: string): string {
  return `#/dag/${encodeURIComponent(table)}`;
}

function DefaultFieldLink(props: FieldLinkProps): JSX.Element {
  return (
    <a href={fieldHref(props.table, props.field)} className={props.className}>
      {props.children}
    </a>
  );
}

function DefaultTableLink(props: TableLinkProps): JSX.Element {
  return (
    <a href={tableHref(props.table)} className={props.className}>
      {props.children}
    </a>
  );
}

function defaultNavigate(table: string, field: string): void {
  if (typeof window !== "undefined") window.location.hash = fieldHref(table, field).slice(1);
}
function defaultNavigateTable(table: string): void {
  if (typeof window !== "undefined") window.location.hash = tableHref(table).slice(1);
}

// The default routing — used when the host app supplies nothing. Exported so the
// page wrappers (FieldDag / TablePage / TablesIndex) can fill gaps in a partial
// routing prop from ONE place instead of re-declaring the fallbacks each.
export const defaultRouting: Required<ExplainerDagRouting> = {
  FieldLink: DefaultFieldLink,
  TableLink: DefaultTableLink,
  onBack: () => {
    if (typeof window !== "undefined") window.history.back();
  },
  onHome: () => {
    // Default "home" leaves the explainer entirely — to the app root.
    if (typeof window !== "undefined") window.location.hash = "#/";
  },
  navigate: defaultNavigate,
  navigateTable: defaultNavigateTable,
};

// Fill any missing handlers on a partial routing prop with the defaults above.
//
// Hosts commonly wire only FieldLink + navigate (e.g. a react-router app that has
// just the field route). If we filled TableLink/navigateTable with the *hash*
// defaults (#/dag/...), those collide with a path router: from /dag/Customers a
// relative "#/dag/Customers" appends to the path → /dag/Customers/#/dag/Customers/.
// So before falling back to the hash defaults we DERIVE the table handlers from
// whatever path-based navigation the host DID supply:
//   • navigateTable ← host.navigate(table, "")  (the table page is the field route
//     with an empty field, which every host that has /dag/:table/:field can honor;
//     hosts with a dedicated /dag/:table should pass their own navigateTable)
//   • TableLink ← a button-style <a> that calls the (now-resolved) navigateTable,
//     so the click goes through the host's router instead of a hash href.
// Only when the host supplied NOTHING navigational do we keep the pure-hash
// defaults (a standalone, router-less embedding).
export function mergeRouting(
  r: ExplainerDagRouting | undefined,
): Required<ExplainerDagRouting> {
  const hostNavigate = r?.navigate;

  // Resolve navigateTable first — TableLink below depends on it.
  const navigateTable: (table: string) => void =
    r?.navigateTable ??
    (hostNavigate
      ? (table: string) => hostNavigate(table, "")
      : defaultRouting.navigateTable);

  const TableLink: (props: TableLinkProps) => JSX.Element =
    r?.TableLink ??
    (r?.navigateTable || hostNavigate
      ? (props: TableLinkProps) => (
          <a
            href={tableHref(props.table)}
            className={props.className}
            onClick={(e) => {
              // Let modifier-clicks (open in new tab) use the href; otherwise route.
              if (e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
              e.preventDefault();
              navigateTable(props.table);
            }}
          >
            {props.children}
          </a>
        )
      : defaultRouting.TableLink);

  return {
    FieldLink: r?.FieldLink ?? defaultRouting.FieldLink,
    TableLink,
    onBack: r?.onBack ?? defaultRouting.onBack,
    onHome: r?.onHome ?? defaultRouting.onHome,
    navigate: hostNavigate ?? defaultRouting.navigate,
    navigateTable,
  };
}

export const RoutingContext = createContext<Required<ExplainerDagRouting>>(defaultRouting);

export function useFieldLink(): (props: FieldLinkProps) => JSX.Element {
  return useContext(RoutingContext).FieldLink;
}

export function useTableLink(): (props: TableLinkProps) => JSX.Element {
  return useContext(RoutingContext).TableLink;
}

export function useOnBack(): () => void {
  return useContext(RoutingContext).onBack;
}

export function useOnHome(): () => void {
  return useContext(RoutingContext).onHome;
}

export function useNavigateField(): (table: string, field: string) => void {
  return useContext(RoutingContext).navigate;
}

export function useNavigateTable(): (table: string) => void {
  return useContext(RoutingContext).navigateTable;
}

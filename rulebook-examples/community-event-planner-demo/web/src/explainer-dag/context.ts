import { createContext } from 'react';

export interface RoutingAPI {
  navigate: (table: string, field: string) => void;
  onBack: () => void;
  FieldLink?: ({ table, field, children }: { table: string; field: string; children: React.ReactNode }) => React.ReactNode;
}

export const RoutingContext = createContext<RoutingAPI | null>(null);
export const DagToggleContext = createContext<boolean>(false);
export const DagToggleSetterContext = createContext<((val: boolean) => void) | null>(null);

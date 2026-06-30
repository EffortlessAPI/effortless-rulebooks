import type { ReactNode } from 'react';
import { V } from './DagValue';

export function Cell({
  table,
  col,
  children,
  className,
}: {
  table: string;
  col: string;
  children: ReactNode;
  className?: string;
}) {
  return (
    <V table={table} col={col} className={className}>
      {children}
    </V>
  );
}

export function TrTypeBadge({ type }: { type: string | null | undefined }) {
  if (!type) return null;
  const cls = type.startsWith('C') ? 'badge badge-type-c' : `badge badge-type-${type.toLowerCase()}`;
  return (
    <span className={cls}>
      Type <V table="TreatmentRankings" col="distortion_type">{type}</V>
    </span>
  );
}

export function TrTierPill({ tier }: { tier: string | null | undefined }) {
  if (!tier) return null;
  return (
    <span className={`tier-pill ${tier.toLowerCase()}`}>
      <V table="TreatmentRankings" col="screening_tier">{tier}</V>
    </span>
  );
}

export function TrCell({
  col,
  children,
  className,
}: {
  col: string;
  children: ReactNode;
  className?: string;
}) {
  return (
    <V table="TreatmentRankings" col={col} className={className}>
      {children}
    </V>
  );
}

export function SsCell({
  col,
  children,
  className,
}: {
  col: string;
  children: ReactNode;
  className?: string;
}) {
  return (
    <V table="StratumSummaries" col={col} className={className}>
      {children}
    </V>
  );
}

export function MsCell({
  col,
  children,
  className,
}: {
  col: string;
  children: ReactNode;
  className?: string;
}) {
  return (
    <V table="ModelSummary" col={col} className={className}>
      {children}
    </V>
  );
}

export function StudyCell({
  col,
  children,
  className,
}: {
  col: string;
  children: ReactNode;
  className?: string;
}) {
  return (
    <V table="Studies" col={col} className={className}>
      {children}
    </V>
  );
}

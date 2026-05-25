import { useState, useContext } from 'react';
import { RoutingContext, DagToggleContext } from './context';

interface Props {
  table: string;
  field: string;
  children: React.ReactNode;
  className?: string;
}

export function DagCell({ table, field, children, className = '' }: Props) {
  const [hover, setHover] = useState(false);
  const routing = useContext(RoutingContext);
  const dagEnabled = useContext(DagToggleContext);

  if (!dagEnabled || !routing) {
    return <>{children}</>;
  }

  const handleClick = () => {
    routing.navigate(table, field);
  };

  return (
    <span
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      onClick={handleClick}
      className={`dag-cell ${hover ? 'dag-cell-hover' : ''} ${className}`}
      style={{
        position: 'relative',
        cursor: 'pointer',
        outline: hover ? '2px solid #667eea' : 'none',
        outlineOffset: '2px',
        borderRadius: '2px',
        transition: 'outline 0.2s',
        padding: hover ? '2px' : '0px',
      }}
      title={`Click to explore ${table}.${field}`}
    >
      {children}
    </span>
  );
}

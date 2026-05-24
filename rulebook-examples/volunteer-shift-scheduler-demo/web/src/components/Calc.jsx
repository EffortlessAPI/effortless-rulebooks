import { useNavigate } from 'react-router-dom';
import { useExplain } from './ExplainContext.jsx';

// A clickable derived value.
//
//   value      — displayed text/number
//   field      — key into the entity's explain object
//   entity     — object with .explain[field] and an id (used for breadcrumbs)
//   entityKind — 'event' | 'shift' | 'volunteer'
//   entityLabel — friendly label for the explain card title
//
// Click → opens the (single-row) inline explain card.
// Shift-click / right-click "open DAG" → navigates to /dag/<table>/<field>
// for the full inference graph.
const ENTITY_TO_TABLE = {
  event: 'events',
  shift: 'shifts',
  volunteer: 'volunteer_event',
};

export default function Calc({ value, field, entity, entityKind, entityLabel }) {
  const { open } = useExplain();
  const navigate = useNavigate();

  if (!entity?.explain?.[field]) {
    return <span>{value}</span>;
  }

  const dagTable = ENTITY_TO_TABLE[entityKind] || entityKind;
  const dagHref = `/dag/${dagTable}/${field}`;

  const onClick = (e) => {
    e.stopPropagation();
    if (e.shiftKey || e.metaKey || e.ctrlKey) {
      navigate(dagHref);
      return;
    }
    open({
      title: `${entityLabel} · ${field}`,
      kind: entityKind,
      id: entity.id,
      field,
      explain: entity.explain[field],
    });
  };

  const onContext = (e) => {
    e.preventDefault();
    e.stopPropagation();
    navigate(dagHref);
  };

  return (
    <span
      className="calc"
      role="button"
      tabIndex={0}
      onClick={onClick}
      onContextMenu={onContext}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault();
          onClick(e);
        }
      }}
      title="Click for inline explain · shift-click (or right-click) for full DAG"
    >
      {value}
    </span>
  );
}

import type { Study } from '../types';

interface Props {
  studies: Study[];
  selected: string;
  onChange: (id: string) => void;
}

export function StudySelector({ studies, selected, onChange }: Props) {
  return (
    <div className="study-selector">
      <label htmlFor="study-pick">Study:</label>
      <select
        id="study-pick"
        value={selected}
        onChange={e => onChange(e.target.value)}
      >
        {studies.map(s => (
          <option key={s.study_id} value={s.study_id}>
            {s.title || s.name}
          </option>
        ))}
      </select>
    </div>
  );
}

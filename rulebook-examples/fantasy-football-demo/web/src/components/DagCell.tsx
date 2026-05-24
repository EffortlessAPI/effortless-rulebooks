import { useState } from 'react'
import './DagCell.css'

interface DagCellProps {
  label: string
  value: string | number
  formula?: string
  dependencies?: string[]
}

export function DagCell({ label, value, formula, dependencies }: DagCellProps) {
  const [showFormula, setShowFormula] = useState(false)

  return (
    <div className="dag-cell" onClick={() => setShowFormula(!showFormula)}>
      <div className="dag-cell-content">
        <div className="dag-cell-label">{label}</div>
        <div className="dag-cell-value">{value}</div>
      </div>
      {(formula || dependencies) && (
        <div className="dag-cell-indicator">
          {showFormula && (
            <div className="dag-cell-popup">
              {formula && (
                <div className="dag-cell-formula">
                  <strong>Formula:</strong> {formula}
                </div>
              )}
              {dependencies && dependencies.length > 0 && (
                <div className="dag-cell-deps">
                  <strong>Depends on:</strong>
                  <ul>
                    {dependencies.map((dep, i) => (
                      <li key={i}>{dep}</li>
                    ))}
                  </ul>
                </div>
              )}
            </div>
          )}
          <span className="dag-indicator-dot">📊</span>
        </div>
      )}
    </div>
  )
}

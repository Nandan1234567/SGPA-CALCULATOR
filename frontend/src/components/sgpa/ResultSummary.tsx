import type { SgpaResponse } from '../../api/types'
import CreditGraph from './CreditGraph'

interface Props {
  result: SgpaResponse
  fileName: string | null
}

function sgpaHex(v: number) {
  // Pass = Deep Pine Green (Brand-800), Fail = Crimson Red
  if (v >= 5.0) return '#1a3a2a'
  return '#b91c1c'
}

function sgpaLabel(v: number) {
  if (v >= 9.0) return 'Outstanding'
  if (v >= 8.0) return 'Excellent'
  if (v >= 7.0) return 'Very Good'
  if (v >= 6.0) return 'Good'
  if (v >= 5.0) return 'Average'
  if (v >= 4.0) return 'Pass'
  return 'Below Threshold'
}

export default function ResultSummary({ result, fileName }: Props) {
  const pct = result.sgpa > 0.75 ? ((result.sgpa - 0.75) * 10).toFixed(2) : '0.00'
  const included = result.subjects.filter((s) => s.isIncludedInSgpa).length
  const color = sgpaHex(result.sgpa)

  return (
    <div className="card mb-5 bg-white border border-black/5 shadow-sm">
      {/* Student info row */}
      <div className="flex flex-wrap gap-x-6 gap-y-2 pb-4 mb-5 border-b border-[rgba(0,0,0,0.06)]">
        <div>
          <p className="text-caption text-ink-muted">Student</p>
          <p className="text-body-sm font-semibold text-ink-primary mt-0.5">
            {result.studentName || '—'}
          </p>
        </div>
        <div>
          <p className="text-caption text-ink-muted">USN</p>
          <p className="text-body-sm font-mono text-ink-primary mt-0.5">{result.usn || '—'}</p>
        </div>
        <div>
          <p className="text-caption text-ink-muted">Semester</p>
          <p className="text-body-sm text-ink-primary mt-0.5">Semester {result.semester}</p>
        </div>
        {fileName && (
          <div className="ml-auto self-start hidden sm:block">
            <p className="text-caption text-ink-muted">Source</p>
            <p className="text-[11px] font-mono text-ink-subtle mt-0.5 max-w-[160px] truncate">
              {fileName}
            </p>
          </div>
        )}
      </div>

      {/* SGPA + stats */}
      <div className="flex flex-wrap items-end gap-6 sm:gap-10 mb-6">
        <div>
          <p className="text-caption text-ink-muted mb-1">SGPA</p>
          <p className="font-bold leading-none tracking-tight" style={{ fontSize: '3.25rem', color }}>
            {result.sgpa.toFixed(2)}
          </p>
          <p className="text-caption mt-1 font-medium" style={{ color }}>{sgpaLabel(result.sgpa)}</p>
        </div>
        <div className="pb-1">
          <p className="text-caption text-ink-muted mb-1">Percentage</p>
          <p className="text-h2 font-bold text-ink-primary">{pct}%</p>
          {/* <p className="text-[10px] text-ink-muted mt-0.5">(SGPA − 0.75) × 10</p> */}
        </div>
        <div className="pb-1">
          <p className="text-caption text-ink-muted mb-1">Credits</p>
          <p className="text-h2 font-bold text-ink-primary">{result.totalCredits}</p>
          {/* <p className="text-[10px] text-ink-muted mt-0.5">subjects</p> */}
        </div>
      </div>

      {/* Chart */}
      {result.subjects.length > 0 && (
        <div>
          {/* <p className="text-caption text-ink-muted mb-3">Grade Points per Subject</p> */}
          <CreditGraph subjects={result.subjects} />
          <div className="flex flex-wrap gap-4 mt-3">
            {[
              { bg: '#1a3a2a', label: 'Pass' },
              { bg: '#b91c1c', label: 'Fail' },
              { bg: '#c5d4cf', label: 'Excluded' },
            ].map((item) => (
              <div key={item.label} className="flex items-center gap-1.5">
                <span className="w-2.5 h-2.5 rounded-sm shrink-0" style={{ background: item.bg }} />
                <span className="text-[10px] text-ink-muted">{item.label}</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}



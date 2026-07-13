import { CheckCircle2 } from 'lucide-react'
import type { CgpaResult } from './CgpaTypes'
import { cgpaColor, DIVISION_ROWS } from './CgpaTypes'
import type { DivisionInfo } from '../../constants/vtuCredits'
import SemesterGraph from '../sgpa/SemesterGraph'
import { cn } from '../../lib/cn'

interface Props {
  result: CgpaResult
  division: DivisionInfo | null
  percentage: string
}

export default function CgpaResultCard({ result, division, percentage }: Props) {
  return (
    <div id="cgpa-result" className="card p-0 overflow-hidden bg-white border border-black/5 shadow-sm">

      <div className="px-4 sm:px-5 py-4 border-b border-[rgba(0,0,0,0.06)]">
        <h2 className="text-body-sm font-semibold text-ink-primary">CGPA Result</h2>
      </div>

      <div className="px-4 sm:px-5 py-5">

        {result.studentName && (
          <p className="text-h4 font-semibold text-ink-primary mb-5">{result.studentName}</p>
        )}

        <div className="flex flex-wrap items-end gap-6 sm:gap-10 mb-5">
          <div>
            <p className="text-caption text-ink-muted mb-1">CGPA</p>
            <p
              className="font-bold leading-none tracking-tight"
              style={{ fontSize: '3.25rem', color: cgpaColor(result.cgpa) }}
            >
              {result.cgpa.toFixed(2)}
            </p>
            <p className="text-caption font-medium mt-1" style={{ color: cgpaColor(result.cgpa) }}>
              {result.semesterCount} semester{result.semesterCount > 1 ? 's' : ''}
            </p>
          </div>

          <div className="pb-1">
            <p className="text-caption text-ink-muted mb-1">Percentage</p>
            <p className="text-h2 font-bold text-ink-primary">{percentage}%</p>
            <p className="text-[10px] text-ink-muted mt-0.5">(CGPA − 0.75) × 10</p>
          </div>

          <div className="pb-1">
            <p className="text-caption text-ink-muted mb-1">Total Credits</p>
            <p className="text-h2 font-bold text-ink-primary">{result.totalCredits}</p>
            <p className="text-[10px] text-ink-muted mt-0.5">{result.semesterCount} semesters</p>
          </div>

          {division && (
            <div className="pb-1">
              <p className="text-caption text-ink-muted mb-2">Division</p>
              <span
                className="inline-flex items-center px-3 py-1.5 rounded-full text-caption font-bold text-white"
                style={{ background: division.color }}
              >
                {division.short}
              </span>
              <p className="text-[10px] text-ink-muted mt-1">{division.full}</p>
            </div>
          )}
        </div>

      

        <div>
          <p className="text-caption text-ink-muted py-2 mb-1">SGPA per Semester</p>
          <SemesterGraph data={result.chartData} />
          <div className="flex flex-wrap gap-4 mt-3">
            {[
              { color: 'rgba(26,58,42,1)', label: '≥ 6.0  ·  FC or FCD' },
              { color: 'rgba(26,58,42,0.35)', label: '5.0 – 5.99  ·  SC' },
              { color: '#b91c1c', label: '< 5.0  ·  Below threshold' },
            ].map((item) => (
              <div key={item.label} className="flex items-center gap-1.5">
                <span className="w-2.5 h-2.5 rounded-sm shrink-0" style={{ background: item.color }} />
                <span className="text-[10px] text-ink-muted">{item.label}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="border-t border-[rgba(0,0,0,0.04)] px-4 sm:px-5 py-4">
        <p className="text-[10px] font-semibold text-ink-muted uppercase tracking-wide mb-3">
          VTU 22 Scheme — Division Classification
        </p>
        <div className="overflow-x-auto">
          <table className="w-full" style={{ minWidth: 340 }}>
            <thead>
              <tr className="bg-cream-100/50">
                {[ 'CGPA Range', 'Division', 'Your result' ].map((h) => (
                  <th key={h} className="px-4 py-2 text-[10px] font-semibold text-ink-muted text-left">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-[rgba(0,0,0,0.04)]">
              {DIVISION_ROWS.map((row) => {
                const isMatch = division?.full === row.full
                return (
                  <tr key={row.short} className={cn('transition-colors', isMatch && 'bg-brand-50/50')}>
                    <td className="px-4 py-2.5 text-caption font-mono text-ink-secondary">{row.range}</td>
                    <td className="px-4 py-2.5 text-caption text-ink-secondary">
                      {row.full} <span className="text-ink-muted">({row.short})</span>
                    </td>
                    <td className="px-4 py-2.5 text-caption">
                      {isMatch && (
                        <span className="flex items-center gap-1 font-semibold text-brand-800">
                          <CheckCircle2 size={11} />
                          Your result
                        </span>
                      )}
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}

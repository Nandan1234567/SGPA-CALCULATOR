import { CheckCircle2 } from 'lucide-react'
import type { RowState } from './CgpaTypes'
import { cn } from '../../lib/cn'

interface Props {
  rows: Record<number, RowState>
  onRowChange: (sem: number, field: 'sgpa' | 'credits', value: string) => void
  selectedBranch: string
}

const GRID = { gridTemplateColumns: '60px 1fr 90px 20px' }

function dotColor(sgpa: number): string {
  if (sgpa >= 7) return '#1a3a2a'
  if (sgpa >= 5) return '#d97706'
  return '#b91c1c'
}

export default function SemesterTable({ rows, onRowChange, selectedBranch }: Props) {
  return (
    <>
      <div
        className="grid gap-2 px-4 sm:px-5 py-2.5 bg-cream-100/50 border-b border-[rgba(0,0,0,0.04)]"
        style={GRID}
      >
        <p className="text-[10px] font-semibold text-ink-muted uppercase tracking-wide">Semester</p>
        <p className="text-[10px] font-semibold text-ink-muted uppercase tracking-wide">SGPA (0–10)</p>
        <p className="text-[10px] font-semibold text-ink-muted uppercase tracking-wide">Credits</p>
        <div />
      </div>

      <div className="divide-y divide-[rgba(0,0,0,0.04)]">
        {[ 1, 2, 3, 4, 5, 6, 7, 8 ].map((sem) => {
          const row = rows[ sem ]
          const sgpaNum = parseFloat(row.sgpa)
          const hasValue = row.sgpa.trim() !== '' && !isNaN(sgpaNum) && sgpaNum > 0
          const isHistory = row.source === 'history'

          return (
            <div
              key={sem}
              className={cn(
                'grid gap-2 items-center px-4 sm:px-5 py-3 transition-colors',
                isHistory && 'bg-brand-50/40',
              )}
              style={GRID}
            >
              <div>
                <p className="text-body-sm font-semibold text-ink-primary">Sem {sem}</p>
                {isHistory && (
                  <p className="text-[9px] text-brand-800 font-semibold mt-0.5 flex items-center gap-0.5">
                    <CheckCircle2 size={8} />
                    saved
                  </p>
                )}
              </div>

              <input
                type="number"
                min="0"
                max="10"
                step="0.01"
                value={row.sgpa}
                onChange={(e) => onRowChange(sem, 'sgpa', e.target.value)}
                placeholder="e.g. 7.50"
                aria-label={`Semester ${sem} SGPA`}
                className={cn(
                  'input text-body-sm tabular-nums text-center transition-all',
                  isHistory && '!border-brand-200 !bg-brand-50/30',
                  hasValue && !isHistory && '!border-brand-800',
                )}
              />

              <input
                type="number"
                min="1"
                max="50"
                step="1"
                value={row.credits}
                onChange={(e) => onRowChange(sem, 'credits', e.target.value)}
                placeholder={selectedBranch ? '—' : 'credits'}
                aria-label={`Semester ${sem} credits`}
                className={cn(
                  'input text-body-sm tabular-nums text-center transition-all',
                  isHistory && '!border-brand-200 !bg-brand-50/30',
                )}
              />

              <div className="flex justify-center">
                <div
                  className="w-2 h-2 rounded-full transition-all duration-200"
                  style={{ background: hasValue ? dotColor(sgpaNum) : '#e0dcd3' }}
                />
              </div>
            </div>
          )
        })}
      </div>
    </>
  )
}

import { ChevronDown, ChevronUp, History, CheckCircle2 } from 'lucide-react'
import type { SemesterRecord } from '../../hooks/useLocalHistory'
import { cn } from '../../lib/cn'

interface Props {
  historyByUsn: Map<string, SemesterRecord[]>
  totalHistoryCount: number
  selectedKeys: Set<string>
  historyOpen: boolean
  setHistoryOpen: (v: boolean) => void
  onToggle: (record: SemesterRecord) => void
}

export default function HistoryPanel({
  historyByUsn, totalHistoryCount, selectedKeys,
  historyOpen, setHistoryOpen, onToggle,
}: Props) {
  if (totalHistoryCount === 0) return null

  return (
    <div className="border-b border-[rgba(0,0,0,0.04)]">
      <button
        onClick={() => setHistoryOpen(!historyOpen)}
        className="w-full flex items-center justify-between px-4 sm:px-5 py-3 hover:bg-cream-50/50 transition-colors text-left"
      >
        <div className="flex items-center gap-2 flex-wrap">
          <History size={14} className="text-brand-800 shrink-0" />
          <span className="text-body-sm font-semibold text-ink-primary">
            Load from your uploaded results
          </span>
          <span className="badge text-[10px] px-1.5 py-0.5">{totalHistoryCount} saved</span>
          {selectedKeys.size > 0 && (
            <span className="text-caption text-brand-800 font-medium">
              · {selectedKeys.size} selected
            </span>
          )}
        </div>
        {historyOpen
          ? <ChevronUp size={14} className="text-ink-muted shrink-0" />
          : <ChevronDown size={14} className="text-ink-muted shrink-0" />
        }
      </button>

      {historyOpen && (
        <div className="px-4 sm:px-5 pb-4">
          <p className="text-caption text-ink-muted mb-3">
            Tap a chip to load its SGPA and credits. Tap again to remove. You can pick from multiple USNs.
          </p>

          <div className="space-y-4">
            {[ ...historyByUsn.entries() ].map(([ usn, records ]) => (
              <div key={usn}>
                <p className="text-[10px] font-mono font-semibold text-ink-muted uppercase tracking-wide mb-2">
                  {usn}
                </p>
                <div className="flex flex-wrap gap-2">
                  {records.map((rec) => {
                    const key = `${rec.usn}:${rec.semester}`
                    const isSelected = selectedKeys.has(key)
                    return (
                      <button
                        key={key}
                        onClick={() => onToggle(rec)}
                        className={cn(
                          'flex items-center gap-1.5 px-3 py-1.5 rounded-full border text-caption font-medium transition-all active:scale-95',
                          isSelected
                            ? 'bg-brand-800 border-brand-800 text-white'
                            : 'bg-cream-50 border-[rgba(0,0,0,0.12)] text-ink-secondary hover:border-brand-800 hover:text-brand-800',
                        )}
                      >
                        {isSelected && <CheckCircle2 size={10} />}
                        <span>Sem {rec.semester}</span>
                        <span className="tabular-nums">{rec.sgpa.toFixed(2)}</span>
                      </button>
                    )
                  })}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}





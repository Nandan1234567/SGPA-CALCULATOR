import React, { useCallback, useEffect, useState } from 'react'
import { AlertTriangle, ArrowLeft, ArrowRight, RotateCcw } from 'lucide-react'
import { Link, useNavigate } from 'react-router'
import ResultSummary from '../components/sgpa/ResultSummary'
import SubjectRow from '../components/sgpa/SubjectRow'
import { useCalculate } from '../hooks/useCalculate'
import { saveToHistory } from '../hooks/useLocalHistory'
import { cn } from '../lib/cn'
import { useSgpaStore } from '../store/useSgpaStore'

export default function ResultsPage() {
  const navigate = useNavigate()
  const sgpaResult = useSgpaStore((s) => s.sgpaResult)
  const uploadedFileName = useSgpaStore((s) => s.uploadedFileName)
  const clearResult = useSgpaStore((s) => s.clearResult)

  const { recalculate, isPending: isRecalculating, isSuccess: recalcDone, reset: resetCalc } = useCalculate()

  const [ creditInputs, setCreditInputs ] = useState<Record<string, string>>({})

  useEffect(() => {
    if (sgpaResult) saveToHistory(sgpaResult)
  }, [ sgpaResult ])

  useEffect(() => {
    if (!sgpaResult) navigate('/', { replace: true })
  }, [ sgpaResult, navigate ])

  useEffect(() => {
    if (recalcDone) {
      setCreditInputs({})
      resetCalc()
    }
  }, [ recalcDone, resetCalc ])

  const handleCreditChange = useCallback((code: string, val: string) => {
    setCreditInputs((prev) => ({ ...prev, [ code ]: val }))
  }, [])

  function handleRecalculate() {
    const overrides: Record<string, number> = {}

    for (const [ code, val ] of Object.entries(creditInputs) as [ string, string ][]) {
      if (!val.trim()) continue
      const n = parseInt(val, 10)
      if (!isNaN(n) && n >= 0 && n <= 10) {
        overrides[ code ] = n
      }
    }

    if (!Object.keys(overrides).length) return
    recalculate(overrides)
  }

  if (!sgpaResult) return null

  // ✅ FIX: Use the actual unresolvedCodes array sent by the backend!
  const unresolvedArray = sgpaResult.unresolvedCodes || []
  const unresolvedCount = unresolvedArray.length
  const filledCount = (Object.values(creditInputs) as string[]).filter((v) => v.trim() !== '').length

  return (
    <div className="w-full flex flex-col items-center px-4 sm:px-6 py-6 pb-16">
      <div className="w-full max-w-5xl">

      {/* Navigation strip */}
      <div className="flex items-center justify-between mb-5">
        <button
          onClick={() => { clearResult(); navigate('/', { replace: true }) }}
          className="flex items-center gap-1.5 text-body-sm text-ink-muted hover:text-ink-primary transition-colors"
        >
          <ArrowLeft size={15} />
          Upload another
        </button>
        <div className="flex items-center gap-4">
          <Link to="/cgpa" className="flex items-center gap-1.5 text-body-sm text-ink-muted hover:text-ink-primary transition-colors">
            Calculate CGPA
            <ArrowRight size={15} />
          </Link>
        </div>
      </div>

      {/* Summary card + chart */}
      <ResultSummary result={sgpaResult} fileName={uploadedFileName} />

      {/* Unresolved warning (Only shows if subjects are missing) */}
      {unresolvedCount > 0 && (
        <div className="flex items-start gap-3 p-4 bg-amber-50 border border-amber-200 rounded-xl mb-5 shadow-sm">
          <AlertTriangle size={17} className="text-amber-600 shrink-0 mt-0.5" />
          <div>
            <p className="text-body-sm font-semibold text-amber-800">
              {unresolvedCount} subject{unresolvedCount > 1 ? 's are' : ' is'} missing credits
            </p>
            <p className="text-caption text-amber-700 mt-0.5">
              Select the missing credit value in the table below, then tap Recalculate.
            </p>
          </div>
        </div>
      )}

      {/* Subject table (Pure White Card with Mobile Scrolling) */}
      <div className="card p-0 overflow-hidden mb-5 bg-white border border-black/5 shadow-sm relative flex flex-col">
        <div className="flex items-center justify-between px-4 sm:px-5 py-4 border-b border-[rgba(0,0,0,0.06)]">
          <h2 className="text-body-sm font-semibold text-ink-primary">Subject Breakdown</h2>
          <span className="text-caption text-ink-muted">{sgpaResult.subjects.length} subjects</span>
        </div>

        <div className="overflow-x-auto w-full">
          <table className="w-full min-w-[600px]">
            <thead>
              <tr className="bg-cream-100/50">
                {[ 'Code', 'Subject', 'INT', 'EXT', 'Total', 'Credits', 'GP', 'Grade', 'Status' ].map((h, i) => (
                  <th
                    key={h}
                    className={cn(
                      'px-4 py-3 text-[11px] font-semibold text-ink-muted text-left whitespace-nowrap',
                      (i >= 2 && i <= 6) && 'text-center',
                      (i >= 7) && 'text-center'
                    )}
                  >
                    {h}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-black/5">
              {sgpaResult.subjects.map((sub) => (
                <SubjectRow
                  key={sub.subjectCode}
                  subject={sub}
                  isUnresolvedCode={unresolvedArray.includes(sub.subjectCode)}
                  creditValue={creditInputs[ sub.subjectCode ] ?? ''}
                  onCreditChange={handleCreditChange}
                  isRecalculating={isRecalculating}
                />
              ))}
            </tbody>
          </table>
        </div>

        {/* Floating Pure White Sticky Recalculate Bar */}
        {unresolvedCount > 0 && (
          <div className="sticky bottom-0 z-20 bg-white border-t border-black/10 shadow-[0_-15px_20px_-5px_rgba(0,0,0,0.05)] flex flex-col sm:flex-row items-center gap-4 justify-between p-4 sm:px-6 transition-all mt-auto">
            <p className="text-body-sm font-medium text-ink-secondary text-center sm:text-left">
              {filledCount > 0
                ? `${filledCount} credit value${filledCount > 1 ? 's' : ''} ready to apply`
                : 'Select credits in the table above to recalculate'}
            </p>
            <button
              className="btn-primary shrink-0 shadow-md"
              onClick={handleRecalculate}
              disabled={filledCount === 0 || isRecalculating}
            >
              {isRecalculating ? (
                <span className="flex items-center gap-2">
                  <RotateCcw size={13} className="animate-spin" />
                  Recalculating…
                </span>
              ) : (
                `Recalculate SGPA${filledCount > 0 ? ` (${filledCount})` : ''}`
              )}
            </button>
          </div>
        )}
      </div>

        {/* Section 4: Important Notes (Unified Card Style) */}
        <section className="mt-8 mb-5 w-full">
          <div className="card p-0 overflow-hidden bg-white border border-black/5 shadow-sm">

            {/* Heading INSIDE the white card with the divider line */}
            <div className="px-4 sm:px-5 py-4 border-b border-[rgba(0,0,0,0.06)]">
              <h2 className="text-body-sm font-semibold text-ink-primary">Important Notes</h2>
            </div>

            <ul className="divide-y divide-black/5">
              {[
                <><strong className="text-ink-secondary font-semibold">Non-credit subjects (NSS, PE, Yoga):</strong> Displayed on your result but excluded from SGPA calculation by VTU rules.</>,
                <><strong className="text-ink-secondary font-semibold">Unresolved subjects:</strong> If a subject's credits are missing, select the correct value and tap recalculate to get an accurate SGPA.</>,
                <><strong className="text-ink-secondary font-semibold">Absent (A) / Withheld (W):</strong> Excluded from calculation as official marks are unavailable.</>,
                <>For a full breakdown of the VTU grading system, visit our <Link to="/guide" className="text-brand-800 font-medium hover:underline">Guide Page ↗</Link>.</>
              ].map((note, i) => (
                <li key={i} className="px-4 sm:px-5 py-4 flex items-start gap-3.5 hover:bg-cream-50/50 transition-colors">
                  <span className="h-5 flex items-center justify-center shrink-0">
                    <span className="w-1.5 h-1.5 rounded-full bg-brand-800" />
                  </span>
                  <span className="text-caption font-medium text-ink-secondary leading-relaxed">
                    {note}
                  </span>
                </li>
              ))}
            </ul>
          </div>
        </section>

      </div>

    </div>
  )
}

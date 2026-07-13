import { useEffect, useState } from 'react'
import { AlertTriangle } from 'lucide-react'
import { cn } from '../lib/cn'

// Static VTU 22-scheme grading thresholds
const GRADE_TABLE = [
  { range: '90–100', grade: 'O', points: 10, label: 'Outstanding' },
  { range: '80–89', grade: 'A+', points: 9, label: 'Excellent' },
  { range: '70–79', grade: 'A', points: 8, label: 'Very Good' },
  { range: '60–69', grade: 'B+', points: 7, label: 'Good' },
  { range: '55–59', grade: 'B', points: 6, label: 'Above Average' },
  { range: '50–54', grade: 'C', points: 5, label: 'Average' },
  { range: '40–49', grade: 'P', points: 4, label: 'Pass' },
  { range: '< 40', grade: 'F', points: 0, label: 'Fail' },
]

// Unified badge styling perfectly matching the ResultsPage
const getBadgeStyle = (grade: string) => {
  if (grade === 'F') return 'bg-red-50 text-red-700'
  return 'bg-cream-200 text-brand-800'
}

const IMPORTANT_NOTES = [
  'As of now, SGPA Calculator supports VTU 2022 (22) scheme only — all branches.',
  'Upload the official result PDF from results.vtu.ac.in. Screenshots and scans do not work.',
  'Your privacy is protected — PDFs are never saved or stored on our servers.',
  'Subjects marked W (Withheld), A (Absent), or X (Not Eligible) are shown but excluded from SGPA.',
  'Zero Credit subjects and failed subjects are excluded from SGPA calculation by scheme rules.',
  'If credits could not be determined, use the lookup tool below or enter the value manually in the calculator.'
]

// Mirrors the response shape of ASP.NET SgpaController.ResolveCode()
interface ResolveResult {
  subjectCode: string
  credits: number
  isNonCreditForSgpa: boolean
  isResolved: boolean
  resolutionMethod: string
}

export default function GuidePage() {
  const [ codeInput, setCodeInput ] = useState('')
  const [ resolveResult, setResolveResult ] = useState<ResolveResult | null>(null)
  const [ isLooking, setIsLooking ] = useState(false)
  const [ lookupError, setLookupError ] = useState<string | null>(null)

  // IMP: Auto-dismiss BOTH the result card and error messages after 5 seconds
  useEffect(() => {
    if (resolveResult || lookupError) {
      const timer = setTimeout(() => {
        setResolveResult(null)
        setLookupError(null)
      }, 5000)
      return () => clearTimeout(timer)
    }
  }, [ resolveResult, lookupError ])

  async function handleLookup() {
    const code = codeInput.trim().toUpperCase()
    if (!code) return

    setIsLooking(true)
    setLookupError(null)
    setResolveResult(null)

    try {
      const baseUrl = import.meta.env.VITE_API_BASE_URL as string
      const url = `${baseUrl}/api/sgpa/resolve?code=${encodeURIComponent(code)}`

      const res = await fetch(url)
      if (!res.ok) {
        throw new Error(`Server returned ${res.status}`)
      }

      const data: ResolveResult = await res.json()
      setResolveResult(data)
      setCodeInput('')
    } catch (err) {
      setLookupError(
        err instanceof Error
          ? err.message
          : 'Lookup failed. Is the ASP.NET server running?'
      )
    } finally {
      setIsLooking(false)
    }
  }

  return (
    <div className="w-full flex flex-col items-center px-4 sm:px-6 py-6 pb-16">
      <div className="w-full max-w-4xl">

        {/* Page Header (Stays outside the cards) */}
        <div className="mb-10">
          <h1 className="text-h2 sm:text-h1 font-bold text-ink-primary">Grading Guide</h1>
          <p className="text-body-sm sm:text-body text-ink-secondary mt-2 max-w-2xl">
            How SGPA is calculated under the VTU 2022 grading scheme.
          </p>
        </div>

        {/* Section 1: Grade Scale Table */}
        <section className="mb-8 w-full">
          <div className="card p-0 overflow-hidden bg-white border border-black/5 shadow-sm">
            <div className="px-4 sm:px-5 py-4 border-b border-[rgba(0,0,0,0.06)]">
              <h2 className="text-body-sm font-semibold text-ink-primary">Grade Scale</h2>
            </div>
            <div className="overflow-x-auto w-full">
              <table className="w-full text-left min-w-[500px]">
                <thead>
                  <tr className="bg-cream-100/50">
                    <th className="px-5 py-3 text-[11px] font-semibold text-ink-muted">Marks</th>
                    <th className="px-5 py-3 text-[11px] font-semibold text-ink-muted">Grade</th>
                    <th className="px-5 py-3 text-[11px] font-semibold text-ink-muted">Points</th>
                    <th className="px-5 py-3 text-[11px] font-semibold text-ink-muted">Description</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-black/5">
                  {GRADE_TABLE.map((row) => (
                    <tr key={row.grade} className="hover:bg-cream-50/50 transition-colors">
                      <td className="px-5 py-3 text-caption font-mono font-medium text-ink-secondary tabular-nums">
                        {row.range}
                      </td>
                      <td className="px-5 py-3">
                        <span className={cn(
                          "inline-flex items-center justify-center min-w-[28px] px-1.5 py-0.5 rounded text-[11px] font-bold",
                          getBadgeStyle(row.grade)
                        )}>
                          {row.grade}
                        </span>
                      </td>
                      <td className="px-5 py-3 text-caption font-mono font-bold text-ink-primary tabular-nums">
                        {row.points}
                      </td>
                      <td className="px-5 py-3 text-caption font-medium text-ink-secondary">
                        {row.label}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </section>

        {/* Section 2: Important Notes */}
        <section className="mb-8 w-full">
          <div className="card p-0 overflow-hidden bg-white border border-black/5 shadow-sm">
            <div className="px-4 sm:px-5 py-4 border-b border-[rgba(0,0,0,0.06)]">
              <h2 className="text-body-sm font-semibold text-ink-primary">Important Notes</h2>
            </div>
            <ul className="divide-y divide-black/5">
              {IMPORTANT_NOTES.map((note, i) => (
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

        {/* Section 3: Check Subject Credits Tool */}
        <section className="mb-10 w-full">
          <div className="card p-0 overflow-hidden bg-white border border-black/5 shadow-sm">
            <div className="px-4 sm:px-5 py-4 border-b border-[rgba(0,0,0,0.06)]">
              <h2 className="text-body-sm font-semibold text-ink-primary">Check Subject Credits</h2>
            </div>

            <div className="p-4 sm:p-5">
              <p className="text-caption text-ink-muted mb-5 max-w-xl leading-relaxed">
                Enter a VTU subject code to see its credit value. Use this tool if your result shows an unrecognised subject or you need to enter credits manually.
              </p>

              <div className="flex gap-3 max-w-sm">
                <input
                  className="w-full text-body-sm border border-[rgba(0,0,0,0.15)] bg-white rounded-lg px-4 py-2.5 outline-none transition-all focus:border-brand-800 focus:shadow-[0_0_0_2px_rgba(26,58,42,0.12)]"
                  type="text"
                  value={codeInput}
                  onChange={(e) => setCodeInput(e.target.value.toUpperCase())}
                  onKeyDown={(e) => { if (e.key === 'Enter') handleLookup() }}
                  placeholder="e.g. BCS301, BCSL305"
                  maxLength={12}
                />
                <button
                  className="btn-primary shrink-0 px-6"
                  onClick={handleLookup}
                  disabled={isLooking || !codeInput.trim()}
                >
                  {isLooking ? 'Looking…' : 'Look up'}
                </button>
              </div>

              {/* Error display */}
              {lookupError && (
                <div className="flex items-start gap-3 p-4 bg-amber-50 border border-amber-200 rounded-xl mt-5 max-w-sm shadow-sm">
                  <AlertTriangle size={17} className="text-amber-600 shrink-0 mt-0.5" />
                  <p className="text-body-sm font-medium text-amber-800 leading-snug">
                    {lookupError}
                  </p>
                </div>
              )}

              {/* Lookup Result Panel */}
              {resolveResult && (
                <div className="mt-5 bg-cream-50 border border-black/5 max-w-sm p-5 rounded-xl">
                  <div className="flex items-start justify-between gap-4">
                    <div className="min-w-0">
                      <div className="text-body font-bold text-ink-primary font-mono truncate">
                        {resolveResult.subjectCode}
                      </div>
                      <div className="text-[11px] font-medium text-ink-muted mt-1 leading-relaxed uppercase tracking-wide">
                        {resolveResult.resolutionMethod}
                      </div>
                    </div>

                    <div className="text-right shrink-0">
                      <div className="text-h2 font-bold text-ink-primary leading-none">
                        {resolveResult.credits}
                      </div>
                      <div className="text-[10px] font-semibold text-ink-muted uppercase tracking-wider mt-1.5">
                        Credits
                      </div>
                    </div>
                  </div>

                  {resolveResult.isNonCreditForSgpa && (
                    <div className="mt-4">
                      <span className="inline-flex items-center px-2 py-1 rounded text-[10px] font-bold bg-cream-200 text-ink-muted">
                        NON-CREDIT (EXCLUDED)
                      </span>
                    </div>
                  )}

                  {!resolveResult.isResolved && (
                    <p className="mt-4 text-caption font-medium text-amber-700 bg-amber-50 p-3 rounded-lg border border-amber-200/50">
                      Code not recognised. Verify against the VTU scheme document or enter credits manually in the calculator.
                    </p>
                  )}
                </div>
              )}
            </div>
          </div>
        </section>

      </div>
    </div>
  )
}

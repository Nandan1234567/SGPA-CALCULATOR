import { cn } from '../../lib/cn'
import type { SubjectResultDto } from '../../api/types'

interface Props {
  key?: string
  subject: SubjectResultDto
  creditValue: string
  onCreditChange: (code: string, value: string) => void
  isRecalculating: boolean
  isUnresolvedCode: boolean // Explicitly passed from the parent to guarantee detection
}

export default function SubjectRow({ subject, creditValue, onCreditChange, isRecalculating, isUnresolvedCode }: Props) {
  const {
    subjectCode,
    subjectName,
    internalMarks,
    externalMarks,
    totalMarks,
    credits,
    gradePoints,
    grade,
    isPass,
    isNonCreditForSgpa,
  } = subject

  // ✅ FIX: VTU uses 'A' for Excellent AND 'A' for Absent. We must check !isPass to confirm absence.
  const isAbsent = grade === 'A' && !isPass
  const isWithheldOrExcluded = grade === 'W' || grade === 'X' || grade === 'NE'

  // Safety fallback: Trust the parent's unresolved array
  const isUnresolved = subject.isUnresolved || isUnresolvedCode

  let statusText = isPass ? 'Pass' : 'Fail'
  let statusColor = isPass ? 'text-brand-800' : 'text-red-700'

  if (isNonCreditForSgpa) {
    statusText = 'Non-Credit'
    statusColor = 'text-ink-muted'
  } else if (isAbsent) {
    statusText = 'Absent'
    statusColor = 'text-amber-600'
  } else if (isWithheldOrExcluded) {
    statusText = grade === 'W' ? 'Withheld' : 'Excluded'
    statusColor = 'text-amber-600'
  } else if (isUnresolved) {
    statusText = 'Needs Credits'
    statusColor = 'text-amber-600'
  }

  // The unified grade text (Converts 'A' fail to 'AB' for clarity, otherwise uses grade)
  const displayGrade = isAbsent ? 'AB' : (grade || '—')

  // The strict allowed credit values
  const allowedCredits = [ "", "0", "1", "2", "3", "4", "6", "8", "10" ]

  return (
    <tr className="hover:bg-cream-50/50 transition-colors border-b border-black/5 last:border-0">
      <td className="px-4 py-3 text-[11px] font-bold text-ink-primary whitespace-nowrap">
        {subjectCode}
      </td>
      <td className="px-4 py-3 text-[11px] font-medium text-ink-secondary uppercase min-w-[200px]">
        {subjectName}
      </td>

      {/* ✅ FIX: Removed mobile hiding. Replaced zeros with '—' for absent/withheld students */}
      <td className="px-4 py-3 text-caption text-ink-primary text-center tabular-nums">
        {isAbsent || isWithheldOrExcluded ? '—' : internalMarks}
      </td>
      <td className="px-4 py-3 text-caption text-ink-primary text-center tabular-nums">
        {isAbsent || isWithheldOrExcluded ? '—' : externalMarks}
      </td>
      <td className="px-4 py-3 text-caption text-ink-primary text-center tabular-nums">
        {isAbsent || isWithheldOrExcluded ? '—' : totalMarks}
      </td>

      <td className="px-4 py-3 text-center">
        {isUnresolved ? (
          <select
            value={creditValue}
            onChange={(e) => onCreditChange(subjectCode, e.target.value)}
            disabled={isRecalculating}
            className="w-[50px] mx-auto text-[11px] font-bold border border-amber-300 bg-amber-50 text-amber-900 rounded px-1 py-1 focus:outline-none focus:ring-2 focus:ring-amber-500 disabled:opacity-50 cursor-pointer"
          >
            <option value="" disabled>—</option>
            {allowedCredits.filter(c => c !== "").map(c => (
              <option key={c} value={c}>{c}</option>
            ))}
          </select>
        ) : (
          <span className="text-caption font-medium text-ink-primary">
            {isNonCreditForSgpa ? '—' : credits}
          </span>
        )}
      </td>

      <td className="px-4 py-3 text-caption font-medium text-ink-primary text-center tabular-nums">
        { isUnresolved ? '—' : gradePoints.toFixed(1)}
      </td>

      <td className="px-4 py-3 text-center">
        <span className={cn(
          "inline-flex items-center justify-center min-w-[28px] px-1.5 py-0.5 rounded text-[11px] font-bold",
          isNonCreditForSgpa
            ? "bg-cream-200 text-ink-muted"
            : (isPass ? "bg-cream-200 text-brand-800" : "bg-red-50 text-red-700")
        )}>
          {displayGrade}
        </span>
      </td>

      <td className="px-4 py-3 text-center">
        <span className={cn("text-[11px] font-bold whitespace-nowrap", statusColor)}>
          {statusText}
        </span>
      </td>
    </tr>
  )
}


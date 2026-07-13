import { useMutation } from '@tanstack/react-query'
import { calculateSgpa } from '../api/sgpa'
import { useSgpaStore } from '../store/useSgpaStore'
import type { SgpaRequest, SgpaResponse, SubjectResultDto, ApiError } from '../api/types'

function getResultField(sub: SubjectResultDto): string | undefined {
  if ([ 'W', 'X', 'NE' ].includes(sub.grade)) return sub.grade
  if (sub.grade === 'A' && !sub.isPass) return 'A'
  if (sub.isUnresolved) return undefined
  if (sub.isPass) return 'P'
  return undefined
}

function buildRequest(
  result: SgpaResponse,
  overrides: Record<string, number>,
): SgpaRequest {
  return {
    studentName: result.studentName,
    usn: result.usn,
    semester: result.semester,
    subjects: result.subjects.map((sub) => ({
      subjectCode: sub.subjectCode,
      subjectName: sub.subjectName,
      internalMarks: sub.internalMarks,
      externalMarks: sub.externalMarks,
      totalMarks: sub.totalMarks,
      result: getResultField(sub),
      manualCreditOverride:
        overrides[ sub.subjectCode ] !== undefined
          ? overrides[ sub.subjectCode ]
          : sub.isNonCreditForSgpa
            ? 0
            : undefined,
    })),
  }
}

export function useCalculate() {
  const sgpaResult = useSgpaStore((s) => s.sgpaResult)
  const setSgpaResult = useSgpaStore((s) => s.setSgpaResult)

  const mutation = useMutation({
    mutationFn: (request: SgpaRequest) => calculateSgpa(request),
    onSuccess: (result) => setSgpaResult(result),
  })

  function recalculate(creditOverrides: Record<string, number>) {
    if (!sgpaResult) return
    mutation.mutate(buildRequest(sgpaResult, creditOverrides))
  }

  return {
    recalculate,
    isPending: mutation.isPending,
    isSuccess: mutation.isSuccess,
    isError: mutation.isError,
    error: mutation.error as ApiError | null,
    reset: mutation.reset,
  }
}

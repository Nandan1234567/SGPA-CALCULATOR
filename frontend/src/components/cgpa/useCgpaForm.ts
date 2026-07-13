import { useCallback, useMemo, useState } from 'react'
import { VTU_22_BRANCHES, getDivision } from '../../constants/vtuCredits'
import type { SemesterRecord } from '../../hooks/useLocalHistory'
import { getAllUsns, getHistory } from '../../hooks/useLocalHistory'
import type { CgpaResult, RowState, SemPoint } from './CgpaTypes'
import { makeEmptyRows } from './CgpaTypes'

export function useCgpaForm() {
  const [ studentName, setStudentName ] = useState('')
  const [ selectedBranch, setSelectedBranch ] = useState('')
  const [ rows, setRows ] = useState<Record<number, RowState>>(makeEmptyRows)
  const [ selectedKeys, setSelectedKeys ] = useState<Set<string>>(new Set())
  const [ historyOpen, setHistoryOpen ] = useState(false)
  const [ result, setResult ] = useState<CgpaResult | null>(null)
  const [ error, setError ] = useState<string | null>(null)

  const allUsns = useMemo(() => getAllUsns(), [])

  const historyByUsn = useMemo(() => {
    const map = new Map<string, SemesterRecord[]>()
    for (const usn of allUsns) {
      const recs = getHistory(usn).sort((a, b) => a.semester - b.semester)
      if (recs.length) map.set(usn, recs)
    }
    return map
  }, [ allUsns ])

  const totalHistoryCount = useMemo(
    () => [ ...historyByUsn.values() ].reduce((n, recs) => n + recs.length, 0),
    [ historyByUsn ],
  )

  function handleBranchChange(code: string) {
    setSelectedBranch(code)
    const branch = VTU_22_BRANCHES.find((b) => b.code === code)
    if (!branch) return
    setRows((prev) => {
      const next = { ...prev }
      for (let sem = 1; sem <= 8; sem++) {
        if (next[ sem ].source !== 'history') {
          next[ sem ] = {
            ...next[ sem ],
            credits: String(branch.credits[ sem - 1 ]),
            source: next[ sem ].source === 'empty' ? 'branch' : next[ sem ].source,
          }
        }
      }
      return next
    })
    setResult(null)
  }

  const handleHistoryToggle = useCallback(
    (record: SemesterRecord) => {
      const key = `${record.usn}:${record.semester}`
      setSelectedKeys((prev) => {
        const next = new Set(prev)
        if (next.has(key)) {
          next.delete(key)
          setRows((r) => {
            const branch = VTU_22_BRANCHES.find((b) => b.code === selectedBranch)
            const defCr = branch ? String(branch.credits[ record.semester - 1 ]) : ''
            return {
              ...r,
              [ record.semester ]: { sgpa: '', credits: defCr, source: defCr ? 'branch' : 'empty' },
            }
          })
        } else {
          next.add(key)
          setRows((r) => ({
            ...r,
            [ record.semester ]: {
              sgpa: record.sgpa.toFixed(2),
              credits: String(record.totalCredits),
              source: 'history',
            },
          }))
        }
        return next
      })
      setResult(null)
    },
    [ selectedBranch ],
  )

  function handleRowChange(sem: number, field: 'sgpa' | 'credits', value: string) {
    if (field === 'sgpa') {
      const matchKey = [ ...selectedKeys ].find((k) => k.endsWith(`:${sem}`))
      if (matchKey) {
        setSelectedKeys((prev) => { const n = new Set(prev); n.delete(matchKey); return n })
      }
    }
    setRows((prev) => ({
      ...prev,
      [ sem ]: { ...prev[ sem ], [ field ]: value, source: 'manual' },
    }))
    setResult(null)
  }

  function handleCalculate() {
    setError(null)
    let weighted = 0
    let totalCr = 0
    const chart: SemPoint[] = []

    for (let sem = 1; sem <= 8; sem++) {
      const { sgpa, credits } = rows[ sem ]
      if (!sgpa.trim()) continue
      const s = parseFloat(sgpa)
      const c = parseInt(credits, 10)
      if (isNaN(s) || s < 0 || s > 10 || isNaN(c) || c <= 0) continue
      weighted += s * c
      totalCr += c
      chart.push({ label: `Sem ${sem}`, sgpa: s, credits: c })
    }

    if (!chart.length) {
      setError('Enter at least one semester SGPA to calculate.')
      return
    }

    const cgpa = weighted / totalCr
    setResult({
      cgpa: Math.round(cgpa * 10000) / 10000,
      totalCredits: totalCr,
      weightedSum: Math.round(weighted * 100) / 100,
      semesterCount: chart.length,
      chartData: chart,
      studentName: studentName.trim(),
    })

    setTimeout(() => {
      document.getElementById('cgpa-result')?.scrollIntoView({ behavior: 'smooth', block: 'start' })
    }, 80)
  }

  function handleReset() {
    setRows(makeEmptyRows())
    setSelectedKeys(new Set())
    setSelectedBranch('')
    setStudentName('')
    setResult(null)
    setError(null)
  }

  const filledCount = Object.values(rows).filter((r: RowState) => r.sgpa.trim() !== '').length
  const division = result ? getDivision(result.cgpa) : null
  const percentage = result && result.cgpa > 0.75
    ? ((result.cgpa - 0.75) * 10).toFixed(2)
    : '0.00'

  return {
    studentName,
    setStudentName,
    selectedBranch,
    rows,
    selectedKeys,
    historyOpen,
    setHistoryOpen,
    historyByUsn,
    totalHistoryCount,
    result,
    error,
    filledCount,
    division,
    percentage,
    handleBranchChange,
    handleHistoryToggle,
    handleRowChange,
    handleCalculate,
    handleReset,
  }
}



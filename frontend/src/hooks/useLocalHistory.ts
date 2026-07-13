import type { SgpaResponse } from '../api/types'

export interface SemesterRecord {
  semester: number
  sgpa: number
  totalCredits: number
  studentName: string
  usn: string
  savedAt: string
}

const KEY = 'sgpa_history'

function readAll(): SemesterRecord[] {
  try {
    const raw = localStorage.getItem(KEY)
    return raw ? (JSON.parse(raw) as SemesterRecord[]) : []
  } catch {
    return []
  }
}

function writeAll(records: SemesterRecord[]): void {
  try {
    localStorage.setItem(KEY, JSON.stringify(records))
  } catch {
    // Storage full — fail silently
  }
}

export function saveToHistory(result: SgpaResponse): void {
  if (!result.sgpa || !result.semester || !result.usn) return

  const others = readAll().filter(
    (r) => !(r.usn === result.usn && r.semester === result.semester),
  )

  const newRecord: SemesterRecord = {
    semester: result.semester,
    sgpa: result.sgpa,
    totalCredits: result.totalCredits,
    studentName: result.studentName,
    usn: result.usn,
    savedAt: new Date().toISOString(),
  }

  const forThisUsn = [ ...others.filter((r) => r.usn === result.usn), newRecord ]
    .sort((a, b) => a.semester - b.semester)
    .slice(0, 8)

  writeAll([ ...others.filter((r) => r.usn !== result.usn), ...forThisUsn ])
}

export function getHistory(usn?: string): SemesterRecord[] {
  const all = readAll()
  if (!usn) return all
  return all.filter((r) => r.usn === usn).sort((a, b) => a.semester - b.semester)
}

export function getAllUsns(): string[] {
  return [ ...new Set(readAll().map((r) => r.usn)) ]
}

export function clearHistory(usn?: string): void {
  if (!usn) { localStorage.removeItem(KEY); return }
  writeAll(readAll().filter((r) => r.usn !== usn))
}


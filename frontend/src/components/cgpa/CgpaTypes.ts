import type { SemPoint } from '../sgpa/SemesterGraph'

export type { SemPoint }

export type RowSource = 'empty' | 'branch' | 'history' | 'manual'

export interface RowState {
  sgpa: string
  credits: string
  source: RowSource
}

export interface CgpaResult {
  cgpa: number
  totalCredits: number
  weightedSum: number
  semesterCount: number
  chartData: SemPoint[]
  studentName: string
}

export const DIVISION_ROWS = [
  { range: '≥ 7.00', short: 'FCD', full: 'First Class with Distinction', min: 7.0 },
  { range: '6.00 – 6.99', short: 'FC', full: 'First Class', min: 6.0 },
  { range: '5.00 – 5.99', short: 'SC', full: 'Second Class', min: 5.0 },
  { range: '4.00 – 4.99', short: 'P', full: 'Pass', min: 4.0 },
] as const

export function makeEmptyRows(): Record<number, RowState> {
  const out: Record<number, RowState> = {}
  for (let i = 1; i <= 8; i++) out[ i ] = { sgpa: '', credits: '', source: 'empty' }
  return out
}

export function cgpaColor(v: number): string {
  if (v >= 8.5) return '#1a3a2a'
  if (v >= 7.0) return '#1a5435'
  if (v >= 5.0) return '#92400e'
  return '#b91c1c'
}

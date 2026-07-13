export interface BranchInfo {
  code: string
  name: string
  credits: number[]
}

export interface DivisionInfo {
  short: string
  full: string
  color: string
}

export const VTU_22_BRANCHES: BranchInfo[] = [
  { code: 'CSE', name: 'Computer Science & Engineering', credits: [ 20, 21, 21, 22, 22, 21, 22, 16 ] },
  { code: 'ISE', name: 'Information Science & Engineering', credits: [ 20, 21, 21, 22, 22, 21, 22, 16 ] },
  { code: 'AIML', name: 'AI & Machine Learning', credits: [ 20, 21, 21, 22, 22, 21, 22, 16 ] },
  { code: 'IOT', name: 'Internet of Things', credits: [ 20, 21, 21, 22, 22, 21, 22, 16 ] },
  { code: 'DS', name: 'Data Science', credits: [ 20, 21, 21, 22, 22, 21, 22, 16 ] },
  { code: 'CY', name: 'Cyber Security', credits: [ 20, 21, 21, 22, 22, 21, 22, 16 ] },
  { code: 'ECE', name: 'Electronics & Communication Engg.', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'ETE', name: 'Electronics & Telecommunication Engg.', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'EIE', name: 'Electronics & Instrumentation Engg.', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'ML', name: 'Medical Electronics', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'EEE', name: 'Electrical & Electronics Engineering', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'ME', name: 'Mechanical Engineering', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'IM', name: 'Industrial Engineering & Management', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'AU', name: 'Automobile Engineering', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'CV', name: 'Civil Engineering', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'CH', name: 'Chemical Engineering', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'BT', name: 'Biotechnology', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'AS', name: 'Aerospace Engineering', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
  { code: 'TE', name: 'Textile Technology', credits: [ 20, 21, 22, 22, 22, 21, 22, 16 ] },
]

export function getDivision(cgpa: number): DivisionInfo {
  if (cgpa >= 7.0) return { short: 'FCD', full: 'First Class with Distinction', color: '#1a3a2a' }
  if (cgpa >= 6.0) return { short: 'FC', full: 'First Class', color: '#1a3a2a' }
  if (cgpa >= 5.0) return { short: 'SC', full: 'Second Class', color: '#d97706' }
  if (cgpa >= 4.0) return { short: 'P', full: 'Pass', color: '#92400e' }
  return { short: '—', full: 'Below Pass', color: '#b91c1c' }
}

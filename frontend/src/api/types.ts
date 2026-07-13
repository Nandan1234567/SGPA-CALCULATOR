// ─── REQUEST TYPES (sent TO backend) ────────────────────────────────────────

export interface SubjectInput {
  subjectCode: string
  subjectName: string
  internalMarks: number           // CIE, 0–50. Controller clamps to this range.
  externalMarks: number           // SEE, 0–100 (usually 0–50)
  totalMarks: number              // internalMarks + externalMarks
  result?: string | null          // PDF result: "P"|"F"|"W"|"A"|"X"|"NE"|""
  manualCreditOverride?: number | null

}
//  the format of the error
export interface ApiError {
  statusCode: number
  message: string
  requestId?: string
  retryAfter?: string
}
export interface SgpaRequest {
  studentName: string
  usn: string
  semester: number
  subjects: SubjectInput[]
}

// ─── RESPONSE TYPES (received FROM backend) ──────────────────────────────────

export interface SubjectResultDto {
  subjectCode: string
  subjectName: string
  internalMarks: number
  externalMarks: number
  totalMarks: number
  credits: number                 // 0 for unresolved or non-credit
  gradePoints: number             // 0–10
  creditPoints: number            // credits × gradePoints
  grade: string                   // "O"|"A+"|"A"|"B+"|"B"|"C"|"P"|"F"|"?"|"W"|"X"|"NE"
  isPass: boolean
  isNonCreditForSgpa: boolean     // NSS/Yoga/PE/Industry Connect
  resolutionMethod: string        // debug info — how credits were found
  isIncludedInSgpa: boolean       // false if excluded for any reason
  isUnresolved: boolean           // true if credits could not be determined
}

export interface SgpaResponse {
  studentName: string
  usn: string
  semester: number
  scheme: string                  // "22" — always, currently
  sgpa: number
  totalCredits: number
  subjects: SubjectResultDto[]
  unresolvedCodes: string[]       // codes where isUnresolved === true
  hasWarnings: boolean            // true if unresolvedCodes.length > 0
}

// ─── ERROR TYPES ─────────────────────────────────────────────────────────────

// From ExceptionHandleMiddleware.cs → ApiErrorResponse c
export interface ApiErrorResponse {
  requestId: string               // "ERR-XXXXXXXX" — find this in server logs
  statusCode: number
  error: string                   // user-readable message
  timeStamp: string               // ISO date string
}

// From Program.cs → rate limiter OnRejected
export interface RateLimitErrorResponse {
  error: string
  statusCode: 429
  retryAfter: string              // "60 seconds" — NOTE: STRING not number
}

// Our typed error class — lets calling code check error.statusCode

export class ApiError extends Error {
  public statusCode: number
  public requestId?: string
  public retryAfter?: string

  constructor(
    statusCode: number,
    message: string,
    requestId?: string,
    retryAfter?: string,   // for 429: "60 seconds"
  ) {
    super(message)

    this.statusCode = statusCode
    this.requestId = requestId
    this.retryAfter = retryAfter

    // super(message): calls Error's constructor, sets this.message
    this.name = 'ApiError'

  }
}

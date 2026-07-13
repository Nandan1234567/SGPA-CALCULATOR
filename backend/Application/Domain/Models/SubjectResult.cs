// Domain/Models/SubjectResult.cs
// ─────────────────────────────────────────────────────────────────────────────
// BUGS FIXED:
//
// BUG 1 (Critical) — internal-only subjects always showed grade=F
//   Old:  if (externalMarks < 18) return 0.0;
//   BNSK459: external=0 → 0 < 18 = TRUE → grade=F  ← WRONG, PDF says P
//   Fix:  if (normExternal > 0 && normExternal < 18)
//   Now:  external=0 skips the check → grade computed from total alone ✓
//
// BUG 2 — Withheld / Absent subjects
//   W (Withheld) and A (Absent) need special grades, not computed from marks.
//   We accept a pdfResult param ("P","F","W","A","X") and use it to override
//   the computed grade when marks can't be trusted (W, A).
//
// LEARNING — Why accept pdfResult at all?
//   Because the PDF is the official source of truth. VTU prints "P" or "F"
//   directly. Our formula is a fallback when pdfResult is empty or unavailable.
//   When pdfResult is available, we trust it for IsPass and Grade display.
//   We still compute GradePoints from marks because SGPA needs the number.
// ─────────────────────────────────────────────────────────────────────────────

namespace SGPA_CALCULATOR.Domain.Models
{
    public class SubjectResult
    {
        public string SubjectCode { get; private set; } = string.Empty;
        public string SubjectName { get; private set; } = string.Empty;
        public int InternalMarks { get; private set; }   // CIE — out of 50
        public int ExternalMarks { get; private set; }   // SEE — out of 50 (0 = no external exam)
        public int TotalMarks { get; private set; }   // out of 100
        public int Credits { get; private set; }
        public bool IsLab { get; private set; }
        public double GradePoints { get; private set; }
        public double CreditPoints { get; private set; }
        public string Grade { get; private set; } = string.Empty;
        public bool IsPass { get; private set; }

        /// <param name="cie">Internal marks out of 50</param>
        /// <param name="see">External marks out of 50. 0 = internal-only subject.</param>
        /// <param name="totalMarks">As read from PDF (may differ from cie+see for 200-mark subjects)</param>
        /// <param name="credits">From VtuCreditResolver</param>
        /// <param name="pdfResult">
        ///   Official result from PDF: "P" / "F" / "W" / "A" / "X" / "NE" / ""
        ///   When non-empty, overrides computed IsPass and Grade for display.
        ///   GradePoints are still computed from marks (needed for SGPA math).
        /// </param>
        public SubjectResult(
            string subjectCode,
            string subjectName,
            int cie,
            int see,
            int totalMarks,
            int credits,
            string pdfResult = "",
            bool isLab = false)
        {
            SubjectCode = subjectCode;
            SubjectName = subjectName;
            InternalMarks = cie;
            ExternalMarks = see;
            TotalMarks = totalMarks;
            Credits = credits;
            IsLab = isLab;

            // ── Handle special PDF result codes first ──────────────────────
            // W = Withheld, A = Absent, X/NE = Not Eligible
            // For these, marks are unreliable (often 0). Don't compute grade from marks.
            var result = pdfResult.Trim().ToUpperInvariant();
            if (result is "W" or "A" or "X" or "NE")
            {
                GradePoints = 0.0;
                CreditPoints = 0.0;
                Grade = result;   // display exactly as PDF says
                IsPass = false;    // withheld/absent = not yet passed
                return;
            }

            // ── Normalization (handles 200-mark subjects) ──────────────────
            // Standard: CIE 0-50 + SEE 0-50 = Total 0-100 → scale = 1.0
            // 200-mark: scale = 2.0. Thresholds stay the same (40, 18).
            double scale = (totalMarks > 100) ? 2.0 : 1.0;
            int normTotal = (int)Math.Round(totalMarks / scale);
            int normExternal = (int)Math.Round(see / scale);

            // ── Grade points from marks ────────────────────────────────────
            GradePoints = ComputeGradePoints(normTotal, normExternal);
            CreditPoints = Math.Round(GradePoints * credits, 2);

            // ── Grade and IsPass: trust PDF when available ─────────────────
            // If pdfResult = "P" → IsPass = true regardless of our formula.
            // This correctly handles edge cases VTU allows that we can't model:
            //   - Grace marks, revaluation results, special passes.
            if (!string.IsNullOrWhiteSpace(result))
            {
                IsPass = result == "P";
                Grade = IsPass ? ComputeGrade(normTotal, GradePoints) : "F";
            }
            else
            {
                // No PDF result available → use computed values
                IsPass = GradePoints > 0;
                Grade = ComputeGrade(normTotal, GradePoints);
            }
        }

        // ── VTU 22 Scheme pass rule ────────────────────────────────────────
        // Both conditions must be true:
        //   1. Total >= 40/100
        //   2. SEE >= 18/50  (equivalent to ≥36/100, ≥35 threshold)
        //      SKIP condition 2 when normExternal == 0 (no external exam):
        //      Mini Project, Yoga, Social Connect, NSS — internal-only.
        //      Pass for these is decided by total alone.
        private static double ComputeGradePoints(int normTotal, int normExternal)
        {
            // External exam minimum — only when there IS an external exam
            if (normExternal > 0 && normExternal < 18)
                return 0.0;  // Failed SEE minimum

            // Total minimum
            if (normTotal < 40)
                return 0.0;  // Failed total minimum

            return normTotal switch
            {
                >= 90 => 10.0,   // O
                >= 80 => 9.0,    // A+
                >= 70 => 8.0,    // A
                >= 60 => 7.0,    // B+
                >= 55 => 6.0,    // B
                >= 50 => 5.0,    // C
                >= 40 => 4.0,    // P
                _ => 0.0,
            };
        }

        private static string ComputeGrade(int normTotal, double gradePoints)
        {
            if (gradePoints == 0.0) return "F";
            return normTotal switch
            {
                >= 90 => "O",
                >= 80 => "A+",
                >= 70 => "A",
                >= 60 => "B+",
                >= 55 => "B",
                >= 50 => "C",
                >= 40 => "P",
                _ => "F",
            };
        }
    }
}
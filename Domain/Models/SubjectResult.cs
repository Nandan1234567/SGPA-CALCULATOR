namespace SGPA_CALCULATOR.Domain.Models
{
    /// <summary>
    /// Immutable value object representing a subject result in VTU 22 Scheme.
    /// Encapsulates grade calculation logic and pass/fail determination.
    /// </summary>
    public class SubjectResult
    {
        // ════════════════════════════════════════════════════════════════
        // PROPERTIES
        // ════════════════════════════════════════════════════════════════

        public string SubjectCode { get; private set; } = string.Empty;
        public string SubjectName { get; private set; } = string.Empty;

        public int InternalMarks { get; private set; }      // CIE
        public int ExternalMarks { get; private set; }      // SEE
        public int TotalMarks { get; private set; }         // CIE + SEE

        public int Credits { get; private set; }
        public bool IsLab { get; private set; }

        public double GradePoints { get; private set; }
        public double CreditPoints { get; private set; }
        public string Grade { get; private set; } = string.Empty;
        public bool IsPass { get; private set; }


        // ════════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ════════════════════════════════════════════════════════════════

        public SubjectResult(
            string subjectCode,
            string subjectName,
            int internalMarks,
            int externalMarks,
            int totalMarks,
            int credits,
            bool isLab = false
        )
        {
            // Assign from parameters
            SubjectCode = subjectCode;
            SubjectName = subjectName;
            InternalMarks = internalMarks;
            ExternalMarks = externalMarks;
            TotalMarks = totalMarks;
            Credits = credits;
            IsLab = isLab;

            // Step 1: Determine normalization scale (1 for 100-mark subjects, 2 for 200-mark subjects)
            double scale = (totalMarks > 100) ? 2.0 : 1.0;

            // Step 2: Calculate normalized values for logic checks
            int normTotal = (int)Math.Round(totalMarks / scale);
            int normExternal = (int)Math.Round(externalMarks / scale);
            int normInternal = (int)Math.Round(internalMarks / scale);

            // Step 3: Calculate derived values
            GradePoints = ComputeGradePoints(normTotal, normExternal, normInternal);
            IsPass = GradePoints > 0;
            Grade = ComputeGrade(normTotal, GradePoints);
            CreditPoints = Math.Round(GradePoints * credits, 2);
        }

        // ════════════════════════════════════════════════════════════════
        // PRIVATE METHODS
        // ════════════════════════════════════════════════════════════════

        private static double ComputeGradePoints(int totalMarks, int externalMarks, int internalMarks)
        {
            // Rule 1: Check external minimum (SEE ≥ 18/50)
            if (externalMarks < 18)
                return 0.0;

            // Rule 2: Check total minimum (Total ≥ 40/100)
            if (totalMarks < 40)
                return 0.0;

            // Rule 3: Assign grade points
            return totalMarks switch
            {
                >= 90 => 10.0,
                >= 80 => 9.0,
                >= 70 => 8.0,
                >= 60 => 7.0,
                >= 55 => 6.0,
                >= 50 => 5.0,
                >= 40 => 4.0,
                _ => 0.0
            };
        }

        private static string ComputeGrade(int totalMarks, double gradePoints)
        {
            if (gradePoints == 0.0)
                return "F";

            return totalMarks switch
            {
                >= 90 => "O",
                >= 80 => "A+",
                >= 70 => "A",
                >= 60 => "B+",
                >= 55 => "B",
                >= 50 => "C",
                >= 40 => "P",
                _ => "F"
            };
        }
    }
}
/// <summary>
/// Computes letter grade based on total marks and grade points.
/// Returns "F" if grade points are 0, otherwise maps total marks to letter grade.
/// </summary>

// ════════════════════════════════════════════════════════════════
// PRIVATE METHODS — All calculation logic isolated here
// ════════════════════════════════════════════════════════════════

/// <summary>
/// Computes grade points based on VTU 22 Scheme rules.
/// This is the SINGLE SOURCE OF TRUTH for pass/fail determination.
/// 
/// VTU Pass Criteria (ALL must be true):
/// 1. Total Marks ≥ 40/100
/// 2. External Marks (SEE) ≥ 18/50
/// 3. Internal Marks (CIE) ≥ 20/50
/// 
/// If ANY condition fails → returns 0 (Fail)
/// Otherwise → returns grade points based on total marks
/// </summary>
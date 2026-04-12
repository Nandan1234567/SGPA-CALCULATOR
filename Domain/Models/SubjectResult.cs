namespace SGPA_CALCULATOR.Domain.Models
{
    public class SubjectResult
    {
        // ── properties ────────────────────────────────────────────────
        // private set = only this class can write to these, nobody outside can change them

        public string SubjectCode { get; private set; } = string.Empty;
        public string SubjectName { get; private set; } = string.Empty;

        // comes from PDF
        public int InternalMarks { get; private set; }
        public int ExternalMarks { get; private set; }
        public int TotalMarks { get; private set; }
        public string ResultOfSubj { get; private set; } = string.Empty; // "P" or "F" from PDF

        // comes from DB (SubjectMaster table)
        public int Credits { get; private set; }
        public bool IsLab { get; private set; }

        // calculated inside constructor — needs nothing external
        public int GradePoints { get; private set; }
        public int CreditPoints { get; private set; }


        // ── constructor ───────────────────────────────────────────────
        // you pass EVERYTHING it needs — PDF values + DB values
        // it then calculates GradePoints and CreditPoints itself

        public SubjectResult(
            string subjectCode,
            string subjectName,
            int internalMarks,
            int externalMarks,
            string resultOfSubj,   // "P" or "F" — directly from PDF
            int credits, 
            int totalMarks// from SubjectMaster DB
                     // from SubjectMaster DB
        )
        {
            // from PDF
            SubjectCode = subjectCode;
            SubjectName = subjectName;
            InternalMarks = internalMarks;
            ExternalMarks = externalMarks;
            ResultOfSubj = resultOfSubj;
           

            // from DB
            Credits = credits;


            // calculated — private method below does the work
            GradePoints = CalculateGradePoints(internalMarks, externalMarks, totalMarks, resultOfSubj);
            CreditPoints = GradePoints * Credits;
        }


        // ── private method ─────────────────────────────────────────────
        // private = nobody outside can call this, only this class uses it
        // this method has ONE job: given marks, return grade points

        private static int CalculateGradePoints(
            int totalMarks,
            int externalMarks,
            int internalMarks,
            string subjectResult   // trust the PDF if it says "F"
          
        )
        {
            // Rule 1 — if PDF itself says F, trust it, return 0
            // This handles edge cases like BCSL305 (44 internal + 7 external = 51 total, but F)
            if (subjectResult == "F") return 0;
            if (subjectResult == "W") return 0;

            // Rule 2 — VTU external minimum and internal minimum
            if (internalMarks < 20) return 0;

            // Rule 3 — external minimum (18/50)
            if (externalMarks < 18) return 0;

            // Rule 4 — total minimum (40/100)
            if (totalMarks < 40) return 0;


            // Rule 3 — grade points from total marks
            if (totalMarks >= 90) return 10; // O
            if (totalMarks >= 80) return 9;  // A+
            if (totalMarks >= 70) return 8;  // A
            if (totalMarks >= 60) return 7;  // B+
            if (totalMarks >= 55) return 6;  // B
            if (totalMarks >= 50) return 5;  // C
            if (totalMarks >= 40) return 4;  // P

            return 0; // F
        }
    }
}
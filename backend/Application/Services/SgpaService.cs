// Application/Services/SgpaService.cs
// ─────────────────────────────────────────────────────────────────────────────
// CHANGES IN THIS VERSION:
//
// 1. SubjectResult now receives pdfResult ("P","F","W","A","X")
//    → IsPass and Grade reflect the PDF, not just our formula
//    → Withheld (W) and Absent (A) show correctly instead of "F"
//
// 2. Withheld / Absent subjects are EXCLUDED from SGPA credits
//    If a subject is W or A, its marks are unreliable (often 0 0 0).
//    Including it would lower SGPA with junk data.
//    It appears in the response with grade="W"/"A" so the frontend can show it.
//
// 3. Arrear subjects (fail/withheld from a previous semester appearing
//    in the current PDF) are handled via the credit resolver — they resolve
//    to whatever their semester position says. If unresolved, they get
//    added to unresolvedCodes with a clear message.
//
// LEARNING — Why pass pdfResult into SubjectResult?
//   Single Responsibility: SubjectResult owns grade display logic.
//   If we overrode IsPass in SgpaService after construction, we'd be
//   scattering grade logic across two files. Better to give SubjectResult
//   all the info it needs and let it decide.
// ─────────────────────────────────────────────────────────────────────────────

using SGPA_CALCULATOR.Application.Dtos;
using SGPA_CALCULATOR.Application.Interface;
using SGPA_CALCULATOR.Domain.Models;

namespace SGPA_CALCULATOR.Application.Services
{
    public class SgpaService : ISgpaService
    {
        private readonly VtuCreditResolver _resolver;

        public SgpaService(VtuCreditResolver resolver)
        {
            _resolver = resolver;
        }

        public SgpaResponse Calculate(SgpaRequest request)
        {
            var results = new List<SubjectResultDto>();
            var unresolvedCodes = new List<string>();
            int totalCredits = 0;
            double totalPoints = 0;

            // get a subjects forn the request , so go through controller what request has
            // tnen here we get subjects so for each subject we get as a sub
            // this sub goes to model subject result to get a credit and other  things
            foreach (var sub in request.Subjects)
            {
                // ── Resolve credits ─────────────────────────────────────────
                var creditInfo = _resolver.Resolve(sub.SubjectCode);

                int credits = creditInfo.IsResolved
                    ? creditInfo.Credits
                    : sub.ManualCreditOverride ?? 0;

                if (credits == 0 && !creditInfo.IsNonCreditForSgpa)
                {
                    unresolvedCodes.Add(sub.SubjectCode);
                    //continue;  // can't calculate — skip rather than crash
                }

                // ── Compute total marks ──────────────────────────────────────
                // Trust the PDF total when available; fallback to cie + see.
                int finalTotal = (sub.TotalMarks > 0)
                    ? sub.TotalMarks
                    : (sub.InternalMarks + sub.ExternalMarks);

                // ── Detect withheld / absent ─────────────────────────────────
                // "W" = withheld, "A" = absent, "X"/"NE" = not eligible.
                // These marks are not real — don't use them for SGPA.
                var pdfResult = sub.Result?.Trim().ToUpperInvariant() ?? "";
                bool isSpecial = pdfResult is "W" or "A" or "X" or "NE";

                // ── Build SubjectResult (grade + pass computation) ────────────
                var subResult = new SubjectResult(
                    sub.SubjectCode,
                    sub.SubjectName,
                    sub.InternalMarks,
                    sub.ExternalMarks,
                    finalTotal,
                    credits,
                    pdfResult   // ← new: lets SubjectResult trust PDF for IsPass
                );

                results.Add(new SubjectResultDto
                {
                    SubjectCode = subResult.SubjectCode,
                    SubjectName = subResult.SubjectName,
                    InternalMarks = subResult.InternalMarks,
                    ExternalMarks = subResult.ExternalMarks,
                    TotalMarks = finalTotal,
                    Credits = subResult.Credits,
                    GradePoints = subResult.GradePoints,
                    CreditPoints = subResult.CreditPoints,
                    Grade = subResult.Grade,
                    IsPass = subResult.IsPass,
                    IsNonCreditForSgpa = creditInfo.IsNonCreditForSgpa,
                    ResolutionMethod = creditInfo.ResolutionMethod,
                });

                // ── Add to SGPA totals ───────────────────────────────────────
                // Rules for inclusion:
                //   ✓ Credit-bearing (credits > 0)
                //   ✓ Not excluded (IsNonCreditForSgpa = false)
                //   ✗ Withheld / Absent / Not-eligible — marks unreliable
                if (!creditInfo.IsNonCreditForSgpa && credits > 0 && !isSpecial)
                {
                    totalCredits += credits;
                    totalPoints += subResult.CreditPoints;
                }
            }

            double sgpa = totalCredits > 0
                ? Math.Round(totalPoints / totalCredits, 2)
                : 0;

            return new SgpaResponse
            {
                StudentName = request.StudentName,
                Usn = request.Usn,
                Semester = request.Semester,
                Scheme = DetectScheme(request.Usn),
                Sgpa = sgpa,
                TotalCredits = totalCredits,
                Subjects = results,
                UnresolvedCodes = unresolvedCodes,
                HasWarnings = unresolvedCodes.Count > 0,
            };
        }

        // USN format: 4MK22CS030 → chars [3..4] = "22" → scheme "22"
        private static string DetectScheme(string usn)
        {
            if (string.IsNullOrWhiteSpace(usn) || usn.Length < 5) return "22";
            if (char.IsDigit(usn[3]) && char.IsDigit(usn[4]))
                return usn.Substring(3, 2);
            return "22";
        }
    }
}
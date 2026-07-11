using SGPA_CALCULATOR.Application.Dtos;
using SGPA_CALCULATOR.Application.Interface;
using SGPA_CALCULATOR.Domain.Models;

namespace SGPA_CALCULATOR.Application.Services
{
    public class SgpaService : ISgpaService
    {
        // Injected resolver is a Singleton; read-only cache access is thread-safe across requests
        private readonly VtuCreditResolver _resolver;

        public SgpaService(VtuCreditResolver resolver)
        {
            _resolver = resolver;
        }

        /// <summary>
        /// Evaluates subject grades, applies credit overrides, and computes the semester SGPA.
        /// </summary>
        public SgpaResponse Calculate(SgpaRequest request)
        {
            var results = new List<SubjectResultDto>();
            var unresolvedCodes = new List<string>();
            int totalCredits = 0;
            double totalPoints = 0;

            foreach (var sub in request.Subjects)
            {
                // Step 1: Resolve credits via manual override or static resolver lookup
                var creditInfo = _resolver.Resolve(sub.SubjectCode);
                int? overrideValue = sub.ManualCreditOverride;

                // Silently discard out-of-bounds manual overrides so valid subjects still process
                if (overrideValue.HasValue && (overrideValue.Value < 0 || overrideValue.Value > 10))
                {
                    overrideValue = null;
                }

                int credits = overrideValue.HasValue
                    ? overrideValue.Value
                    : creditInfo.IsResolved ? creditInfo.Credits : 0;

                // Step 2: Differentiate between intentional zero-credit (audit/override) and unrecognized codes
                bool userExplicitlySetZero = overrideValue.HasValue && overrideValue.Value == 0;
                bool isUnresolved = credits == 0 && !creditInfo.IsNonCreditForSgpa && !userExplicitlySetZero;

                // Step 3: Handle unrecognized subjects — exclude from math but return DTO so UI can prompt for input
                if (isUnresolved)
                {
                    unresolvedCodes.Add(sub.SubjectCode);

                    results.Add(new SubjectResultDto
                    {
                        SubjectCode = sub.SubjectCode,
                        SubjectName = sub.SubjectName,
                        InternalMarks = sub.InternalMarks,
                        ExternalMarks = sub.ExternalMarks,
                        TotalMarks = sub.TotalMarks,
                        Credits = 0,
                        GradePoints = 0,
                        CreditPoints = 0,
                        Grade = "?",
                        IsPass = false,
                        IsNonCreditForSgpa = false,
                        ResolutionMethod = creditInfo.ResolutionMethod,
                        IsIncludedInSgpa = false,
                        IsUnresolved = true,
                    });

                    continue;
                }

                // Step 4: Handle declared non-credit subjects (override=0) — compute grade for display only
                if (userExplicitlySetZero)
                {
                    var pdfRes0 = sub.Result?.Trim().ToUpperInvariant() ?? "";
                    var zeroResult = new SubjectResult(
                        sub.SubjectCode,
                        sub.SubjectName,
                        sub.InternalMarks,
                        sub.ExternalMarks,
                        sub.TotalMarks,
                        0,
                        pdfRes0
                    );

                    results.Add(new SubjectResultDto
                    {
                        SubjectCode = zeroResult.SubjectCode,
                        SubjectName = zeroResult.SubjectName,
                        InternalMarks = zeroResult.InternalMarks,
                        ExternalMarks = zeroResult.ExternalMarks,
                        TotalMarks = sub.TotalMarks,
                        Credits = 0,
                        GradePoints = zeroResult.GradePoints,
                        CreditPoints = 0,
                        Grade = zeroResult.Grade,
                        IsPass = zeroResult.IsPass,
                        IsNonCreditForSgpa = true,
                        ResolutionMethod = "User declared non-credit (manualCreditOverride=0)",
                        IsIncludedInSgpa = false,
                        IsUnresolved = false,
                    });

                    continue;
                }

                // Step 5: Handle standard subjects — special exam results (W/A/X/NE) are displayed but excluded from math
                var pdfResult = sub.Result?.Trim().ToUpperInvariant() ?? "";
                bool isSpecial = pdfResult is "W" or "A" or "X" or "NE";

                var subResult = new SubjectResult(
                    sub.SubjectCode,
                    sub.SubjectName,
                    sub.InternalMarks,
                    sub.ExternalMarks,
                    sub.TotalMarks,
                    credits,
                    pdfResult
                );

                bool includedInSgpa = !creditInfo.IsNonCreditForSgpa && credits > 0 && !isSpecial;

                results.Add(new SubjectResultDto
                {
                    SubjectCode = subResult.SubjectCode,
                    SubjectName = subResult.SubjectName,
                    InternalMarks = subResult.InternalMarks,
                    ExternalMarks = subResult.ExternalMarks,
                    TotalMarks = sub.TotalMarks,
                    Credits = subResult.Credits,
                    GradePoints = subResult.GradePoints,
                    CreditPoints = subResult.CreditPoints,
                    Grade = subResult.Grade,
                    IsPass = subResult.IsPass,
                    IsNonCreditForSgpa = creditInfo.IsNonCreditForSgpa,
                    ResolutionMethod = creditInfo.ResolutionMethod,
                    IsIncludedInSgpa = includedInSgpa,
                    IsUnresolved = false,
                });

                // Accumulate points for standard credit-bearing subjects passing qualification
                if (includedInSgpa)
                {
                    totalCredits += credits;
                    totalPoints += subResult.CreditPoints;
                }
            }

            // Guard against division by zero when all subjects are unresolved or withheld
            double sgpa = totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;

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

        // Extracts the admission year from USN indices 3-4 (e.g., "22" from "4MK22CS030") to determine scheme
        private static string DetectScheme(string usn)
        {
            if (string.IsNullOrWhiteSpace(usn) || usn.Length < 5)
                return "22";

            if (char.IsDigit(usn[3]) && char.IsDigit(usn[4]))
                return usn.Substring(3, 2);

            return "22";
        }
    }
}
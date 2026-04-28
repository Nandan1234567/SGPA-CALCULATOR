using SGPA_CALCULATOR.Application.Dtos;
using SGPA_CALCULATOR.Application.Interface;
using SGPA_CALCULATOR.Domain.Models;

namespace SGPA_CALCULATOR.Application.Services
{
    public class SgpaService: ISgpaService
    {


        // this is di loosley coupled 
        private readonly VtuCreditResolver _resolver;

        public SgpaService(VtuCreditResolver resolver)
        {
            _resolver = resolver;
        }

        // why we use vtu credit resolver
        // bcz it having all methods and calcuation , like regex , zero credit calcuation etc


        public SgpaResponse Calculate(SgpaRequest request)
        {
            var results = new List<SubjectResultDto>();
            var unresolvedCodes = new List<string>();
            int totalCredits = 0;
            double totalPoints = 0;

            foreach (var sub in request.Subjects)
            {
                var creditInfo = _resolver.Resolve(sub.SubjectCode);


                int finalTotal = (sub.TotalMarks > 0)
        ? sub.TotalMarks
        : (sub.InternalMarks + sub.ExternalMarks);
                // If code is unresolved and caller provided a manual override, use it
                int credits = creditInfo.IsResolved
                    ? creditInfo.Credits
                    : sub.ManualCreditOverride ?? 0;

                if (credits == 0 && !creditInfo.IsNonCreditForSgpa)
                {
                    unresolvedCodes.Add(sub.SubjectCode);
                    continue;  // skip unresolvable subjects rather than crashing
                }

                var subResult = new SubjectResult(
                    sub.SubjectCode, sub.SubjectName,
                    sub.InternalMarks, sub.ExternalMarks,finalTotal, credits
                    
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

                // Only include credit-bearing subjects in SGPA
                if (!creditInfo.IsNonCreditForSgpa && credits > 0)
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


        // Detect VTU scheme from USN format: 4MK22CS030 → year=22 → scheme "22"
        // 4MK25CS030 → year=25 → scheme "25"
        private static string DetectScheme(string usn)
        {
            if (string.IsNullOrWhiteSpace(usn) || usn.Length < 5) return "22";
            // USN format: [digit][2-char college code][2-digit year][2-char branch]...
            // Year is at index 3-4 for standard VTU USN
            if (usn.Length >= 5 && char.IsDigit(usn[3]) && char.IsDigit(usn[4]))
            {
                string year = usn.Substring(3, 2);
                return year; // "22" for 2022 scheme, "25" for 2025 scheme, etc.
            }
            return "22"; // default
        }

    }
}

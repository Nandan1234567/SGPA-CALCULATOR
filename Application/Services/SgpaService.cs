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

                int? overrideValue = sub.ManualCreditOverride;  // we gave new name 

                if (overrideValue.HasValue)
                {
                    // .HasValue = the nullable int is not null (user sent a value)
                    if (overrideValue.Value < 0 || overrideValue.Value > 10)
                    {
                        // Bad value — discard the override, fall through to resolver
                        // Don't crash. Don't return error. Just ignore invalid override.
                        // WHY not return error?
                        // Because user might have 9 valid subjects and 1 bad override.
                        // Returning 400 blocks all 9 good subjects from calculating.
                        // Better: ignore the bad override, use resolver for that one subject.
                        overrideValue = null;
                    }
                }





                    int credits = overrideValue.HasValue
                  ? overrideValue.Value        // valid override → use it, period
                  : creditInfo.IsResolved
                  ? creditInfo.Credits     // resolver found it → use resolver
                  : 0;                     // unknown → unresolved

                // resolver also failed → 0 → subject gets skipped below
                // here are the things need to change and check




                // ════════════════════════════════════════════════════════════════
                // STEP 2: Handle unresolved subjects — FIX: don't skip, include
                // ════════════════════════════════════════════════════════════════

                bool userExplicitlySetZero = overrideValue.HasValue && overrideValue.Value == 0;


                bool isUnresolved = credits == 0
                  && !creditInfo.IsNonCreditForSgpa
                  && !userExplicitlySetZero;
                // IsNonCreditForSgpa = intentionally zero (NSS, Yoga, IKS)
                // isUnresolved       = accidentally zero (unknown subject code)
                // They look the same (credits=0) but mean different things.
                // IsNonCreditForSgpa → show as "not counted, by design"
                // isUnresolved       → show grey row with credit input field



                if (isUnresolved)
                {
                    // Add to warning list — response.HasWarnings will be true
                    unresolvedCodes.Add(sub.SubjectCode);

                    // ── FIX: Build a partial DTO instead of skipping ──────────
                    // OLD: continue → subject disappears from results entirely
                    // NEW: add to results with zeroed math, marked as unresolved
                    //
                    // React receives this row and renders:
                    //   Grey row with subject code + name visible
                    //   Grade = "?" (unknown until credit provided)
                    //   Credits input field: user types 3 → sends /calculate again
                    //   SGPA shows as partial with warning banner
                    results.Add(new SubjectResultDto
                    {
                        SubjectCode = sub.SubjectCode,
                        SubjectName = sub.SubjectName,
                        InternalMarks = sub.InternalMarks,
                        ExternalMarks = sub.ExternalMarks,
                        TotalMarks = sub.TotalMarks,   // controller already set this
                        Credits = 0,
                        GradePoints = 0,
                        CreditPoints = 0,
                        Grade = "?",              // unknown — not computable without credits
                        IsPass = false,
                        IsNonCreditForSgpa = false,
                        ResolutionMethod = creditInfo.ResolutionMethod,
                        // "UNRESOLVED — check VTU scheme or override manually"
                        IsIncludedInSgpa = false,            // excluded from SGPA math
                        IsUnresolved = true,             // React: show credit input field
                    });

                    // Skip SGPA accumulation — subject not included
                    // continue is correct here: we already added to results above
                    // we just don't want to run the math below for this subject
                    continue;
                }



                if (userExplicitlySetZero)
                {
                    // Build full SubjectResult — marks are real, grade is computable
                    var pdfRes = sub.Result?.Trim().ToUpperInvariant() ?? "";
                    var zeroResult = new SubjectResult(
                        sub.SubjectCode,
                        sub.SubjectName,
                        sub.InternalMarks,
                        sub.ExternalMarks,
                        sub.TotalMarks,
                        0,        // credits = 0, user declared
                        pdfRes
                    );

                    results.Add(new SubjectResultDto
                    {
                        SubjectCode = zeroResult.SubjectCode,
                        SubjectName = zeroResult.SubjectName,
                        InternalMarks = zeroResult.InternalMarks,
                        ExternalMarks = zeroResult.ExternalMarks,
                        TotalMarks = sub.TotalMarks,
                        Credits = 0,
                        GradePoints = zeroResult.GradePoints,   // computed — still meaningful
                        CreditPoints = 0,                        // 0 × anything = 0
                        Grade = zeroResult.Grade,         // shows "A" or "P" etc — informational
                        IsPass = zeroResult.IsPass,
                        IsNonCreditForSgpa = true,     // ← user declared it non-credit
                        ResolutionMethod = "User declared non-credit (manualCreditOverride=0)",
                        IsIncludedInSgpa = false,    // excluded from SGPA math
                        IsUnresolved = false,    // NOT unresolved — user resolved it
                    });
                    continue;  // skip SGPA accumulation — credits=0 contributes nothing
                }

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
                    sub.TotalMarks,
                    credits,
                    pdfResult   // ← new: lets SubjectResult trust PDF for IsPass
                );

                bool includedInSgpa = !creditInfo.IsNonCreditForSgpa
                       && credits > 0
                       && !isSpecial;


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
                    IsIncludedInSgpa = includedInSgpa,   // new field
                    IsUnresolved = false,
                });

                // ── Add to SGPA totals ───────────────────────────────────────
                // Rules for inclusion:
                //   ✓ Credit-bearing (credits > 0)
                //   ✓ Not excluded (IsNonCreditForSgpa = false)
                //   ✗ Withheld / Absent / Not-eligible — marks unreliable
                if (includedInSgpa)
                {
                    totalCredits += credits;
                    totalPoints += subResult.CreditPoints;
                    // Running totals for SGPA formula:
                    // SGPA = Σ(Credits × GradePoints) / Σ(Credits)
                    //      = totalPoints / totalCredits
                }
            } // end foreach


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
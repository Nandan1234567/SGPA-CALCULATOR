// Application/Mappers/PdfResultMapper.cs
//
// CONCEPT — Single Responsibility Principle (SRP):
// Before this file: SgpaController.FromPdf() was doing 3 things:
//   1. Validate the HTTP request (file null check, size check)
//   2. Call Flask to extract PDF
//   3. MAP PdfExtractResult → SgpaRequest  ← this doesn't belong in a controller
//   4. Call SgpaService.Calculate()
//   5. Return HTTP response
//
// A controller's job is to handle HTTP IN and HTTP OUT.
// Mapping between internal models is a separate concern.
// If Flask adds a new field (say, "CollegeCode"), you change ONE file here.
// The controller stays unchanged.
//
// This is also now unit-testable:
//   var result = PdfResultMapper.ToSgpaRequest(fakeExtracted);
//   Assert.Equal(3, result.Semester);
// Before: you couldn't test mapping without spinning up a controller.

using SGPA_CALCULATOR.Application.Dtos;
using SGPA_CALCULATOR.DTOs;

namespace SGPA_CALCULATOR.Application.Mappers
{
    public static class PdfResultMapper
    {
        public static SgpaRequest ToSgpaRequest(PdfExtractResult extracted)
        {
            // Flask returns semester as string ("3", "6") because PDF text is always string.
            // SgpaRequest wants int because it's a number semantically.
            // int.TryParse is safe — it never throws, returns false instead.
            if (!int.TryParse(extracted.Semester, out int semester))
                semester = 0;   // SgpaService handles 0 gracefully via DetectScheme

            return new SgpaRequest
            {
                StudentName = extracted.StudentName,
                Usn = extracted.Usn,
                Semester = semester,
                Subjects = extracted.Subjects.Select(row => new SubjectInput
                {
                    SubjectCode = row.SubjectCode,
                    SubjectName = row.SubjectName,
                    InternalMarks = row.InternalMarks,
                    ExternalMarks = row.ExternalMarks,
                    TotalMarks = row.Total,
                    Result = row.Result
                    // ManualCreditOverride = null intentionally
                    // PDF path never needs manual credit — resolver handles it
                }).ToList()
            };
        }
    }
}
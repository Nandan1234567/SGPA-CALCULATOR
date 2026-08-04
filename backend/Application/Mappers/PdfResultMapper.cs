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

namespace SGPA_CALCULATOR.Application.Dtos
{
    public class SgpaRequest
    {
        public string StudentName { get; set; } = string.Empty;
        public string Usn { get; set; } = string.Empty;
        public int Semester { get; set; }
        public List<SubjectInput> Subjects { get; set; } = new();
    }



    public class SubjectInput
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int InternalMarks { get; set; }   // CIE marks (out of 50)
        public int ExternalMarks { get; set; }   // SEE marks (out of 50)
        public int TotalMarks { get; set; }   // SEE marks (out of 100)

        // Optional: if pdfplumber extracts the credit from the PDF directly, use it.
        // Also serves as manual override when the pattern resolver can't determine credits.
        public int? ManualCreditOverride { get; set; }
    }
}

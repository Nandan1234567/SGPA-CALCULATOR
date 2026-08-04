

namespace SGPA_CALCULATOR.DTOs
{
    /// <summary>
    /// Matches the JSON response from POST http://localhost:5050/extract
    /// </summary>
    public class PdfExtractResult
    {
        public string Usn { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;

        /// <summary>One entry per subject row found in the PDF.</summary>
        public List<PdfSubjectRow> Subjects { get; set; } = new();

        // Flask puts this field when extraction partially failed
        public string? Error { get; set; }
    }

    /// <summary>
    /// One row from the PDF marks table.
    /// Note: field names are camelCase here because System.Text.Json
    ///       by default deserialises camelCase JSON → PascalCase C# properties.
    /// </summary>
    public class PdfSubjectRow
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int InternalMarks { get; set; }
        public int ExternalMarks { get; set; }
        public int Total { get; set; }   // reference only — we recompute
        public string Result { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}

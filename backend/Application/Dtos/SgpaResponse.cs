namespace SGPA_CALCULATOR.Application.Dtos
{
    public class SgpaResponse
    {
        public string StudentName { get; set; } = string.Empty;
        public string Usn { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string Scheme { get; set; } = "22";
        public double Sgpa { get; set; }
        public int TotalCredits { get; set; }
        public List<SubjectResultDto> Subjects { get; set; } = new();
        public List<string> UnresolvedCodes { get; set; } = new();
        public bool HasWarnings { get; set; }
    }

    public class SubjectResultDto
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int InternalMarks { get; set; }
        public int ExternalMarks { get; set; }
        public int TotalMarks { get; set; }
        public int Credits { get; set; }
        public double GradePoints { get; set; }
        public double CreditPoints { get; set; }
        public string Grade { get; set; } = string.Empty;
        public bool IsPass { get; set; }
        public bool IsNonCreditForSgpa { get; set; }
        public string ResolutionMethod { get; set; } = string.Empty; // for debugging

        public bool IsIncludedInSgpa { get; set; }= true;

        public bool  IsUnresolved { get; set; }=false;
    }
}

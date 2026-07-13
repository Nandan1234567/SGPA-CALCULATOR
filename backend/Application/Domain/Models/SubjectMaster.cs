// Domain/Models/SubjectMaster.cs


    public class SubjectMaster
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string SubjectType { get; set; } = string.Empty;
        public int Semester { get; set; }
        public bool IsNonCreditForSgpa { get; set; } = false;
    }

// Domain/Models/SubjectMaster.cs


// this subject master created  for the db properites to sert

    public class SubjectMaster
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string SubjectType { get; set; } = string.Empty;
        public int Semester { get; set; }
        public bool IsNonCreditForSgpa { get; set; } = false;
    }

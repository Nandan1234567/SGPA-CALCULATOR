

namespace SGPA_CALCULATOR.Domain.Models
{
    public class SgpaResult
    {

        // here we need to use the constructor to initlize or what the best prctices and to use

        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public string USN { get; set; } = string.Empty;

        public int Semester { get; set; }

        public string Branch { get; set; } = string.Empty;


        // it slected based on the subject code if subject code is cs = computer scinece 
        // based on the branch and semester so we get the subject code related crdit points // how to do this?

        // and how effective use the oop concepts so secure and  used in industry 
        public List<SubjectResult> SubjectResults { get; set; } = new List<SubjectResult>();


        // here in this total crdits select subjectResults.sum those (in those suming  credits
        public int TotalCredits => SubjectResults.Sum(s => s.Credits);




        public SgpaResult(string studentName, string usn, int semester, string branch, List<SubjectResult> subjectResults)
        {
            StudentName = studentName;
            USN = usn;
            Semester = semester;
            Branch = branch;
            SubjectResults = subjectResults;
        }



        // total crdit points divide by   totalCredits points = sgpa

        public double SGPA
        {
            get
            {

                if (TotalCredits == 0) return 0; // calculated based on grade points and credits, eg A=10*3=30, B=8*3=24, C=6*3=18, D=4*3=12, F=0*3=0

                double TotalCreditPoints = SubjectResults.Sum(s => s.CreditPoints);

                return Math.Round((double) TotalCreditPoints / TotalCredits , 2);
            }
        }


        // bonus — useful for frontend to show backlog warning
        //public bool HasBacklog =>
        //    SubjectResults.Any(s => s.GradePoints == 0);
    }
}

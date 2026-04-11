namespace SGPA_CALCULATOR.Domain.Models
{
    public class SubjectResult
    {

        public int  Id { get; set; }    

        public string SubjectCode { get; set; } = string.Empty; 

        public string SubjectName { get; set; }  = string.Empty;

        public int InternalMarks { get; set; }  
        public int ExternalMarks { get; set; }  


        public int TotalMarks { get; set; }

        // based on total marks grade points will be added ,  where to use this ? i need to get know 

        //grade ponts and crdit points here or in sgpa domain model , 
        public int GradePoints { get; set; } // calculated based on total marks, eg A=10, B=8, C=6, D=4, F=0

        public string Grade { get; set; } = string.Empty; // calculated based on total marks, eg A, B, C, D, F
        public int Credits { get; set; } = 0;  // eg 3, 4,2 // i feel like credit is not here // as soon as subject code is assigned it  need to get credits    
     //   public int CreditPoints { get; set; } // calculated based on grade points and credits, eg A=10*3=30, B=8*3=24, C=6*3=18, D=4*3=12, F=0*3=0


        // where exactly use this credit points , how pdf upload calculate what subject need to do here

    }
}

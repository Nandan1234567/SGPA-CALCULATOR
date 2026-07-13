namespace SGPA_CALCULATOR.Middelware
{
    public class ApiErrorResponse
    {

        public string Error { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public string  RequestId { get; set; } = string.Empty ;

        public DateTime TimeStamp { get; set; }

        //public static implicit operator ApiErrorResponse(ApiErrorResponse v)
        //{
        //    throw new NotImplementedException();
        //}


        //  do i  need to create the constructor? here ? and my given name with property are correct? what else? 
    }
}

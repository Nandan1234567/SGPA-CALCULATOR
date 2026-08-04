namespace SGPA_CALCULATOR.Middleware
{
    public class ApiErrorResponse
    {

        public string Error { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public string  RequestId { get; set; } = string.Empty ;

        public DateTime TimeStamp { get; set; }
        
    }
}

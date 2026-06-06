// FILE: Application/Exceptions/PdfValidationException.cs  ← CREATE THIS FILE

namespace SGPA_CALCULATOR.Application.Exceptions
{
    // This exception means: "the user uploaded something we can't process"
    // It's the user's fault, not a server failure
    // Maps to HTTP 422 Unprocessable Entity
    public class PdfValidationException : Exception
    {
        public PdfValidationException(string message) : base(message) { }
    }
}
using SGPA_CALCULATOR.Application.Dtos;

namespace SGPA_CALCULATOR.Application.Interface
{
    public interface ISgpaService
    {
        // its like we are creating a service next with the calcuate the output should be sgpa response and input should be in format of sgpa request
        SgpaResponse Calculate(SgpaRequest request);



    }
}

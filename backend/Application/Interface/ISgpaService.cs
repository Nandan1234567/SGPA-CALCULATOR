using SGPA_CALCULATOR.Application.Dtos;

namespace SGPA_CALCULATOR.Application.Interface
{
    public interface ISgpaService
    {
       
        SgpaResponse Calculate(SgpaRequest request);



    }
}

using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetProvidersRequest: IGetApiRequest
    {
        public string GetUrl => "providers";
    }
}
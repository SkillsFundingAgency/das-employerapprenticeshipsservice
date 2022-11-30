using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Providers
{
    public class GetProvidersRequest: IGetApiRequest
    {
        public string GetUrl => "providers";
    }
}
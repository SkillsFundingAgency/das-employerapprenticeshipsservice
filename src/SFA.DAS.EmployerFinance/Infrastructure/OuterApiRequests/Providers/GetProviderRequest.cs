using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Providers
{
    public class GetProviderRequest : IGetApiRequest
    {
        private readonly long _id;

        public GetProviderRequest (long id)
        {
            _id = id;
        }

        public string GetUrl => $"providers/{_id}";
    }
}
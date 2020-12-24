using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
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
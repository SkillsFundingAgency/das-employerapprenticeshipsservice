using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetProviderRequest : IGetApiRequest
    {
        private readonly int _id;

        public GetProviderRequest (int id)
        {
            _id = id;
        }
        public string GetUrl => $"providers/{_id}";
    }
}
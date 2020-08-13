using SFA.DAS.EmployerFinance.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ClientContentService : IClientContentService
    {
        private readonly IClientContentApiClient _client;

        public ClientContentService(IClientContentApiClient client)
        {
            _client = client;
        }

        public async Task<string> Get(string type, string applicationId)
        {
            var cachedContent = await _client.Get(type, applicationId);

            return cachedContent;
        }
    }
}

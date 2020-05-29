using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Services
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
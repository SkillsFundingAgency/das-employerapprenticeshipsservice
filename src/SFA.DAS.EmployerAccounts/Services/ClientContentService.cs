using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetClientContent;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ClientContentService : IClientContentService
    {
        private readonly IClientContentApiClient _client;
        
        public ClientContentService(IClientContentApiClient client)
        {
            _client = client;
        }

        public async Task<string> GetContentByClientId(ContentType type, string clientId)
        {
            var cachedContent = await _client.GetContentByClientId(type, clientId);

            return cachedContent;
        }
    }
}
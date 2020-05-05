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

        public async Task<string> GetContent(string type, string clientId)
        {
            var cachedContent = await _client.GetContent(type, clientId);

            return cachedContent;
        }
    }
}
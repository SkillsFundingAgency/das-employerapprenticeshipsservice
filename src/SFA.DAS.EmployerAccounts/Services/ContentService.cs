using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ContentService : IContentService
    {
        private readonly IContentApiClient _client;
        
        public ContentService(IContentApiClient client)
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
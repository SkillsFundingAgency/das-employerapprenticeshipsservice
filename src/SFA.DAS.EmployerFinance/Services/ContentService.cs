using SFA.DAS.EmployerFinance.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
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
            var content = await _client.Get(type, applicationId);

            return content;
        }
    }
}

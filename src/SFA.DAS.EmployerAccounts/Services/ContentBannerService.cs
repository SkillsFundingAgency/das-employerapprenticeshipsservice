using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ContentBannerService : IContentBannerService
    {
       private readonly IContentBannerApiClient _client;

        public ContentBannerService(IContentBannerApiClient client)
        {
            _client = client;
        }

        public async Task<string> GetBannerContent(int bannerId, bool useCDN)
        {
            return await _client.GetBanner(bannerId, useCDN);
        }
    }
}
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ClientContentApiClient : ApiClientBase, IClientContentApiClient
    {
        private readonly string ApiBaseUrl;

        public ClientContentApiClient(HttpClient client, IContentClientApiConfiguration configuration) : base(client)
        {
            ApiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
                ? configuration.ApiBaseUrl
                : configuration.ApiBaseUrl + "/";
        }

        public Task<string> Get(string type, string applicationId)
        {
            string banner = string.Empty;
            switch (type)
            {
                case "banner":
                    var uri = $"{ApiBaseUrl}api/content?applicationId={applicationId}&type={type}";
                    banner = GetAsync(uri).Result;
                    break;
            }
            return Task.FromResult(banner);
        }
    }
}
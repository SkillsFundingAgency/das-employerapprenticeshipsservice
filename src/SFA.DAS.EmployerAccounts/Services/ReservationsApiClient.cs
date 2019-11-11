using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ReservationsApiClient : ApiClientBase, IReservationsApiClient
    {
        private readonly string ApiBaseUrl;

        public ReservationsApiClient(HttpClient client, IReservationsClientApiConfiguration configuration) : base(client)
        {
            ApiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
                ? configuration.ApiBaseUrl
                : configuration.ApiBaseUrl + "/";
        }

        public Task<string> Get(long accountId)
        {
            var uri = $"{ApiBaseUrl}accounts/{accountId}/reservations";
            return GetAsync(uri);
        }
    }
}
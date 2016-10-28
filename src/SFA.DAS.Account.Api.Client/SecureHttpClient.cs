using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Account.Api.Client
{
    internal class SecureHttpClient
    {
        private readonly string _clientToken;

        public SecureHttpClient(string clientToken)
        {
            _clientToken = clientToken;
        }
        protected SecureHttpClient()
        {
            // So we can mock for testing
        }

        public virtual async Task<string> GetAsync(string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _clientToken);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}

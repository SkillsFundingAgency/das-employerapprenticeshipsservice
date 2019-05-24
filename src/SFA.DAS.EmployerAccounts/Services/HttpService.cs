using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class HttpService : IHttpService
    {
        private readonly string _clientId;     
        private readonly string _clientSecret;
        private readonly string _identifierUri;
        private readonly string _tenant;

        public HttpService(string clientId, string clientSecret, string identifierUri, string tenant)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _identifierUri = identifierUri;
            _tenant = tenant;
        }
       
        private async Task<AuthenticationResult> GetAuthenticationResult(string clientId, string clientSecret, string identifierUri, string tenant)
        {
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var clientCredential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(identifierUri, clientCredential);
            return result;
        }

        public virtual Task<string> GetAsync(string url, bool exceptionOnNotFound = true)
        {
            return GetAsync(url, response => exceptionOnNotFound || response.StatusCode != HttpStatusCode.NotFound);
        }
      
        public virtual async Task<string> GetAsync(string url, Func<HttpResponseMessage, bool> responseChecker)
        {
            var authenticationResult = await GetAuthenticationResult(
                _clientId,
                _clientSecret,
                _identifierUri, 
                _tenant);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                var response = await client.GetAsync(url);

                if (responseChecker != null)
                {
                    if (!responseChecker(response))
                    {
                        return null;
                    }
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}

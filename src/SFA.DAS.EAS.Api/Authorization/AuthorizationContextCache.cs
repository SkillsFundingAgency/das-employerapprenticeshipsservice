using System.Net.Http;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Infrastructure.Authorization;

namespace SFA.DAS.EAS.Account.Api.Authorization
{
    public class AuthorizationContextCache : IAuthorizationContextCache
    {
        private static readonly string Key = typeof(AuthorizationContext).FullName;

        private readonly HttpRequestMessage _httpRequestMessage;

        public AuthorizationContextCache(HttpRequestMessage httpRequestMessage)
        {
            _httpRequestMessage = httpRequestMessage;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            if (_httpRequestMessage.Properties.ContainsKey(Key))
            {
                return _httpRequestMessage.Properties[Key] as AuthorizationContext;
            }

            return null;
        }

        public void SetAuthorizationContext(IAuthorizationContext authorizationContext)
        {
            _httpRequestMessage.Properties[Key] = authorizationContext;
        }
    }
}
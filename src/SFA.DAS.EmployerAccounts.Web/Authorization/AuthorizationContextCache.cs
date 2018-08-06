using SFA.DAS.EmployerAccounts.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class AuthorizationContextCache : IAuthorizationContextCache
    {
        private static readonly string Key = typeof(AuthorizationContext).FullName;

        private readonly HttpContextBase _httpContext;

        public AuthorizationContextCache(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            if (_httpContext.Items.Contains(Key))
            {
                return _httpContext.Items[Key] as AuthorizationContext;
            }

            return null;
        }

        public void SetAuthorizationContext(IAuthorizationContext authorizationContext)
        {
            _httpContext.Items[Key] = authorizationContext;
        }
    }
}

using System;
using System.Web;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.EmployerAccounts.Data;
using System.Linq;
using SFA.DAS.EmployerAccounts.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Routing;


namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class ImpersonationAuthorizationContext : IAuthorizationContextProvider
    {
        private readonly HttpContextBase _httpContext;
        private readonly IAuthorizationContextProvider _authorizationContextProvider;
        private readonly IEmployerAccountTeamRepository _employerAccountTeamRepository;

        public ImpersonationAuthorizationContext(HttpContextBase httpContext,
            IAuthorizationContextProvider authorizationContextProvider,
            IEmployerAccountTeamRepository employerAccountTeamRepository)
        {
            _httpContext = httpContext;
            _authorizationContextProvider = authorizationContextProvider;
            _employerAccountTeamRepository = employerAccountTeamRepository;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            if (!_httpContext.User.IsInRole(AuthorizationConstants.Tier2User))
                return _authorizationContextProvider.GetAuthorizationContext();

            
            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(RouteValueKeys.AccountHashedId, out var accountHashedId))
            {
                throw new UnauthorizedAccessException();
            }

            var teamMembers = Task.Run(() => _employerAccountTeamRepository.GetAccountTeamMembers(accountHashedId?.ToString())).Result;
            var accountOwner = teamMembers.First(tm => tm.Role == Role.Owner);
            var claimsIdentity = (ClaimsIdentity)_httpContext.User.Identity;
            claimsIdentity.AddClaim(new Claim("sub", accountOwner.UserRef));
            claimsIdentity.AddClaim(new Claim(DasClaimTypes.Id, accountOwner.UserRef));
            claimsIdentity.AddClaim(new Claim(DasClaimTypes.Email, accountOwner.Email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, accountOwner.UserRef)); // to validate antiforgery

            var authorizationContext = _authorizationContextProvider.GetAuthorizationContext();
            authorizationContext.Set("ClaimsIdentity", claimsIdentity);
            var route = _httpContext.Request.RequestContext.RouteData.Route as Route;
            var resource = new Resource { Value = route?.Url };
            authorizationContext.Set("Resource", resource);   
            return authorizationContext;
        }

        public class Resource
        {
            public string Value { get; set; }
        }

    }
}
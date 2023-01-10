using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerUsers.WebClientComponents;


namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public class ImpersonationAuthorizationContext : IAuthorizationContextProvider
    {
        private readonly IAuthorizationContextProvider _authorizationContextProvider;
        private readonly IEmployerAccountTeamRepository _employerAccountTeamRepository;
        private readonly IUserContext _userContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public ImpersonationAuthorizationContext(
            IAuthorizationContextProvider authorizationContextProvider,
            IEmployerAccountTeamRepository employerAccountTeamRepository, 
            IUserContext userContext,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            _authorizationContextProvider = authorizationContextProvider;
            _employerAccountTeamRepository = employerAccountTeamRepository;
            _userContext = userContext;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            if (!_userContext.IsSupportConsoleUser())
                return _authorizationContextProvider.GetAuthorizationContext();
            
            if (!_actionContextAccessor.ActionContext.RouteData.Values.TryGetValue(RouteValueKeys.AccountHashedId, out var accountHashedId))
            {
                throw new UnauthorizedAccessException();
            }

            var teamMembers = Task.Run(() => _employerAccountTeamRepository.GetAccountTeamMembers(accountHashedId?.ToString())).Result;
            var accountOwner = teamMembers.First(tm => tm.Role == Role.Owner);
            var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext?.User.Identity;
            claimsIdentity.AddClaim(new Claim("sub", accountOwner.UserRef.ToString()));
            claimsIdentity.AddClaim(new Claim(RouteValueKeys.AccountHashedId, accountHashedId?.ToString()));
            claimsIdentity.AddClaim(new Claim(DasClaimTypes.Id, accountOwner.UserRef.ToString()));
            claimsIdentity.AddClaim(new Claim(DasClaimTypes.Email, accountOwner.Email));

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
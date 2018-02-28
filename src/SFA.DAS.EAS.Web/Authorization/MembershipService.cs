using System;
using System.Linq;
using System.Web;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Web.Authorization
{
    public class MembershipService : IMembershipService
    {
        private static readonly string Key = typeof(MembershipContext).FullName;

        private readonly EmployerAccountDbContext _db;
        private readonly HttpContextBase _httpContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IHashingService _hashingService;

        public MembershipService(EmployerAccountDbContext db, HttpContextBase httpContext, IAuthenticationService authenticationService, IConfigurationProvider configurationProvider, IHashingService hashingService)
        {
            _db = db;
            _httpContext = httpContext;
            _authenticationService = authenticationService;
            _configurationProvider = configurationProvider;
            _hashingService = hashingService;
        }

        public IMembershipContext GetMembershipContext()
        {
            if (_httpContext.Items.Contains(Key))
            {
                return _httpContext.Items[Key] as MembershipContext;
            }

            if (!_authenticationService.IsUserAuthenticated())
            {
                return null;
            }

            if (!_authenticationService.TryGetClaimValue(ControllerConstants.UserExternalIdClaimKeyName, out var userExternalIdClaimValue))
            {
                return null;
            }

            if (!Guid.TryParse(userExternalIdClaimValue, out var userExternalId))
            {
                return null;
            }

            if (!_httpContext.Request.RequestContext.RouteData.Values.TryGetValue(ControllerConstants.AccountHashedIdRouteKeyName, out var accountHashedId))
            {
                return null;
            }

            if (!_hashingService.TryDecodeValue(accountHashedId.ToString(), out var accountId))
            {
                return null;
            }

            var membershipContext = _db.Memberships
                .Where(m => m.Account.Id == accountId && m.User.ExternalId == userExternalId)
                .ProjectTo<MembershipContext>(_configurationProvider)
                .SingleOrDefault();

            _httpContext.Items[Key] = membershipContext;

            return membershipContext;
        }

        public void ValidateMembership()
        {
            var membership = GetMembershipContext();

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
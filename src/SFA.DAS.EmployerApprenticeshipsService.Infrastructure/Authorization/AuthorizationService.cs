using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.EAS.Infrastructure.Features;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
	public class AuthorizationService : IAuthorizationService
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IAuthorizationContextCache _authorizationContextCache;
        private readonly IEnumerable<IAuthorizationHandler> _handlers;
        private readonly ICallerContextProvider _callerContextProvider;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFeatureService _featureService;

        public AuthorizationService(
            EmployerAccountDbContext db,
            IAuthorizationContextCache authorizationContextCache,
            IEnumerable<IAuthorizationHandler> handlers,
            ICallerContextProvider callerContextProvider,
            IConfigurationProvider configurationProvider,
            IFeatureService featureService)
        {
            _db = db;
            _authorizationContextCache = authorizationContextCache;
            _handlers = handlers;
            _callerContextProvider = callerContextProvider;
            _configurationProvider = configurationProvider;
            _featureService = featureService;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            return GetAuthorizationContextAsync().GetAwaiter().GetResult();
        }

        public async Task<IAuthorizationContext> GetAuthorizationContextAsync()
        {
            IAuthorizationContext cachedAuthorizationContext;

            if ((cachedAuthorizationContext = _authorizationContextCache.GetAuthorizationContext()) != null)
            {
                return cachedAuthorizationContext;
            }

            var callerContext = _callerContextProvider.GetCallerContext();

            var accountContextQuery = callerContext.AccountId == null ? null : _db.Accounts
                .Where(a => a.Id == callerContext.AccountId.Value)
                .ProjectTo<AccountContext>(_configurationProvider)
                .Future();

            var userContextQuery = callerContext.UserExternalId == null ? null : _db.Users
                .Where(u => u.ExternalId == callerContext.UserExternalId.Value)
                .ProjectTo<UserContext>(_configurationProvider)
                .Future();

            var membershipContextQuery = callerContext.AccountId == null || callerContext.UserExternalId == null ? null : _db.Memberships
                .Where(m => m.Account.Id == callerContext.AccountId.Value && m.User.ExternalId == callerContext.UserExternalId.Value)
                .ProjectTo<MembershipContext>(_configurationProvider)
                .Future();

            var authorizationContext = new AuthorizationContext();

            if (accountContextQuery != null)
            {
                authorizationContext.AccountContext = await accountContextQuery.SingleOrDefaultAsync().ConfigureAwait(false) ?? throw new UnauthorizedAccessException();
            }

            if (userContextQuery != null)
            {
                authorizationContext.UserContext = await userContextQuery.SingleOrDefaultAsync() ?? throw new UnauthorizedAccessException();
            }

            if (membershipContextQuery != null)
            {
                authorizationContext.MembershipContext = await membershipContextQuery.SingleOrDefaultAsync() ?? throw new UnauthorizedAccessException();
            }

            _authorizationContextCache.SetAuthorizationContext(authorizationContext);

            return authorizationContext;
        }

        public AuthorizationResult GetAuthorizationResult(FeatureType featureType)
        {
            return GetAuthorizationResultAsync(featureType).GetAwaiter().GetResult();
        }

        public async Task<AuthorizationResult> GetAuthorizationResultAsync(FeatureType featureType)
        {
            var authorisationContext = await GetAuthorizationContextAsync().ConfigureAwait(false);
            var feature = _featureService.GetFeature(featureType);
            var authorizationResults = await Task.WhenAll(_handlers.Select(h => h.CanAccessAsync(authorisationContext, feature))).ConfigureAwait(false);
            var authorizationResult = authorizationResults.FirstOrDefault(r => r != AuthorizationResult.Ok);

            return authorizationResult;
        }

        public bool IsAuthorized(FeatureType featureType)
        {
            return IsAuthorizedAsync(featureType).GetAwaiter().GetResult();
        }

        public async Task<bool> IsAuthorizedAsync(FeatureType featureType)
        {
            var authorizationResult = await GetAuthorizationResultAsync(featureType).ConfigureAwait(false);
            var isAuthorized = authorizationResult == AuthorizationResult.Ok;

            return isAuthorized;
        }

        public void ValidateMembership()
        {
            var authorizationContext = GetAuthorizationContext();

            if (authorizationContext.MembershipContext == null)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
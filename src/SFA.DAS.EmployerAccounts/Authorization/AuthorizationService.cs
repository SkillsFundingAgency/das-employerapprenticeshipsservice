using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Features;
using SFA.DAS.EmployerAccounts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Models.Features;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthorizationContextCache _authorizationContextCache;
        private readonly IEnumerable<IAuthorizationHandler> _handlers;
        private readonly ICallerContextProvider _callerContextProvider;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFeatureService _featureService;
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public AuthorizationService(
            IAuthorizationContextCache authorizationContextCache,
            IEnumerable<IAuthorizationHandler> handlers,
            ICallerContextProvider callerContextProvider,
            IConfigurationProvider configurationProvider,
            IFeatureService featureService,
            Lazy<EmployerAccountsDbContext> db)
        {
            _authorizationContextCache = authorizationContextCache;
            _handlers = handlers;
            _callerContextProvider = callerContextProvider;
            _configurationProvider = configurationProvider;
            _featureService = featureService;
            _db = db;
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

            var accountContextQuery = callerContext.AccountId == null ? null : _db.Value.Accounts
                .Where(a => a.Id == callerContext.AccountId.Value)
                .ProjectTo<AccountContext>(_configurationProvider)
                .Future();

            var userContextQuery = callerContext.UserRef == null ? null : _db.Value.Users
                .Where(u => u.Ref == callerContext.UserRef.Value)
                .ProjectTo<UserContext>(_configurationProvider)
                .Future();

            var membershipContextQuery = callerContext.AccountId == null || callerContext.UserRef == null ? null : _db.Value.Memberships
                .Where(m => m.Account.Id == callerContext.AccountId.Value && m.User.Ref == callerContext.UserRef.Value)
                .ProjectTo<MembershipContext>(_configurationProvider)
                .Future();

            var authorizationContext = new AuthorizationContext();

            if (accountContextQuery != null)
            {
                authorizationContext.AccountContext = await accountContextQuery.SingleOrDefaultAsync().ConfigureAwait(false) ?? throw new UnauthorizedAccessException();
            }

            if (userContextQuery != null)
            {
                authorizationContext.UserContext = await userContextQuery.SingleOrDefaultAsync().ConfigureAwait(false) ?? throw new UnauthorizedAccessException();
            }

            if (membershipContextQuery != null)
            {
                authorizationContext.MembershipContext = await membershipContextQuery.SingleOrDefaultAsync().ConfigureAwait(false) ?? throw new UnauthorizedAccessException();
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

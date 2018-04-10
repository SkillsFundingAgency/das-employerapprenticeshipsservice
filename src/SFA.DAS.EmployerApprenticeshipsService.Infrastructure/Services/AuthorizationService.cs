using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EAS.Infrastructure.Services
{
	public class AuthorizationService : IAuthorizationService
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IAuthorizationContextCache _authorizationContextCache;
        private readonly IAuthorizationHandler[] _handlers;
        private readonly ICallerContextProvider _callerContextProvider;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFeatureService _featureService;

        public AuthorizationService(
            EmployerAccountDbContext db,
            IAuthorizationContextCache authorizationContextCache,
            IAuthorizationHandler[] handlers,
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

            var accountContext = accountContextQuery?.SingleOrDefault();
            var userContext = userContextQuery?.SingleOrDefault();
            var membershipContext = membershipContextQuery?.SingleOrDefault();
            var featureTask = _featureService.GetFeatureThatAllowsAccessToOperationAsync(callerContext.ControllerName, callerContext.ActionName);

            if (!featureTask.Wait(60 * 1000))
            {
                throw new Exception("Time out waiting for feature definition");
            }

            var authorizationContext = new AuthorizationContext
            {
                AccountContext = accountContext,
                CurrentFeature = featureTask.Result,
                UserContext = userContext,
                MembershipContext = membershipContext
            };

            _authorizationContextCache.SetAuthorizationContext(authorizationContext);

            return authorizationContext;
        }

        public bool IsOperationAuthorized()
        {
            var authorisationContext = GetAuthorizationContext();

            if (authorisationContext.CurrentFeature == null)
            {
                return true;
            }

            var isOperationAuthorised = _handlers.All(h =>
            {
                var canAccess = Task.Run(async () => await h.CanAccessAsync(authorisationContext)).Result;
                return canAccess;
            });

            return isOperationAuthorised;
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
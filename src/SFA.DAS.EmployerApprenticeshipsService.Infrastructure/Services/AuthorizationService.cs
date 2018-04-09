using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.NLog.Logger;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EAS.Infrastructure.Services
{
	public class AuthorizationService : IAuthorizationService
    {

        private readonly EmployerAccountDbContext _db;

        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFeatureService _featureService;
        private readonly ILog _logger;
        private readonly IOperationAuthorisationHandler[] _pipeSections;
        private readonly ICallerContext _callerContext;

        public AuthorizationService(
            EmployerAccountDbContext db,
            IConfigurationProvider configurationProvider, 
            ICallerContext callerContext,
            IFeatureService featureService, 
            ILog logger,
            IOperationAuthorisationHandler[] pipeSections)
        {
            _db = db;
            _configurationProvider = configurationProvider;
            _featureService = featureService;
            _logger = logger;
            _pipeSections = pipeSections;
            _callerContext = callerContext;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            AuthorizationContext existingContext = null;
            if ((existingContext = _callerContext.GetAuthorizationContext()) != null)
            {
                return existingContext;
            }

            var accountId = _callerContext.GetAccountId();
            var userExternalId = _callerContext.GetUserExternalId();

            var accountContextQuery = accountId == null ? null : _db.Accounts
                .Where(a => a.Id == accountId.Value)
                .ProjectTo<AccountContext>(_configurationProvider)
                .Future();

            var userContextQuery = userExternalId == null ? null : _db.Users
                .Where(u => u.ExternalId == userExternalId)
                .ProjectTo<UserContext>(_configurationProvider)
                .Future();

            var membershipContextQuery = accountId == null || userExternalId == null ? null : _db.Memberships
                .Where(m => m.Account.Id == accountId && m.User.ExternalId == userExternalId)
                .ProjectTo<MembershipContext>(_configurationProvider)
                .Future();

            var accountContext = accountContextQuery?.SingleOrDefault();
            var userContext = userContextQuery?.SingleOrDefault();
            var membershipContext = membershipContextQuery?.SingleOrDefault();

            var featureTask = _featureService.GetFeatureThatAllowsAccessToOperationAsync(_callerContext.GetControllerName(), _callerContext.GetOperationName());

            if (!featureTask.Wait(60 * 1000))
            {
                throw new Exception("Time out waiting for feature definition");
            }

            var authorizationContext = new AuthorizationContext
            {
                AccountContext = accountContext,
                UserContext = userContext,
                MembershipContext = membershipContext,
                CurrentFeature = featureTask.Result
            };

            _callerContext.SetAuthorizationContext(authorizationContext);
           

            return authorizationContext;
        }

        public void ValidateMembership()
        {
            var authorizationContext = GetAuthorizationContext();

            if (authorizationContext?.MembershipContext == null)
            {
                throw new UnauthorizedAccessException();
            }
        }


        public bool IsOperationAuthorised(IAuthorizationContext authorisationContext)
        {
            if (authorisationContext.CurrentFeature == null)
            {
                return true;
            }

            var allowedByAllHandlers = _pipeSections.All(handler =>
            {
                var handlerTask = Task.Run(() => handler.CanAccessAsync(authorisationContext)).ConfigureAwait(false);
                return !handlerTask.GetAwaiter().GetResult();
            });

            return allowedByAllHandlers;
        }

        public async Task<bool> CanAccessAsync(IAuthorizationContext authorisationContext)
        {
            if (authorisationContext == null)
            {
                return false;
            }

            try
            {
                foreach (var handler in _pipeSections)
                {
                    if (await handler.CanAccessAsync(authorisationContext) == false)
                    {
                        _logger.Info($"context {authorisationContext.AccountContext?.Id} has been blocked from {authorisationContext.CurrentFeature.FeatureType} by {handler.GetType().Name}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error has occurred when processing a feature toggle pipeline context.");
                throw;
            }
        }
    }
}
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class CommitmentsBaseOrchestrator
    {
        private readonly IMediator _mediator;

        private readonly IHashingService _hashingService;

        private readonly ILogger _logger;

        public CommitmentsBaseOrchestrator(
            IMediator mediator, 
            IHashingService hashingService,
            ILogger logger)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<bool> AuthorizeRole(string hashedAccountId, string externalUserId, Role[] roles)
        {
            var response = await _mediator.SendAsync(new GetUserAccountRoleQuery
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId
            });
            return roles.Contains(response.UserRole);
        }

        protected async Task<OrchestratorResponse<T>> CheckUserAuthorization<T>(Func<Task<OrchestratorResponse<T>>> code, string hashedAccountId, string externalUserId) where T : class
        {
            try
            {
                var response = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
                {
                    HashedAccountId = hashedAccountId,
                    UserId = externalUserId
                });

                return await code.Invoke();
            }
            catch (UnauthorizedAccessException exception)
            {
                LogUnauthorizedUserAttempt(hashedAccountId, externalUserId);

                return new OrchestratorResponse<T>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = exception
                };
            }
        }

        protected async Task<OrchestratorResponse<T>> CheckUserAuthorization<T>(Func<OrchestratorResponse<T>> code, string hashedAccountId, string externalUserId) where T : class
        {
            try
            {
                var response = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
                {
                    HashedAccountId = hashedAccountId,
                    UserId = externalUserId
                });

                return code.Invoke();
            }
            catch (UnauthorizedAccessException exception)
            {
                LogUnauthorizedUserAttempt(hashedAccountId, externalUserId);

                return new OrchestratorResponse<T>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = exception
                };
            }
        }

        protected async Task CheckUserAuthorization(Func<Task> code, string hashedAccountId, string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
                {
                    HashedAccountId = hashedAccountId,
                    UserId = externalUserId
                });

                await code.Invoke();
            }
            catch (UnauthorizedAccessException)
            {
                LogUnauthorizedUserAttempt(hashedAccountId, externalUserId);
            }
        }

        private void LogUnauthorizedUserAttempt(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Warn($"User not associated to account. UserId:{externalUserId} AccountId:{accountId}");
        }
    }
}
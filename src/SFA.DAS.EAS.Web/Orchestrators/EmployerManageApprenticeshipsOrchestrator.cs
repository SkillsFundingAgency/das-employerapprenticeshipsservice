using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using MediatR;

using NLog;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetAllApprenticeships;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Models.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public sealed class EmployerManageApprenticeshipsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;

        private readonly ILogger _logger;

        public EmployerManageApprenticeshipsOrchestrator(
            IMediator mediator, 
            IHashingService hashingService,
            ILogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<OrchestratorResponse<ManageApprenticeshipsViewModel>> GetApprenticeships(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting On-programme apprenticeships for empployer: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                    var data = await _mediator.SendAsync(new GetAllApprenticeshipsRequest { AccountId = accountId });

                    var apprenticeships = data.Apprenticeships
                        .OrderBy(m => m.ApprenticeshipName)
                        .Select(MapFrom)
                        .ToList();

                    var model = new ManageApprenticeshipsViewModel
                                    {
                                        HashedAccountId = hashedAccountId,
                                        Apprenticeships = apprenticeships
                                    };

                return new OrchestratorResponse<ManageApprenticeshipsViewModel> { Data = model };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ApprenticeshipDetailsViewModel>> GetApprenticeship(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Getting On-programme apprenticeships Provider: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
                {
                    var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    return new OrchestratorResponse<ApprenticeshipDetailsViewModel>
                               {
                                   Data = MapFrom(data.Apprenticeship)
                               };
                }, hashedAccountId, externalUserId);
        }

        private ApprenticeshipDetailsViewModel MapFrom(Apprenticeship apprenticeship)
        {
            return new ApprenticeshipDetailsViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                DateOfBirth = apprenticeship.DateOfBirth,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                Status = MapPaymentStatus(apprenticeship.PaymentStatus),
                ProviderName = string.Empty
            };
        }

        private async Task<OrchestratorResponse<T>> CheckUserAuthorization<T>(Func<Task<OrchestratorResponse<T>>> code, string hashedAccountId, string externalUserId) where T : class
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

        private string MapPaymentStatus(PaymentStatus paymentStatus)
        {
            switch (paymentStatus)
            {
                case PaymentStatus.PendingApproval:
                    return "Approval needed";
                case PaymentStatus.Active:
                    return "On programme";
                case PaymentStatus.Paused:
                    return "Paused";
                case PaymentStatus.Withdrawn:
                    return "Stopped";
                case PaymentStatus.Completed:
                    return "Completed";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }

        private void LogUnauthorizedUserAttempt(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Warn($"User not associated to account. UserId:{externalUserId} AccountId:{accountId}");
        }
    }
}


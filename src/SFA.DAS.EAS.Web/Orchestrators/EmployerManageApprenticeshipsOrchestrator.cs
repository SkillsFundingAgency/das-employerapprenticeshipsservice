using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using NLog;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetAllApprenticeships;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Application.Queries.GetProvider;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Models.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public sealed class EmployerManageApprenticeshipsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly ILogger _logger;
        private readonly ICommitmentStatusCalculator _statusCalculator;

        public EmployerManageApprenticeshipsOrchestrator(IMediator mediator, IHashingService hashingService, ICommitmentStatusCalculator statusCalculator, ILogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (statusCalculator == null)
                throw new ArgumentNullException(nameof(statusCalculator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _hashingService = hashingService;
            _statusCalculator = statusCalculator;
            _logger = logger;
        }

        public async Task<ManageApprenticeshipsViewModel> GetApprenticeships(string hashedaccountId, string getClaimValue)
        {
            var accountId = _hashingService.DecodeValue(hashedaccountId);
            var data = await _mediator.SendAsync(new GetAllApprenticeshipsRequest { AccountId = accountId });

            var apprenticeships = data.Apprenticeships.Select(MapFrom).ToList();

            var model = new ManageApprenticeshipsViewModel
                            {
                                HashedaccountId = hashedaccountId,
                                Apprenticeships = await Task.WhenAll(apprenticeships)
                            };

            return await Task.FromResult(model);
        }

        public async Task<ApprenticeshipDetailsViewModel> GetApprenticeship(string hashedaccountId, long apprenticeshipId)
        {
            var accountId = _hashingService.DecodeValue(hashedaccountId);
            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

            return await MapFrom(data.Apprenticeship);
        }

        private async Task<ApprenticeshipDetailsViewModel> MapFrom(Apprenticeship apprenticeship)
        {
            var data = await _mediator.SendAsync(new GetProviderQueryRequest
                                       {
                                           ProviderId = (int)apprenticeship.ProviderId
                                       } ) ;

            var result = data.ProvidersView.Providers.FirstOrDefault();
            return new ApprenticeshipDetailsViewModel
            {
                Id = apprenticeship.Id,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                DateOfBirth = apprenticeship.DateOfBirth,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                Status = MapPaymentStatus(apprenticeship.PaymentStatus),
                ProviderName = result?.ProviderName ?? string.Empty
            };
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
                case PaymentStatus.Cancelled:
                    return "Stopped";
                case PaymentStatus.Completed:
                    return "Completed";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }
    }
}


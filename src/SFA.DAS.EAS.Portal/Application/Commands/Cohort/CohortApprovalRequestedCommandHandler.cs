using SFA.DAS.Commitments.Api.Client.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Portal.Application.Commands.Cohort
{
    public class CohortApprovalRequestedCommandHandler : Command, ICommandHandler<CohortApprovalRequestedCommand>
    {
        private readonly IAccountDocumentService _accountsService;
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;
        private readonly IHashingService _hashingService;
        private readonly ILogger<CohortApprovalRequestedCommandHandler> _logger;

        public CohortApprovalRequestedCommandHandler(
            IAccountDocumentService accountsService, 
            IProviderCommitmentsApi providerCommitmentsApi,
            IHashingService hashingService,
            ILogger<CohortApprovalRequestedCommandHandler> logger)
        : base(accountsService)
        {
            _accountsService = accountsService;
            _providerCommitmentsApi = providerCommitmentsApi;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task Handle(CohortApprovalRequestedCommand command, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Executing {nameof(CohortApprovalRequestedCommandHandler)}");

            var accountDocument = await GetOrCreateAccountDocument(command.AccountId, cancellationToken);
            var commitment = await _providerCommitmentsApi.GetProviderCommitment(command.ProviderId, command.CommitmentId);            
            long accountLegalEntityId = _hashingService.DecodeValue(commitment.AccountLegalEntityPublicHashedId);
                      
            var account = accountDocument.Account;
            var cohortReference = commitment.Reference;
            var cohortId = commitment.Id;

            var organisation = account.Organisations.FirstOrDefault(o => o.AccountLegalEntityId.Equals(accountLegalEntityId));
            if (organisation == null)
            {
                organisation = new Organisation()
                {
                    AccountLegalEntityId = accountLegalEntityId,
                    Name = commitment.LegalEntityName
                };

                account.Organisations.Add(organisation);
            }

            var cohort = organisation.Cohorts.FirstOrDefault(c => c.Id != null && c.Id.Equals(cohortId.ToString(), StringComparison.OrdinalIgnoreCase));

            if (cohort == null)
            {
                cohort = new Client.Types.Cohort { Id = cohortId.ToString(), Reference = cohortReference };
                account.Organisations.First().Cohorts.Add(cohort);
            }
            
            commitment.Apprenticeships.ForEach(a =>
            {
                var apprenticeship = cohort.Apprenticeships.FirstOrDefault(ca => ca.Id == a.Id);

                if (apprenticeship == null)
                {
                    apprenticeship = new Apprenticeship { Id = a.Id};
                    cohort.Apprenticeships.Add(apprenticeship);
                }
                apprenticeship.FirstName = a.FirstName;
                apprenticeship.LastName  = a.LastName;
                apprenticeship.CourseName = a.TrainingName;
                apprenticeship.ProposedCost = a.Cost;
                apprenticeship.StartDate = a.StartDate;
                apprenticeship.EndDate = a.EndDate;
            });

            await _accountsService.Save(accountDocument, cancellationToken);
        }
    }
}

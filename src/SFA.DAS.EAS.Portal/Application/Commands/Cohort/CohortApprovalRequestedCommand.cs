using SFA.DAS.Commitments.Api.Client.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Portal.Application.Commands.Cohort
{
    public class CohortApprovalRequestedCommand : Command, ICommand<CohortApprovalRequestedByProvider>
    {
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;
        private readonly IHashingService _hashingService;
        private readonly ILogger<CohortApprovalRequestedCommand> _logger;

        public CohortApprovalRequestedCommand(
            IAccountDocumentService accountsService, 
            IProviderCommitmentsApi providerCommitmentsApi,
            IHashingService hashingService,
            ILogger<CohortApprovalRequestedCommand> logger)
        : base(accountsService)
        {
            _providerCommitmentsApi = providerCommitmentsApi;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task Execute(CohortApprovalRequestedByProvider cohortApprovalRequestedByProvider, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Executing {nameof(CohortApprovalRequestedCommand)}");

            var accountDocument = await GetOrCreateAccountDocument(cohortApprovalRequestedByProvider.AccountId, cancellationToken);
            var commitment = await _providerCommitmentsApi.GetProviderCommitment(cohortApprovalRequestedByProvider.ProviderId, cohortApprovalRequestedByProvider.CommitmentId);
            long accountLegalEntityId = _hashingService.DecodeValue(commitment.AccountLegalEntityPublicHashedId);
                      
            var account = accountDocument.Account;
            var cohortReference = commitment.Reference;
            var cohortId = commitment.Id;

            var (organisation, organisationCreation) = GetOrAddOrganisation(accountDocument, accountLegalEntityId);
            if (organisationCreation == EntityCreation.Created)
            {
                organisation.Name = commitment.LegalEntityName;
            }

            //todo: use GetOrAdd pattern for cohort
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

            await AccountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}

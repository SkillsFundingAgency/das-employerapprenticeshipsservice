using SFA.DAS.Commitments.Api.Client.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
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

            var accountDocumentTask = GetOrCreateAccountDocument(cohortApprovalRequestedByProvider.AccountId, cancellationToken);
            var commitment = await _providerCommitmentsApi.GetProviderCommitment(cohortApprovalRequestedByProvider.ProviderId, cohortApprovalRequestedByProvider.CommitmentId);
            long accountLegalEntityId = _hashingService.DecodeValue(commitment.AccountLegalEntityPublicHashedId);

            var accountDocument = await accountDocumentTask;
            var (organisation, organisationCreation) = GetOrAddOrganisation(accountDocument, accountLegalEntityId);
            if (organisationCreation == EntityCreation.Created)
            {
                organisation.Name = commitment.LegalEntityName;
            }

            var (cohort, cohortCreated) = GetOrAddCohort(organisation, commitment.Id);
            if (cohortCreated == EntityCreation.Created)
            {
                cohort.Reference = commitment.Reference;
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments
{
    public class CohortApprovalRequestedByProviderEventHandler : EventHandler<CohortApprovalRequestedByProvider>
    {
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;
        private readonly IHashingService _hashingService;

        public CohortApprovalRequestedByProviderEventHandler(
            IAccountDocumentService accountDocumentService,
            IMessageContextInitialisation messageContextInitialisation,
            ILogger<CohortApprovalRequestedByProviderEventHandler> logger,
            IProviderCommitmentsApi providerCommitmentsApi,
            IHashingService hashingService)
                : base(accountDocumentService, messageContextInitialisation, logger)
        {
            _providerCommitmentsApi = providerCommitmentsApi;
            _hashingService = hashingService;
        }

        protected override async Task Handle(CohortApprovalRequestedByProvider cohortApprovalRequestedByProvider, CancellationToken cancellationToken = default)
        {
            var accountDocumentTask = GetOrCreateAccountDocument(cohortApprovalRequestedByProvider.AccountId, cancellationToken);
            var commitment = await _providerCommitmentsApi.GetProviderCommitment(cohortApprovalRequestedByProvider.ProviderId, cohortApprovalRequestedByProvider.CommitmentId);
            long accountLegalEntityId = _hashingService.DecodeValue(commitment.AccountLegalEntityPublicHashedId);

            var accountDocument = await accountDocumentTask;
            var (organisation, organisationCreation) = GetOrAddOrganisation(accountDocument, accountLegalEntityId);
            if (organisationCreation == EntityCreation.Created)
            {
                organisation.Name = commitment.LegalEntityName;
            }

            var (cohort, cohortCreated) = GetOrAddCohort(organisation, cohortApprovalRequestedByProvider.CommitmentId);
            if (cohortCreated == EntityCreation.Created)
            {
                cohort.Reference = commitment.Reference;
            }
 
            commitment.Apprenticeships.ForEach(a =>
            {
                var apprenticeship = cohort.Apprenticeships.FirstOrDefault(ca => ca.Id == a.Id);

                if (apprenticeship == null)
                {
                    apprenticeship = new Apprenticeship {Id = a.Id};
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

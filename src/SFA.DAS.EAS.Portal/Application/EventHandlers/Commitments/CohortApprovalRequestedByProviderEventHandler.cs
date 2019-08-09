using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Services.AccountDocumentService;
using SFA.DAS.EAS.Portal.Application.Services.Commitments;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.TypesExtensions;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers.Commitments
{
    public class CohortApprovalRequestedByProviderEventHandler : IEventHandler<CohortApprovalRequestedByProvider>
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly ILogger<CohortApprovalRequestedByProviderEventHandler> _logger;
        private readonly ICommitmentsService _commitmentsService;
        private readonly IHashingService _hashingService;

        public CohortApprovalRequestedByProviderEventHandler(
                IAccountDocumentService accountDocumentService,
                ILogger<CohortApprovalRequestedByProviderEventHandler> logger,
                ICommitmentsService commitmentsService,
                IHashingService hashingService)
        {
            _accountDocumentService = accountDocumentService;
            _logger = logger;
            _commitmentsService = commitmentsService;
            _hashingService = hashingService;
        }

        public async Task Handle(
            CohortApprovalRequestedByProvider cohortApprovalRequestedByProvider,
            CancellationToken cancellationToken = default)
        {
            var accountDocumentTask = _accountDocumentService.GetOrCreate(cohortApprovalRequestedByProvider.AccountId, cancellationToken);
            var commitment = await _commitmentsService.GetProviderCommitment(
                cohortApprovalRequestedByProvider.ProviderId, cohortApprovalRequestedByProvider.CommitmentId, cancellationToken);
            
            long accountLegalEntityId = _hashingService.DecodeValue(commitment.AccountLegalEntityPublicHashedId);

            var accountDocument = await accountDocumentTask;
            var organisation = accountDocument.Account.GetOrAddOrganisation(accountLegalEntityId,
                addedOrganisation => addedOrganisation.Name = commitment.LegalEntityName);

            var cohort = organisation.GetOrAddCohort(cohortApprovalRequestedByProvider.CommitmentId,
                addedCohort => addedCohort.Reference = commitment.Reference);
 
            commitment.Apprenticeships.ForEach(a =>
            {
                //todo: GetOrAddApprenticeship
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

            await _accountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}
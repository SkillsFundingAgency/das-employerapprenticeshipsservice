using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.HashingService;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Extensions;
using SFA.DAS.EAS.Portal.Client.Types;
using System.Linq;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers.Cohort
{
    public class CohortApprovalRequestedByProviderHandler : IEventHandler<CohortApprovalRequestedByProvider>
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;
        private readonly IHashingService _hashingService;

        public CohortApprovalRequestedByProviderHandler(
            IAccountDocumentService accountDocumentService,
            IProviderCommitmentsApi providerCommitmentsApi,
            IHashingService hashingService)
        {
            _accountDocumentService = accountDocumentService;
            _providerCommitmentsApi = providerCommitmentsApi;
            _hashingService = hashingService;
        }

        public async Task Handle(CohortApprovalRequestedByProvider @event, CancellationToken cancellationToken = default)
        {
            var accountDocumentTask = _accountDocumentService.GetOrCreate(@event.AccountId, cancellationToken);
            var commitment = await _providerCommitmentsApi.GetProviderCommitment(@event.ProviderId, @event.CommitmentId);
            long accountLegalEntityId = _hashingService.DecodeValue(commitment.AccountLegalEntityPublicHashedId);

            var accountDocument = await accountDocumentTask;
            var organisation = accountDocument.Account.GetOrAddOrganisation(accountLegalEntityId, addedOrganisation =>
            {
                addedOrganisation.Name = commitment.LegalEntityName;
            });

            var cohort = organisation.GetOrAddCohort(
                @event.CommitmentId, 
                addedCohort =>
                {
                    addedCohort.Reference = commitment.Reference;
                });

            commitment.Apprenticeships.ForEach(a =>
            {
                var apprenticeship = cohort.Apprenticeships.FirstOrDefault(ca => ca.Id == a.Id);

                if (apprenticeship == null)
                {
                    apprenticeship = new Apprenticeship { Id = a.Id };
                    cohort.Apprenticeships.Add(apprenticeship);
                }
                apprenticeship.FirstName = a.FirstName;
                apprenticeship.LastName = a.LastName;
                apprenticeship.CourseName = a.TrainingName;
                apprenticeship.ProposedCost = a.Cost;
                apprenticeship.StartDate = a.StartDate;
                apprenticeship.EndDate = a.EndDate;
            });

            await _accountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}

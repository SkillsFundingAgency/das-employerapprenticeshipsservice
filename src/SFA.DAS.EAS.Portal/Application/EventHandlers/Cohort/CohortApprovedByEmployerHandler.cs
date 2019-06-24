using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.HashingService;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers.Cohort
{
    public class CohortApprovedByEmployerHandler : IEventHandler<CohortApprovedByEmployer>
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;
        private readonly IHashingService _hashingService;

        public CohortApprovedByEmployerHandler(
            IAccountDocumentService accountDocumentService,
            IProviderCommitmentsApi providerCommitmentsApi,
            IHashingService hashingService)
        {
            _accountDocumentService = accountDocumentService;
            _providerCommitmentsApi = providerCommitmentsApi;
            _hashingService = hashingService;
        }

        public async Task Handle(CohortApprovedByEmployer @event, CancellationToken cancellationToken = default)
        {
            var accountDocument = await _accountDocumentService.GetOrCreate(@event.AccountId, cancellationToken);

            var cohort = accountDocument.Account.Organisations.SelectMany(org => org.Cohorts)
                .SingleOrDefault(co => co.Id == @event.CommitmentId.ToString());

            if (cohort != null)
                cohort.IsApproved = true;

            await _accountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}

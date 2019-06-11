using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments
{
    public class CohortApprovedByEmployerEventHandler : EventHandler<CohortApprovedByEmployer>
    {

        public CohortApprovedByEmployerEventHandler(
            IAccountDocumentService accountDocumentService,
            IMessageContextInitialisation messageContextInitialisation,
            ILogger<CohortApprovedByEmployerEventHandler> logger)
                : base(accountDocumentService, messageContextInitialisation, logger)
        {

        }

        protected override async Task Handle(CohortApprovedByEmployer cohortApprovedEvent, CancellationToken cancellationToken = default)
        {
            var accountDocument = await GetOrCreateAccountDocument(cohortApprovedEvent.AccountId, cancellationToken);

            var cohort = accountDocument.Account.Organisations.SelectMany(org => org.Cohorts)
                .SingleOrDefault(co => co.Id == cohortApprovedEvent.CommitmentId.ToString());

            if(cohort != null)
            cohort.IsApproved = true;

            await AccountDocumentService.Save(accountDocument, cancellationToken);
        }
    }

    public class CohortApprovedByEmployer : IEvent
    {
        public long AccountId { get; set; }
        public int CommitmentId { get; set; }
    }
}

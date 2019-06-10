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
    public class CohortApprovedByEmployerEventHandler : EventHandler<CohortApprovedByEmployer>
    {

        public CohortApprovedByEmployerEventHandler(
            IAccountDocumentService accountDocumentService,
            IMessageContextInitialisation messageContextInitialisation,
            ILogger<CohortApprovedByEmployerEventHandler> logger)
                : base(accountDocumentService, messageContextInitialisation, logger)
        {

        }

        protected override async Task Handle(CohortApprovedByEmployer cohortApprovedEvent)
        {
            var cancellationToken = default(CancellationToken);

            var accountDocument = await GetOrCreateAccountDocument(cohortApprovedEvent.AccountId, cancellationToken);

            accountDocument.Account.Organisations.Select(x => x.Cohorts.Where(y => y.Id == cohortApprovedEvent.CommitmentId.ToString())).First().First().IsApproved = true;

            await AccountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}

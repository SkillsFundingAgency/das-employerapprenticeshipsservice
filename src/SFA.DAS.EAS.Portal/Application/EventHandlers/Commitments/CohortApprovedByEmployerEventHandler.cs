using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Services.AccountDocumentService;

namespace SFA.DAS.EAS.Portal.Application.EventHandlers.Commitments
{
    public class CohortApprovedByEmployerEventHandler : IEventHandler<CohortApprovedByEmployer>
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly ILogger<CohortApprovedByEmployerEventHandler> _logger;

        public CohortApprovedByEmployerEventHandler(
            IAccountDocumentService accountDocumentService,
            ILogger<CohortApprovedByEmployerEventHandler> logger)
        {
            _accountDocumentService = accountDocumentService;
            _logger = logger;
        }

        public async Task Handle(CohortApprovedByEmployer cohortApprovedByEmployer, CancellationToken cancellationToken = default)
        {
            var accountDocument = await _accountDocumentService.GetOrCreate(cohortApprovedByEmployer.AccountId, cancellationToken);

            var cohort = accountDocument.Account.Organisations.SelectMany(org => org.Cohorts)
                .SingleOrDefault(co => co.Id == cohortApprovedByEmployer.CommitmentId.ToString());

            if(cohort != null)
                cohort.IsApproved = true;

            await _accountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}
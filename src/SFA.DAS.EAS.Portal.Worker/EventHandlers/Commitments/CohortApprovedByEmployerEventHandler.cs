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

        protected override async Task Handle(CohortApprovedByEmployer cohortApprovedEvent)
        {
            var cancellationToken = default(CancellationToken);

            var accountDocument = await GetOrCreateAccountDocument(cohortApprovedEvent.AccountId, cancellationToken);

            //Maybe one of these might work
            #region Option 1
            accountDocument.Account.Organisations.Select(x => x.Cohorts.Where(y => y.Id == cohortApprovedEvent.CommitmentId.ToString())).First().First().IsApproved = true;
            #endregion

            #region Option 2
            accountDocument.Account.Organisations.GetEnumerator().Reset();
            while (accountDocument.Account.Organisations.GetEnumerator().MoveNext())
            {
                accountDocument.Account.Organisations.GetEnumerator().Current.Cohorts.GetEnumerator().Reset();
                while (accountDocument.Account.Organisations.GetEnumerator().Current.Cohorts.GetEnumerator().MoveNext())
                {
                    if (accountDocument.Account.Organisations.GetEnumerator().Current.Cohorts.GetEnumerator().Current
                            .Id == cohortApprovedEvent.CommitmentId.ToString())
                    {
                        accountDocument.Account.Organisations.GetEnumerator().Current.Cohorts.GetEnumerator().Current
                            .IsApproved = true;
                    }
                }
                accountDocument.Account.Organisations.GetEnumerator().Current.Cohorts.GetEnumerator().Dispose();
            }
            accountDocument.Account.Organisations.GetEnumerator().Dispose();
            #endregion

            await AccountDocumentService.Save(accountDocument, cancellationToken);
        }
    }

    public class CohortApprovedByEmployer : IEvent
    {
        public long AccountId { get; set; }
        public int CommitmentId { get; set; }
    }
}

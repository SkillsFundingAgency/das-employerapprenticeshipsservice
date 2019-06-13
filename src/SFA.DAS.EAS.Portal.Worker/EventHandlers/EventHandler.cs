using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.Worker.TypesExtensions;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers
{
    public abstract class EventHandler<TEvent> : IHandleMessages<TEvent>
    {
        protected readonly IAccountDocumentService AccountDocumentService;
        protected readonly ILogger Logger;
        private readonly IMessageContextInitialisation _messageContextInitialisation;

        protected EventHandler(
            IAccountDocumentService accountDocumentService, 
            IMessageContextInitialisation messageContextInitialisation,
            ILogger logger)
        {
            AccountDocumentService = accountDocumentService;
            _messageContextInitialisation = messageContextInitialisation;
            Logger = logger;
        }
        
        public Task Handle(TEvent message, IMessageHandlerContext context)
        {
            _messageContextInitialisation.Initialise(context);
            return Handle(message);
        }

        protected abstract Task Handle(TEvent message);
        
        protected async Task<AccountDocument> GetOrCreateAccountDocument(long accountId, CancellationToken cancellationToken = default)
        {
            return await AccountDocumentService.Get(accountId, cancellationToken) ?? new AccountDocument(accountId);
        }

        protected (Client.Types.Cohort, EntityCreation) GetOrAddCohort(Organisation organisation, long cohortId)
        {
            //todo: is there a reason for this?
            var cohortIdAsString = cohortId.ToString();
            var cohort = organisation.Cohorts.SingleOrDefault(c => cohortIdAsString.Equals(c.Id, StringComparison.OrdinalIgnoreCase));
            if (cohort == null)
            {
                cohort = new Client.Types.Cohort {Id = cohortIdAsString};
                organisation.Cohorts.Add(cohort);
                return (cohort, EntityCreation.Created);
            }

            return (cohort, EntityCreation.Existed);
        }
    }
}
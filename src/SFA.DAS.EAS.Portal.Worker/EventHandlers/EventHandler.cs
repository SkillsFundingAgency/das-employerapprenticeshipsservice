using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;

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
    }
}
using SFA.DAS.EAS.Portal.Client.Database.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public class AccountDocumentServiceWithDuplicateCheck : IAccountDocumentService
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly IMessageContext _messageContext;
        public AccountDocumentServiceWithDuplicateCheck(IAccountDocumentService accountDocumentService, IMessageContext messageContext)
        {
            _accountDocumentService = accountDocumentService;
            _messageContext = messageContext;
        }

        public Task<AccountDocument> Get(long id, CancellationToken cancellationToken = default)
        {
            return _accountDocumentService.Get(id, cancellationToken);
        }

        public Task Save(AccountDocument accountDocument, CancellationToken cancellationToken = default)
        {
            accountDocument.DeleteOldMessages();
            if (accountDocument.IsMessageProcessed(_messageContext.Id)) { return Task.CompletedTask; };

            accountDocument.AddOutboxMessage(_messageContext.Id, _messageContext.CreatedDateTime);         

            return _accountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}

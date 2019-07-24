using SFA.DAS.EAS.Portal.Client.Database.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public class AccountDocumentServiceWithSetProperties : IAccountDocumentService
    {
        private readonly IAccountDocumentService _accountDocumentService;
        private readonly IMessageContext _messageContext;
        public AccountDocumentServiceWithSetProperties(IAccountDocumentService accountDocumentService, IMessageContext messageContext)
        {
            _accountDocumentService = accountDocumentService;
            _messageContext = messageContext;
        }

        public Task<AccountDocument> Get(long id, CancellationToken cancellationToken = default)
        {
            return _accountDocumentService.Get(id, cancellationToken);
        }

        public Task<AccountDocument> GetOrCreate(long id, CancellationToken cancellationToken = default)
        {
            return _accountDocumentService.GetOrCreate(id, cancellationToken);
        }

        public Task Save(AccountDocument accountDocument, CancellationToken cancellationToken = default)
        {
            if (accountDocument.IsNew)
            {
                accountDocument.Created = _messageContext.CreatedDateTime;
            }
            else
            {
                accountDocument.Updated = _messageContext.CreatedDateTime;
            }

            return _accountDocumentService.Save(accountDocument, cancellationToken);
        }
    }
}

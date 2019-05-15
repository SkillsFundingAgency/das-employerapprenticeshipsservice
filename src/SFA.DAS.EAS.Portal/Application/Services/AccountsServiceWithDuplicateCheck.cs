using SFA.DAS.EAS.Portal.Database.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public class AccountsServiceWithDuplicateCheck : IAccountsService
    {
        private readonly IAccountsService _accountsService;
        private readonly IMessageContext _messageContext;
        public AccountsServiceWithDuplicateCheck(IAccountsService accountsService, IMessageContext messageContext)
        {
            _accountsService = accountsService;
            _messageContext = messageContext;
        }

        public Task<Account> Get(long id, CancellationToken cancellationToken = default)
        {
            return _accountsService.Get(id, cancellationToken);
        }

        public async Task Save(Account account, CancellationToken cancellationToken = default)
        {
            account.DeleteOldMessages();
            if (account.IsMessageProcessed(_messageContext.Id)) { return; };

            account.AddOutboxMessage(_messageContext.Id, DateTime.Now);

            await _accountsService.Save(account, cancellationToken);
        }
    }
}

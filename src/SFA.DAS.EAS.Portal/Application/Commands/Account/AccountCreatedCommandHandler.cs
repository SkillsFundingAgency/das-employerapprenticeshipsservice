using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;

namespace SFA.DAS.EAS.Portal.Application.Commands.Account
{
    public class AccountCreatedCommandHandler : ICommandHandler<AccountCreatedCommand>
    {
        private readonly IAccountDocumentService _accountsService;

        public AccountCreatedCommandHandler(IAccountDocumentService accountsService)
        {
            _accountsService = accountsService;            
        }

        public async Task Handle(AccountCreatedCommand command, CancellationToken cancellationToken = default)
        {
            var accountDocument = await _accountsService.Get(command.Id, cancellationToken);

            if(accountDocument != null)
            {
                return;  // already handled 
            }

            accountDocument = AccountDocument.Create(command.Id);
            accountDocument.Account.Name = command.Name;

            await _accountsService.Save(accountDocument, cancellationToken);
        }
    }
}

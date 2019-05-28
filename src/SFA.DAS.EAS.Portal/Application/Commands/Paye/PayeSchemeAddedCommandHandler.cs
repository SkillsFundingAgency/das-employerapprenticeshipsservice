using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Application.Commands.Account;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Application.Commands.Paye
{
    public class PayeSchemeAddedCommandHandler : ICommandHandler<PayeSchemeAddedCommand>
    {
        private ICommandHandler<AccountCreatedCommand> _accountCreatedCommandHandler;
        private readonly IAccountDocumentService _accountService;

        public PayeSchemeAddedCommandHandler(ICommandHandler<AccountCreatedCommand> accountCreatedCommandHandler, IAccountDocumentService accountService)
        {
            _accountCreatedCommandHandler = accountCreatedCommandHandler;
            _accountService = accountService;
        }

        public async Task Handle(PayeSchemeAddedCommand command, CancellationToken cancellationToken = default)
        {
            await _accountCreatedCommandHandler.Handle(new AccountCreatedCommand(command.AccountId, ""));

            var accountDoc = await _accountService.Get(command.AccountId, cancellationToken);

            var existingPaye = accountDoc.Account.PayeSchemes.FirstOrDefault(paye => paye.PayeRef == command.PayeRef);

            if(existingPaye == null)
            {
                accountDoc.Account.PayeSchemes.Add(new PayeScheme { PayeRef = command.PayeRef });
            }

            await _accountService.Save(accountDoc, cancellationToken);
        }
    }
}

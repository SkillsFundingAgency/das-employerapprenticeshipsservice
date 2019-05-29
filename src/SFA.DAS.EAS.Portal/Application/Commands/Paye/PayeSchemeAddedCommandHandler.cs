using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Application.AccountHelper;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Application.Commands.Paye
{
    public class PayeSchemeAddedCommandHandler : ICommandHandler<PayeSchemeAddedCommand>
    {
        private readonly IAccountHelperService _accountHelper;
        private readonly IAccountDocumentService _accountService;

        public PayeSchemeAddedCommandHandler(IAccountHelperService accountHelper, IAccountDocumentService accountService)
        {
            _accountHelper = accountHelper;
            _accountService = accountService;
        }

        public async Task Handle(PayeSchemeAddedCommand command, CancellationToken cancellationToken = default)
        {
            var accountDoc = await _accountHelper.GetOrCreateAccount(command.AccountId, cancellationToken);

            var existingPaye = accountDoc.Account.PayeSchemes.FirstOrDefault(paye => paye.PayeRef == command.PayeRef);

            if(existingPaye == null)
            {
                accountDoc.Account.PayeSchemes.Add(new PayeScheme { PayeRef = command.PayeRef });
                await _accountService.Save(accountDoc, cancellationToken);
            }
        }
    }
}

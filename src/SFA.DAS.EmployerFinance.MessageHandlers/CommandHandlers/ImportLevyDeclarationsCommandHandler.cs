using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ImportLevyDeclarationsCommandHandler : IHandleMessages<ImportLevyDeclarationsCommand>
    {
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly ILog _logger;
        private readonly IPayeRepository _payeRepository;

        public ImportLevyDeclarationsCommandHandler(IEmployerAccountRepository accountRepository, ILog logger, IPayeRepository payeRepository)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _payeRepository = payeRepository;
        }

        public async Task Handle(ImportLevyDeclarationsCommand message, IMessageHandlerContext context)
        {
            var employerAccounts = await _accountRepository.GetAllAccounts();

            _logger.Debug($"Updating {employerAccounts.Count} levy accounts");

            var tasks = new List<Task>();

            foreach (var account in employerAccounts)
            {
                var schemes = await _payeRepository.GetGovernmentGatewayOnlySchemesByEmployerId(account.Id);

                if (schemes?.SchemesList == null)
                {
                    continue;
                }

                foreach (var scheme in schemes.SchemesList)
                {
                    _logger.Debug($"Creating update levy account message for account {account.Name} (ID: {account.Id}) scheme {scheme.EmpRef}");

                    tasks.Add(context.SendLocal<ImportAccountLevyDeclarationsCommand>(c =>
                    {
                        c.AccountId = account.Id;
                        c.PayeRef = scheme.EmpRef;
                    }));
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}

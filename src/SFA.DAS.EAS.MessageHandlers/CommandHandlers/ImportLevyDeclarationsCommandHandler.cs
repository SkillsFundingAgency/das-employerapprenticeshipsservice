using NServiceBus;
using NServiceBus.Logging;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Messages.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.CommandHandlers
{
    public class ImportLevyDeclarationsCommandHandler : IHandleMessages<IImportLevyDeclarationsCommand>
    {
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly ILog _logger;
        private readonly IEmployerSchemesRepository _employerSchemesRepository;

        public ImportLevyDeclarationsCommandHandler(IEmployerAccountRepository accountRepository, ILog logger, IEmployerSchemesRepository employerSchemesRepository)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _employerSchemesRepository = employerSchemesRepository;
        }

        public async Task Handle(IImportLevyDeclarationsCommand message, IMessageHandlerContext context)
        {
            var employerAccounts = await _accountRepository.GetAllAccounts();

            _logger.Debug($"Updating {employerAccounts.Count} levy accounts");

            var tasks = new List<Task>();

            foreach (var account in employerAccounts)
            {
                var schemes = await _employerSchemesRepository.GetSchemesByEmployerId(account.Id);

                if (schemes?.SchemesList == null)
                {
                    continue;
                }

                foreach (var scheme in schemes.SchemesList)
                {
                    _logger.Debug($"Creating update levy account message for account {account.Name} (ID: {account.Id}) scheme {scheme.Ref}");

                    tasks.Add(context.SendLocal<IImportAccountLevyDeclarationsCommand>(c =>
                    {
                        c.AccountId = account.Id;
                        c.PayeRef = scheme.Ref;
                    }));
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}

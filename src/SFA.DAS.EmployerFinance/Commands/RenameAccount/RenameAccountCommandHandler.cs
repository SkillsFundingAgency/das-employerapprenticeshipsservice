using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.RenameAccount
{
    public class RenameAccountCommandHandler : AsyncRequestHandler<RenameAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILog _logger;

        public RenameAccountCommandHandler(IAccountRepository accountRepository, ILog logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        protected override async Task HandleCore(RenameAccountCommand message)
        {
            try
            {
                await _accountRepository.RenameAccount(message.Id, message.Name);

                _logger.Info($"Account {message.Id} renamed");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not rename account");
                throw;
            }
        }
    }
}

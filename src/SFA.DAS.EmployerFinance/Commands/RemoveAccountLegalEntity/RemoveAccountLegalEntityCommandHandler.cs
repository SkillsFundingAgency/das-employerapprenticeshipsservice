using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.RemoveAccountLegalEntity
{
    public class RemoveAccountLegalEntityCommandHandler : AsyncRequestHandler<RemoveAccountLegalEntityCommand>
    {
        private readonly IAccountLegalEntityRepository _accountLegalEntityRepository;
        private readonly ILog _logger;

        public RemoveAccountLegalEntityCommandHandler(IAccountLegalEntityRepository accountLegalEntityRepository, ILog logger)
        {
            _accountLegalEntityRepository = accountLegalEntityRepository;
            _logger = logger;
        }

        protected override async Task HandleCore(RemoveAccountLegalEntityCommand message)
        {
            try
            {
                await _accountLegalEntityRepository.RemoveAccountLegalEntity(message.Id);
                _logger.Info($"Account Legal Entity {message.Id} removed");
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Could not remove Account Legal Entity");
                throw;
            }
        }
    }
}

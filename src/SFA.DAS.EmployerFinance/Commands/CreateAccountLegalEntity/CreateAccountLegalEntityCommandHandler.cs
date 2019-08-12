using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.CreateAccountLegalEntity
{
    public class CreateAccountLegalEntityCommandHandler
    {
        private readonly IAccountLegalEntityRepository _accountLegalEntityRepository;
        private readonly ILog _logger;

        public CreateAccountLegalEntityCommandHandler(IAccountLegalEntityRepository accountLegalEntityRepository, ILog logger)
        {
            _accountLegalEntityRepository = accountLegalEntityRepository;
            _logger = logger;
        }

        public async Task Handle(CreateAccountLegalEntityCommand command)
        {
            try
            {
                await _accountLegalEntityRepository.CreateAccountLegalEntity(
                    command.Id,
                    command.Deleted,
                    command.PendingAgreementId,
                    command.SignedAgreementId,
                    command.SignedAgreementVersion,
                    command.AccountId,
                    command.LegalEntityId
                );
                _logger.Info($"Account Legal Entity {command.Id} created");
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Could not create Account Legal Entity");
                throw;
            }
            
        }
    }
}

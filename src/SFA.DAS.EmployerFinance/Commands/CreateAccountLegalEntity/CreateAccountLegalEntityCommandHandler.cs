using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.CreateAccountLegalEntity
{
    public class CreateAccountLegalEntityCommandHandler : AsyncRequestHandler<CreateAccountLegalEntityCommand>
    {
        private readonly IAccountLegalEntityRepository _accountLegalEntityRepository;
        private readonly ILog _logger;

        public CreateAccountLegalEntityCommandHandler(IAccountLegalEntityRepository accountLegalEntityRepository, ILog logger)
        {
            _accountLegalEntityRepository = accountLegalEntityRepository;
            _logger = logger;
        }

        protected override async Task HandleCore(CreateAccountLegalEntityCommand message)
        {
            try
            {
                await _accountLegalEntityRepository.CreateAccountLegalEntity(
                    message.Id,
                    message.PendingAgreementId,
                    message.SignedAgreementId,
                    message.SignedAgreementVersion,
                    message.AccountId,
                    message.LegalEntityId
                );
                _logger.Info($"Account Legal Entity {message.Id} created");
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Could not create Account Legal Entity");
                throw;
            }
            
        }
    }
}

using System;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.CreateAccountPaye
{
    public class CreateAccountPayeCommandHandler : AsyncRequestHandler<CreateAccountPayeCommand>
    {
        private readonly IPayeRepository _payeRepository;
        private readonly IMessageSession _messageSession;
        private readonly ILog _logger;

        public CreateAccountPayeCommandHandler(IPayeRepository payeRepository, IMessageSession messageSession, ILog logger)
        {
            _payeRepository = payeRepository;
            _messageSession = messageSession;
            _logger = logger;
        }

        protected override async Task HandleCore(CreateAccountPayeCommand message)
        {
            try
            {
                var payeScheme = new Paye(message.EmpRef, message.AccountId, message.Name, message.Aorn);
                await _payeRepository.CreatePayeScheme(payeScheme);

                await GetLevyForNoneAornPayeSchemes(payeScheme);

                _logger.Info($"Account Paye scheme created - Account Id: {payeScheme.AccountId}; Emp Ref: {payeScheme.Ref}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not create account paye scheme");
                throw;
            }
        }

        private async Task GetLevyForNoneAornPayeSchemes(Paye payeScheme)
        {
            if (string.IsNullOrEmpty(payeScheme.Aorn))
            {
                await _messageSession.Send(new ImportAccountLevyDeclarationsCommand(payeScheme.AccountId, payeScheme.Ref));

                _logger.Info($"Requested levy for - Account Id: {payeScheme.AccountId}; Emp Ref: {payeScheme.Ref}");
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.CreateAccountPaye
{
    public class CreateAccountPayeCommandHandler : IHandleMessages<CreateAccountPayeCommand>
    {
        private readonly IPayeRepository _payeRepository;
        private readonly ILog _logger;

        public CreateAccountPayeCommandHandler(IPayeRepository payeRepository, ILog logger)
        {
            _payeRepository = payeRepository;
            _logger = logger;
        }

        public async Task Handle(CreateAccountPayeCommand message, IMessageHandlerContext context)
        {
            try
            {
                var payeScheme = new Paye(message.EmpRef, message.AccountId, message.Name, message.Aorn);
                await _payeRepository.CreatePayeScheme(payeScheme);

                _logger.Info($"Account Paye scheme created - Account Id: {payeScheme.AccountId}; Emp Ref: {payeScheme.Ref}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not create account paye scheme");
                throw;
            }
        }
    }
}

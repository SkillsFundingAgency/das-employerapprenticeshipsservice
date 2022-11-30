using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
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
                _logger.Info($"Account Paye scheme created via {(string.IsNullOrEmpty(message.Aorn) ? "Gov gateway" : "Aorn")} - Account Id: {message.AccountId}; Emp Ref: {message.EmpRef};");

                var payeScheme = new Paye(message.EmpRef, message.AccountId, message.Name, message.Aorn);
                await _payeRepository.CreatePayeScheme(payeScheme);

                await GetLevyForNoneAornPayeSchemes(payeScheme, context);

                _logger.Info($"Account Paye scheme created - Account Id: {payeScheme.AccountId}; Emp Ref: {payeScheme.EmpRef}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not create account paye scheme");
                throw;
            }
        }

        private async Task GetLevyForNoneAornPayeSchemes(Paye payeScheme, IMessageHandlerContext context)
        {
            if (string.IsNullOrEmpty(payeScheme.Aorn))
            {
                await context.SendLocal(new ImportAccountLevyDeclarationsCommand(payeScheme.AccountId, payeScheme.EmpRef));

                _logger.Info($"Requested levy for - Account Id: {payeScheme.AccountId}; Emp Ref: {payeScheme.EmpRef}");
            }
        }
    }
}

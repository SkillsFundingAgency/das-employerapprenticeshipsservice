using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.RemoveAccountPaye
{
    public class RemoveAccountPayeCommandHandler : AsyncRequestHandler<RemoveAccountPayeCommand>
    {
        private readonly IPayeRepository _payeRepository;
        private readonly ILog _logger;

        public RemoveAccountPayeCommandHandler(IPayeRepository payeRepository, ILog logger)
        {
            _payeRepository = payeRepository;
            _logger = logger;
        }

        protected override async Task HandleCore(RemoveAccountPayeCommand message)
        {
            try
            {
                await _payeRepository.RemovePayeScheme(message.AccountId, message.PayeRef);

                _logger.Info($"Paye scheme removed - account id: {message.AccountId}; paye ref: {message.PayeRef}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not remove account paye scheme");
                throw;
            }
        }
    }
}

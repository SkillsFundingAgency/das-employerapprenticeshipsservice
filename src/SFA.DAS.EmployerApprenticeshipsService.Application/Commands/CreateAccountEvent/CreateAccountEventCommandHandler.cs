using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.CreateAccountEvent
{
    public class CreateAccountEventCommandHandler : AsyncRequestHandler<CreateAccountEventCommand>
    {
        private readonly IEventsApi _eventsApi;
        private readonly ILogger _logger;

        public CreateAccountEventCommandHandler(IEventsApi eventsApi, ILogger logger)
        {
            _eventsApi = eventsApi;
            _logger = logger;
        }

        protected override async Task HandleCore(CreateAccountEventCommand message)
        {
            try
            {
                await _eventsApi.CreateAccountEvent(new AccountEvent { EmployerAccountId = message.HashedAccountId, Event = message.Event });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
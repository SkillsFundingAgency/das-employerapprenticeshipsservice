using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.CreateAccountEvent
{
    public class CreateAccountEventCommandHandler : IAsyncNotificationHandler<CreateAccountEventCommand>
    {
        private readonly IEventsApi _eventsApi;
        private readonly ILogger _logger;

        public CreateAccountEventCommandHandler(IEventsApi eventsApi, ILogger logger)
        {
            _eventsApi = eventsApi;
            _logger = logger;
        }

        public async Task Handle(CreateAccountEventCommand notification)
        {
            try
            {
                await _eventsApi.CreateAccountEvent(new AccountEvent { EmployerAccountId = notification.HashedAccountId, Event = notification.Event });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
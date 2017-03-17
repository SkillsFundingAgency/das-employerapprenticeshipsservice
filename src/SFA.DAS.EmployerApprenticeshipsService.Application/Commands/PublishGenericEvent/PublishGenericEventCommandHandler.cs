using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Events.Api.Client;

namespace SFA.DAS.EAS.Application.Commands.PublishGenericEvent
{
    public class PublishGenericEventCommandHandler : IAsyncRequestHandler<PublishGenericEventCommand, PublishGenericEventCommandResponse>
    {
        private readonly IEventsApi _eventsApi;
        private readonly ILogger _logger;

        public PublishGenericEventCommandHandler(IEventsApi eventsApi, ILogger logger)
        {
            _eventsApi = eventsApi;
            _logger = logger;
        }

        public async Task<PublishGenericEventCommandResponse> Handle(PublishGenericEventCommand command)
        {
            _logger.Info($"Publishing Generic event of type {command.Event.Type}");

            await _eventsApi.CreateGenericEvent(command.Event);

            return new PublishGenericEventCommandResponse();
        }
    }
}

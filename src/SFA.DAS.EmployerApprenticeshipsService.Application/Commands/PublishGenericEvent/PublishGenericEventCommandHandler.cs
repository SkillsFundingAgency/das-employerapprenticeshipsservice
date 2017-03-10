using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Events.Api.Client;

namespace SFA.DAS.EAS.Application.Commands.PublishGenericEvent
{
    public class PublishGenericEventCommandHandler : IAsyncRequestHandler<PublishGenericEventCommand, PublishGenericEventCommandResponse>
    {
        private readonly IEventsApi _eventsApi;

        public PublishGenericEventCommandHandler(IEventsApi eventsApi)
        {
            _eventsApi = eventsApi;
        }

        public async Task<PublishGenericEventCommandResponse> Handle(PublishGenericEventCommand command)
        {
            await _eventsApi.CreateGenericEvent(command.Event);

            return new PublishGenericEventCommandResponse();
        }
    }
}

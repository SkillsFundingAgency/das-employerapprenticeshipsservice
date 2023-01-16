using System.Threading;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;

public class PublishGenericEventCommandHandler : IRequestHandler<PublishGenericEventCommand, PublishGenericEventCommandResponse>
{
    private readonly IEventsApi _eventsApi;
    private readonly ILog _logger;

    public PublishGenericEventCommandHandler(IEventsApi eventsApi, ILog logger)
    {
        _eventsApi = eventsApi;
        _logger = logger;
    }

    public async Task<PublishGenericEventCommandResponse> Handle(PublishGenericEventCommand command, CancellationToken cancellationToken)
    {
        _logger.Info($"Publishing Generic event of type {command.Event.Type}");

        await _eventsApi.CreateGenericEvent(command.Event);

        return new PublishGenericEventCommandResponse();
    }
}
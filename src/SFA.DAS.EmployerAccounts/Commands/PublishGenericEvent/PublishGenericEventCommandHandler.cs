using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.Events.Api.Client;

namespace SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;

public class PublishGenericEventCommandHandler : IRequestHandler<PublishGenericEventCommand>
{
    private readonly IEventsApi _eventsApi;
    private readonly ILogger<PublishGenericEventCommandHandler> _logger;

    public PublishGenericEventCommandHandler(IEventsApi eventsApi, ILogger<PublishGenericEventCommandHandler> logger)
    {
        _eventsApi = eventsApi;
        _logger = logger;
    }

    public async Task<Unit> Handle(PublishGenericEventCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing Generic event of type {EventType}", command.Event.Type);

        await _eventsApi.CreateGenericEvent(command.Event);

        return Unit.Value;
    }
}
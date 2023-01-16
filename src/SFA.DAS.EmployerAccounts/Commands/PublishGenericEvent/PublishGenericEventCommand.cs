using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;

public class PublishGenericEventCommand : IRequest<PublishGenericEventCommandResponse>
{
    public GenericEvent Event { get; set; }
}
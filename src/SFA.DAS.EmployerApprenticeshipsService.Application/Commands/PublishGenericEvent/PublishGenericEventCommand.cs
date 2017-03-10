using MediatR;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.PublishGenericEvent
{
    public class PublishGenericEventCommand : IAsyncRequest<PublishGenericEventCommandResponse>
    {
        public GenericEvent Event { get; set; }
    }
}

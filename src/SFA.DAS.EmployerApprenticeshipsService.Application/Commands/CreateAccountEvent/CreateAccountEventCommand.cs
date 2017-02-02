using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateAccountEvent
{
    public class CreateAccountEventCommand : IAsyncNotification
    {
        public string ResourceUri { get; set; }
        public string Event { get; set; }
    }
}
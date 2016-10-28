using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateEmployerAccountCreatedNotification
{
    public class CreateEmployerAccountCreatedNotificationCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
    }
}

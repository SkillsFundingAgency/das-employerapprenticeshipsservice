using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccountCreatedNotification
{
    public class CreateEmployerAccountCreatedNotificationCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
    }
}

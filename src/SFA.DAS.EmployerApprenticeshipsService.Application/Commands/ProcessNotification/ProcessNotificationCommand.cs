using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ProcessNotification
{
    public class ProcessNotificationCommand : IAsyncRequest
    {
        public long Id { get; set; }
    }
}

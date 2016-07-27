using System;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ProcessNotification
{
    public class ProcessNotificationCommandHandler : AsyncRequestHandler<ProcessNotificationCommand>
    {
        protected override Task HandleCore(ProcessNotificationCommand message)
        {
            throw new NotImplementedException();
        }
    }
}
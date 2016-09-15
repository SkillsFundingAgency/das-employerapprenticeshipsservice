using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateTask
{
    public sealed class CreateTaskCommand : IAsyncRequest
    {
        public long ProviderId { get; set; }
    }
}

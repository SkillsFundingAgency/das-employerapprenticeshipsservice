using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Commands.RunHealthCheckCommand
{
    public class RunHealthCheckCommand : UserMessage, IAsyncRequest
    {
    }
}
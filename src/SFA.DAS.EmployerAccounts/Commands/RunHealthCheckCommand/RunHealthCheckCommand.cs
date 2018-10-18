using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand
{
    public class RunHealthCheckCommand : UserMessage, IAsyncRequest
    {
    }
}
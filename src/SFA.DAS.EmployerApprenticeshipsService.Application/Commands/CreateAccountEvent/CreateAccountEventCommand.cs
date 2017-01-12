using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateAccount;

namespace SFA.DAS.EAS.Application.Commands.CreateAccountEvent
{
    public class CreateAccountEventCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public string Event { get; set; }
    }
}
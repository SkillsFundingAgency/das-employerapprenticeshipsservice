using MediatR;

namespace SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public string PayeRef { get; set; }
        public string UserId { get; set; }
        public bool RemoveScheme { get; set; }
    }
}
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
        public string UserId { get; set; }
    }
}
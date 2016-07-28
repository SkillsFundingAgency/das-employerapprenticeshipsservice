using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommand : IAsyncRequest
    {
        public long UserId { get; set; }
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
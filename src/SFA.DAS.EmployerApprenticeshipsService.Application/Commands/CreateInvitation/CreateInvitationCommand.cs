using MediatR;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommand : IAsyncRequest
    {
        public string ExternalUserId { get; set; }

        public string HashedAccountId { get; set; }

        public string NameOfPersonBeingInvited { get; set; }

        public string EmailOfPersonBeingInvited { get; set; }

        public Role RoleOfPersonBeingInvited { get; set; }
    }
}
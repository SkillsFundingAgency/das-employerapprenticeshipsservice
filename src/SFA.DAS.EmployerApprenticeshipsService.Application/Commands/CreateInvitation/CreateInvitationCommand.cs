using MediatR;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommand : IAsyncRequest
    {
        public string ExternalUserId { get; set; }
        public string HashedAccountId { get; set; }

        /// <summary>
        /// The person being invited
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The person being invited
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The person being invited
        /// </summary>
        public Role RoleId { get; set; }
    }
}
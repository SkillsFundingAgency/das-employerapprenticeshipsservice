using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.ResendInvitation
{
    public class ResendInvitationCommand : IAsyncRequest
    {
        public string Email { get; set; }
        public string AccountId { get; set; }
        public Guid ExternalUserId { get; set; }
        public string FirstName { get; set; }
    }
}
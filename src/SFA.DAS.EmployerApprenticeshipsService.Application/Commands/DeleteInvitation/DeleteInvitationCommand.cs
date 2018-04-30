using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.DeleteInvitation
{
    public class DeleteInvitationCommand : IAsyncRequest
    {
        public string Email { get; set; }
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
    }
}
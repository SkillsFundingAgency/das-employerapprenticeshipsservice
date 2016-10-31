﻿using MediatR;

namespace SFA.DAS.EAS.Application.Commands.DeleteInvitation
{
    public class DeleteInvitationCommand : IAsyncRequest
    {
        public string Email { get; set; }
        public string HashedId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
﻿using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Commands.RejectTransferConnectionInvitation
{
    public class RejectTransferConnectionInvitationCommand : MembershipMessage, IAsyncRequest
    {
        [Required]
        public int? TransferConnectionInvitationId { get; set; }
    }
}
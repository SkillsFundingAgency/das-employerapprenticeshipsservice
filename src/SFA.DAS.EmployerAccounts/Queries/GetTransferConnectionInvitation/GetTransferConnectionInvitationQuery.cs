﻿using MediatR;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationQuery : MembershipMessage, IAsyncRequest<GetTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}
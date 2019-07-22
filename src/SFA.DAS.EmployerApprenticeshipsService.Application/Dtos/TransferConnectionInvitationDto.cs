using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Dtos
{
    public class TransferConnectionInvitationDto
    {
        public int Id { get; set; }
        public ICollection<TransferConnectionInvitationChangeDto> Changes { get; set; }
        public DateTime CreatedDate { get; set; }
        public AccountDto ReceiverAccount { get; set; }
        public AccountDto SenderAccount { get; set; }
        public TransferConnectionInvitationStatus Status { get; set; }
    }
}
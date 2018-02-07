using System;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Dtos
{
    public class TransferConnectionInvitationDto
    {
        public long Id { get; set; }
        public TransferConnectionInvitationStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public virtual UserDto SenderUser { get; set; }
        public virtual AccountDto SenderAccount { get; set; }
        public virtual AccountDto ReceiverAccount { get; set; }
        public virtual UserDto ApproverUser { get; set; }
        public virtual UserDto RejectorUser { get; set; }

        public AccountDto GetPeerAccount(long accountId)
        {
            return SenderAccount.Id == accountId ? ReceiverAccount : SenderAccount;
        }
    }
}
using System;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EAS.Application.Dtos
{
    public class TransferRequestDto
    {
        public DateTime CreatedDate { get; set; }
        public AccountDto ReceiverAccount { get; set; }
        public AccountDto SenderAccount { get; set; }
        public TransferApprovalStatus Status { get; set; }
        public decimal TransferCost { get; set; }
        public string TransferRequestHashedId { get; set; }
    }
}
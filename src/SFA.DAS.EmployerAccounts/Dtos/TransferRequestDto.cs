using SFA.DAS.CommitmentsV2.Types;
using System;

namespace SFA.DAS.EmployerAccounts.Dtos
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
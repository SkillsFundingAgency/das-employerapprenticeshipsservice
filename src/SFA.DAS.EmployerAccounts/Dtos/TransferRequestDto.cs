using SFA.DAS.CommitmentsV2.Types;
//using SFA.DAS.Commitments.Api.Types;
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

//public enum TransferApprovalStatus : byte
//{
//    Pending = 0,
//    Approved = 1,
//    Rejected = 2
//}

//public enum TransferApprovalStatus
//{
//    Pending = 0,
//    Approved = 1,
//    Rejected = 2
//}
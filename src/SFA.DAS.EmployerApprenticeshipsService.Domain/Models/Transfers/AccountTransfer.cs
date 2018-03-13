using System;

namespace SFA.DAS.EAS.Domain.Models.Transfers
{
    public class AccountTransfer
    {
        public long SenderAccountId { get; set; }
        public long RecieverAccountId { get; set; }
        public long CommitmentId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime TransferDate { get; set; }
    }
}

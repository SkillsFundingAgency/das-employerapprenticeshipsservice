using System;

namespace SFA.DAS.EAS.Domain.Models.Transfers
{
    //TODO: Remove this once the enum is present in the payment events api types nuget package
    public enum AccountTransferType
    {
        None = 0
    }

    public class AccountTransfer
    {
        public long SenderAccountId { get; set; }
        public long RecieverAccountId { get; set; }
        public long CommitmentId { get; set; }
        public decimal Amount { get; set; }
        public AccountTransferType Type { get; set; }
        public DateTime TransferDate { get; set; }
    }
}

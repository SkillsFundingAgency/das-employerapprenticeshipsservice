using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Models.Transfers
{
    public class TransferTransactionLine : TransactionLine
    {
        public string PeriodEnd { get; set; }
        public long ReceiverAccountId { get; set; }
        public string ReceiverAccountName { get; set; }
    }
}

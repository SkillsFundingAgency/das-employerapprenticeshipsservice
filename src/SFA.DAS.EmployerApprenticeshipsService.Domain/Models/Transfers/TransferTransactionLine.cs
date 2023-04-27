using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain.Models.Transfers;

public class TransferTransactionLine : TransactionLine
{
    public string PeriodEnd { get; set; }
    public long ReceiverAccountId { get; set; }
    public string ReceiverAccountPublicHashedId { get; set; }
    public string ReceiverAccountName { get; set; }
    public long SenderAccountId { get; set; }
    public string SenderAccountPublicHashedId { get; set; }
    public string SenderAccountName { get; set; }
    public bool TransactionAccountIsTransferSender => AccountId == SenderAccountId;
}

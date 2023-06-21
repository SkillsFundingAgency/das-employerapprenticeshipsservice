namespace SFA.DAS.EAS.Domain.Models.Transaction;

public enum TransactionItemType : short
{
    Unknown = 0,
    Declaration = 1,
    TopUp = 2,
    Payment = 3,
    Transfer = 4,
    ExpiredFund = 5
}
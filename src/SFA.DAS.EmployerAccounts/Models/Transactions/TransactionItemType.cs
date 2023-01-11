namespace SFA.DAS.EmployerAccounts.Models.Transactions;

public enum TransactionItemType
{
    Unknown = 0,
    Declaration = 1,
    TopUp = 2,
    Payment = 3,
    Transfer = 4,
    ExpiredFund = 5
}
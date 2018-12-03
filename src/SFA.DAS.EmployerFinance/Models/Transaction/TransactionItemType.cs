namespace SFA.DAS.EmployerFinance.Models.Transaction
{
    public enum TransactionItemType : byte
    {
        Unknown = 0,
        Declaration = 1,
        TopUp = 2,
        Payment = 3,
        Transfer = 4
    }
}
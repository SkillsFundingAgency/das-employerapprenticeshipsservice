namespace SFA.DAS.EmployerFinance.Models.Transaction
{
    public class AggregationData
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public decimal Balance { get; set; }
        public TransactionLine[] TransactionLines { get; set; }
    }
}
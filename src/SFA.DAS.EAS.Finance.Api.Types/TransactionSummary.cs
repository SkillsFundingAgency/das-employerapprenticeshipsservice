namespace SFA.DAS.EmployerFinance.Api.Types
{
    public class TransactionSummary
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public string Href { get; set; }
    }
}

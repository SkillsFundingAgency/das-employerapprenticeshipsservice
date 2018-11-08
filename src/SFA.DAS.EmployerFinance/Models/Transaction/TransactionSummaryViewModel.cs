namespace SFA.DAS.EmployerFinance.Models.Transaction
{
    public class TransactionSummaryViewModel : EmployerFinance.Models.IAccountResource
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public string Href { get; set; }
    }
}

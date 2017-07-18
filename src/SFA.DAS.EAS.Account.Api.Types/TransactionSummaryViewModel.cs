namespace SFA.DAS.EAS.Account.Api.Types
{
    public class TransactionSummaryViewModel : IAccountResource
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public string Href { get; set; }
    }
}

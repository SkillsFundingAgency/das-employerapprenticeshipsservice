namespace SFA.DAS.EAS.Finance.Api.Types
{
    public class TransactionSummaryViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public string Href { get; set; }
    }
}

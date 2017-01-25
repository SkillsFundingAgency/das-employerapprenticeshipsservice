namespace SFA.DAS.EAS.Api.Models
{
    public class AccountWithBalanceViewModel
    {
        public string AccountName { get; set; }

        public string AccountHashId { get; set; }

        public long AccountId { get; set; }

        public decimal Balance { get; set; }

        public string Href { get; set; }
    }
}
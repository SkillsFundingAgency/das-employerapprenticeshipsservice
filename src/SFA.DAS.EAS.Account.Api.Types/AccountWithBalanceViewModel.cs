namespace SFA.DAS.EAS.Account.Api.Types
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

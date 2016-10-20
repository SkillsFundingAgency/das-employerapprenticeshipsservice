namespace SFA.DAS.Account.Api.Client.Dtos
{
    public class AccountWithBalanceViewModel
    {
        public string AccountName { get; set; }

        public string AccountHashId { get; set; }

        public long AccountId { get; set; }

        public decimal Balance { get; set; }
    }
}

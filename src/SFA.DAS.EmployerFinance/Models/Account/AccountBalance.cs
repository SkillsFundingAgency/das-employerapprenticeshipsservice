namespace SFA.DAS.EmployerFinance.Models.Account
{
    public class AccountBalance
    {
        public long AccountId { get; set; }
        public decimal Balance { get; set; }
        public decimal TransferAllowance { get; set; }
        public int IsLevyPayer { get; set; }
    }
}
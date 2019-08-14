﻿namespace SFA.DAS.EmployerAccounts.Models.Account
{
    public class AccountBalance
    {
        public long AccountId { get; set; }
        public decimal Balance { get; set; }
        public decimal RemainingTransferAllowance { get; set; }
        public decimal StartingTransferAllowance { get; set; }
        public int IsLevyPayer { get; set; }
    }
}
﻿namespace SFA.DAS.EmployerAccounts.Models.TransferConnections
{
    public class TransferConnectionViewModel
    {
        public long FundingEmployerAccountId { get; set; }
        public string FundingEmployerHashedAccountId { get; set; }
        public string FundingEmployerPublicHashedAccountId { get; set; }
        public string FundingEmployerAccountName { get; set; }
    }
}
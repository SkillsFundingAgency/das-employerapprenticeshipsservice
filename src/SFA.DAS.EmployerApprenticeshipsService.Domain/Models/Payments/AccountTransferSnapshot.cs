using System;

namespace SFA.DAS.EAS.Domain.Models.Payments
{
    public class AccountTransferAllowanceSnapshot
    {
        public int Id { get; set; }
        public long AccountId { get; set; }
        public int  Year { get; set; }
        public decimal TransferAllowance { get; set; }
        public DateTime SnapshotTime { get; set; }
    }
}
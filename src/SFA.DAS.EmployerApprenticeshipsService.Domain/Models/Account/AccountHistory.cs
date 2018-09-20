using System;

namespace SFA.DAS.EAS.Domain.Models.Account
{
    /// <summary>
    ///     This matches the database layout
    /// </summary>
    public class AccountHistory
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
    }
}
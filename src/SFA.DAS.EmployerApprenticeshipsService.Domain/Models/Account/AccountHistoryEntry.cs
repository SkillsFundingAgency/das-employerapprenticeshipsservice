using System;

namespace SFA.DAS.EAS.Domain.Models.Account
{
    /// <remarks>
    ///     This is used by older code projecting from the results of a stored proc.
    /// </remarks>>
    public class AccountHistoryEntry
    {
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateRemoved { get; set; }
    }
}

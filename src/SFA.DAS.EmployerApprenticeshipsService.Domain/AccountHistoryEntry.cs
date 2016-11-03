using System;

namespace SFA.DAS.EAS.Domain
{
    public class AccountHistoryEntry
    {
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateRemoved { get; set; }
    }
}

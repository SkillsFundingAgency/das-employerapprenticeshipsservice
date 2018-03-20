using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("levy_declarations_processed")]
    public class LevyDeclarationsProcessedEvent: AccountMessageBase
    {
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public decimal LevyDeclaredInMonth { get; set; }

        public LevyDeclarationsProcessedEvent() : base(0, null, null) { }
        public LevyDeclarationsProcessedEvent(long accountId, string creatorName, string creatorUserRef)
            : base(accountId, creatorName, creatorUserRef) { }
    }
}
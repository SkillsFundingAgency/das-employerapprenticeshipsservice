using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage : AccountMessageBase
    {
        public string EmpRef { get; }

        public PayeSchemeCreatedMessage()
        { }

        public PayeSchemeCreatedMessage(string empRef, long accountId, string creatorName, string creatorUserRef) 
            : base(accountId, creatorName, creatorUserRef)
        {
            EmpRef = empRef;
        }
    }
}
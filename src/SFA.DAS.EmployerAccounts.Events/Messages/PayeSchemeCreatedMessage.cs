using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage
    {
        public PayeSchemeCreatedMessage()
        {

        }

        public PayeSchemeCreatedMessage(string empRef, long accountId, string signedByName)
        {
            EmpRef = empRef;
            AccountId = accountId;
            SignedByName = signedByName;
        }

        public string EmpRef { get; }

        public long AccountId { get; }

        public string SignedByName { get; set; }
    }
}
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage
    {
        public PayeSchemeCreatedMessage()
        {

        }

        public PayeSchemeCreatedMessage(string empRef, string hashedAccountId)
        {
            EmpRef = empRef;
            HashedAccountId = hashedAccountId;
        }

        public string EmpRef { get; }

        public string HashedAccountId { get; }
    }
}
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage
    {
        public PayeSchemeCreatedMessage()
        {

        }

        public PayeSchemeCreatedMessage(string empRef, string hashedAccountId, string signedByName)
        {
            EmpRef = empRef;
            HashedAccountId = hashedAccountId;
            SignedByName = signedByName;
        }

        public string EmpRef { get; }

        public string HashedAccountId { get; }

        public string SignedByName { get; set; }
    }
}
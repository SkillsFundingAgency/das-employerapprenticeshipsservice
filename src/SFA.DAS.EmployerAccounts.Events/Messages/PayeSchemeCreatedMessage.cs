using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage : Message
    {
        public PayeSchemeCreatedMessage()
        {

        }

        public PayeSchemeCreatedMessage(string payeSchemeRef, long accountId, string signedByName) : base(signedByName, accountId)
        {
            EmpRef = payeSchemeRef;
        }

        public string EmpRef { get; }
    }
}
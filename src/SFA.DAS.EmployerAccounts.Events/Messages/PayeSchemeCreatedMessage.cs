using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage : PersonMessage
    {
        public PayeSchemeCreatedMessage():base(string.Empty)
        {

        }

        public PayeSchemeCreatedMessage(string empRef, string signedInName) :base(signedInName)
        {
            EmpRef = empRef;
        }

        public string EmpRef { get; }
    }
}
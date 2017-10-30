using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage : Message
    {
        public PayeSchemeCreatedMessage()
        {

        }

        public PayeSchemeCreatedMessage(string empRef)
        {
            EmpRef = empRef;
        }

        public string EmpRef { get; }
    }
}
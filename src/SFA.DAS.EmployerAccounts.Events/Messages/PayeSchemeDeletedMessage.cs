using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("delete_paye_scheme")]
    public class PayeSchemeDeletedMessage : Message
    {
        public PayeSchemeDeletedMessage()
        {

        }

        public PayeSchemeDeletedMessage(string empRef)
        {
            EmpRef = empRef;
        }


        public string EmpRef { get; }
    }
}
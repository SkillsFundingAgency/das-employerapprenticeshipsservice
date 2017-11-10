using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("delete_paye_scheme")]
    public class PayeSchemeDeletedMessage : Message
    {
        public PayeSchemeDeletedMessage()
        {

        }

        public PayeSchemeDeletedMessage(string empRef, string companyName, long accountId, string signedByName) : base(signedByName, accountId)
        {
            EmpRef = empRef;
            CompanyName = companyName;
        }

        public string EmpRef { get; }

        public string CompanyName { get; }
    }
}
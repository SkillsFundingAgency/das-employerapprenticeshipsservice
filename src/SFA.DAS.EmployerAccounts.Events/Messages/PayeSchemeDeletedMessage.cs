using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("delete_paye_scheme")]
    public class PayeSchemeDeletedMessage : Message
    {
        public PayeSchemeDeletedMessage()
        {

        }

        public PayeSchemeDeletedMessage(string empRef, long accountId, string companyName)
        {
            EmpRef = empRef;
            AccountId = accountId;
            CompanyName = companyName;
        }

        public string EmpRef { get; }

        public long AccountId { get; }

        public string CompanyName { get; }
    }
}
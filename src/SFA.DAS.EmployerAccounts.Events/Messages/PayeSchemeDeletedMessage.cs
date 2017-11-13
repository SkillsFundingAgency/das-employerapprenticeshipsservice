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
            PayeSchemeRef = empRef;
            CompanyName = companyName;
        }

        public string PayeSchemeRef { get; }

        public string CompanyName { get; }
    }
}
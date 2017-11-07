using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("delete_paye_scheme")]
    public class PayeSchemeDeletedMessage : Message
    {
        public PayeSchemeDeletedMessage() : base(string.Empty, string.Empty)
        {

        }

        public PayeSchemeDeletedMessage(string empRef, string companyName, string hashedAccountId, string signedByName) : base(signedByName,  hashedAccountId)
        {
            EmpRef = empRef;
            CompanyName = companyName;
        }

        public string EmpRef { get; }

        public long AccountId { get; }

        public string CompanyName { get; }
    }
}
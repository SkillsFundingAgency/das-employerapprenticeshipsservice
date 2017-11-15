using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("delete_paye_scheme")]
    public class PayeSchemeDeletedMessage : Message
    {
        public string EmpRef { get; }
        public string CompanyName { get; }
        public string DeletedByName { get; }

        public PayeSchemeDeletedMessage(string empRef, string companyName, long accountId, string deletedByName) : base(accountId)
        {
            EmpRef = empRef;
            CompanyName = companyName;
            DeletedByName = deletedByName;
        }
    }
}
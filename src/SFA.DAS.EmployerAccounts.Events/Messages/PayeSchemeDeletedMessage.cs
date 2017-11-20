using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("delete_paye_scheme")]
    public class PayeSchemeDeletedMessage : Message
    {
        public string EmpRef { get; }
        public string OrganisationName { get; }
        public string DeletedByName { get; }
        public PayeSchemeDeletedMessage()
        { }

        public PayeSchemeDeletedMessage(string empRef, string organisationName, long accountId, string deletedByName) : base(accountId)
        {
            EmpRef = empRef;
            OrganisationName = organisationName;
            DeletedByName = deletedByName;
        }
    }
}
using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("delete_paye_scheme")]
    public class PayeSchemeDeletedMessage : AccountMessageBase
    {
        public string PayeScheme { get; }
        public string OrganisationName { get; }
       
        public PayeSchemeDeletedMessage()
        { }

        public PayeSchemeDeletedMessage(string payeScheme, string organisationName, long accountId, string creatorName, string creatorUserRef) 
            : base(accountId, creatorName, creatorUserRef)
        {
            PayeScheme = payeScheme;
            OrganisationName = organisationName;
        }
    }
}
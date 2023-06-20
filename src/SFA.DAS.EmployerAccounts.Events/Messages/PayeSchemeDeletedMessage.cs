using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
[MessageGroup("delete_paye_scheme")]
public class PayeSchemeDeletedMessage : AccountMessageBase
{
    //We have protected setters to support json serialsation due to the empty constructor
    public string PayeScheme { get; protected set; }
    public string OrganisationName { get; protected set; }
       
    public PayeSchemeDeletedMessage()
    { }

    public PayeSchemeDeletedMessage(string payeScheme, string organisationName, long accountId, string creatorName, string creatorUserRef) 
        : base(accountId, creatorName, creatorUserRef)
    {
        PayeScheme = payeScheme;
        OrganisationName = organisationName;
    }
}
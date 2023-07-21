using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
[MessageGroup("legal_entity_added")]
public class LegalEntityAddedMessage : AccountMessageBase
{
    //We have protected setters to support json serialsation due to the empty constructor
    public string OrganisationName { get; protected set; }
    public long AgreementId { get; protected set; }
    public long LegalEntityId { get; protected set; }

    public LegalEntityAddedMessage()
    { }

    public LegalEntityAddedMessage(long accountId, long aggreementId, string organisationName, long legalEntityId, string creatorName, string creatorUserRef) : base(accountId, creatorName, creatorUserRef)
    {
        AgreementId = aggreementId;
        OrganisationName = organisationName;
        LegalEntityId = legalEntityId;
    }
}
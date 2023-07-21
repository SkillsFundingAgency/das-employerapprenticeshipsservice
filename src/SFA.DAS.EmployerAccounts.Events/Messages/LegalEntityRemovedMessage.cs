using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
[MessageGroup("legal_entity_removed")]
public class LegalEntityRemovedMessage : AccountMessageBase
{
    //We have protected setters to support json serialsation due to the empty constructor
    public long AgreementId { get; protected set; }
    public bool AgreementSigned { get; protected set; }
    public long LegalEntityId { get; protected set; }
    public string OrganisationName { get; set; }
       
    public LegalEntityRemovedMessage()
    { }

    public LegalEntityRemovedMessage(long accountId,  long aggreementId, bool agreementSigned, long legalEntityId, string organisationName, string creatorName, string creatorUserRef) 
        : base(accountId, creatorName, creatorUserRef)
    {
        AgreementId = aggreementId;
        AgreementSigned = agreementSigned;
        LegalEntityId = legalEntityId;
        OrganisationName = organisationName;
    }
}
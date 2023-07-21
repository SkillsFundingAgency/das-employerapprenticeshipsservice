using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
[MessageGroup("agreement_signed")]
public class AgreementSignedMessage : AccountMessageBase
{
    //We have protected setters to support json serialsation due to the empty constructor
    public string OrganisationName { get; protected set; }
    public long AgreementId { get; protected set; }
    public long LegalEntityId { get; protected set; }
    public bool CohortCreated { get; protected set; }

    public AgreementSignedMessage()
    {   }

    public AgreementSignedMessage(long accountId, long agreementId, string organisationName, long legalEntityId, bool cohortCreated, string creatorName, string creatorUserRef) : base(accountId, creatorName, creatorUserRef)
    {
        AgreementId = agreementId;
        LegalEntityId = legalEntityId;
        OrganisationName = organisationName;
        CohortCreated = cohortCreated;
    }
}
﻿using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("agreement_created")]
    public class AgreementCreatedMessage : AccountMessageBase
    {
        //We have protected setters to support json serialsation due to the empty constructor
        public string OrganisationName { get; protected set; }
        public long AgreementId { get; protected set; }
        public long LegalEntityId { get; protected set; }

        public AgreementCreatedMessage()
        { }

        public AgreementCreatedMessage(long accountId, long aggreementId, string organisationName, long legalEntityId, string creatorName, Guid externalUserId) : base(accountId, creatorName, externalUserId)
        {
            AgreementId = aggreementId;
            OrganisationName = organisationName;
            LegalEntityId = legalEntityId;
        }
    }
}

﻿using System;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class SignedAgreementEvent : Event
    {
        public long AccountId { get; set; }
        public string UserName { get; set; }
        public Guid UserRef { get; set; }
        public string OrganisationName { get; set; }
        public long AgreementId { get; set; }
        public long LegalEntityId { get; set; }
        public bool CohortCreated { get; set; }
        public AgreementType AgreementType { get; set; }
        public int SignedAgreementVersion { get; set; }
        public string CorrelationId { get; set; }
    }
}

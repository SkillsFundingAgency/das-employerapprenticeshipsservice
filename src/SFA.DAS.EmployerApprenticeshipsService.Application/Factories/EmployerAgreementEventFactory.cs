﻿using SFA.DAS.EAS.Account.Api.Types.Events.Agreement;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Factories
{
    public class EmployerAgreementEventFactory : IEmployerAgreementEventFactory
    {
        private readonly EmployerApprenticeshipApiConfiguration _configuration;

        public EmployerAgreementEventFactory(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration.EmployerApprenticeshipApi;
        }

        public AgreementSignedEvent CreateSignedEvent(string hashedAccountId, string hashedLegalEntityId, string hashedAgreementId)
        {
           return new AgreementSignedEvent
           {
               ResourceUrl = $"{_configuration.BaseUrl}api/accounts/{hashedAccountId}/legalEntities/{hashedLegalEntityId}/agreements/{hashedAgreementId}"
           };
        }

        public AgreementRemovedEvent RemoveAgreementEvent(string hashedAgreementId)
        {
            return new AgreementRemovedEvent {HashedAgreementId = hashedAgreementId};
        }
    }
}

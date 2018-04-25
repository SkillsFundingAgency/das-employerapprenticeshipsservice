﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesQueryHandler : IAsyncRequestHandler<GetAccountLegalEntitiesRequest, GetAccountLegalEntitiesResponse>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IValidator<GetAccountLegalEntitiesRequest> _validator;

        public GetAccountLegalEntitiesQueryHandler(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository, IValidator<GetAccountLegalEntitiesRequest> validator)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (employerAgreementRepository == null)
                throw new ArgumentNullException(nameof(employerAgreementRepository));
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _validator = validator;
        }

        public async Task<GetAccountLegalEntitiesResponse> Handle(GetAccountLegalEntitiesRequest message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var membership = await _membershipRepository.GetCaller(message.HashedLegalEntityId, message.UserId);

            if (membership == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Caller is not a member of this account" } });

            var legalEntities = await _employerAgreementRepository.GetLegalEntitiesLinkedToAccount(membership.AccountId, message.SignedOnly);

            return new GetAccountLegalEntitiesResponse
            {
                Entites = new LegalEntities
                {
                    LegalEntityList = legalEntities
                }
            };
        }
    }
}

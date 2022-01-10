﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Organisation;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove
{
    public class GetAccountLegalEntityRemoveQueryHandler : IAsyncRequestHandler<GetAccountLegalEntityRemoveRequest, GetAccountLegalEntityRemoveResponse>
    {
        private readonly IValidator<GetAccountLegalEntityRemoveRequest> _validator;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityHashingService;
        private readonly ICommitmentsV2ApiClient _commitmentV2ApiClient;

        public GetAccountLegalEntityRemoveQueryHandler(
            IValidator<GetAccountLegalEntityRemoveRequest> validator,
            IEmployerAgreementRepository employerAgreementRepository, 
            IHashingService hashingService,
            IAccountLegalEntityPublicHashingService accountLegalEntityHashingService,
            ICommitmentsV2ApiClient commitmentV2ApiClient)
        {
            _validator = validator;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _accountLegalEntityHashingService = accountLegalEntityHashingService;
            _commitmentV2ApiClient = commitmentV2ApiClient;
        }

        public async Task<GetAccountLegalEntityRemoveResponse> Handle(GetAccountLegalEntityRemoveRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var accountLegalEntityId = _accountLegalEntityHashingService.DecodeValue(message.HashedAccountLegalEntityId);
            var accountLegalEntity = await _employerAgreementRepository.GetAccountLegalEntity(accountLegalEntityId);

            var result = await _employerAgreementRepository.GetAccountLegalEntityAgreements(accountLegalEntityId);
            if (result == null) return new GetAccountLegalEntityRemoveResponse();

            if (result.Any(x => x.SignedDate.HasValue))
            {
                return new GetAccountLegalEntityRemoveResponse
                {
                    CanBeRemoved = await SetRemovedStatusBasedOnCommitments(accountId, accountLegalEntity),
                    HasSignedAgreement = true,
                    Name = accountLegalEntity.Name
                };
            }

            return new GetAccountLegalEntityRemoveResponse
            {
                CanBeRemoved = true,
                HasSignedAgreement = false,
                Name = accountLegalEntity.Name
            };
        }

        private async Task<bool> SetRemovedStatusBasedOnCommitments(long accountId, AccountLegalEntityModel accountLegalEntityModel)
        {
            var commitments = await _commitmentV2ApiClient.GetEmployerAccountSummary(accountId);

            var commitmentConnectedToEntity = commitments?.ApprenticeshipStatusSummaryResponse.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.LegalEntityIdentifier)
                && c.LegalEntityIdentifier.Equals(accountLegalEntityModel.Identifier)
                && c.LegalEntityOrganisationType == accountLegalEntityModel.OrganisationType);

            return commitmentConnectedToEntity == null || (commitmentConnectedToEntity.ActiveCount +
                                                           commitmentConnectedToEntity.PendingApprovalCount +
                                                           commitmentConnectedToEntity.WithdrawnCount +
                                                           commitmentConnectedToEntity.PausedCount) == 0;
        }
    }
}
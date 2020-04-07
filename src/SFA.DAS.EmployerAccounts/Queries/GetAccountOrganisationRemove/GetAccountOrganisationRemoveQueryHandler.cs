using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.Organisation;
using OrganisationType = SFA.DAS.Common.Domain.Types.OrganisationType;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountOrganisationRemove
{
    public class GetAccountOrganisationRemoveQueryHandler : IAsyncRequestHandler<GetAccountOrganisationRemoveRequest, GetAccountOrganisationRemoveResponse>
    {
        private readonly IValidator<GetAccountOrganisationRemoveRequest> _validator;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityHashingService;
        private readonly IEmployerCommitmentApi _employerCommitmentApi;

        public GetAccountOrganisationRemoveQueryHandler(
            IValidator<GetAccountOrganisationRemoveRequest> validator,
            IEmployerAgreementRepository employerAgreementRepository, 
            IHashingService hashingService,
            IAccountLegalEntityPublicHashingService accountLegalEntityHashingService,
            IEmployerCommitmentApi employerCommitmentApi)
        {
            _validator = validator;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _accountLegalEntityHashingService = accountLegalEntityHashingService;
            _employerCommitmentApi = employerCommitmentApi;
        }

        public async Task<GetAccountOrganisationRemoveResponse> Handle(GetAccountOrganisationRemoveRequest message)
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
            if (result == null) return new GetAccountOrganisationRemoveResponse();

            if (result.Any(x => x.StatusId == EmployerAgreementStatus.Signed))
            {
                return new GetAccountOrganisationRemoveResponse
                {
                    CanBeRemoved = await SetRemovedStatusBasedOnCommitments(accountId, accountLegalEntity),
                    HasSignedAgreement = true,
                    Name = accountLegalEntity.Name
                };
            }

            return new GetAccountOrganisationRemoveResponse
            {
                CanBeRemoved = true,
                HasSignedAgreement = false,
                Name = accountLegalEntity.Name
            };
        }

        private async Task<bool> SetRemovedStatusBasedOnCommitments(long accountId, AccountLegalEntityModel accountLegalEntityModel)
        {
            var commitments = await _employerCommitmentApi.GetEmployerAccountSummary(accountId);

            commitments.Add(new ApprenticeshipStatusSummary
            {
                ActiveCount = 1,
                LegalEntityIdentifier = "12107942",
                LegalEntityOrganisationType = OrganisationType.CompaniesHouse
            });

            var commitmentConnectedToEntity = commitments.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.LegalEntityIdentifier)
                && c.LegalEntityIdentifier.Equals(accountLegalEntityModel.Identifier)
                && c.LegalEntityOrganisationType == accountLegalEntityModel.OrganisationType);

            return commitmentConnectedToEntity == null || (commitmentConnectedToEntity.ActiveCount +
                                                           commitmentConnectedToEntity.PendingApprovalCount +
                                                           commitmentConnectedToEntity.PausedCount) == 0;
        }
    }
}
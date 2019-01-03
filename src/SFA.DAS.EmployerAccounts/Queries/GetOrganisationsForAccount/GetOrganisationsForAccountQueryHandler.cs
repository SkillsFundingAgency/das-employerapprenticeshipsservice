using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationsForAccount
{
    public class GetOrganisationsForAccountQueryHandler : IAsyncRequestHandler<GetOrganisationsForAccountRequest, GetOrganisationsForAccountResponse>
    {
        private readonly IValidator<GetOrganisationsForAccountRequest> _validator;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;
        private readonly IEmployerCommitmentApi _employerCommitmentApi;

        public GetOrganisationsForAccountQueryHandler(IValidator<GetOrganisationsForAccountRequest> validator, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService, IEmployerCommitmentApi employerCommitmentApi)
        {
            _validator = validator;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _employerCommitmentApi = employerCommitmentApi;
        }

        public async Task<GetOrganisationsForAccountResponse> Handle(GetOrganisationsForAccountRequest message)
        {
            await ValidateRequest(message);

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            var organisations = await GetOrganisationsForAccount(accountId, message.HashedAccountId);

            await DetermineIfOrganisationsCanBeRemoved(organisations, accountId);

            return new GetOrganisationsForAccountResponse { Organisation = organisations };
        }

        private async Task<List<RemoveOrganisationView>> GetOrganisationsForAccount(long accountId, string hashedAccountId)
        {
            var organisations = await _employerAgreementRepository.GetLegalEntitiesLinkedToAccount(accountId, signedOnly: false);
            return organisations.Select(organisation => new RemoveOrganisationView
            {
                AccountLegalEntityId = organisation.AccountLegalEntityId,
                CanBeRemoved = false,
                HashedAccountId = hashedAccountId,
                HasSignedAgreement = organisation.SignedAgreementId.HasValue,
                LegalEntityCode = organisation.Code,
                LegalEntitySource = organisation.Source,
                Name = organisation.Name
            }).ToList();
        }

        private async Task DetermineIfOrganisationsCanBeRemoved(List<RemoveOrganisationView> organisations, long accountId)
        {
            if (organisations.Count < 2)
            {
                return;
            }

            var lazyCommitments = new Lazy<Task<List<ApprenticeshipStatusSummary>>>(() =>_employerCommitmentApi.GetEmployerAccountSummary(accountId));

            foreach (var organisation in organisations)
            {
                if (organisation.HasSignedAgreement)
                {
                    var hasCommitments = await HasCommitments(organisation, lazyCommitments);
                    organisation.CanBeRemoved = !hasCommitments;
                }
                else
                { 
                    organisation.CanBeRemoved = true;
                }
            }
        }

        private async Task<bool> HasCommitments(RemoveOrganisationView organisation, Lazy<Task<List<ApprenticeshipStatusSummary>>> lazyCommitments)
        {
            var commitments = await lazyCommitments.Value;

            var commitmentConnectedToEntity = commitments.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.LegalEntityIdentifier)
                && c.LegalEntityIdentifier.Equals(organisation.LegalEntityCode)
                && c.LegalEntityOrganisationType == organisation.LegalEntitySource);

            return commitmentConnectedToEntity != null && (commitmentConnectedToEntity.ActiveCount > 0 || commitmentConnectedToEntity.PendingApprovalCount > 0 || commitmentConnectedToEntity.PausedCount > 0);
        }

        private async Task ValidateRequest(GetOrganisationsForAccountRequest message)
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
        }
    }
}
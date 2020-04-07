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
using OrganisationType = SFA.DAS.Common.Domain.Types.OrganisationType;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementsRemove
{
    public class GetAccountEmployerAgreementsRemoveQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementsRemoveRequest, GetAccountEmployerAgreementsRemoveResponse>
    {
        private readonly IValidator<GetAccountEmployerAgreementsRemoveRequest> _validator;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;
        private readonly IEmployerCommitmentApi _employerCommitmentApi;

        public GetAccountEmployerAgreementsRemoveQueryHandler(IValidator<GetAccountEmployerAgreementsRemoveRequest> validator, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService, IEmployerCommitmentApi employerCommitmentApi)
        {
            _validator = validator;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _employerCommitmentApi = employerCommitmentApi;
        }

        public async Task<GetAccountEmployerAgreementsRemoveResponse> Handle(GetAccountEmployerAgreementsRemoveRequest message)
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

            var result = await _employerAgreementRepository.GetEmployerAgreementsToRemove(accountId);
            if (result == null) return new GetAccountEmployerAgreementsRemoveResponse();

            SetAgreementHashedIds(result, message.HashedAccountId);

            if (result.Count == 1)
            {
                result.First().CanBeRemoved = false;
                return new GetAccountEmployerAgreementsRemoveResponse { Agreements = result };
            }

            SetPendingAgreementsAsCanBeRemoved(result);
            await SetSignedAgreementsCanBeRemovedBasedOnCommitments(accountId, result);
            SetOtherAgreementsAsCantBeRemoved(result);

            return new GetAccountEmployerAgreementsRemoveResponse { Agreements = result };
        }

        private void SetOtherAgreementsAsCantBeRemoved(List<RemoveEmployerAgreementView> result)
        {
            foreach (var agreement in result.Where(x => x.Status != EmployerAgreementStatus.Signed && x.Status != EmployerAgreementStatus.Pending))
            {
                agreement.CanBeRemoved = false;
            }
        }

        private void SetAgreementHashedIds(List<RemoveEmployerAgreementView> result, string hashedAccountId)
        {
            foreach (var removeEmployerAgreementView in result)
            {
                removeEmployerAgreementView.HashedAgreementId = _hashingService.HashValue((long)removeEmployerAgreementView.Id);
                removeEmployerAgreementView.HashedAccountId = hashedAccountId;
            }
        }

        private async Task SetSignedAgreementsCanBeRemovedBasedOnCommitments(long accountId, List<RemoveEmployerAgreementView> result)
        {
            if(!result.Any(x => x.Status == EmployerAgreementStatus.Signed))
            {
                return;
            }

            var commitments = await _employerCommitmentApi.GetEmployerAccountSummary(accountId);

            commitments.Add(new ApprenticeshipStatusSummary
            {
                ActiveCount = 1,
                LegalEntityIdentifier = "12107942",
                LegalEntityOrganisationType = OrganisationType.CompaniesHouse
            });

            foreach (var removeEmployerAgreementView in result.Where(x => x.Status == EmployerAgreementStatus.Signed))
            {
                var commitmentConnectedToEntity = commitments.FirstOrDefault(c =>
                    !string.IsNullOrEmpty(c.LegalEntityIdentifier)
                    && c.LegalEntityIdentifier.Equals(removeEmployerAgreementView.LegalEntityCode)
                    && c.LegalEntityOrganisationType == removeEmployerAgreementView.LegalEntitySource);

                if (commitmentConnectedToEntity != null &&
                    (commitmentConnectedToEntity.ActiveCount +
                     commitmentConnectedToEntity.PendingApprovalCount +
                     commitmentConnectedToEntity.PausedCount) != 0)
                {
                    removeEmployerAgreementView.CanBeRemoved = false;
                }
                else
                {
                    removeEmployerAgreementView.CanBeRemoved = true;
                }
            }
        }

        private static void SetPendingAgreementsAsCanBeRemoved(List<RemoveEmployerAgreementView> result)
        {
            foreach (var agreement in result.Where(x => x.Status == EmployerAgreementStatus.Pending))
            {
                agreement.CanBeRemoved = true;
            }
        }
    }
}
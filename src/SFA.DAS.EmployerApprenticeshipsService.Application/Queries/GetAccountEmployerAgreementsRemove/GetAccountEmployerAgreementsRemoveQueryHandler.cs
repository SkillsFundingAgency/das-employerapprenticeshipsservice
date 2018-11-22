using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove
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

            var commitments = new List<ApprenticeshipStatusSummary>();

            if (result != null && result.Count == 1)
            {
                result.First().CanBeRemoved = false;
            }
            else
            {
                commitments = await _employerCommitmentApi.GetEmployerAccountSummary(accountId);
            }

            if (result == null) return new GetAccountEmployerAgreementsRemoveResponse();

            foreach (var removeEmployerAgreementView in result)
            {
                removeEmployerAgreementView.HashedAccountId = message.HashedAccountId;

                if (result.Count == 1) continue;

                switch (removeEmployerAgreementView.Status)
                {
                    case EmployerAgreementStatus.Pending:
                        removeEmployerAgreementView.CanBeRemoved = true;
                        break;
                    case EmployerAgreementStatus.Signed:

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

                        break;
                    default:
                        removeEmployerAgreementView.CanBeRemoved = false;
                        break;
                }
            }
            return new GetAccountEmployerAgreementsRemoveResponse { Agreements = result };
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Commands.RemoveLegalEntity
{
    public class RemoveLegalEntityCommandValidator : IValidator<RemoveLegalEntityCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;
        private readonly IEmployerCommitmentApi _employerCommitmentApi;

        public RemoveLegalEntityCommandValidator(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService, IEmployerCommitmentApi employerCommitmentApi)
        {
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _employerCommitmentApi = employerCommitmentApi;
        }

        public ValidationResult Validate(RemoveLegalEntityCommand item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(RemoveLegalEntityCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }
            if (string.IsNullOrEmpty(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId));
            }
            if (string.IsNullOrEmpty(item.HashedLegalAgreementId))
            {
                validationResult.AddError(nameof(item.HashedLegalAgreementId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var member = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

            if (member == null || !member.RoleId.Equals((short)Role.Owner))
            {
                validationResult.IsUnauthorized = true;
                return validationResult;
            }

            var accountId = _hashingService.DecodeValue(item.HashedAccountId);
            var legalEntites = await _employerAgreementRepository.GetLegalEntitiesLinkedToAccount(accountId, false);

            if (legalEntites != null && legalEntites.Count == 1)
            {
                validationResult.AddError(nameof(item.HashedLegalAgreementId), "There must be at least one legal entity on the account");
                return validationResult;
            }

            var agreementId = _hashingService.DecodeValue(item.HashedLegalAgreementId);
            var agreement = await _employerAgreementRepository.GetEmployerAgreement(agreementId);

            if (agreement.Status == EmployerAgreementStatus.Signed)
            {
                
                var commitments = await _employerCommitmentApi.GetEmployerAccountSummary(accountId);

                // BUG: the look up on LegalEntityIdentifier by itself is invalid - it should also be by source, but this is not provided by commitments API
                var returnValue = commitments
                                    .FirstOrDefault(c => 
                                        !string.IsNullOrEmpty(c.LegalEntityIdentifier) 
                                        && c.LegalEntityIdentifier.Equals(agreement.LegalEntityCode));
                
                if (returnValue != null && (returnValue.ActiveCount + returnValue.PausedCount + returnValue.PendingApprovalCount) != 0)
                {
                    validationResult.AddError(nameof(item.HashedLegalAgreementId), "Agreement has already been signed and has active commitments");
                    return validationResult;
                }
            }
            
            
            if (agreement.AccountId != accountId )
            {
                validationResult.IsUnauthorized = true;
                return validationResult;
            }

            return validationResult;
        }
    }
}

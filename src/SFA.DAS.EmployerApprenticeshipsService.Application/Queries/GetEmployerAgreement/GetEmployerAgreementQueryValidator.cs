using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementQueryValidator : IValidator<GetEmployerAgreementRequest>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerAgreementQueryValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetEmployerAgreementRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAgreementRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAgreementId))
            {
                validationResult.AddError(nameof(item.HashedAgreementId));
            }

            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                validationResult.AddError(nameof(item.ExternalUserId));
            }

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var caller = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);
            
            if (caller == null || caller.RoleId != (int)Role.Owner)
            {
                validationResult.IsUnauthorized = true;
            }
                
            return validationResult;
        }
    }
}

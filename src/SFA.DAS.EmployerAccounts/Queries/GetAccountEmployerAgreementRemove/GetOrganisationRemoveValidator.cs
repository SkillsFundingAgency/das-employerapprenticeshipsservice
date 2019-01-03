using System;
using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementRemove
{
    public class GetOrganisationRemoveValidator : IValidator<GetOrganisationRemoveRequest>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetOrganisationRemoveValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetOrganisationRemoveRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetOrganisationRemoveRequest item)
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
            if (string.IsNullOrEmpty(item.AccountLegalEntityPublicHashedId))
            {
                validationResult.AddError(nameof(item.AccountLegalEntityPublicHashedId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var user = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

            if (user == null || user.RoleId != (short) Role.Owner)
            {
                validationResult.IsUnauthorized = true;
            }

            return validationResult;
        }
    }
}
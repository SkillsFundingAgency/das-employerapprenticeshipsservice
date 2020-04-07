using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountOrganisationRemove
{
    public class GetAccountOrganisationRemoveValidator : IValidator<GetAccountOrganisationRemoveRequest>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetAccountOrganisationRemoveValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetAccountOrganisationRemoveRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountOrganisationRemoveRequest item)
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
            if (string.IsNullOrEmpty(item.HashedAccountLegalEntityId))
            {
                validationResult.AddError(nameof(item.HashedAccountLegalEntityId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var user = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

            if (user == null || user.Role != Role.Owner)
            {
                validationResult.IsUnauthorized = true;
            }

            return validationResult;
        }
    }
}
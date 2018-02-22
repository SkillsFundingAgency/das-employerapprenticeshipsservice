using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceRequestValidator : IValidator<GetTransferAllowanceQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetTransferAllowanceRequestValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetTransferAllowanceQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetTransferAllowanceQuery item)
        {
            var result = new ValidationResult();

            var caller = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);
            result.IsUnauthorized = caller == null;

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                result.ValidationDictionary.Add(nameof(item.HashedAccountId),
                    "Hashed Account Id cannot be null or empty.");
            }

            return result;
        }
    }
}

using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetAccountStats
{
    public class GetAccountStatsQueryValidator : IValidator<GetAccountStatsQuery>
    {
        private readonly IMembershipRepository _repository;

        public GetAccountStatsQueryValidator(IMembershipRepository repository)
        {
            _repository = repository;
        }

        public ValidationResult Validate(GetAccountStatsQuery item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountStatsQuery item)
        {
            var validationResult = new ValidationResult();
            if (item.ExternalUserId.Equals(Guid.Empty))
            {
                validationResult.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }

            if (validationResult.IsValid())
            {
                var member = await _repository.GetCaller(item.HashedAccountId, item.ExternalUserId);
                if (member == null)
                {
                    validationResult.AddError(nameof(member), "Unauthorised: User not connected to account");
                    validationResult.IsUnauthorized = true;
                }
            }

            return validationResult;
        }
    }
}

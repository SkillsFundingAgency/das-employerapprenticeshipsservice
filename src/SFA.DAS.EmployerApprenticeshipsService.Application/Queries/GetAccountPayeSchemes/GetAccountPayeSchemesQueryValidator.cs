using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesQueryValidator : IValidator<GetAccountPayeSchemesQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetAccountPayeSchemesQueryValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetAccountPayeSchemesQuery item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountPayeSchemesQuery query)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(query.HashedAccountId))
            {
                validationResult.ValidationDictionary.Add(nameof(query.HashedAccountId), "Hashed account ID has not been supplied");
            }

            if (string.IsNullOrEmpty(query.ExternalUserId))
            {
                validationResult.ValidationDictionary.Add(nameof(query.ExternalUserId), "User ID has not been supplied");
            }

            if (validationResult.IsValid())
            {
                var member = await _membershipRepository.GetCaller(query.HashedAccountId, query.ExternalUserId);
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

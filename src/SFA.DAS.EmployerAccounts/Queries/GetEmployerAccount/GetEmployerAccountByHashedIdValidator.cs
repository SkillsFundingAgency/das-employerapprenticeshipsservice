using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount
{
    public class GetEmployerAccountByHashedIdValidator : IValidator<GetEmployerAccountHashedQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerAccountByHashedIdValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetEmployerAccountHashedQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAccountHashedQuery item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.UserId))
            {
                result.AddError(nameof(item.UserId), "UserId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                result.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }

            if (result.IsValid())
            {
                var membership = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

                if (membership == null)
                    result.IsUnauthorized = true;
            }


            return result;
        }
    }
}
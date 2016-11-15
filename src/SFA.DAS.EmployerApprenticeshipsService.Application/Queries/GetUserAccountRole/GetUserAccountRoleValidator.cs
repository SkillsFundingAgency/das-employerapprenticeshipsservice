using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetUserAccountRole
{
    public class GetUserAccountRoleValidator : IValidator<GetUserAccountRoleQuery>
    {
        public ValidationResult Validate(GetUserAccountRoleQuery item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                result.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
            }

            if (item.AccountId < 1)
            {
                result.AddError(nameof(item.AccountId), "AccountId has not been supplied");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetUserAccountRoleQuery item)
        {
            throw new NotImplementedException();
        }
    }
}

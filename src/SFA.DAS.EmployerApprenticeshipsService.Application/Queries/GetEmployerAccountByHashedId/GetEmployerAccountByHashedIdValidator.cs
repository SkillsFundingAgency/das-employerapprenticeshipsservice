using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId
{
    public class GetEmployerAccountByHashedIdValidator : IValidator<GetEmployerAccountByHashedIdQuery>
    {
        public ValidationResult Validate(GetEmployerAccountByHashedIdQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetEmployerAccountByHashedIdQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
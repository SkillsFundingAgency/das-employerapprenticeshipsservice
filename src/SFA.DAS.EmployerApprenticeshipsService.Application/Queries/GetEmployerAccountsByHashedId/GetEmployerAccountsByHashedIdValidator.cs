using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountsByHashedId
{
    public class GetEmployerAccountsByHashedIdValidator : IValidator<GetEmployerAccountsByHashedIdQuery>
    {
        public ValidationResult Validate(GetEmployerAccountsByHashedIdQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetEmployerAccountsByHashedIdQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
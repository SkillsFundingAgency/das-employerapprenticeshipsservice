using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetUserByRef
{
    public class GetUserByRefQueryValidator : IValidator<GetUserByRefQuery>
    {
        public ValidationResult Validate(GetUserByRefQuery query)
        {
            var validationResult = new ValidationResult();

            if (query.ExternalUserId.Equals(Guid.Empty))
            {
                validationResult.AddError(nameof(query.ExternalUserId), "User ref must not be empty or null");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetUserByRefQuery query)
        {
            throw new NotImplementedException();
        }
    }
}

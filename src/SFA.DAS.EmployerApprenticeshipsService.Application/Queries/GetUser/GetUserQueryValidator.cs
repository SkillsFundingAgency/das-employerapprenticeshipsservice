using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUser
{
    class GetUserQueryValidator : IValidator<GetUserQuery>
    {
        public ValidationResult Validate(GetUserQuery item)
        {
            var result = new ValidationResult();

            if (item.UserId < 1)
            {
                result.AddError(nameof(item.UserId), "UserId has not been supplied");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetUserQuery item)
        {
            throw new NotImplementedException();
        }
    }
}

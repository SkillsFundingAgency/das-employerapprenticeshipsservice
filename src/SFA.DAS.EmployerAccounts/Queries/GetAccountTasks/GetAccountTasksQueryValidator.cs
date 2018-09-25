using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTasks
{
    public class GetAccountTasksQueryValidator : IValidator<GetAccountTasksQuery>
    {
        public ValidationResult Validate(GetAccountTasksQuery item)
        {
            var validationResult = new ValidationResult();

            if (item?.AccountId == null || item.AccountId == default(int))
            {
                validationResult.AddError(nameof(item.AccountId), "Account Id must be supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountTasksQuery item)
        {
            throw new NotImplementedException();
        }
    }
}

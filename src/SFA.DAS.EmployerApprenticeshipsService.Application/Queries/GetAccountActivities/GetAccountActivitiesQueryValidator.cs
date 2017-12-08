using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetAccountActivities
{
    public class GetAccountActivitiesQueryValidator : IValidator<GetAccountActivitiesQuery>
    {
        public ValidationResult Validate(GetAccountActivitiesQuery item)
        {
            var validationResult = new ValidationResult();

            if (item?.AccountId == null || item.AccountId == default(int))
            {
                validationResult.AddError(nameof(item.AccountId), "Account Id must be supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountActivitiesQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
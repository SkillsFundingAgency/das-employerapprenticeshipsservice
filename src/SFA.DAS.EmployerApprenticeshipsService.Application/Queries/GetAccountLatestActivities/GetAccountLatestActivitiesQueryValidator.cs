using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLatestActivities
{
    public class GetAccountLatestActivitiesQueryValidator : IValidator<GetAccountLatestActivitiesQuery>
    {
        public ValidationResult Validate(GetAccountLatestActivitiesQuery item)
        {
            var validationResult = new ValidationResult();

            if (item?.AccountId == null || item.AccountId == default(int))
            {
                validationResult.AddError(nameof(item.AccountId), "Account Id must be supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountLatestActivitiesQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccountsByDateRange
{
    public class GetPagedEmployerAccountsByDateRangeValidator : IValidator<GetPagedEmployerAccountsByDateRangeQuery>
    {
        public ValidationResult Validate(GetPagedEmployerAccountsByDateRangeQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.PageNumber == 0)
            {
                validationResult.AddError(nameof(item.PageNumber));
            }
            if (item.PageSize== 0)
            {
                validationResult.AddError(nameof(item.PageSize));
            }
            return validationResult;
            
        }

        public Task<ValidationResult> ValidateAsync(GetPagedEmployerAccountsByDateRangeQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
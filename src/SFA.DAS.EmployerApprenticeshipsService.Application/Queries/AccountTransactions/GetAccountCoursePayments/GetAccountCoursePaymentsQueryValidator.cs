using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountCoursePayments
{
    public class GetAccountCoursePaymentsQueryValidator : IValidator<GetAccountCoursePaymentsQuery>
    {
        public ValidationResult Validate(GetAccountCoursePaymentsQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountCoursePaymentsQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId), "Account ID has not been supplied");
            }

            if (item.UkPrn == 0)
            {
                validationResult.AddError(nameof(item.UkPrn), "UkPrn has not been supplied");
            }

            if (string.IsNullOrEmpty(item.CourseName))
            {
                validationResult.AddError(nameof(item.CourseName), "Course name has not been supplied");
            }

            if (item.FromDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.FromDate), "From date has not been supplied");
            }

            if (item.ToDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.ToDate), "To date has not been supplied");
            }
            
            return validationResult;
        }
    }
}

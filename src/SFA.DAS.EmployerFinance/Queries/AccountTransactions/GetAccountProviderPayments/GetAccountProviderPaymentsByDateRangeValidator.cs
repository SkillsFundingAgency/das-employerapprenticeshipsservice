using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.AccountTransactions.GetAccountProviderPayments
{
    public class GetAccountProviderPaymentsByDateRangeValidator : IValidator<GetAccountProviderPaymentsByDateRangeQuery>
    {
        public ValidationResult Validate(GetAccountProviderPaymentsByDateRangeQuery item)
        {
           throw new NotImplementedException();
        }

        public Task<ValidationResult> ValidateAsync(GetAccountProviderPaymentsByDateRangeQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId), "Account ID has not been supplied");
            }

            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.UkPrn), "UKPRN has not been supplied");
            }

            if (item.FromDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.FromDate), "From date has not been supplied");
            }

            if (item.ToDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.ToDate), "To date has not been supplied");
            }

            return Task.FromResult(validationResult);
        }
    }
}
using System;
using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.FindAccountProviderPayments
{
    public class FindAccountProviderPaymentsQueryValidator : IValidator<FindAccountProviderPaymentsQuery>
    {
        private readonly IAuthorizationService _authorizationService;

        public FindAccountProviderPaymentsQueryValidator(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public ValidationResult Validate(FindAccountProviderPaymentsQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(FindAccountProviderPaymentsQuery item)
        {
            var result = new ValidationResult();

            if (item.UkPrn == default(long))
            {
                result.AddError(nameof(item.UkPrn), "UkPrn has not been supplied");
            }

            if (item.FromDate == DateTime.MinValue)
            {
                result.AddError(nameof(item.FromDate), "From date has not been supplied");
            }
            if (item.ToDate == DateTime.MinValue)
            {
                result.AddError(nameof(item.ToDate), "To date has not been supplied");
            }
            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                result.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                result.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
            }

            if (!result.IsValid())
                return result;

            result.IsUnauthorized = !_authorizationService.IsAuthorized(EmployerUserRole.Any);

            return result;
        }
    }
}
using System;
using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview
{
    public class GetAccountFinanceOverviewQueryValidator : IValidator<GetAccountFinanceOverviewQuery>
    {
        private readonly IAuthorizationService _authorizationService;

        public GetAccountFinanceOverviewQueryValidator(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public ValidationResult Validate(GetAccountFinanceOverviewQuery query)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountFinanceOverviewQuery query)
        {
            var result = new ValidationResult();

            result.IsUnauthorized = !_authorizationService.IsAuthorized(EmployerUserRole.Any);

            return result;
        }
    }
}

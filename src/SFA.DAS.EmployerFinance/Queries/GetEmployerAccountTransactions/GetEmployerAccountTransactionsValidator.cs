using SFA.DAS.Validation;
using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;


namespace SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsValidator : IValidator<GetEmployerAccountTransactionsQuery>
    {
        private readonly IAuthorizationService _authorizationService;

        public GetEmployerAccountTransactionsValidator(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public ValidationResult Validate(GetEmployerAccountTransactionsQuery item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAccountTransactionsQuery item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                result.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }

            if (result.IsValid() && !string.IsNullOrEmpty(item.ExternalUserId))
            {
                result.IsUnauthorized = !_authorizationService.IsAuthorized(EmployerUserRole.Any);
            }

            return result;
        }
    }
}
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.Validation;
using System;
using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.EmployerFinance.Queries.GetEmployerAccount
{
    public class GetEmployerAccountByHashedIdValidator : IValidator<GetEmployerAccountHashedQuery>
    {
        private readonly IAuthorizationService _authorizationService;

        public GetEmployerAccountByHashedIdValidator(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public ValidationResult Validate(GetEmployerAccountHashedQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAccountHashedQuery item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.UserId))
            {
                result.AddError(nameof(item.UserId), "UserId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                result.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }

            if (result.IsValid())
            {
                result.IsUnauthorized = !_authorizationService.IsAuthorized(EmployerUserRole.Any);
            }


            return result;
        }
    }
}
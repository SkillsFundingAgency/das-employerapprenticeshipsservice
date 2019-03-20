using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview
{
    public class GetAccountFinanceOverviewQueryValidator : IValidator<GetAccountFinanceOverviewQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetAccountFinanceOverviewQueryValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetAccountFinanceOverviewQuery query)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountFinanceOverviewQuery query)
        {
            var result = new ValidationResult();
           
            if (string.IsNullOrEmpty(query.AccountHashedId))
            {
                result.AddError(nameof(query.AccountHashedId), "AccountHashedId has not been supplied");
            }
            if (!query.UserRef.HasValue)
            {
                result.AddError(nameof(query.UserRef), "UserRef has not been supplied");
            }
            if (query.AccountId == null)
            {
                result.AddError(nameof(query.AccountId), "AccountId has not been supplied");
            }

            if (!result.IsValid())
                return result;

            var caller = await _membershipRepository.GetCaller(query.AccountHashedId, query.UserRef.ToString());
            result.IsUnauthorized = caller == null;

            return result;
        }
    }
}

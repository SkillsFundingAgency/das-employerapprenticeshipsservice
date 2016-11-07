using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsValidator : IValidator<GetEmployerAccountTransactionsQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerAccountTransactionsValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetEmployerAccountTransactionsQuery item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAccountTransactionsQuery item)
        {
            var result = new ValidationResult();

            if (item.AccountId == 0)
            {
                result.AddError(nameof(item.AccountId),"AccountId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.HashedId))
            {
                result.AddError(nameof(item.HashedId), "HashedId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                result.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
            }

            if (result.IsValid())
            {
                var caller = await _membershipRepository.GetCaller(item.AccountId, item.ExternalUserId);
                if (caller == null)
                {
                    result.IsUnauthorized = true;
                }
            }

            return result;
        }
    }
}
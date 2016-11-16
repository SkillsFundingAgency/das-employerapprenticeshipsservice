using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail
{
    public class GetEmployerAccountTransactionDetailQueryValidator : IValidator<GetEmployerAccountTransactionDetailQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerAccountTransactionDetailQueryValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetEmployerAccountTransactionDetailQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAccountTransactionDetailQuery item)
        {
            var result = new ValidationResult();

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

            var caller = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);
            result.IsUnauthorized = caller == null;

            return result;
        }
    }
}
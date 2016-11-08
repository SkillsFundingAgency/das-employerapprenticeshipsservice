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

            if (item.Id == 0)
            {
                result.AddError(nameof(item.Id), "Id has not been supplied");
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
                var caller = await _membershipRepository.GetCaller(item.HashedId, item.ExternalUserId);
                if (caller == null)
                {
                    result.IsUnauthorized = true;
                }
            }

            return result;
        }
    }
}
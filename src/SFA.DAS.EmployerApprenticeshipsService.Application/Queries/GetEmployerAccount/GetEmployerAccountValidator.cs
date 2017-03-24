using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountValidator : IValidator<GetEmployerAccountQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerAccountValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetEmployerAccountQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAccountQuery item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                result.AddError(nameof(item.ExternalUserId),"UserId has not been supplied");
            }
            if (item.AccountId == 0)
            {
                result.AddError(nameof(item.AccountId), "AccountId has not been supplied");
            }

            if (result.IsValid())
            {
                var membership = await _membershipRepository.GetCaller(item.AccountId, item.ExternalUserId);

                if (membership == null)
                    result.IsUnauthorized = true;
            }


            return result;
        }
    }
}

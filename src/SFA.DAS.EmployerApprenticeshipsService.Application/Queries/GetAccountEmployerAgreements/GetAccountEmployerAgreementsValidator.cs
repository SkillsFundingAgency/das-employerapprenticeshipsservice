using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsValidator : IValidator<GetAccountEmployerAgreementsRequest> 
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetAccountEmployerAgreementsValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetAccountEmployerAgreementsRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountEmployerAgreementsRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                validationResult.AddError(nameof(item.ExternalUserId),"ExternalUserId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var membership = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);

            if (membership == null)
            {
                validationResult.IsUnauthorized = true;
            }
                

            return validationResult;
        }
    }
}

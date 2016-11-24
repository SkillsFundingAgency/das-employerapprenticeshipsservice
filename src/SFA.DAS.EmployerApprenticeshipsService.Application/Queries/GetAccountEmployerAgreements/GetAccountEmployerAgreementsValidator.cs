using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

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
            if (string.IsNullOrEmpty(item.HashedId))
            {
                validationResult.AddError(nameof(item.HashedId),"AccountId has not been supplied");
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var membership = await _membershipRepository.GetCaller(item.HashedId, item.ExternalUserId);

            if (membership == null)
            {
                validationResult.IsUnauthorized = true;
                validationResult.AddError("Membership", "Caller is not a member of this account");
            }
                

            return validationResult;
        }
    }
}

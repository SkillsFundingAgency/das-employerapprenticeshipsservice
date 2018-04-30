using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove
{
    public class GetAccountEmployerAgreementsRemoveValidator : IValidator<GetAccountEmployerAgreementsRemoveRequest>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetAccountEmployerAgreementsRemoveValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetAccountEmployerAgreementsRemoveRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountEmployerAgreementsRemoveRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            if (item.ExternalUserId.Equals(Guid.Empty))
            {
                validationResult.AddError(nameof(item.ExternalUserId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var member = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);

            if (member == null || member.Role != Role.Owner)
            {
                validationResult.IsUnauthorized = true;
            }

            return validationResult;
        }
    }
}
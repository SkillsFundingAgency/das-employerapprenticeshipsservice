using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementRemove
{
    public class GetAccountEmployerAgreementRemoveValidator : IValidator<GetAccountEmployerAgreementRemoveRequest>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetAccountEmployerAgreementRemoveValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetAccountEmployerAgreementRemoveRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountEmployerAgreementRemoveRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }
            if (string.IsNullOrEmpty(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId));
            }
            if (string.IsNullOrEmpty(item.HashedAgreementId))
            {
                validationResult.AddError(nameof(item.HashedAgreementId));
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var user = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

            if (user == null || user.Role != Role.Owner)
            {
                validationResult.IsUnauthorized = true;
            }

            return validationResult;
        }
    }
}
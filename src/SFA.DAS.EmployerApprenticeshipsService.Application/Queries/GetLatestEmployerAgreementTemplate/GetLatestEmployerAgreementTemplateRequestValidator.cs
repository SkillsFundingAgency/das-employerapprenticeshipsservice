using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate
{
    public class GetLatestEmployerAgreementTemplateRequestValidator :
        IValidator<GetLatestEmployerAgreementTemplateRequest>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetLatestEmployerAgreementTemplateRequestValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetLatestEmployerAgreementTemplateRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetLatestEmployerAgreementTemplateRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId), "HashedId has not been supplied");
            }

            if (item.ExternalUserId.Equals(Guid.Empty))
            {
                validationResult.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
            }

            if (validationResult.IsValid())
            {
                var member = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);
                if (member == null || member.Role != Role.Owner)
                {
                    validationResult.IsUnauthorized = true;
                }
            }

            return validationResult;
        }
    }
}
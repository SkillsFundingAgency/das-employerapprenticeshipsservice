﻿using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

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

            if (string.IsNullOrEmpty(item.HashedId))
            {
                validationResult.AddError(nameof(item.HashedId), "HashedId has not been supplied");
            }

            if (string.IsNullOrWhiteSpace(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId), "UserId has not been supplied");
            }

            if (validationResult.IsValid())
            {
                var member = await _membershipRepository.GetCaller(item.HashedId, item.UserId);
                if (member == null || member.RoleId != (short) Role.Owner)
                {
                    validationResult.IsUnauthorized = true;
                }
            }

            return validationResult;
        }
    }
}
﻿using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf
{
    public class GetEmployerAgreementPdfValidator : IValidator<GetEmployerAgreementPdfRequest>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetEmployerAgreementPdfValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetEmployerAgreementPdfRequest item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAgreementPdfRequest item)
        {
            
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            if (string.IsNullOrEmpty(item.HashedLegalAgreementId))
            {
                validationResult.AddError(nameof(item.HashedLegalAgreementId));
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
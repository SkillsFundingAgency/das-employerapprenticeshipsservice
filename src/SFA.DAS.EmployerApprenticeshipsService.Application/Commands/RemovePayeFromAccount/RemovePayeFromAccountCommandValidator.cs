﻿using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommandValidator : IValidator<RemovePayeFromAccountCommand>
    {
        private readonly IMembershipRepository _membershipRepository;

        public RemovePayeFromAccountCommandValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(RemovePayeFromAccountCommand item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(RemovePayeFromAccountCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }
            if (string.IsNullOrWhiteSpace(item.PayeRef))
            {
                validationResult.AddError(nameof(item.PayeRef), "PayeRef has not been supplied");
            }
            if (item.ExternalUserId.Equals(Guid.Empty))
            {
                validationResult.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
            }
            if (!item.RemoveScheme)
            {
                validationResult.AddError(nameof(item.RemoveScheme), "Please confirm you wish to remove the scheme");
            }

            if (validationResult.IsValid())
            {
                var member = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);
                if (member== null || member.Role != Role.Owner)
                {
                    validationResult.IsUnauthorized = true;
                }
            }

            return validationResult;
        }
    }
}
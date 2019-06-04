﻿using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesValidator : IValidator<GetAccountLegalEntitiesRequest>
    {
        public ValidationResult Validate(GetAccountLegalEntitiesRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedLegalEntityId))
            {
                validationResult.AddError(nameof(item.HashedLegalEntityId), "HashedLegalEntityId has not been supplied");
            }

            if (string.IsNullOrWhiteSpace(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId), "User Id has not been supplied");
            }
            else
            {
                if (!Guid.TryParse(item.UserId, out _))
                {
                    validationResult.AddError(nameof(item.UserId), "User Id has not been supplied in the correct format");
                }
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountLegalEntitiesRequest item)
        {
            throw new NotImplementedException();
        }
    }
}
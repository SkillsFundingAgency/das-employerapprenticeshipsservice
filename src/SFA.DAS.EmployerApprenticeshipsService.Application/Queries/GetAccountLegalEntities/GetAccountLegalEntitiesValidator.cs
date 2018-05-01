using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

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
            if (item.ExternalUserId.Equals(Guid.Empty))
            {
                validationResult.AddError(nameof(item.ExternalUserId), "User Id has not been supplied");
            }
            //else
            //{
            //    Guid output;
            //    if (!Guid.TryParse(item.ExternalUserId, out output))
            //    {
            //        validationResult.AddError(nameof(item.ExternalUserId), "User Id has not been supplied in the correct format");
            //    }
            //}

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountLegalEntitiesRequest item)
        {
            throw new NotImplementedException();
        }
    }
}
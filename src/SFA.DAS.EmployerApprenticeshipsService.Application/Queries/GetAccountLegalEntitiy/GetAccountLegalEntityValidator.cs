using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLegalEntitiy
{
    public class GetAccountLegalEntityValidator : IValidator<GetAccountLegalEntityRequest>
    {
        public ValidationResult Validate(GetAccountLegalEntityRequest item)
        {
            var validationResult = new ValidationResult();

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountLegalEntityRequest item)
        {
            throw new NotImplementedException();
        }
    }
}
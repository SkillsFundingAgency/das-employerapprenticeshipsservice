using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationValidator : IValidator<GetLevyDeclarationRequest>
    {
        public ValidationResult Validate(GetLevyDeclarationRequest item)
        {
            var result = new ValidationResult();

            if (item.AccountId == 0)
            {
                result.AddError(nameof(item.AccountId),"AccountId has not been supplied");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetLevyDeclarationRequest item)
        {
            throw new NotImplementedException();
        }
    }
}
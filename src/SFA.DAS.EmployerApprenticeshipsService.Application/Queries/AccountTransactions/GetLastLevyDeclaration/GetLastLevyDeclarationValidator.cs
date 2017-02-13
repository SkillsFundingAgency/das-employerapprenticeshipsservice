using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationValidator : IValidator<GetLastLevyDeclarationRequest>
    {
        public ValidationResult Validate(GetLastLevyDeclarationRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.Empref))
            {
                validationResult.AddError(nameof(item.Empref));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetLastLevyDeclarationRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}
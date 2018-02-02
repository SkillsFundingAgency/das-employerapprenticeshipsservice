using SFA.DAS.EAS.Application.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferBalance
{
    public class GetTransferBalanceRequestValidator : IValidator<GetTransferBalanaceRequest>
    {
        public ValidationResult Validate(GetTransferBalanaceRequest item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                result.ValidationDictionary.Add(nameof(item.HashedAccountId),
                    "Hashed Account Id cannot be null or empty.");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetTransferBalanaceRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}

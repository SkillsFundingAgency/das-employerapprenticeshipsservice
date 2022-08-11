using MediatR;
using SFA.DAS.EmployerFinance.Data;

using SFA.DAS.HashingService;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationValidator : IValidator<GetLevyDeclarationRequest>
    {
        public ValidationResult Validate(GetLevyDeclarationRequest item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                result.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetLevyDeclarationRequest item)
        {
            throw new NotImplementedException();
        }
    }
}

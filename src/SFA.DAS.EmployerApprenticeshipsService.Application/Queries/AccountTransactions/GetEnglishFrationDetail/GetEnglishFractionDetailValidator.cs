using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailValidator : IValidator<GetEnglishFractionDetailByEmpRefQuery>
    {
        public ValidationResult Validate(GetEnglishFractionDetailByEmpRefQuery item)
        {
            throw new NotImplementedException();
        }

        public Task<ValidationResult> ValidateAsync(GetEnglishFractionDetailByEmpRefQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
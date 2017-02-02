using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetPostcodeAddress
{
    public class GetPostcodeAddressValidator : IValidator<GetPostcodeAddressRequest>
    {
        public ValidationResult Validate(GetPostcodeAddressRequest item)
        {
            throw new NotImplementedException();
        }

        public Task<ValidationResult> ValidateAsync(GetPostcodeAddressRequest item)
        {
            throw new NotImplementedException();
        }
    }
}

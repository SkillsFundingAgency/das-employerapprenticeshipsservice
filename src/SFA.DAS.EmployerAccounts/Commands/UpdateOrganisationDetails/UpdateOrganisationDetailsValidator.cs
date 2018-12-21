using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.UpdateOrganisationDetails
{
    public class UpdateOrganisationDetailsValidator : IValidator<UpdateOrganisationDetailsRequest>
    {
        public ValidationResult Validate(UpdateOrganisationDetailsRequest item)
        {
            var results = new ValidationResult();

            return results;
        }

        public Task<ValidationResult> ValidateAsync(UpdateOrganisationDetailsRequest item)
        {
            throw new NotImplementedException();
        }
    }
}

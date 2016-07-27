using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountTeamMembers
{
    public class GetAccountTeamMembersValidator : IValidator<GetAccountTeamMembersQuery>
    {
        public ValidationResult Validate(GetAccountTeamMembersQuery item)
        {
            var validationResult = new ValidationResult();
            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                validationResult.AddError("ExternalUserId", "can't be null");
            }
            return validationResult;
        }
    }
}

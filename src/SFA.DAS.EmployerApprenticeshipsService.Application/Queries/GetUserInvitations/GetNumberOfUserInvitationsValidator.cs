using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetUserInvitations
{
    public class GetNumberOfUserInvitationsValidator : IValidator<GetNumberOfUserInvitationsQuery>
    {
        public ValidationResult Validate(GetNumberOfUserInvitationsQuery item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.UserId))
            {
                result.AddError(nameof(item.UserId), "UserId has not been supplied");
            }
            else
            {
                Guid value;
                var guidResult = Guid.TryParse(item.UserId, out value);
                if (!guidResult)
                {
                    result.AddError(nameof(item.UserId), "UserId is not in the correct format");
                }
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetNumberOfUserInvitationsQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
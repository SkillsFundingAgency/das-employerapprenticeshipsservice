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

            if (item.ExternalUserId.Equals(Guid.Empty))
            {
                result.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
            }
            //else
            //{
            //    Guid value;
            //    var guidResult = Guid.TryParse(item.ExternalUserId, out value);
            //    if (!guidResult)
            //    {
            //        result.AddError(nameof(item.ExternalUserId), "ExternalUserId is not in the correct format");
            //    }
            //}

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetNumberOfUserInvitationsQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
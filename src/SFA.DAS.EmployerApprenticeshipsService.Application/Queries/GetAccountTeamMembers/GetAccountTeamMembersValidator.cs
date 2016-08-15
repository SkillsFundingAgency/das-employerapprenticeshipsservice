using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountTeamMembers
{
    public class GetAccountTeamMembersValidator : IValidator<GetAccountTeamMembersQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetAccountTeamMembersValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetAccountTeamMembersQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountTeamMembersQuery item)
        {
            var validationResult = new ValidationResult();
            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                validationResult.AddError(nameof(item.ExternalUserId), "UserId has not been supplied");
            }
            if (item.Id == 0)
            {
                validationResult.AddError(nameof(item.Id), "AccountId has not been supplied");
            }

            if (validationResult.IsValid())
            {
                var member = await _membershipRepository.GetCaller(item.Id, item.ExternalUserId);
                if (member == null)
                {
                    validationResult.AddError(nameof(member), "Unauthorised: User not connected to account");
                    validationResult.IsUnauthorized = true;
                }
                else if(member.RoleId != (short)Role.Owner)
                {
                    validationResult.AddError(nameof(member), "Unauthorised: User is not an owner of this account");
                    validationResult.IsUnauthorized = true;
                }
            }

            return validationResult;
        }
    }
}

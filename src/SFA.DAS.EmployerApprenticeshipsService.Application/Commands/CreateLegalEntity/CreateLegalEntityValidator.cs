using System;
using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityValidator: IValidator<CreateLegalEntityCommand>
    {
        private readonly IMembershipRepository _membershipRepository;

        public CreateLegalEntityValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }


        public ValidationResult Validate(CreateLegalEntityCommand item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(CreateLegalEntityCommand item)
        {
            var validationResult = new ValidationResult();

            var member = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

            if (member == null || member.RoleId != (short)Role.Owner)
            {
                validationResult.IsUnauthorized = true;
            }

            return validationResult;
        }
    }
}
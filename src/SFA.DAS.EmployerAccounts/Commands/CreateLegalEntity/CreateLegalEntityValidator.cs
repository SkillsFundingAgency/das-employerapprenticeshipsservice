using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;

public class CreateLegalEntityValidator : IValidator<CreateLegalEntityCommand>
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

        var member = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);

        if (member == null || member.Role != Role.Owner)
        {
            validationResult.IsUnauthorized = true;
        }

        return validationResult;
    }
}
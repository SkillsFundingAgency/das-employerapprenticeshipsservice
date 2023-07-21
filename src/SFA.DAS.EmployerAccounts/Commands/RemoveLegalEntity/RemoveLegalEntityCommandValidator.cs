using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;

public class RemoveLegalEntityCommandValidator : IValidator<RemoveLegalEntityCommand>
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;

    public RemoveLegalEntityCommandValidator(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository)
    {
        _membershipRepository = membershipRepository;
        _employerAgreementRepository = employerAgreementRepository;
    }

    public ValidationResult Validate(RemoveLegalEntityCommand item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(RemoveLegalEntityCommand item)
    {
        var validationResult = new ValidationResult();

        if (item.AccountId <= 0)
        {
            validationResult.AddError(nameof(item.AccountId));
        }
        if (string.IsNullOrEmpty(item.UserId))
        {
            validationResult.AddError(nameof(item.UserId));
        }
        if (item.AccountLegalEntityId <= 0)
        {
            validationResult.AddError(nameof(item.AccountLegalEntityId));
        }

        if (!validationResult.IsValid())
        {
            return validationResult;
        }

        var member = await _membershipRepository.GetCaller(item.AccountId, item.UserId);

        if (member == null || !member.Role.Equals(Role.Owner))
        {
            validationResult.IsUnauthorized = true;
            return validationResult;
        }
        
        var legalEntities = await _employerAgreementRepository.GetLegalEntitiesLinkedToAccount(item.AccountId, false);

        if (legalEntities != null && legalEntities.Count == 1)
        {
            validationResult.AddError(nameof(item.AccountLegalEntityId), "There must be at least one legal entity on the account");
            return validationResult;
        }

        return validationResult;
    }
}
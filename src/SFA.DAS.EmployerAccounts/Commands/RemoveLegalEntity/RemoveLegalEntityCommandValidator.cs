using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;

public class RemoveLegalEntityCommandValidator : IValidator<RemoveLegalEntityCommand>
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IHashingService _hashingService;

    public RemoveLegalEntityCommandValidator(IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService)
    {
        _membershipRepository = membershipRepository;
        _employerAgreementRepository = employerAgreementRepository;
        _hashingService = hashingService;
    }

    public ValidationResult Validate(RemoveLegalEntityCommand item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(RemoveLegalEntityCommand item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.AddError(nameof(item.HashedAccountId));
        }
        if (string.IsNullOrEmpty(item.UserId))
        {
            validationResult.AddError(nameof(item.UserId));
        }
        if (string.IsNullOrEmpty(item.HashedAccountLegalEntityId))
        {
            validationResult.AddError(nameof(item.HashedAccountLegalEntityId));
        }

        if (!validationResult.IsValid())
        {
            return validationResult;
        }

        var member = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

        if (member == null || !member.Role.Equals(Role.Owner))
        {
            validationResult.IsUnauthorized = true;
            return validationResult;
        }

        var accountId = _hashingService.DecodeValue(item.HashedAccountId);
        var legalEntities = await _employerAgreementRepository.GetLegalEntitiesLinkedToAccount(accountId, false);

        if (legalEntities != null && legalEntities.Count == 1)
        {
            validationResult.AddError(nameof(item.HashedAccountLegalEntityId), "There must be at least one legal entity on the account");
            return validationResult;
        }

        return validationResult;
    }
}
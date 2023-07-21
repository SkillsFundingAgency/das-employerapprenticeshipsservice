using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;

public class SignEmployerAgreementCommandValidator : IValidator<SignEmployerAgreementCommand>
{
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IEncodingService _encodingService;

    public SignEmployerAgreementCommandValidator(IEmployerAgreementRepository employerAgreementRepository, IEncodingService encodingService)
    {
        _employerAgreementRepository = employerAgreementRepository;
        _encodingService = encodingService;
    }

    public ValidationResult Validate(SignEmployerAgreementCommand item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(SignEmployerAgreementCommand item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.HashedAgreementId))
            validationResult.AddError(nameof(item.HashedAgreementId));

        if (string.IsNullOrEmpty(item.HashedAccountId))
            validationResult.AddError(nameof(item.HashedAccountId));

        if (string.IsNullOrWhiteSpace(item.ExternalUserId))
            validationResult.AddError(nameof(item.ExternalUserId));

        if (item.SignedDate == default(DateTime))
            validationResult.AddError(nameof(item.SignedDate));

        if (!validationResult.IsValid())
        {
            return validationResult;
        }

        var agreementId = _encodingService.Decode(item.HashedAgreementId, EncodingType.AccountId);
        var employerAgreementStatus = await _employerAgreementRepository.GetEmployerAgreementStatus(agreementId);

        if (employerAgreementStatus == null)
        {
            validationResult.AddError(nameof(employerAgreementStatus), "Agreement does not exist");
            return validationResult;
        }

        if (employerAgreementStatus == EmployerAgreementStatus.Signed ||
            employerAgreementStatus == EmployerAgreementStatus.Expired ||
            employerAgreementStatus == EmployerAgreementStatus.Superseded)
        {
            validationResult.AddError(nameof(employerAgreementStatus),$"Agreement status is {employerAgreementStatus}");
            return validationResult;
        }
            
        return validationResult;
    }
}